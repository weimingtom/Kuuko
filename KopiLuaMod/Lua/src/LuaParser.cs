/*
 ** $Id: lparser.c,v 2.42.1.3 2007/12/28 15:32:23 roberto Exp $
 ** Lua Parser
 ** See Copyright Notice in lua.h
 */
using System;

namespace KopiLua
{
	//using lu_byte = System.Byte;
	//using lua_Number = System.Double;

	public class LuaParser
	{
		public static int hasmultret(expkind k)		
		{
			return ((k) == expkind.VCALL || (k) == expkind.VVARARG) ? 1 : 0;
		}

		public static LocVar getlocvar(FuncState fs, int i)	
		{
			return fs.f.locvars[fs.actvar[i]];
		}

		public static void luaY_checklimit(FuncState fs, int v, int l, CharPtr m) 
		{ 
			if ((v) > (l)) 
			{
				errorlimit(fs, l, m);
			}
		}

		private static void anchor_token(LexState ls) 
		{
			if (ls.t.token == (int)RESERVED.TK_NAME || ls.t.token == (int)RESERVED.TK_STRING) 
			{
				TString ts = ls.t.seminfo.ts;
				LuaLex.luaX_newstring(ls, LuaObject.getstr(ts), ts.getTsv().len);
			}
		}

		private static void error_expected(LexState ls, int token) 
		{
			LuaLex.luaX_syntaxerror(ls,
				LuaObject.luaO_pushfstring(ls.L, CharPtr.toCharPtr(LuaConf.getLUA_QS() + " expected"), LuaLex.luaX_token2str(ls, token)));
		}

		private static void errorlimit(FuncState fs, int limit, CharPtr what) 
		{
			CharPtr msg = (fs.f.linedefined == 0) ?
				LuaObject.luaO_pushfstring(fs.L, CharPtr.toCharPtr("main function has more than %d %s"), limit, what) :
				LuaObject.luaO_pushfstring(fs.L, CharPtr.toCharPtr("function at line %d has more than %d %s"),
					fs.f.linedefined, limit, what);
			LuaLex.luaX_lexerror(fs.ls, msg, 0);
		}

		private static int testnext(LexState ls, int c) 
		{
			if (ls.t.token == c) 
			{
				LuaLex.luaX_next(ls);
				return 1;
			}
			else 
			{
				return 0;
			}
		}

		private static void check(LexState ls, int c) 
		{
			if (ls.t.token != c)
			{
				error_expected(ls, c);
			}
		}

		private static void checknext(LexState ls, int c) 
		{
			check(ls, c);
			LuaLex.luaX_next(ls);
		}

		public static void check_condition(LexState ls, bool c, CharPtr msg)
		{
			if (!(c)) 
			{
				LuaLex.luaX_syntaxerror(ls, msg);
			}
		}

		private static void check_match(LexState ls, int what, int who, int where) 
		{
			if (testnext(ls, what) == 0) 
			{
				if (where == ls.linenumber)
				{
					error_expected(ls, what);
				}
				else 
				{
					LuaLex.luaX_syntaxerror(ls, LuaObject.luaO_pushfstring(ls.L,
						CharPtr.toCharPtr(LuaConf.getLUA_QS() + " expected (to close " + LuaConf.getLUA_QS() + " at line %d)"),
					    LuaLex.luaX_token2str(ls, what), 
					    LuaLex.luaX_token2str(ls, who), 
					    where));
				}
			}
		}

		private static TString str_checkname(LexState ls) 
		{
			TString ts;
			check(ls, (int)RESERVED.TK_NAME);
			ts = ls.t.seminfo.ts;
			LuaLex.luaX_next(ls);
			return ts;
		}

		private static void init_exp(expdesc e, expkind k, int i) 
		{
			e.f = e.t = LuaCode.NO_JUMP;
			e.k = k;
			e.u.s.info = i;
		}

		private static void codestring(LexState ls, expdesc e, TString s) 
		{
			init_exp(e, expkind.VK, LuaCode.luaK_stringK(ls.fs, s));
		}

		private static void checkname(LexState ls, expdesc e) 
		{
			codestring(ls, e, str_checkname(ls));
		}

		private static int registerlocalvar (LexState ls, TString varname)
		{
			FuncState fs = ls.fs;
			Proto f = fs.f;
			int oldsize = f.sizelocvars;
			LocVar[][] locvars_ref = new LocVar[1][];
			locvars_ref[0] = f.locvars;
			int[] sizelocvars_ref = new int[1];
			sizelocvars_ref[0] = f.sizelocvars;
			LuaMem.luaM_growvector_LocVar(ls.L, /*ref*/ locvars_ref, fs.nlocvars,
			    /*ref*/ sizelocvars_ref, (int)LuaConf.SHRT_MAX, 
				CharPtr.toCharPtr("too many local variables"), new ClassType(ClassType.TYPE_LOCVAR));
			f.sizelocvars = sizelocvars_ref[0];
			f.locvars = locvars_ref[0];
			while (oldsize < f.sizelocvars) 
			{
				f.locvars[oldsize++].varname = null;
			}
			f.locvars[fs.nlocvars].varname = varname;
			LuaGC.luaC_objbarrier(ls.L, f, varname);
			return fs.nlocvars++;
		}

		public static void new_localvarliteral(LexState ls, CharPtr v, int n) 
		{
			new_localvar(ls, LuaLex.luaX_newstring(ls, CharPtr.toCharPtr("" + v), /*(uint)*/(v.chars.Length - 1)), n);
		}

		private static void new_localvar(LexState ls, TString name, int n) 
		{
			FuncState fs = ls.fs;
			luaY_checklimit(fs, fs.nactvar + n + 1, LuaConf.LUAI_MAXVARS, CharPtr.toCharPtr("local variables"));
			fs.actvar[fs.nactvar + n] = (int/*ushort*/)registerlocalvar(ls, name);
		}

		private static void adjustlocalvars(LexState ls, int nvars) 
		{
			FuncState fs = ls.fs;
			fs.nactvar = LuaLimits.cast_byte(fs.nactvar + nvars);
			for (; nvars != 0; nvars--) 
			{
				getlocvar(fs, fs.nactvar - nvars).startpc = fs.pc;
			}
		}

		private static void removevars(LexState ls, int tolevel) 
		{
			FuncState fs = ls.fs;
			while (fs.nactvar > tolevel)
			{
				getlocvar(fs, --fs.nactvar).endpc = fs.pc;
			}
		}

		private static int indexupvalue(FuncState fs, TString name, expdesc v) 
		{
			int i;
			Proto f = fs.f;
			int oldsize = f.sizeupvalues;
			for (i=0; i<f.nups; i++) 
			{
				if ((int)fs.upvalues[i].k == (int)expkindUtil.expkindToInt(v.k) && fs.upvalues[i].info == v.u.s.info) 
				{
					LuaLimits.lua_assert(f.upvalues[i] == name);
					return i;
				}
			}
			/* new one */
			luaY_checklimit(fs, f.nups + 1, LuaConf.LUAI_MAXUPVALUES, CharPtr.toCharPtr("upvalues"));
			TString[][] upvalues_ref = new TString[1][];
			upvalues_ref[0] = f.upvalues;
			int[] sizeupvalues_ref = new int[1];
			sizeupvalues_ref[0] = f.sizeupvalues;
			LuaMem.luaM_growvector_TString(fs.L, /*ref*/ upvalues_ref, f.nups, /*ref*/ sizeupvalues_ref, LuaLimits.MAX_INT, CharPtr.toCharPtr(""), new ClassType(ClassType.TYPE_TSTRING));
			f.sizeupvalues = sizeupvalues_ref[0];
			f.upvalues = upvalues_ref[0];
			while (oldsize < f.sizeupvalues)
			{
				f.upvalues[oldsize++] = null;
			}
			f.upvalues[f.nups] = name;
			LuaGC.luaC_objbarrier(fs.L, f, name);
			LuaLimits.lua_assert(v.k == expkind.VLOCAL || v.k == expkind.VUPVAL);
            fs.upvalues[f.nups].k = LuaLimits.cast_byte(expkindUtil.expkindToInt(v.k));
			fs.upvalues[f.nups].info = LuaLimits.cast_byte(v.u.s.info);
			return f.nups++;
		}

		private static int searchvar(FuncState fs, TString n) 
		{
			int i;
			for (i = fs.nactvar - 1; i >= 0; i--) 
			{
				if (n == getlocvar(fs, i).varname)
				{
					return i;
				}
			}
			return -1;  /* not found */
		}

		private static void markupval(FuncState fs, int level) 
		{
			BlockCnt bl = fs.bl;
			while ((bl!=null) && bl.nactvar > level) 
			{
				bl = bl.previous;
			}
			if (bl != null) 
			{
				bl.upval = 1;
			}
		}

		private static expkind singlevaraux(FuncState fs, TString n, expdesc var, int base_)
		{
			if (fs == null) 
			{  
				/* no more levels? */
				init_exp(var, expkind.VGLOBAL, LuaOpCodes.NO_REG);  /* default is global variable */
				return expkind.VGLOBAL;
			}
			else 
			{
				int v = searchvar(fs, n);  /* look up at current level */
				if (v >= 0) 
				{
					init_exp(var, expkind.VLOCAL, v);
					if (base_ == 0)
					{
						markupval(fs, v);  /* local will be used as an upval */
					}
					return expkind.VLOCAL;
				}
				else 
				{  
					/* not found at current level; try upper one */
					if (singlevaraux(fs.prev, n, var, 0) == expkind.VGLOBAL)
					{
						return expkind.VGLOBAL;
					}
					var.u.s.info = indexupvalue(fs, n, var);  /* else was LOCAL or UPVAL */
					var.k = expkind.VUPVAL;  /* upvalue in this level */
					return expkind.VUPVAL;
				}
			}
		}

		private static void singlevar(LexState ls, expdesc var) 
		{
			TString varname = str_checkname(ls);
			FuncState fs = ls.fs;
			if (singlevaraux(fs, varname, var, 1) == expkind.VGLOBAL)
			{
				var.u.s.info = LuaCode.luaK_stringK(fs, varname);  /* info points to global name */
			}
		}

		private static void adjust_assign(LexState ls, int nvars, int nexps, expdesc e) 
		{
			FuncState fs = ls.fs;
			int extra = nvars - nexps;
			if (hasmultret(e.k) != 0) 
			{
				extra++;  /* includes call itself */
				if (extra < 0) 
				{
					extra = 0;
				}
				LuaCode.luaK_setreturns(fs, e, extra);  /* last exp. provides the difference */
				if (extra > 1) 
				{
					LuaCode.luaK_reserveregs(fs, extra - 1);
				}
			}
			else 
			{
				if (e.k != expkind.VVOID) 
				{
					LuaCode.luaK_exp2nextreg(fs, e);  /* close last expression */
				}
				if (extra > 0) 
				{
					int reg = fs.freereg;
					LuaCode.luaK_reserveregs(fs, extra);
					LuaCode.luaK_nil(fs, reg, extra);
				}
			}
		}

		private static void enterlevel (LexState ls) 
		{
			if (++ls.L.nCcalls > LuaConf.LUAI_MAXCCALLS)
			{
				LuaLex.luaX_lexerror(ls, CharPtr.toCharPtr("chunk has too many syntax levels"), 0);
			}
		}

		private static void leavelevel(LexState ls) 
		{ 
			ls.L.nCcalls--; 
		}

		private static void enterblock(FuncState fs, BlockCnt bl, Byte/*lu_byte*/ isbreakable)
		{
			bl.breaklist = LuaCode.NO_JUMP;
			bl.isbreakable = isbreakable;
			bl.nactvar = fs.nactvar;
			bl.upval = 0;
			bl.previous = fs.bl;
			fs.bl = bl;
			LuaLimits.lua_assert(fs.freereg == fs.nactvar);
		}

		private static void leaveblock(FuncState fs) 
		{
			BlockCnt bl = fs.bl;
			fs.bl = bl.previous;
			removevars(fs.ls, bl.nactvar);
			if (bl.upval != 0)
			{
				LuaCode.luaK_codeABC(fs, OpCode.OP_CLOSE, bl.nactvar, 0, 0);
			}
			/* a block either controls scope or breaks (never both) */
			LuaLimits.lua_assert((bl.isbreakable == 0) || (bl.upval == 0));
			LuaLimits.lua_assert(bl.nactvar == fs.nactvar);
			fs.freereg = fs.nactvar;  /* free registers */
			LuaCode.luaK_patchtohere(fs, bl.breaklist);
		}

		private static void pushclosure(LexState ls, FuncState func, expdesc v) 
		{
			FuncState fs = ls.fs;
			Proto f = fs.f;
			int oldsize = f.sizep;
			int i;
			Proto[][] p_ref = new Proto[1][];
			p_ref[0] = f.p;
			int[] sizep_ref = new int[1];
			sizep_ref[0] = f.sizep;
			LuaMem.luaM_growvector_Proto(ls.L, /*ref*/ p_ref, fs.np, /*ref*/ sizep_ref,
				LuaOpCodes.MAXARG_Bx, CharPtr.toCharPtr("constant table overflow"), new ClassType(ClassType.TYPE_PROTO));
			f.sizep = sizep_ref[0];
			f.p = p_ref[0];
			while (oldsize < f.sizep)
			{
				f.p[oldsize++] = null;
			}
			f.p[fs.np++] = func.f;
			LuaGC.luaC_objbarrier(ls.L, f, func.f);
			init_exp(v, expkind.VRELOCABLE, LuaCode.luaK_codeABx(fs, OpCode.OP_CLOSURE, 0, fs.np - 1));
			for (i = 0; i < func.f.nups; i++) 
			{
				OpCode o = ((int)func.upvalues[i].k == (int)expkind.VLOCAL) ? OpCode.OP_MOVE : OpCode.OP_GETUPVAL;
				LuaCode.luaK_codeABC(fs, o, 0, func.upvalues[i].info, 0);
			}
		}

		private static void open_func(LexState ls, FuncState fs) 
		{
			lua_State L = ls.L;
			Proto f = LuaFunc.luaF_newproto(L);
			fs.f = f;
			fs.prev = ls.fs;  /* linked list of funcstates */
			fs.ls = ls;
			fs.L = L;
			ls.fs = fs;
			fs.pc = 0;
			fs.lasttarget = -1;
			fs.jpc = LuaCode.NO_JUMP;
			fs.freereg = 0;
			fs.nk = 0;
			fs.np = 0;
			fs.nlocvars = 0;
			fs.nactvar = 0;
			fs.bl = null;
			f.source = ls.source;
			f.maxstacksize = 2;  /* registers 0/1 are always valid */
			fs.h = LuaTable.luaH_new(L, 0, 0);
			/* anchor table of constants and prototype (to avoid being collected) */
			LuaObject.sethvalue2s(L, L.top, fs.h);
			LuaDo.incr_top(L);
			LuaObject.setptvalue2s(L, L.top, f);
			LuaDo.incr_top(L);
		}

		static Proto lastfunc;

		private static void close_func(LexState ls) 
		{
			lua_State L = ls.L;
			FuncState fs = ls.fs;
			Proto f = fs.f;
			lastfunc = f;
			removevars(ls, 0);
			LuaCode.luaK_ret(fs, 0, 0);  /* final return */
			long[][] code_ref = new long[1][];
			code_ref[0] = f.code;
			LuaMem.luaM_reallocvector_long(L, /*ref*/ code_ref, f.sizecode, fs.pc/*, typeof(Instruction)*/, new ClassType(ClassType.TYPE_LONG));
			f.code = code_ref[0];
			f.sizecode = fs.pc;
			int[][] lineinfo_ref = new int[1][];
			lineinfo_ref[0] = f.lineinfo;
			LuaMem.luaM_reallocvector_int(L, /*ref*/ lineinfo_ref, f.sizelineinfo, fs.pc/*, typeof(int)*/, new ClassType(ClassType.TYPE_INT));
			f.lineinfo = lineinfo_ref[0];
			f.sizelineinfo = fs.pc;
			TValue[][] k_ref = new TValue[1][];
			k_ref[0] = f.k;
			LuaMem.luaM_reallocvector_TValue(L, /*ref*/ k_ref, f.sizek, fs.nk/*, TValue*/, new ClassType(ClassType.TYPE_TVALUE));
			f.k = k_ref[0];
			f.sizek = fs.nk;
			Proto[][] p_ref = new Proto[1][];
			p_ref[0] = f.p;
			LuaMem.luaM_reallocvector_Proto(L, /*ref*/ p_ref, f.sizep, fs.np/*, Proto*/, new ClassType(ClassType.TYPE_PROTO));
			f.p = p_ref[0];
			f.sizep = fs.np;
			for (int i = 0; i < f.p.Length; i++)
			{
				f.p[i].protos = f.p;
				f.p[i].index = i;
			}
			LocVar[][] locvars_ref = new LocVar[1][];
			locvars_ref[0] = f.locvars;
			LuaMem.luaM_reallocvector_LocVar(L, /*ref*/ locvars_ref, f.sizelocvars, fs.nlocvars/*, LocVar*/, new ClassType(ClassType.TYPE_LOCVAR));
			f.locvars = locvars_ref[0];
			f.sizelocvars = fs.nlocvars;
			TString[][] upvalues_ref = new TString[1][];
			upvalues_ref[0] = f.upvalues;
			LuaMem.luaM_reallocvector_TString(L, /*ref*/ upvalues_ref, f.sizeupvalues, f.nups/*, TString*/, new ClassType(ClassType.TYPE_TSTRING));
			f.upvalues = upvalues_ref[0];
			f.sizeupvalues = f.nups;
			LuaLimits.lua_assert(LuaDebug.luaG_checkcode(f));
			LuaLimits.lua_assert(fs.bl == null);
			ls.fs = fs.prev;
			L.top = TValue.minus(L.top, 2);  /* remove table and prototype from the stack */
			/* last token read was anchored in defunct function; must reanchor it */
			if (fs != null) 
			{
				anchor_token(ls);
			}
		}

		public static Proto luaY_parser(lua_State L, ZIO z, Mbuffer buff, CharPtr name) 
		{
			LexState lexstate = new LexState();
			FuncState funcstate = new FuncState();
			lexstate.buff = buff;
			LuaLex.luaX_setinput(L, lexstate, z, LuaString.luaS_new(L, name));
			open_func(lexstate, funcstate);
			funcstate.f.is_vararg = LuaObject.VARARG_ISVARARG;  /* main func. is always vararg */
			LuaLex.luaX_next(lexstate);  /* read first token */
			chunk(lexstate);
			check(lexstate, (int)RESERVED.TK_EOS);
			close_func(lexstate);
			LuaLimits.lua_assert(funcstate.prev == null);
			LuaLimits.lua_assert(funcstate.f.nups == 0);
			LuaLimits.lua_assert(lexstate.fs == null);
			return funcstate.f;
		}

		/*============================================================*/
		/* GRAMMAR RULES */
		/*============================================================*/
		
		private static void field(LexState ls, expdesc v) 
		{
			/* field . ['.' | ':'] NAME */
			FuncState fs = ls.fs;
			expdesc key = new expdesc();
			LuaCode.luaK_exp2anyreg(fs, v);
			LuaLex.luaX_next(ls);  /* skip the dot or colon */
			checkname(ls, key);
			LuaCode.luaK_indexed(fs, v, key);
		}

		private static void yindex(LexState ls, expdesc v)
		{
			/* index . '[' expr ']' */
			LuaLex.luaX_next(ls);  /* skip the '[' */
			expr(ls, v);
			LuaCode.luaK_exp2val(ls.fs, v);
			checknext(ls, ']');
		}

		/*
		 ** {======================================================================
		 ** Rules for Constructors
		 ** =======================================================================
		 */
		private static void recfield(LexState ls, ConsControl cc) 
		{
			/* recfield . (NAME | `['exp1`]') = exp1 */
			FuncState fs = ls.fs;
			int reg = ls.fs.freereg;
			expdesc key = new expdesc(), val = new expdesc();
			int rkkey;
			if (ls.t.token == (int)RESERVED.TK_NAME) 
			{
				luaY_checklimit(fs, cc.nh, LuaLimits.MAX_INT, CharPtr.toCharPtr("items in a constructor"));
				checkname(ls, key);
			}
			else  /* ls.t.token == '[' */
			{
				yindex(ls, key);
			}
			cc.nh++;
			checknext(ls, '=');
			rkkey = LuaCode.luaK_exp2RK(fs, key);
			expr(ls, val);
			LuaCode.luaK_codeABC(fs, OpCode.OP_SETTABLE, cc.t.u.s.info, rkkey, LuaCode.luaK_exp2RK(fs, val));
			fs.freereg = reg;  /* free registers */
		}

		private static void closelistfield(FuncState fs, ConsControl cc) 
		{
			if (cc.v.k == expkind.VVOID) 
			{
				return;  /* there is no list item */
			}
			LuaCode.luaK_exp2nextreg(fs, cc.v);
			cc.v.k = expkind.VVOID;
			if (cc.tostore == LuaOpCodes.LFIELDS_PER_FLUSH)
			{
				LuaCode.luaK_setlist(fs, cc.t.u.s.info, cc.na, cc.tostore);  /* flush */
				cc.tostore = 0;  /* no more items pending */
			}
		}

		private static void lastlistfield(FuncState fs, ConsControl cc) 
		{
			if (cc.tostore == 0) 
			{
				return;
			}
			if (hasmultret(cc.v.k) != 0) 
			{
				LuaCode.luaK_setmultret(fs, cc.v);
				LuaCode.luaK_setlist(fs, cc.t.u.s.info, cc.na, Lua.LUA_MULTRET);
				cc.na--;  /* do not count last expression (unknown number of elements) */
			}
			else 
			{
				if (cc.v.k != expkind.VVOID)
				{
					LuaCode.luaK_exp2nextreg(fs, cc.v);
				}
				LuaCode.luaK_setlist(fs, cc.t.u.s.info, cc.na, cc.tostore);
			}
		}


		private static void listfield(LexState ls, ConsControl cc) 
		{
			expr(ls, cc.v);
			luaY_checklimit(ls.fs, cc.na, LuaLimits.MAX_INT, CharPtr.toCharPtr("items in a constructor"));
			cc.na++;
			cc.tostore++;
		}

		private static void constructor(LexState ls, expdesc t) 
		{
			/* constructor . ?? */
			FuncState fs = ls.fs;
			int line = ls.linenumber;
			int pc = LuaCode.luaK_codeABC(fs, OpCode.OP_NEWTABLE, 0, 0, 0);
			ConsControl cc = new ConsControl();
			cc.na = cc.nh = cc.tostore = 0;
			cc.t = t;
			init_exp(t, expkind.VRELOCABLE, pc);
			init_exp(cc.v, expkind.VVOID, 0);  /* no value (yet) */
			LuaCode.luaK_exp2nextreg(ls.fs, t);  /* fix it at stack top (for gc) */
			checknext(ls, '{');
			do 
			{
				LuaLimits.lua_assert(cc.v.k == expkind.VVOID || cc.tostore > 0);
				if (ls.t.token == '}') 
				{
					break;
				}
				closelistfield(fs, cc);
				switch (ls.t.token) 
				{
					case (int)RESERVED.TK_NAME: 
						{  
							/* may be listfields or recfields */
							LuaLex.luaX_lookahead(ls);
							if (ls.lookahead.token != '=')  /* expression? */
							{
								listfield(ls, cc);
							}
							else
							{
								recfield(ls, cc);
							}
							break;
						}
					case '[': 
						{
							/* constructor_item . recfield */
							recfield(ls, cc);
							break;
						}
					default: 
						{  
							/* constructor_part . listfield */
							listfield(ls, cc);
							break;
						}
				}
			} 
			while ((testnext(ls, ',')!=0) || (testnext(ls, ';')!=0));
			check_match(ls, '}', '{', line);
			lastlistfield(fs, cc);
			LuaOpCodes.SETARG_B(new InstructionPtr(fs.f.code, pc), LuaObject.luaO_int2fb((int/*uint*/)cc.na)); /* set initial array size */
			LuaOpCodes.SETARG_C(new InstructionPtr(fs.f.code, pc), LuaObject.luaO_int2fb((int/*uint*/)cc.nh));  /* set initial table size */
		}

		/* }====================================================================== */

		private static void parlist(LexState ls) 
		{
			/* parlist . [ param { `,' param } ] */
			FuncState fs = ls.fs;
			Proto f = fs.f;
			int nparams = 0;
			f.is_vararg = 0;
			if (ls.t.token != ')') 
			{
				/* is `parlist' not empty? */
				do 
				{
					switch (ls.t.token) 
					{
						case (int)RESERVED.TK_NAME: 
							{  
								/* param . NAME */
								new_localvar(ls, str_checkname(ls), nparams++);
								break;
							}
						case (int)RESERVED.TK_DOTS: 
							{  
								/* param . `...' */
								LuaLex.luaX_next(ls);
								//#if LUA_COMPAT_VARARG
								/* use `arg' as default name */
								new_localvarliteral(ls, CharPtr.toCharPtr("arg"), nparams++);
								f.is_vararg = LuaObject.VARARG_HASARG | LuaObject.VARARG_NEEDSARG;
								//#endif
								f.is_vararg |= LuaObject.VARARG_ISVARARG;
								break;
							}
						default: 
							{
								LuaLex.luaX_syntaxerror(ls, CharPtr.toCharPtr("<name> or " + LuaConf.LUA_QL("...") + " expected")); 
								break;
							}
					}
				} 
				while ((f.is_vararg == 0) && (testnext(ls, ',') != 0));
			}
			adjustlocalvars(ls, nparams);
			f.numparams = LuaLimits.cast_byte(fs.nactvar - (f.is_vararg & LuaObject.VARARG_HASARG));
			LuaCode.luaK_reserveregs(fs, fs.nactvar);  /* reserve register for parameters */
		}

		private static void body(LexState ls, expdesc e, int needself, int line) 
		{
			/* body .  `(' parlist `)' chunk END */
			FuncState new_fs = new FuncState();
			open_func(ls, new_fs);
			new_fs.f.linedefined = line;
			checknext(ls, '(');
			if (needself != 0) 
			{
				new_localvarliteral(ls, CharPtr.toCharPtr("self"), 0);
				adjustlocalvars(ls, 1);
			}
			parlist(ls);
			checknext(ls, ')');
			chunk(ls);
			new_fs.f.lastlinedefined = ls.linenumber;
			check_match(ls, (int)RESERVED.TK_END, (int)RESERVED.TK_FUNCTION, line);
			close_func(ls);
			pushclosure(ls, new_fs, e);
		}

		private static int explist1(LexState ls, expdesc v) 
		{
			/* explist1 . expr { `,' expr } */
			int n = 1;  /* at least one expression */
			expr(ls, v);
			while (testnext(ls, ',') != 0) 
			{
				LuaCode.luaK_exp2nextreg(ls.fs, v);
				expr(ls, v);
				n++;
			}
			return n;
		}

		private static void funcargs(LexState ls, expdesc f) 
		{
			FuncState fs = ls.fs;
			expdesc args = new expdesc();
			int base_, nparams;
			int line = ls.linenumber;
			switch (ls.t.token) 
			{
				case '(': 
					{  
						/* funcargs . `(' [ explist1 ] `)' */
						if (line != ls.lastline)
						{
							LuaLex.luaX_syntaxerror(ls, CharPtr.toCharPtr("ambiguous syntax (function call x new statement)"));
						}
						LuaLex.luaX_next(ls);
						if (ls.t.token == ')')  /* arg list is empty? */
						{
							args.k = expkind.VVOID;
						}
						else 
						{
							explist1(ls, args);
							LuaCode.luaK_setmultret(fs, args);
						}
						check_match(ls, ')', '(', line);
						break;
					}
				case '{': 
					{  
						/* funcargs . constructor */
						constructor(ls, args);
						break;
					}
				case (int)RESERVED.TK_STRING: 
					{  
						/* funcargs . STRING */
						codestring(ls, args, ls.t.seminfo.ts);
						LuaLex.luaX_next(ls);  /* must use `seminfo' before `next' */
						break;
					}
				default: 
					{
						LuaLex.luaX_syntaxerror(ls, CharPtr.toCharPtr("function arguments expected"));
						return;
					}
			}
			LuaLimits.lua_assert(f.k == expkind.VNONRELOC);
			base_ = f.u.s.info;  /* base_ register for call */
			if (hasmultret(args.k) != 0)
			{
				nparams = Lua.LUA_MULTRET;  /* open call */
			}
			else 
			{
				if (args.k != expkind.VVOID)
				{
					LuaCode.luaK_exp2nextreg(fs, args);  /* close last argument */
				}
				nparams = fs.freereg - (base_+1);
			}
			init_exp(f, expkind.VCALL, LuaCode.luaK_codeABC(fs, OpCode.OP_CALL, base_, nparams + 1, 2));
			LuaCode.luaK_fixline(fs, line);
			fs.freereg = base_ + 1;  /* call remove function and arguments and leaves
									(unless changed) one result */
		}

		/*
		 ** {======================================================================
		 ** Expression parsing
		 ** =======================================================================
		 */
		
		private static void prefixexp(LexState ls, expdesc v) 
		{
			/* prefixexp . NAME | '(' expr ')' */
			switch (ls.t.token) 
			{
				case '(': 
					{
						int line = ls.linenumber;
						LuaLex.luaX_next(ls);
						expr(ls, v);
						check_match(ls, ')', '(', line);
						LuaCode.luaK_dischargevars(ls.fs, v);
						return;
					}
				case (int)RESERVED.TK_NAME: 
					{
						singlevar(ls, v);
						return;
					}
				default: 
					{
						LuaLex.luaX_syntaxerror(ls, CharPtr.toCharPtr("unexpected symbol"));
						return;
					}
			}
		}

		private static void primaryexp (LexState ls, expdesc v) 
		{
			/* primaryexp .
				prefixexp { `.' NAME | `[' exp `]' | `:' NAME funcargs | funcargs } */
			FuncState fs = ls.fs;
			prefixexp(ls, v);
			for (;;) 
			{
				switch (ls.t.token) 
				{
					case '.': 
						{  
							/* field */
							field(ls, v);
							break;
						}
					case '[': 
						{  
							/* `[' exp1 `]' */
							expdesc key = new expdesc();
							LuaCode.luaK_exp2anyreg(fs, v);
							yindex(ls, key);
							LuaCode.luaK_indexed(fs, v, key);
							break;
						}
					case ':': 
						{  
							/* `:' NAME funcargs */
							expdesc key = new expdesc();
							LuaLex.luaX_next(ls);
							checkname(ls, key);
							LuaCode.luaK_self(fs, v, key);
							funcargs(ls, v);
							break;
						}
					case '(': 
					case (int)RESERVED.TK_STRING: 
					case '{': 
						{  
							/* funcargs */
							LuaCode.luaK_exp2nextreg(fs, v);
							funcargs(ls, v);
							break;
						}
					default: 
						{
							return;
						}
				}
			}
		}

		private static void simpleexp(LexState ls, expdesc v) 
		{
			/* simpleexp . NUMBER | STRING | NIL | true | false | ... |
						  constructor | FUNCTION body | primaryexp */
			switch (ls.t.token) 
			{
				case (int)RESERVED.TK_NUMBER: 
					{
						init_exp(v, expkind.VKNUM, 0);
						v.u.nval = ls.t.seminfo.r;
						break;
					}
				case (int)RESERVED.TK_STRING: 
					{
						codestring(ls, v, ls.t.seminfo.ts);
						break;
					}
				case (int)RESERVED.TK_NIL: 
					{
						init_exp(v, expkind.VNIL, 0);
						break;
					}
				case (int)RESERVED.TK_TRUE: 
					{
						init_exp(v, expkind.VTRUE, 0);
						break;
					}
				case (int)RESERVED.TK_FALSE: 
					{
						init_exp(v, expkind.VFALSE, 0);
						break;
					}
				case (int)RESERVED.TK_DOTS: 
					{  
						/* vararg */
						FuncState fs = ls.fs;
						check_condition(ls, fs.f.is_vararg != 0,
							CharPtr.toCharPtr("cannot use " + LuaConf.LUA_QL("...") + " outside a vararg function"));
						fs.f.is_vararg &= /*unchecked*/((Byte/*lu_byte*/)((~LuaObject.VARARG_NEEDSARG) & 0xff));  /* don't need 'arg' */
						init_exp(v, expkind.VVARARG, LuaCode.luaK_codeABC(fs, OpCode.OP_VARARG, 0, 1, 0));
						break;
					}
				case '{': 
					{  
						/* constructor */
						constructor(ls, v);
						return;
					}
				case (int)RESERVED.TK_FUNCTION: 
					{
						LuaLex.luaX_next(ls);
						body(ls, v, 0, ls.linenumber);
						return;
					}
				default: 
					{
						primaryexp(ls, v);
						return;
					}
			}
			LuaLex.luaX_next(ls);
		}

		private static UnOpr getunopr(int op)
		{
			switch (op) 
			{
				case (int)RESERVED.TK_NOT: 
					{
						return UnOpr.OPR_NOT;
					}
				case '-': 
					{
						return UnOpr.OPR_MINUS;
					}
				case '#': 
					{
						return UnOpr.OPR_LEN;
					}
				default: 
					{
						return UnOpr.OPR_NOUNOPR;
					}
			}
		}

		private static BinOpr getbinopr(int op)
		{
			switch (op) 
			{
				case '+': 
					{
						return BinOpr.OPR_ADD;
					}
				case '-': 
					{
						return BinOpr.OPR_SUB;
					}
				case '*': 
					{
						return BinOpr.OPR_MUL;
					}
				case '/': 
					{
						return BinOpr.OPR_DIV;
					}
				case '%': 
					{
						return BinOpr.OPR_MOD;
					}
				case '^': 
					{
						return BinOpr.OPR_POW;
					}
				case (int)RESERVED.TK_CONCAT: 
					{
						return BinOpr.OPR_CONCAT;
					}
				case (int)RESERVED.TK_NE: 
					{
						return BinOpr.OPR_NE;
					}
				case (int)RESERVED.TK_EQ: 
					{
						return BinOpr.OPR_EQ;
					}
				case '<': 
					{
						return BinOpr.OPR_LT;
					}
				case (int)RESERVED.TK_LE: 
					{
						return BinOpr.OPR_LE;
					}
				case '>': 
					{
						return BinOpr.OPR_GT;
					}
				case (int)RESERVED.TK_GE: 
					{
						return BinOpr.OPR_GE;
					}
				case (int)RESERVED.TK_AND: 
					{
						return BinOpr.OPR_AND;
					}
				case (int)RESERVED.TK_OR: 
					{
						return BinOpr.OPR_OR;
					}
				default: 
					{
						return BinOpr.OPR_NOBINOPR;
					}
			}
		}
		
		/* ORDER OPR */
		/* `+' `-' `/' `%' */
		/* power and concat (right associative) */
		/* equality and inequality */
		/* order */
		/* logical (and/or) */
		private static priority_[] priority = {  
			new priority_((byte)6, (byte)6),
			new priority_((byte)6, (byte)6),
			new priority_((byte)7, (byte)7),
			new priority_((byte)7, (byte)7),
			new priority_((byte)7, (byte)7),				

			new priority_((byte)10, (byte)9),
			new priority_((byte)5, (byte)4),				

			new priority_((byte)3, (byte)3),
			new priority_((byte)3, (byte)3),				

			new priority_((byte)3, (byte)3),
			new priority_((byte)3, (byte)3),
			new priority_((byte)3, (byte)3),
			new priority_((byte)3, (byte)3),				

			new priority_((byte)2, (byte)2),
			new priority_((byte)1, (byte)1)					
		};

		public const int UNARY_PRIORITY	= 8;  /* priority for unary operators */

		/*
		 ** subexpr . (simpleexp | unop subexpr) { binop subexpr }
		 ** where `binop' is any binary operator with a priority higher than `limit'
		 */
		private static BinOpr subexpr(LexState ls, expdesc v, int/*uint*/ limit) 
		{
			BinOpr op;// = new BinOpr();
			UnOpr uop;// = new UnOpr();
			enterlevel(ls);
			uop = getunopr(ls.t.token);
			if (uop != UnOpr.OPR_NOUNOPR) 
			{
				LuaLex.luaX_next(ls);
				subexpr(ls, v, UNARY_PRIORITY);
				LuaCode.luaK_prefix(ls.fs, uop, v);
			}
			else 
			{
				simpleexp(ls, v);
			}
			/* expand while operators have priorities higher than `limit' */
			op = getbinopr(ls.t.token);
			while (op != BinOpr.OPR_NOBINOPR && priority[(int)op].left > limit)
			{
				expdesc v2 = new expdesc();
				BinOpr nextop;
				LuaLex.luaX_next(ls);
				LuaCode.luaK_infix(ls.fs, op, v);
				/* read sub-expression with higher priority */
				nextop = subexpr(ls, v2, priority[(int)op].right);
				LuaCode.luaK_posfix(ls.fs, op, v, v2);
				op = nextop;
			}
			leavelevel(ls);
			return op;  /* return first untreated operator */
		}

		private static void expr(LexState ls, expdesc v) 
		{
			subexpr(ls, v, 0);
		}

		/* }==================================================================== */

		/*
		 ** {======================================================================
		 ** Rules for Statements
		 ** =======================================================================
		 */
		
		private static int block_follow(int token) 
		{
			switch (token) 
			{
				case (int)RESERVED.TK_ELSE: 
				case (int)RESERVED.TK_ELSEIF: 
				case (int)RESERVED.TK_END:
				case (int)RESERVED.TK_UNTIL: 
				case (int)RESERVED.TK_EOS:
					{
						return 1;
					}
				default: 
					{
						return 0;
					}
			}
		}

		private static void block(LexState ls) 
		{
			/* block . chunk */
			FuncState fs = ls.fs;
			BlockCnt bl = new BlockCnt();
			enterblock(fs, bl, (byte)0);
			chunk(ls);
			LuaLimits.lua_assert(bl.breaklist == LuaCode.NO_JUMP);
			leaveblock(fs);
		}

		/*
		 ** check whether, in an assignment to a local variable, the local variable
		 ** is needed in a previous assignment (to a table). If so, save original
		 ** local value in a safe place and use this safe copy in the previous
		 ** assignment.
		 */
		private static void check_conflict(LexState ls, LHS_assign lh, expdesc v) 
		{
			FuncState fs = ls.fs;
			int extra = fs.freereg;  /* eventual position to save local variable */
			int conflict = 0;
			for (; lh != null; lh = lh.prev) 
			{
				if (lh.v.k == expkind.VINDEXED) 
				{
					if (lh.v.u.s.info == v.u.s.info) 
					{  
						/* conflict? */
						conflict = 1;
						lh.v.u.s.info = extra;  /* previous assignment will use safe copy */
					}
					if (lh.v.u.s.aux == v.u.s.info) 
					{  
						/* conflict? */
						conflict = 1;
						lh.v.u.s.aux = extra;  /* previous assignment will use safe copy */
					}
				}
			}
			if (conflict != 0) 
			{
				LuaCode.luaK_codeABC(fs, OpCode.OP_MOVE, fs.freereg, v.u.s.info, 0);  /* make copy */
				LuaCode.luaK_reserveregs(fs, 1);
			}
		}


		private static void assignment(LexState ls, LHS_assign lh, int nvars) 
		{
			expdesc e = new expdesc();
            check_condition(ls, 
                expkindUtil.expkindToInt(expkind.VLOCAL) <= expkindUtil.expkindToInt(lh.v.k) && 
                expkindUtil.expkindToInt(lh.v.k) <= expkindUtil.expkindToInt(expkind.VINDEXED),
				CharPtr.toCharPtr("syntax error"));
			if (testnext(ls, ',') != 0) 
			{  
				/* assignment . `,' primaryexp assignment */
				LHS_assign nv = new LHS_assign();
				nv.prev = lh;
				primaryexp(ls, nv.v);
				if (nv.v.k == expkind.VLOCAL)
				{
					check_conflict(ls, lh, nv.v);
				}
				luaY_checklimit(ls.fs, nvars, LuaConf.LUAI_MAXCCALLS - ls.L.nCcalls,
					CharPtr.toCharPtr("variables in assignment"));
				assignment(ls, nv, nvars+1);
			}
			else 
			{  
				/* assignment . `=' explist1 */
				int nexps;
				checknext(ls, '=');
				nexps = explist1(ls, e);
				if (nexps != nvars) 
				{
					adjust_assign(ls, nvars, nexps, e);
					if (nexps > nvars)
					{
						ls.fs.freereg -= nexps - nvars;  /* remove extra values */
					}
				}
				else 
				{
					LuaCode.luaK_setoneret(ls.fs, e);  /* close last expression */
					LuaCode.luaK_storevar(ls.fs, lh.v, e);
					return;  /* avoid default */
				}
			}
			init_exp(e, expkind.VNONRELOC, ls.fs.freereg - 1);  /* default assignment */
			LuaCode.luaK_storevar(ls.fs, lh.v, e);
		}

		private static int cond(LexState ls) 
		{
			/* cond . exp */
			expdesc v = new expdesc();
			expr(ls, v);  /* read condition */
			if (v.k == expkind.VNIL) v.k = expkind.VFALSE;  /* `falses' are all equal here */
			LuaCode.luaK_goiftrue(ls.fs, v);
			return v.f;
		}

		private static void breakstat(LexState ls) 
		{
			FuncState fs = ls.fs;
			BlockCnt bl = fs.bl;
			int upval = 0;
			while ((bl!=null) && (bl.isbreakable==0)) 
			{
				upval |= bl.upval;
				bl = bl.previous;
			}
			if (bl==null)
			{
				LuaLex.luaX_syntaxerror(ls, CharPtr.toCharPtr("no loop to break"));
			}
			if (upval != 0)
			{
				LuaCode.luaK_codeABC(fs, OpCode.OP_CLOSE, bl.nactvar, 0, 0);
			}
			int[] breaklist_ref = new int[1];
			breaklist_ref[0] = bl.breaklist;
			LuaCode.luaK_concat(fs, /*ref*/ breaklist_ref, LuaCode.luaK_jump(fs));
			bl.breaklist = breaklist_ref[0];
		}

		private static void whilestat(LexState ls, int line) 
		{
			/* whilestat . WHILE cond DO block END */
			FuncState fs = ls.fs;
			int whileinit;
			int condexit;
			BlockCnt bl = new BlockCnt();
			LuaLex.luaX_next(ls);  /* skip WHILE */
			whileinit = LuaCode.luaK_getlabel(fs);
			condexit = cond(ls);
			enterblock(fs, bl, (byte)1);
			checknext(ls, (int)RESERVED.TK_DO);
			block(ls);
			LuaCode.luaK_patchlist(fs, LuaCode.luaK_jump(fs), whileinit);
			check_match(ls, (int)RESERVED.TK_END, (int)RESERVED.TK_WHILE, line);
			leaveblock(fs);
			LuaCode.luaK_patchtohere(fs, condexit);  /* false conditions finish the loop */
		}

		private static void repeatstat(LexState ls, int line) 
		{
			/* repeatstat . REPEAT block UNTIL cond */
			int condexit;
			FuncState fs = ls.fs;
			int repeat_init = LuaCode.luaK_getlabel(fs);
			BlockCnt bl1 = new BlockCnt(), bl2 = new BlockCnt();
            enterblock(fs, bl1, (byte)1);  /* loop block */
            enterblock(fs, bl2, (byte)0);  /* scope block */
			LuaLex.luaX_next(ls);  /* skip REPEAT */
			chunk(ls);
			check_match(ls, (int)RESERVED.TK_UNTIL, (int)RESERVED.TK_REPEAT, line);
			condexit = cond(ls);  /* read condition (inside scope block) */
			if (bl2.upval == 0) 
			{  
				/* no upvalues? */
				leaveblock(fs);  /* finish scope */
				LuaCode.luaK_patchlist(ls.fs, condexit, repeat_init);  /* close the loop */
			}
			else 
			{  
				/* complete semantics when there are upvalues */
				breakstat(ls);  /* if condition then break */
				LuaCode.luaK_patchtohere(ls.fs, condexit);  /* else... */
				leaveblock(fs);  /* finish scope... */
				LuaCode.luaK_patchlist(ls.fs, LuaCode.luaK_jump(fs), repeat_init);  /* and repeat */
			}
			leaveblock(fs);  /* finish loop */
		}

		private static int exp1(LexState ls) 
		{
			expdesc e = new expdesc();
			int k;
			expr(ls, e);
			k = (int)expkindUtil.expkindToInt(e.k);
			LuaCode.luaK_exp2nextreg(ls.fs, e);
			return k;
		}

		private static void forbody(LexState ls, int base_, int line, int nvars, int isnum) 
		{
			/* forbody . DO block */
			BlockCnt bl = new BlockCnt();
			FuncState fs = ls.fs;
			int prep, endfor;
			adjustlocalvars(ls, 3);  /* control variables */
			checknext(ls, (int)RESERVED.TK_DO);
			prep = (isnum != 0) ? LuaCode.luaK_codeAsBx(fs, OpCode.OP_FORPREP, base_, LuaCode.NO_JUMP) : LuaCode.luaK_jump(fs);
            enterblock(fs, bl, (byte)0);  /* scope for declared variables */
			adjustlocalvars(ls, nvars);
			LuaCode.luaK_reserveregs(fs, nvars);
			block(ls);
			leaveblock(fs);  /* end of scope for declared variables */
			LuaCode.luaK_patchtohere(fs, prep);
			endfor = (isnum != 0) ? LuaCode.luaK_codeAsBx(fs, OpCode.OP_FORLOOP, base_, LuaCode.NO_JUMP) :
				LuaCode.luaK_codeABC(fs, OpCode.OP_TFORLOOP, base_, 0, nvars);
			LuaCode.luaK_fixline(fs, line);  /* pretend that `OP_FOR' starts the loop */
			LuaCode.luaK_patchlist(fs, ((isnum != 0) ? endfor : LuaCode.luaK_jump(fs)), prep + 1);
		}

		private static void fornum(LexState ls, TString varname, int line) 
		{
			/* fornum . NAME = exp1,exp1[,exp1] forbody */
			FuncState fs = ls.fs;
			int base_ = fs.freereg;
			new_localvarliteral(ls, CharPtr.toCharPtr("(for index)"), 0);
			new_localvarliteral(ls, CharPtr.toCharPtr("(for limit)"), 1);
			new_localvarliteral(ls, CharPtr.toCharPtr("(for step)"), 2);
			new_localvar(ls, varname, 3);
			checknext(ls, '=');
			exp1(ls);  /* initial value */
			checknext(ls, ',');
			exp1(ls);  /* limit */
			if (testnext(ls, ',') != 0)
			{
				exp1(ls);  /* optional step */
			}
			else 
			{
				/* default step = 1 */
				LuaCode.luaK_codeABx(fs, OpCode.OP_LOADK, fs.freereg, LuaCode.luaK_numberK(fs, 1));
				LuaCode.luaK_reserveregs(fs, 1);
			}
			forbody(ls, base_, line, 1, 1);
		}

		private static void forlist(LexState ls, TString indexname) 
		{
			/* forlist . NAME {,NAME} IN explist1 forbody */
			FuncState fs = ls.fs;
			expdesc e = new expdesc();
			int nvars = 0;
			int line;
			int base_ = fs.freereg;
			/* create control variables */
			new_localvarliteral(ls, CharPtr.toCharPtr("(for generator)"), nvars++);
			new_localvarliteral(ls, CharPtr.toCharPtr("(for state)"), nvars++);
			new_localvarliteral(ls, CharPtr.toCharPtr("(for control)"), nvars++);
			/* create declared variables */
			new_localvar(ls, indexname, nvars++);
			while (testnext(ls, ',') != 0)
			{
				new_localvar(ls, str_checkname(ls), nvars++);
			}
			checknext(ls, (int)RESERVED.TK_IN);
			line = ls.linenumber;
			adjust_assign(ls, 3, explist1(ls, e), e);
			LuaCode.luaK_checkstack(fs, 3);  /* extra space to call generator */
			forbody(ls, base_, line, nvars - 3, 0);
		}

		private static void forstat(LexState ls, int line) 
		{
			/* forstat . FOR (fornum | forlist) END */
			FuncState fs = ls.fs;
			TString varname;
			BlockCnt bl = new BlockCnt();
            enterblock(fs, bl, (byte)1);  /* scope for loop and control variables */
			LuaLex.luaX_next(ls);  /* skip `for' */
			varname = str_checkname(ls);  /* first variable name */
			switch (ls.t.token) 
			{
				case '=': 
					{
						fornum(ls, varname, line); break;
					}
				case ',':
				case (int)RESERVED.TK_IN:
					{
						forlist(ls, varname);
						break;
					}
				default: 
					{
						LuaLex.luaX_syntaxerror(ls, CharPtr.toCharPtr(LuaConf.LUA_QL("=") + " or " + LuaConf.LUA_QL("in") + " expected")); 
						break;
					}
			}
			check_match(ls, (int)RESERVED.TK_END, (int)RESERVED.TK_FOR, line);
			leaveblock(fs);  /* loop scope (`break' jumps to this point) */
		}

		private static int test_then_block(LexState ls) 
		{
			/* test_then_block . [IF | ELSEIF] cond THEN block */
			int condexit;
			LuaLex.luaX_next(ls);  /* skip IF or ELSEIF */
			condexit = cond(ls);
			checknext(ls, (int)RESERVED.TK_THEN);
			block(ls);  /* `then' part */
			return condexit;
		}

		private static void ifstat(LexState ls, int line) 
		{
			/* ifstat . IF cond THEN block {ELSEIF cond THEN block} [ELSE block] END */
			FuncState fs = ls.fs;
			int flist;
			int[] escapelist = new int[1];
			escapelist[0] = LuaCode.NO_JUMP;
			flist = test_then_block(ls);  /* IF cond THEN block */
			while (ls.t.token == (int)RESERVED.TK_ELSEIF) 
			{
				LuaCode.luaK_concat(fs, /*ref*/ escapelist, LuaCode.luaK_jump(fs));
				LuaCode.luaK_patchtohere(fs, flist);
				flist = test_then_block(ls);  /* ELSEIF cond THEN block */
			}
			if (ls.t.token == (int)RESERVED.TK_ELSE) 
			{
				LuaCode.luaK_concat(fs, /*ref*/ escapelist, LuaCode.luaK_jump(fs));
				LuaCode.luaK_patchtohere(fs, flist);
				LuaLex.luaX_next(ls);  /* skip ELSE (after patch, for correct line info) */
				block(ls);  /* `else' part */
			}
			else
			{
				LuaCode.luaK_concat(fs, /*ref*/ escapelist, flist);
			}
			LuaCode.luaK_patchtohere(fs, escapelist[0]);
			check_match(ls, (int)RESERVED.TK_END, (int)RESERVED.TK_IF, line);
		}

		private static void localfunc(LexState ls) 
		{
			expdesc v = new expdesc(), b = new expdesc();
			FuncState fs = ls.fs;
			new_localvar(ls, str_checkname(ls), 0);
			init_exp(v, expkind.VLOCAL, fs.freereg);
			LuaCode.luaK_reserveregs(fs, 1);
			adjustlocalvars(ls, 1);
			body(ls, b, 0, ls.linenumber);
			LuaCode.luaK_storevar(fs, v, b);
			/* debug information will only see the variable after this point! */
			getlocvar(fs, fs.nactvar - 1).startpc = fs.pc;
		}

		private static void localstat (LexState ls) 
		{
			/* stat . LOCAL NAME {`,' NAME} [`=' explist1] */
			int nvars = 0;
			int nexps;
			expdesc e = new expdesc();
			do 
			{
				new_localvar(ls, str_checkname(ls), nvars++);
			} while (testnext(ls, ',') != 0);
			if (testnext(ls, '=') != 0)
			{
				nexps = explist1(ls, e);
			}
			else 
			{
				e.k = expkind.VVOID;
				nexps = 0;
			}
			adjust_assign(ls, nvars, nexps, e);
			adjustlocalvars(ls, nvars);
		}

		private static int funcname(LexState ls, expdesc v) 
		{
			/* funcname . NAME {field} [`:' NAME] */
			int needself = 0;
			singlevar(ls, v);
			while (ls.t.token == '.')
			{
				field(ls, v);
			}
			if (ls.t.token == ':') 
			{
				needself = 1;
				field(ls, v);
			}
			return needself;
		}

		private static void funcstat(LexState ls, int line) 
		{
			/* funcstat . FUNCTION funcname body */
			int needself;
			expdesc v = new expdesc(), b = new expdesc();
			LuaLex.luaX_next(ls);  /* skip FUNCTION */
			needself = funcname(ls, v);
			body(ls, b, needself, line);
			LuaCode.luaK_storevar(ls.fs, v, b);
			LuaCode.luaK_fixline(ls.fs, line);  /* definition `happens' in the first line */
		}

		private static void exprstat(LexState ls) 
		{
			/* stat . func | assignment */
			FuncState fs = ls.fs;
			LHS_assign v = new LHS_assign();
			primaryexp(ls, v.v);
			if (v.v.k == expkind.VCALL)  /* stat . func */
			{
				LuaOpCodes.SETARG_C(LuaCode.getcode(fs, v.v), 1);  /* call statement uses no results */
			}
			else 
			{  
				/* stat . assignment */
				v.prev = null;
				assignment(ls, v, 1);
			}
		}

		private static void retstat(LexState ls) 
		{
			/* stat . RETURN explist */
			FuncState fs = ls.fs;
			expdesc e = new expdesc();
			int first, nret;  /* registers with returned values */
			LuaLex.luaX_next(ls);  /* skip RETURN */
			if ((block_follow(ls.t.token)!=0) || ls.t.token == ';')
			{
				first = nret = 0;  /* return no values */
			}
			else 
			{
				nret = explist1(ls, e);  /* optional return values */
				if (hasmultret(e.k) != 0) 
				{
					LuaCode.luaK_setmultret(fs, e);
					if (e.k == expkind.VCALL && nret == 1) 
					{  
						/* tail call? */
						LuaOpCodes.SET_OPCODE(LuaCode.getcode(fs, e), OpCode.OP_TAILCALL);
						LuaLimits.lua_assert(LuaOpCodes.GETARG_A(LuaCode.getcode(fs, e)) == fs.nactvar);
					}
					first = fs.nactvar;
					nret = Lua.LUA_MULTRET;  /* return all values */
				}
				else 
				{
					if (nret == 1)  /* only one single value? */
					{
						first = LuaCode.luaK_exp2anyreg(fs, e);
					}
					else 
					{
						LuaCode.luaK_exp2nextreg(fs, e);  /* values must go to the `stack' */
						first = fs.nactvar;  /* return all `active' values */
						LuaLimits.lua_assert(nret == fs.freereg - first);
					}
				}
			}
			LuaCode.luaK_ret(fs, first, nret);
		}

		private static int statement(LexState ls) 
		{
			int line = ls.linenumber;  /* may be needed for error messages */
			switch (ls.t.token) 
			{
				case (int)RESERVED.TK_IF: 
					{  
						/* stat . ifstat */
						ifstat(ls, line);
						return 0;
					}
				case (int)RESERVED.TK_WHILE: 
					{  
						/* stat . whilestat */
						whilestat(ls, line);
						return 0;
					}
				case (int)RESERVED.TK_DO: 
					{  
						/* stat . DO block END */
						LuaLex.luaX_next(ls);  /* skip DO */
						block(ls);
						check_match(ls, (int)RESERVED.TK_END, (int)RESERVED.TK_DO, line);
						return 0;
					}
				case (int)RESERVED.TK_FOR: 
					{  
						/* stat . forstat */
						forstat(ls, line);
						return 0;
					}
				case (int)RESERVED.TK_REPEAT: 
					{  
						/* stat . repeatstat */
						repeatstat(ls, line);
						return 0;
					}
				case (int)RESERVED.TK_FUNCTION: 
					{
						funcstat(ls, line);  /* stat . funcstat */
						return 0;
					}
				case (int)RESERVED.TK_LOCAL: 
					{  
						/* stat . localstat */
						LuaLex.luaX_next(ls);  /* skip LOCAL */
						if (testnext(ls, (int)RESERVED.TK_FUNCTION) != 0)  /* local function? */
						{
							localfunc(ls);
						}
						else
						{
							localstat(ls);
						}
						return 0;
					}
				case (int)RESERVED.TK_RETURN: 
					{
						/* stat . retstat */
						retstat(ls);
						return 1;  /* must be last statement */
					}
				case (int)RESERVED.TK_BREAK: 
					{  
						/* stat . breakstat */
						LuaLex.luaX_next(ls);  /* skip BREAK */
						breakstat(ls);
						return 1;  /* must be last statement */
					}
				default: 
					{
						exprstat(ls);
						return 0;  /* to avoid warnings */
					}
			}
		}

		private static void chunk(LexState ls) 
		{
			/* chunk . { stat [`;'] } */
			int islast = 0;
			enterlevel(ls);
			while ((islast==0) && (block_follow(ls.t.token)==0)) 
			{
				islast = statement(ls);
				testnext(ls, ';');
				LuaLimits.lua_assert(ls.fs.f.maxstacksize >= ls.fs.freereg &&
					ls.fs.freereg >= ls.fs.nactvar);
				ls.fs.freereg = ls.fs.nactvar;  /* free registers */
			}
			leavelevel(ls);
		}
		
		/* }====================================================================== */
	}
}
