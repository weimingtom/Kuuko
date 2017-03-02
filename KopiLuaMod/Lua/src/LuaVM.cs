/*
 ** $Id: lvm.c,v 2.63.1.3 2007/12/28 15:32:23 roberto Exp $
 ** Lua virtual machine
 ** See Copyright Notice in lua.h
 */
using System;

namespace KopiLua
{
	//using TValue = Lua.TValue;
	//using StkId = TValue;
	//using lua_Number = System.Double;
	//using lu_byte = System.Byte;
	//using ptrdiff_t = System.Int32;
	//using Instruction = System.UInt32;

	public class LuaVM
	{
		public static int tostring(lua_State L, TValue/*StkId*/ o)
		{
			return ((LuaObject.ttype(o) == Lua.LUA_TSTRING) || (luaV_tostring(L, o) != 0)) ? 1 : 0;
		}

		public static int tonumber(/*ref*/ TValue[]/*StkId*/ o, TValue n)
		{
			return ((LuaObject.ttype(o[0]) == Lua.LUA_TNUMBER || (((o[0]) = luaV_tonumber(o[0], n)) != null))) ? 1 : 0;
		}

		public static int equalobj(lua_State L, TValue o1, TValue o2) 
		{
			return ((LuaObject.ttype(o1) == LuaObject.ttype(o2)) && (luaV_equalval(L, o1, o2) != 0)) ? 1 : 0;
		}

		/* limit for table tag-method chains (to avoid loops) */
		public const int MAXTAGLOOP	= 100;

		public static TValue luaV_tonumber(TValue obj, TValue n) 
		{
			Double[]/*lua_Number*/ num = new Double[1];
			if (LuaObject.ttisnumber(obj)) 
			{
				return obj;
			}
			if (LuaObject.ttisstring(obj) && (LuaObject.luaO_str2d(LuaObject.svalue(obj), /*out*/ num) != 0))
			{
				LuaObject.setnvalue(n, num[0]);
				return n;
			}
			else
			{
				return null;
			}
		}


		public static int luaV_tostring(lua_State L, TValue/*StkId*/ obj)
		{
			if (!LuaObject.ttisnumber(obj))
			{
				return 0;
			}
			else 
			{
				Double/*lua_Number*/ n = LuaObject.nvalue(obj);
				CharPtr s = LuaConf.lua_number2str(n);
				LuaObject.setsvalue2s(L, obj, LuaString.luaS_new(L, s));
				return 1;
			}
		}


		private static void traceexec(lua_State L, InstructionPtr pc) 
		{
			Byte/*lu_byte*/ mask = L.hookmask;
			InstructionPtr oldpc = InstructionPtr.Assign(L.savedpc);
			L.savedpc = InstructionPtr.Assign(pc);
			if (((mask & Lua.LUA_MASKCOUNT) != 0) && (L.hookcount == 0))
			{
				LuaDebug.resethookcount(L);
				LuaDo.luaD_callhook(L, Lua.LUA_HOOKCOUNT, -1);
			}
			if ((mask & Lua.LUA_MASKLINE) != 0)
			{
				Proto p = LuaState.ci_func(L.ci).l.p;
				int npc = LuaDebug.pcRel(pc, p);
				int newline = LuaDebug.getline(p, npc);
				/* call linehook when enter a new function, when jump back (loop),
			   or when enter a new line */
                if (npc == 0 || InstructionPtr.lessEqual(pc, oldpc) || newline != LuaDebug.getline(p, LuaDebug.pcRel(oldpc, p)))
				{
					LuaDo.luaD_callhook(L, Lua.LUA_HOOKLINE, newline);
				}
			}
		}

		private static void callTMres(lua_State L, TValue/*StkId*/ res, TValue f,
			TValue p1, TValue p2) 
		{
            int/*Int32*//*ptrdiff_t*/ result = LuaDo.savestack(L, res);
			LuaObject.setobj2s(L, L.top, f);  /* push function */
			LuaObject.setobj2s(L, TValue.plus(L.top, 1), p1);  /* 1st argument */
			LuaObject.setobj2s(L, TValue.plus(L.top, 2), p2);  /* 2nd argument */
			LuaDo.luaD_checkstack(L, 3);
			L.top = TValue.plus(L.top, 3);
			LuaDo.luaD_call(L, TValue.minus(L.top, 3), 1);
			res = LuaDo.restorestack(L, result);
			TValue[] top = new TValue[1];
			top[0] = L.top;
			/*StkId*/TValue.dec(/*ref*/ top);
			L.top = top[0];
			LuaObject.setobjs2s(L, res, L.top);
		}

		private static void callTM(lua_State L, TValue f, TValue p1,
			TValue p2, TValue p3) 
		{
			LuaObject.setobj2s(L, L.top, f);  /* push function */
			LuaObject.setobj2s(L, TValue.plus(L.top, 1), p1);  /* 1st argument */
			LuaObject.setobj2s(L, TValue.plus(L.top, 2), p2);  /* 2nd argument */
			LuaObject.setobj2s(L, TValue.plus(L.top, 3), p3);  /* 3th argument */
			LuaDo.luaD_checkstack(L, 4);
			L.top = TValue.plus(L.top, 4);
			LuaDo.luaD_call(L, TValue.minus(L.top, 4), 0);
		}


		public static void luaV_gettable(lua_State L, TValue t, TValue key, TValue/*StkId*/ val)
		{
			int loop;
			for (loop = 0; loop < MAXTAGLOOP; loop++) 
			{
				TValue tm;
				if (LuaObject.ttistable(t))
				{  
					/* `t' is a table? */
					Table h = LuaObject.hvalue(t);
					TValue res = LuaTable.luaH_get(h, key); /* do a primitive get */
					if (!LuaObject.ttisnil(res) ||  /* result is no nil? */
					    (tm = LuaTM.fasttm(L, h.metatable, TMS.TM_INDEX)) == null)
					{ 
						/* or no TM? */
						LuaObject.setobj2s(L, val, res);
						return;
					}
					/* else will try the tag method */
				}
				else if (LuaObject.ttisnil(tm = LuaTM.luaT_gettmbyobj(L, t, TMS.TM_INDEX)))
				{
					LuaDebug.luaG_typeerror(L, t, CharPtr.toCharPtr("index"));
				}
				if (LuaObject.ttisfunction(tm))
				{
					callTMres(L, val, tm, t, key);
					return;
				}
				t = tm;  /* else repeat with `tm' */
			}
			LuaDebug.luaG_runerror(L, CharPtr.toCharPtr("loop in gettable"));
		}

		public static void luaV_settable(lua_State L, TValue t, TValue key, TValue/*StkId*/ val)
		{
			int loop;

			for (loop = 0; loop < MAXTAGLOOP; loop++) 
			{
				TValue tm;
				if (LuaObject.ttistable(t))
				{  
					/* `t' is a table? */
					Table h = LuaObject.hvalue(t);
					TValue oldval = LuaTable.luaH_set(L, h, key); /* do a primitive set */
					if (!LuaObject.ttisnil(oldval) ||  /* result is no nil? */
					    (tm = LuaTM.fasttm(L, h.metatable, TMS.TM_NEWINDEX)) == null)
					{ 
						/* or no TM? */
						LuaObject.setobj2t(L, oldval, val);
						LuaGC.luaC_barriert(L, h, val);
						return;
					}
					/* else will try the tag method */
				}
				else if (LuaObject.ttisnil(tm = LuaTM.luaT_gettmbyobj(L, t, TMS.TM_NEWINDEX)))
				{
					LuaDebug.luaG_typeerror(L, t, CharPtr.toCharPtr("index"));
				}
				if (LuaObject.ttisfunction(tm))
				{
					callTM(L, tm, t, key, val);
					return;
				}
				t = tm;  /* else repeat with `tm' */
			}
			LuaDebug.luaG_runerror(L, CharPtr.toCharPtr("loop in settable"));
		}

		private static int call_binTM(lua_State L, TValue p1, TValue p2,
			TValue/*StkId*/ res, TMS event_) 
		{
			TValue tm = LuaTM.luaT_gettmbyobj(L, p1, event_);  /* try first operand */
			if (LuaObject.ttisnil(tm))
			{
				tm = LuaTM.luaT_gettmbyobj(L, p2, event_);  /* try second operand */
			}
			if (LuaObject.ttisnil(tm)) 
			{
				return 0;
			}
			callTMres(L, res, tm, p1, p2);
			return 1;
		}

		private static TValue get_compTM(lua_State L, Table mt1, Table mt2,
			TMS event_) 
		{
			TValue tm1 = LuaTM.fasttm(L, mt1, event_);
			TValue tm2;
			if (tm1 == null)
			{
				return null;  /* no metamethod */
			}
			if (mt1 == mt2)
			{
				return tm1;  /* same metatables => same metamethods */
			}
			tm2 = LuaTM.fasttm(L, mt2, event_);
			if (tm2 == null) 
			{
				return null;  /* no metamethod */
			}
			if (LuaObject.luaO_rawequalObj(tm1, tm2) != 0)  /* same metamethods? */
			{
				return tm1;
			}
			return null;
		}

		private static int call_orderTM(lua_State L, TValue p1, TValue p2,
			TMS event_) 
		{
			TValue tm1 = LuaTM.luaT_gettmbyobj(L, p1, event_);
			TValue tm2;
			if (LuaObject.ttisnil(tm1)) 
			{
				return -1;  /* no metamethod? */
			}
			tm2 = LuaTM.luaT_gettmbyobj(L, p2, event_);
			if (LuaObject.luaO_rawequalObj(tm1, tm2) == 0)  /* different metamethods? */
			{
				return -1;
			}
			callTMres(L, L.top, tm1, p1, p2);
			return LuaObject.l_isfalse(L.top) == 0 ? 1 : 0;
		}

		private static int l_strcmp(TString ls, TString rs) 
		{
			CharPtr l = LuaObject.getstr(ls);
            int/*uint*/ ll = ls.getTsv().len;
			CharPtr r = LuaObject.getstr(rs);
            int/*uint*/ lr = rs.getTsv().len;
			for (;;) 
			{
				//int temp = strcoll(l, r);
				int temp = String.Compare(l.ToString(), r.ToString());
				if (temp != 0) 
				{
					return temp;
				}
				else 
				{  
					/* strings are equal up to a `\0' */
					int/*uint*/ len = /*(uint)*/l.ToString().Length;  /* index of first `\0' in both strings */
					if (len == lr)  /* r is finished? */
					{
						return (len == ll) ? 0 : 1;
					}
					else if (len == ll)  /* l is finished? */
					{
						return -1;  /* l is smaller than r (because r is not finished) */
					}
					/* both strings longer than `len'; go on comparing (after the `\0') */
					len++;
					l = CharPtr.plus(l, len);
					ll -= len;
					r = CharPtr.plus(r, len);
					lr -= len;
				}
			}
		}

		public static int luaV_lessthan(lua_State L, TValue l, TValue r) 
		{
			int res;
			if (LuaObject.ttype(l) != LuaObject.ttype(r))
			{
				return LuaDebug.luaG_ordererror(L, l, r);
			}
			else if (LuaObject.ttisnumber(l))
			{
				return LuaConf.luai_numlt(LuaObject.nvalue(l), LuaObject.nvalue(r)) ? 1 : 0;
			}
			else if (LuaObject.ttisstring(l))
			{
				return (l_strcmp(LuaObject.rawtsvalue(l), LuaObject.rawtsvalue(r)) < 0) ? 1 : 0;
			}
			else if ((res = call_orderTM(L, l, r, TMS.TM_LT)) != -1)
			{
				return res;
			}
			return LuaDebug.luaG_ordererror(L, l, r);
		}

		private static int lessequal(lua_State L, TValue l, TValue r) 
		{
			int res;
			if (LuaObject.ttype(l) != LuaObject.ttype(r))
			{
				return LuaDebug.luaG_ordererror(L, l, r);
			}
			else if (LuaObject.ttisnumber(l))
			{
				return LuaConf.luai_numle(LuaObject.nvalue(l), LuaObject.nvalue(r)) ? 1 : 0;
				
			}
			else if (LuaObject.ttisstring(l))
			{
				return (l_strcmp(LuaObject.rawtsvalue(l), LuaObject.rawtsvalue(r)) <= 0) ? 1 : 0;
			}
			else if ((res = call_orderTM(L, l, r, TMS.TM_LE)) != -1)  /* first try `le' */
			{
				return res;
			}
			else if ((res = call_orderTM(L, r, l, TMS.TM_LT)) != -1)  /* else try `lt' */
			{
				return (res == 0) ? 1 : 0;
			}
			return LuaDebug.luaG_ordererror(L, l, r);
		}

		static CharPtr mybuff = null;

		public static int luaV_equalval(lua_State L, TValue t1, TValue t2) 
		{
			TValue tm = null;
			LuaLimits.lua_assert(LuaObject.ttype(t1) == LuaObject.ttype(t2));
			switch (LuaObject.ttype(t1))
			{
				case Lua.LUA_TNIL: 
					{
						return 1;
					}
				case Lua.LUA_TNUMBER: 
					{
						return LuaConf.luai_numeq(LuaObject.nvalue(t1), LuaObject.nvalue(t2)) ? 1 : 0;
					}
				case Lua.LUA_TBOOLEAN: 
					{
						return (LuaObject.bvalue(t1) == LuaObject.bvalue(t2)) ? 1 : 0;  /* true must be 1 !! */
					}
				case Lua.LUA_TLIGHTUSERDATA: 
					{
						return (LuaObject.pvalue(t1) == LuaObject.pvalue(t2)) ? 1 : 0;
					}
				case Lua.LUA_TUSERDATA:
					{
						if (LuaObject.uvalue(t1) == LuaObject.uvalue(t2)) 
						{
							return 1;
						}
						tm = get_compTM(L, LuaObject.uvalue(t1).metatable, LuaObject.uvalue(t2).metatable,
							TMS.TM_EQ);
						break;  /* will try TM */
					}
				case Lua.LUA_TTABLE:
					{
						if (LuaObject.hvalue(t1) == LuaObject.hvalue(t2)) 
						{
							return 1;
						}
						tm = get_compTM(L, LuaObject.hvalue(t1).metatable, LuaObject.hvalue(t2).metatable, TMS.TM_EQ);
						break;  /* will try TM */
					}
				default: 
					{
						return (LuaObject.gcvalue(t1) == LuaObject.gcvalue(t2)) ? 1 : 0;
					}
			}
			if (tm == null) 
			{
				return 0;  /* no TM? */
			}
			callTMres(L, L.top, tm, t1, t2);  /* call TM */
			return LuaObject.l_isfalse(L.top) == 0 ? 1 : 0;
		}

		public static void luaV_concat (lua_State L, int total, int last) 
		{
			do 
			{
				TValue/*StkId*/ top = TValue.plus(L.base_, last + 1);
				int n = 2;  /* number of elements handled in this pass (at least 2) */
				if (!(LuaObject.ttisstring(TValue.minus(top, 2)) || LuaObject.ttisnumber(TValue.minus(top, 2))) || (tostring(L, TValue.minus(top, 1)) == 0))
				{
					if (call_binTM(L, TValue.minus(top, 2), TValue.minus(top, 1), TValue.minus(top, 2), TMS.TM_CONCAT) == 0)
					{
						LuaDebug.luaG_concaterror(L, TValue.minus(top, 2), TValue.minus(top, 1));
					}
				}
				else if (LuaObject.tsvalue(TValue.minus(top, 1)).len == 0)  /* second op is empty? */
				{
					tostring(L, TValue.minus(top, 2));  /* result is first op (as string) */
				}
				else 
				{
					/* at least two string values; get as many as possible */
					int/*uint*/ tl = LuaObject.tsvalue(TValue.minus(top, 1)).len;
					CharPtr buffer;
					int i;
					/* collect total length */
					for (n = 1; n < total && (tostring(L, TValue.minus(TValue.minus(top, n), 1)) != 0); n++) //FIXME:
					{
						int/*uint*/ l = LuaObject.tsvalue(TValue.minus(TValue.minus(top, n), 1)).len;
						if (l >= LuaLimits.MAX_SIZET - tl) 
						{
							LuaDebug.luaG_runerror(L, CharPtr.toCharPtr("string length overflow"));
						}
						tl += l;
					}
					buffer = LuaZIO.luaZ_openspace(L, LuaState.G(L).buff, tl);
					if (CharPtr.isEqual(mybuff, null))
					{
						mybuff = buffer;
					}
					tl = 0;
					for (i = n; i > 0; i--) 
					{  
						/* concat all strings */
						int/*uint*/ l = LuaObject.tsvalue(TValue.minus(top, i)).len;
						LuaConf.memcpy_char(buffer.chars, /*(int)*/tl, LuaObject.svalue(TValue.minus(top, i)).chars, (int)l);
						tl += l;
					}
					LuaObject.setsvalue2s(L, TValue.minus(top, n), LuaString.luaS_newlstr(L, buffer, tl));
				}
				total -= n - 1;  /* got `n' strings to create 1 new */
				last -= n - 1;
			} while (total > 1);  /* repeat until only 1 result left */
		}

		public static void Arith(lua_State L, TValue/*StkId*/ ra, TValue rb,
			TValue rc, TMS op) 
		{
			TValue tempb = new TValue(), tempc = new TValue();
			TValue b, c;
			if ((b = luaV_tonumber(rb, tempb)) != null &&
			    (c = luaV_tonumber(rc, tempc)) != null) 
			{
				Double/*lua_Number*/ nb = LuaObject.nvalue(b), nc = LuaObject.nvalue(c);
				switch (op) 
				{
					case TMS.TM_ADD: 
						{
							LuaObject.setnvalue(ra, LuaConf.luai_numadd(nb, nc)); 
							break;
						}
					case TMS.TM_SUB: 
						{
							LuaObject.setnvalue(ra, LuaConf.luai_numsub(nb, nc)); 
							break;
						}
					case TMS.TM_MUL: 
						{
							LuaObject.setnvalue(ra, LuaConf.luai_nummul(nb, nc)); 
							break;
						}
					case TMS.TM_DIV: 
						{
							LuaObject.setnvalue(ra, LuaConf.luai_numdiv(nb, nc)); 
							break;
						}
					case TMS.TM_MOD: 
						{
							LuaObject.setnvalue(ra, LuaConf.luai_nummod(nb, nc)); 
							break;
						}
					case TMS.TM_POW: 
						{
							LuaObject.setnvalue(ra, LuaConf.luai_numpow(nb, nc)); 
							break;
						}
					case TMS.TM_UNM: 
						{
							LuaObject.setnvalue(ra, LuaConf.luai_numunm(nb)); 
							break;
						}
					default: 
						{
							LuaLimits.lua_assert(false); 
							break;
						}
				}
			}
			else if (call_binTM(L, rb, rc, ra, op) == 0)
			{
				LuaDebug.luaG_aritherror(L, rb, rc);
			}
		}

		/*
		 ** some macros for common tasks in `luaV_execute'
		 */
		public static void runtime_check(lua_State L, bool c)	
		{ 
			ClassType.Assert(c); 
		}

		//#define RA(i)	(base+GETARG_A(i))
		/* to be used after possible stack reallocation */
		//#define RB(i)	check_exp(getBMode(GET_OPCODE(i)) == OpArgMask.OpArgR, base+GETARG_B(i))
		//#define RC(i)	check_exp(getCMode(GET_OPCODE(i)) == OpArgMask.OpArgR, base+GETARG_C(i))
		//#define RKB(i)	check_exp(getBMode(GET_OPCODE(i)) == OpArgMask.OpArgK, \
		//ISK(GETARG_B(i)) ? k+INDEXK(GETARG_B(i)) : base+GETARG_B(i))
		//#define RKC(i)	check_exp(getCMode(GET_OPCODE(i)) == OpArgMask.OpArgK, \
		//	ISK(GETARG_C(i)) ? k+INDEXK(GETARG_C(i)) : base+GETARG_C(i))
		//#define KBx(i)	check_exp(getBMode(GET_OPCODE(i)) == OpArgMask.OpArgK, k+GETARG_Bx(i))

		// todo: implement proper checks, as above
		public static TValue RA(lua_State L, TValue/*StkId*/ base_, long/*UInt32*//*Instruction*/ i) 
		{ 
			return TValue.plus(base_, LuaOpCodes.GETARG_A(i)); 
		}
		
		public static TValue RB(lua_State L, TValue/*StkId*/ base_, long/*UInt32*//*Instruction*/ i) 
		{ 
			return TValue.plus(base_, LuaOpCodes.GETARG_B(i)); 
		}
		
		public static TValue RC(lua_State L, TValue/*StkId*/ base_, long/*UInt32*//*Instruction*/ i) 
		{ 
			return TValue.plus(base_, LuaOpCodes.GETARG_C(i)); 
		}
		
		public static TValue RKB(lua_State L, TValue/*StkId*/ base_, long/*UInt32*//*Instruction*/ i, TValue[] k) 
		{ 
			return LuaOpCodes.ISK(LuaOpCodes.GETARG_B(i)) != 0 ? k[LuaOpCodes.INDEXK(LuaOpCodes.GETARG_B(i))] : TValue.plus(base_, LuaOpCodes.GETARG_B(i)); 
		}
		
		public static TValue RKC(lua_State L, TValue/*StkId*/ base_, long/*UInt32*//*Instruction*/ i, TValue[] k) 
		{ 
			return LuaOpCodes.ISK(LuaOpCodes.GETARG_C(i)) != 0 ? k[LuaOpCodes.INDEXK(LuaOpCodes.GETARG_C(i))] : TValue.plus(base_, LuaOpCodes.GETARG_C(i)); 
		}
		
		public static TValue KBx(lua_State L, long/*UInt32*//*Instruction*/ i, TValue[] k) 
		{ 
			return k[LuaOpCodes.GETARG_Bx(i)]; 
		}

		public static void dojump(lua_State L, InstructionPtr pc, int i) 
		{ 
			pc.pc += i; 
			LuaLimits.luai_threadyield(L);
		}

		//#define Protect(x)	{ L.savedpc = pc; {x;}; base = L.base_; }

		public static void arith_op(lua_State L, op_delegate op, TMS tm, TValue/*StkId*/ base_, long/*UInt32*//*Instruction*/ i, TValue[] k, TValue/*StkId*/ ra, InstructionPtr pc)
		{
			TValue rb = RKB(L, base_, i, k);
			TValue rc = RKC(L, base_, i, k);
			if (LuaObject.ttisnumber(rb) && LuaObject.ttisnumber(rc))
			{
				Double/*lua_Number*/ nb = LuaObject.nvalue(rb), nc = LuaObject.nvalue(rc);
				LuaObject.setnvalue(ra, op.exec(nb, nc));
			}
			else
			{
				//Protect(
				L.savedpc = InstructionPtr.Assign(pc);
				Arith(L, ra, rb, rc, tm);
				base_ = L.base_;
				//);
			}
		}

		private static void Dump(int pc, long/*UInt32*//*Instruction*/ i)
		{
			int A = LuaOpCodes.GETARG_A(i);
			int B = LuaOpCodes.GETARG_B(i);
			int C = LuaOpCodes.GETARG_C(i);
			int Bx = LuaOpCodes.GETARG_Bx(i);
			int sBx = LuaOpCodes.GETARG_sBx(i);
			if ((sBx & 0x100) != 0)
			{
				sBx = - (sBx & 0xff);
			}
            StreamProxy.Write("" + pc + " (" + i + "): "); //FIXME:"{0,5} ({1,10}): "
            StreamProxy.Write("" + LuaOpCodes.luaP_opnames[(int)LuaOpCodes.GET_OPCODE(i)].ToString() + "\t"); //"{0,-10}\t"
			switch (LuaOpCodes.GET_OPCODE(i))
			{
				case OpCode.OP_CLOSE:
					{
                        StreamProxy.Write("" + A + "");
						break;
					}
				case OpCode.OP_MOVE:
				case OpCode.OP_LOADNIL:
				case OpCode.OP_GETUPVAL:
				case OpCode.OP_SETUPVAL:
				case OpCode.OP_UNM:
				case OpCode.OP_NOT:
				case OpCode.OP_RETURN:
					{
                        StreamProxy.Write("" + A + ", " + B + "");
						break;
					}
				case OpCode.OP_LOADBOOL:
				case OpCode.OP_GETTABLE:
				case OpCode.OP_SETTABLE:
				case OpCode.OP_NEWTABLE:
				case OpCode.OP_SELF:
				case OpCode.OP_ADD:
				case OpCode.OP_SUB:
				case OpCode.OP_MUL:
				case OpCode.OP_DIV:
				case OpCode.OP_POW:
				case OpCode.OP_CONCAT:
				case OpCode.OP_EQ:
				case OpCode.OP_LT:
				case OpCode.OP_LE:
				case OpCode.OP_TEST:
				case OpCode.OP_CALL:
				case OpCode.OP_TAILCALL:
					{
                        StreamProxy.Write("" + A + ", " + B + ", " + C + "");
						break;
					}
				case OpCode.OP_LOADK:
					{
                        StreamProxy.Write("" + A + ", " + Bx + "");
						break;
					}
				case OpCode.OP_GETGLOBAL:
				case OpCode.OP_SETGLOBAL:
				case OpCode.OP_SETLIST:
				case OpCode.OP_CLOSURE:
					{
                        StreamProxy.Write("" + A + ", " + Bx + "");
						break;
					}
				case OpCode.OP_TFORLOOP:
					{
                        StreamProxy.Write("" + A + ", " + C + "");
						break;
					}
				case OpCode.OP_JMP:
				case OpCode.OP_FORLOOP:
				case OpCode.OP_FORPREP:
					{
                        StreamProxy.Write("" + A + ", " + sBx + "");
						break;
					}
			}
            StreamProxy.WriteLine();

		}

		public static void luaV_execute(lua_State L, int nexeccalls) 
		{
			LClosure cl;
			TValue/*StkId*/ base_;
			TValue[] k;
			/*const*/ InstructionPtr pc;
			//reentry:  /* entry point */
			while (true)
			{
				bool reentry = false;
				
				LuaLimits.lua_assert(LuaState.isLua(L.ci));
				pc = InstructionPtr.Assign(L.savedpc);
				cl = LuaObject.clvalue(L.ci.func).l;
				base_ = L.base_;
				k = cl.p.k;
				/* main loop of interpreter */
				for (;;) 
				{
					InstructionPtr[] pc_ref = new InstructionPtr[1];
					pc_ref[0] = pc;
					InstructionPtr ret = InstructionPtr.inc(/*ref*/ pc_ref);
					pc = pc_ref[0];
					/*const*/ long/*UInt32*//*Instruction*/ i = ret.get(0);
					TValue/*StkId*/ ra;
					if (((L.hookmask & (Lua.LUA_MASKLINE | Lua.LUA_MASKCOUNT)) != 0) &&
					    (((--L.hookcount) == 0) || ((L.hookmask & Lua.LUA_MASKLINE) != 0)))
					{
						traceexec(L, pc);
						if (L.status == Lua.LUA_YIELD)
						{  
							/* did hook yield? */
							L.savedpc = new InstructionPtr(pc.codes, pc.pc - 1);
							return;
						}
						base_ = L.base_;
					}
					/* warning!! several calls may realloc the stack and invalidate `ra' */
					ra = RA(L, base_, i);
					LuaLimits.lua_assert(base_ == L.base_ && L.base_ == L.ci.base_);
					LuaLimits.lua_assert(TValue.lessEqual(base_, L.top) && ((TValue.minus(L.top, L.stack)) <= L.stacksize));
					LuaLimits.lua_assert(L.top == L.ci.top || (LuaDebug.luaG_checkopenop(i) != 0));
					//Dump(pc.pc, i);
					
					bool reentry2 = false;
					
					switch (LuaOpCodes.GET_OPCODE(i))
					{
						case OpCode.OP_MOVE: 
							{
								LuaObject.setobjs2s(L, ra, RB(L, base_, i));
								continue;
							}
						case OpCode.OP_LOADK: 
							{
								LuaObject.setobj2s(L, ra, KBx(L, i, k));
								continue;
							}
						case OpCode.OP_LOADBOOL: 
							{
								LuaObject.setbvalue(ra, LuaOpCodes.GETARG_B(i));
								if (LuaOpCodes.GETARG_C(i) != 0) 
								{
									InstructionPtr[] pc_ref2 = new InstructionPtr[1];
									pc_ref2[0] = pc;
									InstructionPtr.inc(/*ref*/ pc_ref2);  /* skip next instruction (if C) */
									pc = pc_ref2[0];
								}
								continue;
							}
						case OpCode.OP_LOADNIL: 
							{
								TValue rb = RB(L, base_, i);
								do 
								{
									TValue[] rb_ref = new TValue[1];
									rb_ref[0] = rb;
									TValue ret2 = /*StkId*/TValue.dec(/*ref*/ rb_ref);
									rb = rb_ref[0];
									LuaObject.setnilvalue(ret2);
								} while (TValue.greaterEqual(rb, ra));
								continue;
							}
						case OpCode.OP_GETUPVAL: 
							{
								int b = LuaOpCodes.GETARG_B(i);
								LuaObject.setobj2s(L, ra, cl.upvals[b].v);
								continue;
							}
						case OpCode.OP_GETGLOBAL: 
							{
								TValue g = new TValue();
								TValue rb = KBx(L, i, k);
								LuaObject.sethvalue(L, g, cl.getEnv());
								LuaLimits.lua_assert(LuaObject.ttisstring(rb));
								//Protect(
								L.savedpc = InstructionPtr.Assign(pc);
								luaV_gettable(L, g, rb, ra);
								base_ = L.base_;
								//);
								L.savedpc = InstructionPtr.Assign(pc);
								continue;
							}
						case OpCode.OP_GETTABLE: 
							{
								//Protect(
								L.savedpc = InstructionPtr.Assign(pc);
								luaV_gettable(L, RB(L, base_, i), RKC(L, base_, i, k), ra);
								base_ = L.base_;
								//);
								L.savedpc = InstructionPtr.Assign(pc);
								continue;
							}
						case OpCode.OP_SETGLOBAL: 
							{
								TValue g = new TValue();
								LuaObject.sethvalue(L, g, cl.getEnv());
								LuaLimits.lua_assert(LuaObject.ttisstring(KBx(L, i, k)));
								//Protect(
								L.savedpc = InstructionPtr.Assign(pc);
								luaV_settable(L, g, KBx(L, i, k), ra);
								base_ = L.base_;
								//);
								L.savedpc = InstructionPtr.Assign(pc);
								continue;
							}
						case OpCode.OP_SETUPVAL: 
							{
								UpVal uv = cl.upvals[LuaOpCodes.GETARG_B(i)];
								LuaObject.setobj(L, uv.v, ra);
								LuaGC.luaC_barrier(L, uv, ra);
								continue;
							}
						case OpCode.OP_SETTABLE: 
							{
								//Protect(
								L.savedpc = InstructionPtr.Assign(pc);
								luaV_settable(L, ra, RKB(L, base_, i, k), RKC(L, base_, i, k));
								base_ = L.base_;
								//);
								L.savedpc = InstructionPtr.Assign(pc);
								continue;
							}
						case OpCode.OP_NEWTABLE: 
							{
								int b = LuaOpCodes.GETARG_B(i);
								int c = LuaOpCodes.GETARG_C(i);
								LuaObject.sethvalue(L, ra, LuaTable.luaH_new(L, LuaObject.luaO_fb2int(b), LuaObject.luaO_fb2int(c)));
								//Protect(
								L.savedpc = InstructionPtr.Assign(pc);
								LuaGC.luaC_checkGC(L);
								base_ = L.base_;
								//);
								L.savedpc = InstructionPtr.Assign(pc);
								continue;
							}
						case OpCode.OP_SELF: 
							{
								/*StkId*/TValue rb = RB(L, base_, i);
								LuaObject.setobjs2s(L, TValue.plus(ra, 1), rb);
								//Protect(
								L.savedpc = InstructionPtr.Assign(pc);
								luaV_gettable(L, rb, RKC(L, base_, i, k), ra);
								base_ = L.base_;
								//);
								L.savedpc = InstructionPtr.Assign(pc);
								continue;
							}
						case OpCode.OP_ADD: 
							{
								arith_op(L, new LuaConf.luai_numadd_delegate(), TMS.TM_ADD, base_, i, k, ra, pc);
								continue;
							}
						case OpCode.OP_SUB: 
							{
								arith_op(L, new LuaConf.luai_numsub_delegate(), TMS.TM_SUB, base_, i, k, ra, pc);
								continue;
							}
						case OpCode.OP_MUL: 
							{
								arith_op(L, new LuaConf.luai_nummul_delegate(), TMS.TM_MUL, base_, i, k, ra, pc);
								continue;
							}
						case OpCode.OP_DIV: 
							{
								arith_op(L, new LuaConf.luai_numdiv_delegate(), TMS.TM_DIV, base_, i, k, ra, pc);
								continue;
							}
						case OpCode.OP_MOD: 
							{
								arith_op(L, new LuaConf.luai_nummod_delegate(), TMS.TM_MOD, base_, i, k, ra, pc);
								continue;
							}
						case OpCode.OP_POW: 
							{
								arith_op(L, new LuaConf.luai_numpow_delegate(), TMS.TM_POW, base_, i, k, ra, pc);
								continue;
							}
						case OpCode.OP_UNM: 
							{
								TValue rb = RB(L, base_, i);
								if (LuaObject.ttisnumber(rb))
								{
									Double/*lua_Number*/ nb = LuaObject.nvalue(rb);
									LuaObject.setnvalue(ra, LuaConf.luai_numunm(nb));
								}
								else 
								{
									//Protect(
									L.savedpc = InstructionPtr.Assign(pc);
									Arith(L, ra, rb, rb, TMS.TM_UNM);
									base_ = L.base_;
									//);
									L.savedpc = InstructionPtr.Assign(pc);
								}
								continue;
							}
						case OpCode.OP_NOT: 
							{
								int res = LuaObject.l_isfalse(RB(L, base_, i)) == 0 ? 0 : 1;  /* next assignment may change this value */
								LuaObject.setbvalue(ra, res);
								continue;
							}
						case OpCode.OP_LEN: 
							{
								TValue rb = RB(L, base_, i);
								switch (LuaObject.ttype(rb))
								{
									case Lua.LUA_TTABLE:
										{
											LuaObject.setnvalue(ra, (Double/*lua_Number*/)LuaTable.luaH_getn(LuaObject.hvalue(rb)));
											break;
										}
									case Lua.LUA_TSTRING:
										{
											LuaObject.setnvalue(ra, (Double/*lua_Number*/)LuaObject.tsvalue(rb).len);
											break;
										}
									default: 
										{  
											/* try metamethod */
											//Protect(
											L.savedpc = InstructionPtr.Assign(pc);
											if (call_binTM(L, rb, LuaObject.luaO_nilobject, ra, TMS.TM_LEN) == 0)
											{
												LuaDebug.luaG_typeerror(L, rb, CharPtr.toCharPtr("get length of"));
											}
											base_ = L.base_;
											//)
											break;
										}
								}
								continue;
							}
						case OpCode.OP_CONCAT: 
							{
								int b = LuaOpCodes.GETARG_B(i);
								int c = LuaOpCodes.GETARG_C(i);
								//Protect(
								L.savedpc = InstructionPtr.Assign(pc);
								luaV_concat(L, c - b + 1, c); LuaGC.luaC_checkGC(L);
								base_ = L.base_;
								//);
								LuaObject.setobjs2s(L, RA(L, base_, i), TValue.plus(base_, b));
								continue;
							}
						case OpCode.OP_JMP: 
							{
								dojump(L, pc, LuaOpCodes.GETARG_sBx(i));
								continue;
							}
						case OpCode.OP_EQ: 
							{
								TValue rb = RKB(L, base_, i, k);
								TValue rc = RKC(L, base_, i, k);
								//Protect(
								L.savedpc = InstructionPtr.Assign(pc);
								if (equalobj(L, rb, rc) == LuaOpCodes.GETARG_A(i))
									dojump(L, pc, LuaOpCodes.GETARG_sBx(pc.get(0)));
								base_ = L.base_;
								//);
								InstructionPtr[] pc_ref2 = new InstructionPtr[1];
								pc_ref2[0] = pc;
								InstructionPtr.inc(/*ref*/ pc_ref2);
								pc = pc_ref2[0];
								continue;
							}
						case OpCode.OP_LT: 
							{
								//Protect(
								L.savedpc = InstructionPtr.Assign(pc);
								if (luaV_lessthan(L, RKB(L, base_, i, k), RKC(L, base_, i, k)) == LuaOpCodes.GETARG_A(i))
								{
									dojump(L, pc, LuaOpCodes.GETARG_sBx(pc.get(0)));
								}
								base_ = L.base_;
								//);
								InstructionPtr[] pc_ref3 = new InstructionPtr[1];
								pc_ref3[0] = pc;
								InstructionPtr.inc(/*ref*/ pc_ref3);
								pc = pc_ref3[0];
								continue;
							}
						case OpCode.OP_LE: 
							{
								//Protect(
								L.savedpc = InstructionPtr.Assign(pc);
								if (lessequal(L, RKB(L, base_, i, k), RKC(L, base_, i, k)) == LuaOpCodes.GETARG_A(i))
								{
									dojump(L, pc, LuaOpCodes.GETARG_sBx(pc.get(0)));
								}
								base_ = L.base_;
								//);
								InstructionPtr[] pc_ref4 = new InstructionPtr[1];
								pc_ref4[0] = pc;
								InstructionPtr.inc(/*ref*/ pc_ref4);
								pc = pc_ref4[0];
								continue;
							}
						case OpCode.OP_TEST: 
							{
								if (LuaObject.l_isfalse(ra) != LuaOpCodes.GETARG_C(i))
								{
									dojump(L, pc, LuaOpCodes.GETARG_sBx(pc.get(0)));
								}
								InstructionPtr[] pc_ref5 = new InstructionPtr[1];
								pc_ref5[0] = pc;
								InstructionPtr.inc(/*ref*/ pc_ref5);
								pc = pc_ref5[0];
								continue;
							}
						case OpCode.OP_TESTSET: 
							{
								TValue rb = RB(L, base_, i);
								if (LuaObject.l_isfalse(rb) != LuaOpCodes.GETARG_C(i))
								{
									LuaObject.setobjs2s(L, ra, rb);
									dojump(L, pc, LuaOpCodes.GETARG_sBx(pc.get(0)));
								}
								InstructionPtr[] pc_ref6 = new InstructionPtr[1];
								pc_ref6[0] = pc;
								InstructionPtr.inc(/*ref*/ pc_ref6);
								pc = pc_ref6[0];
								continue;
							}
						case OpCode.OP_CALL: 
							{
								int b = LuaOpCodes.GETARG_B(i);
								int nresults = LuaOpCodes.GETARG_C(i) - 1;
								if (b != 0) 
								{
									L.top = TValue.plus(ra, b);  /* else previous instruction set top */
								}
								L.savedpc = InstructionPtr.Assign(pc);
								bool reentry3 = false;
								switch (LuaDo.luaD_precall(L, ra, nresults))
								{
									case LuaDo.PCRLUA:
										{
											nexeccalls++;
											//goto reentry;  /* restart luaV_execute over new Lua function */
											reentry3 = true;
											break;
										}
									case LuaDo.PCRC:
										{
											/* it was a C function (`precall' called it); adjust results */
											if (nresults >= 0) 
											{
												L.top = L.ci.top;
											}
											base_ = L.base_;
											continue;
										}
									default: 
										{
											return;  /* yield */
										}
								}
								if (reentry3)
								{
									reentry2 = true;
									break;
								}
								else
								{
									break;
								}
							}
						case OpCode.OP_TAILCALL: 
							{
								int b = LuaOpCodes.GETARG_B(i);
								if (b != 0) 
								{
									L.top = TValue.plus(ra, b);  /* else previous instruction set top */
								}
								L.savedpc = InstructionPtr.Assign(pc);
								LuaLimits.lua_assert(LuaOpCodes.GETARG_C(i) - 1 == Lua.LUA_MULTRET);
								bool reentry4 = false;
								switch (LuaDo.luaD_precall(L, ra, Lua.LUA_MULTRET))
								{
									case LuaDo.PCRLUA:
										{
											/* tail call: put new frame in place of previous one */
											CallInfo ci = CallInfo.minus(L.ci, 1);  /* previous frame */
											int aux;
											TValue/*StkId*/ func = ci.func;
											TValue/*StkId*/ pfunc = CallInfo.plus(ci, 1).func;  /* previous function index */
											if (L.openupval != null) 
											{
												LuaFunc.luaF_close(L, ci.base_);
											}
											L.base_ = ci.base_ = TValue.plus(ci.func, TValue.minus(ci.get(1).base_, pfunc));
											for (aux = 0; TValue.lessThan(TValue.plus(pfunc, aux), L.top); aux++)  /* move frame down */
											{
												LuaObject.setobjs2s(L, TValue.plus(func, aux), TValue.plus(pfunc, aux));
											}
											ci.top = L.top = TValue.plus(func, aux);  /* correct top */
											LuaLimits.lua_assert(L.top == TValue.plus(L.base_, LuaObject.clvalue(func).l.p.maxstacksize));
											ci.savedpc = InstructionPtr.Assign(L.savedpc);
											ci.tailcalls++;  /* one more call lost */
											CallInfo[] ci_ref3 = new CallInfo[1];
											ci_ref3[0] = L.ci;
											CallInfo.dec(/*ref*/ ci_ref3);  /* remove new frame */
											L.ci = ci_ref3[0];
											//goto reentry;
											reentry4 = true;
											break;
										}
									case LuaDo.PCRC:
										{  
											/* it was a C function (`precall' called it) */
											base_ = L.base_;
											continue;
										}
									default: 
										{
											return;  /* yield */
										}
								}
								if (reentry4)
								{
									reentry2 = true;
									break;
								}
								else
								{
									break;
								}
							}
						case OpCode.OP_RETURN: 
							{
								int b = LuaOpCodes.GETARG_B(i);
								if (b != 0) 
								{
									L.top = TValue.plus(ra, b - 1); //FIXME:
								}
								if (L.openupval != null) 
								{
									LuaFunc.luaF_close(L, base_);
								}
								L.savedpc = InstructionPtr.Assign(pc);
								b = LuaDo.luaD_poscall(L, ra);
								if (--nexeccalls == 0)  /* was previous function running `here'? */
								{
									return;  /* no: return */
								}
								else 
								{  
									/* yes: continue its execution */
									if (b != 0) 
									{
										L.top = L.ci.top;
									}
									LuaLimits.lua_assert(LuaState.isLua(L.ci));
									LuaLimits.lua_assert(LuaOpCodes.GET_OPCODE(L.ci.savedpc.get(-1)) == OpCode.OP_CALL);
									//goto reentry;
									reentry2 = true;
									break;
								}
							}
						case OpCode.OP_FORLOOP: 
							{
								Double/*lua_Number*/ step = LuaObject.nvalue(TValue.plus(ra, 2));
								Double/*lua_Number*/ idx = LuaConf.luai_numadd(LuaObject.nvalue(ra), step); /* increment index */
								Double/*lua_Number*/ limit = LuaObject.nvalue(TValue.plus(ra, 1));
								if (LuaConf.luai_numlt(0, step) ? LuaConf.luai_numle(idx, limit)
								    : LuaConf.luai_numle(limit, idx))
								{
									dojump(L, pc, LuaOpCodes.GETARG_sBx(i));  /* jump back */
									LuaObject.setnvalue(ra, idx);  /* update internal index... */
									LuaObject.setnvalue(TValue.plus(ra, 3), idx);  /* ...and external index */
								}
								continue;
							}
						case OpCode.OP_FORPREP: 
							{
								TValue init = ra;
								TValue plimit = TValue.plus(ra, 1);
								TValue pstep = TValue.plus(ra, 2);
								L.savedpc = InstructionPtr.Assign(pc);  /* next steps may throw errors */
								int retxxx;
								TValue[] init_ref = new TValue[1];
								init_ref[0] = init;
								retxxx = tonumber(/*ref*/ init_ref, ra);
								init = init_ref[0];
								if (retxxx == 0)
								{
									LuaDebug.luaG_runerror(L, CharPtr.toCharPtr(LuaConf.LUA_QL("for") + " initial value must be a number"));
								} 
								else 
								{
									TValue[] plimit_ref = new TValue[1];
									plimit_ref[0] = plimit;
									retxxx = tonumber(/*ref*/ plimit_ref, TValue.plus(ra, 1));
									plimit = plimit_ref[0];
									if (retxxx == 0)
									{
		                                LuaDebug.luaG_runerror(L, CharPtr.toCharPtr(LuaConf.LUA_QL("for") + " limit must be a number"));
									}
									else
									{
										TValue[] pstep_ref = new TValue[1];
										pstep_ref[0] = pstep;
										retxxx = tonumber(/*ref*/ pstep_ref, TValue.plus(ra, 2));
										pstep = pstep_ref[0];									
										if (retxxx == 0)
										{
											LuaDebug.luaG_runerror(L, CharPtr.toCharPtr(LuaConf.LUA_QL("for") + " step must be a number"));
										}
									}
								}
								LuaObject.setnvalue(ra, LuaConf.luai_numsub(LuaObject.nvalue(ra), LuaObject.nvalue(pstep)));
								dojump(L, pc, LuaOpCodes.GETARG_sBx(i));
								continue;
							}
						case OpCode.OP_TFORLOOP: 
							{
								TValue/*StkId*/ cb = TValue.plus(ra, 3);  /* call base */
								LuaObject.setobjs2s(L, TValue.plus(cb, 2), TValue.plus(ra, 2));
								LuaObject.setobjs2s(L, TValue.plus(cb, 1), TValue.plus(ra, 1));
								LuaObject.setobjs2s(L, cb, ra);
								L.top = TValue.plus(cb, 3);  /* func. + 2 args (state and index) */
								//Protect(
								L.savedpc = InstructionPtr.Assign(pc);
								LuaDo.luaD_call(L, cb, LuaOpCodes.GETARG_C(i));
								base_ = L.base_;
								//);
								L.top = L.ci.top;
								cb = TValue.plus(RA(L, base_, i), 3);  /* previous call may change the stack */
								if (!LuaObject.ttisnil(cb))
								{  
									/* continue loop? */
									LuaObject.setobjs2s(L, TValue.minus(cb, 1), cb);  /* save control variable */
									dojump(L, pc, LuaOpCodes.GETARG_sBx(pc.get(0)));  /* jump back */
								}
								InstructionPtr[] pc_ref3 = new InstructionPtr[1];
								pc_ref3[0] = pc;
								InstructionPtr.inc(/*ref*/ pc_ref3);
								pc = pc_ref3[0];
								continue;
							}
						case OpCode.OP_SETLIST: 
							{
								int n = LuaOpCodes.GETARG_B(i);
								int c = LuaOpCodes.GETARG_C(i);
								int last;
								Table h;
								if (n == 0) 
								{
									n = LuaLimits.cast_int(TValue.minus(L.top, ra)) - 1;
									L.top = L.ci.top;
								}
								if (c == 0)
								{
									c = LuaLimits.cast_int_instruction(pc.get(0));
									InstructionPtr[] pc_ref5 = new InstructionPtr[1];
									pc_ref5[0] = pc;
									InstructionPtr.inc(/*ref*/ pc_ref5);
									pc = pc_ref5[0];
								}
								runtime_check(L, LuaObject.ttistable(ra));
								h = LuaObject.hvalue(ra);
								last = ((c - 1) * LuaOpCodes.LFIELDS_PER_FLUSH) + n;
								if (last > h.sizearray)  /* needs more space? */
								{
									LuaTable.luaH_resizearray(L, h, last);  /* pre-alloc it at once */
								}
								for (; n > 0; n--) 
								{
									TValue val = TValue.plus(ra, n);
									LuaObject.setobj2t(L, LuaTable.luaH_setnum(L, h, last--), val);
									LuaGC.luaC_barriert(L, h, val);
								}
								continue;
							}
						case OpCode.OP_CLOSE: 
							{
								LuaFunc.luaF_close(L, ra);
								continue;
							}
						case OpCode.OP_CLOSURE: 
							{
								Proto p;
								Closure ncl;
								int nup, j;
								p = cl.p.p[LuaOpCodes.GETARG_Bx(i)];
								nup = p.nups;
								ncl = LuaFunc.luaF_newLclosure(L, nup, cl.getEnv());
								ncl.l.p = p;
								for (j = 0; j < nup;) 
								{
									if (LuaOpCodes.GET_OPCODE(pc.get(0)) == OpCode.OP_GETUPVAL)
									{
										ncl.l.upvals[j] = cl.upvals[LuaOpCodes.GETARG_B(pc.get(0))];
									}
									else 
									{
										LuaLimits.lua_assert(LuaOpCodes.GET_OPCODE(pc.get(0)) == OpCode.OP_MOVE);
										ncl.l.upvals[j] = LuaFunc.luaF_findupval(L, TValue.plus(base_, LuaOpCodes.GETARG_B(pc.get(0))));
									}
									
									j++; 
									InstructionPtr[] pc_ref4 = new InstructionPtr[1];
									pc_ref4[0] = pc;
									InstructionPtr.inc(/*ref*/ pc_ref4);
									pc = pc_ref4[0];
								}
								LuaObject.setclvalue(L, ra, ncl);
								//Protect(
								L.savedpc = InstructionPtr.Assign(pc);
								LuaGC.luaC_checkGC(L);
								base_ = L.base_;
								//);
								continue;
							}
						case OpCode.OP_VARARG: 
							{
								int b = LuaOpCodes.GETARG_B(i) - 1;
								int j;
								CallInfo ci = L.ci;
								int n = LuaLimits.cast_int(TValue.minus(ci.base_, ci.func)) - cl.p.numparams - 1;
								if (b == Lua.LUA_MULTRET)
								{
									//Protect(
									L.savedpc = InstructionPtr.Assign(pc);
									LuaDo.luaD_checkstack(L, n);
									base_ = L.base_;
									//);
									ra = RA(L, base_, i);  /* previous call may change the stack */
									b = n;
									L.top = TValue.plus(ra, n);
								}
								for (j = 0; j < b; j++) 
								{
									if (j < n) 
									{
										LuaObject.setobjs2s(L, TValue.plus(ra, j), TValue.plus(TValue.minus(ci.base_, n), j)); //FIXME:
									}
									else 
									{
										LuaObject.setnilvalue(TValue.plus(ra, j));
									}
								}
								continue;
							}
					} //end switch
					if (reentry2 == true)
					{
						reentry = true;
						break;
					}
				}//end for
				if (reentry == true)
				{
					continue;
				}
				else
				{
					break;
				}
			}//end while
		}

	}
}
