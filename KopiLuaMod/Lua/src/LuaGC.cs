/*
 ** $Id: lgc.c,v 2.38.1.1 2007/12/27 13:02:25 roberto Exp $
 ** Garbage Collector
 ** See Copyright Notice in lua.h
 */
using System;

namespace KopiLua
{
	//using lu_int32 = System.UInt32;
	//using l_mem = System.Int32;
	//using lu_mem = System.UInt32;
	//using TValue = Lua.TValue;
	//using StkId = TValue;
	//using lu_byte = System.Byte;
	//using Instruction = System.UInt32;

	public class LuaGC
	{
		/*
		 ** Possible states of the Garbage Collector
		 */
		public const int GCSpause = 0;
		public const int GCSpropagate = 1;
		public const int GCSsweepstring	= 2;
		public const int GCSsweep = 3;
		public const int GCSfinalize = 4;

		/*
		 ** some userful bit tricks
		 */
		public static int resetbits(/*ref*/ Byte[]/*lu_byte*/ x, int m)
		{ 
			x[0] &= (Byte/*lu_byte*/)~m;
			return x[0]; 
		}
		
		public static int setbits(/*ref*/ Byte[]/*lu_byte*/ x, int m)
		{ 
			x[0] |= (Byte/*lu_byte*/)m;
			return x[0];
		}
		
		public static bool testbits(Byte/*lu_byte*/ x, int m) 
		{ 
			return (x & (Byte/*lu_byte*/)m) != 0; 
		}
		
		public static int bitmask(int b)	
		{
			return 1 << b;
		}
		
		public static int bit2mask(int b1, int b2)	
		{
			return (bitmask(b1) | bitmask(b2));
		}
		
		public static int l_setbit(/*ref*/ Byte[]/*lu_byte*/ x, int b)
		{ 
			return setbits(/*ref*/ x, bitmask(b)); 
		}
		
		public static int resetbit(/*ref*/ Byte[]/*lu_byte*/ x, int b)
		{ 
			return resetbits(/*ref*/ x, bitmask(b)); 
		}
		
		public static bool testbit(Byte/*lu_byte*/ x, int b) 
		{ 
			return testbits(x, bitmask(b)); 
		}
		
		public static int set2bits(/*ref*/ Byte[]/*lu_byte*/ x, int b1, int b2)
		{ 
			return setbits(/*ref*/ x, (bit2mask(b1, b2))); 
		}
		
		public static int reset2bits(/*ref*/ Byte[]/*lu_byte*/ x, int b1, int b2)
		{ 
			return resetbits(/*ref*/ x, (bit2mask(b1, b2))); 
		}
		
		public static bool test2bits(Byte/*lu_byte*/ x, int b1, int b2) 
		{ 
			return testbits(x, (bit2mask(b1, b2))); 
		}

		/*
		 ** Layout for bit use in `marked' field:
		 ** bit 0 - object is white (type 0)
		 ** bit 1 - object is white (type 1)
		 ** bit 2 - object is black
		 ** bit 3 - for userdata: has been finalized
		 ** bit 3 - for tables: has weak keys
		 ** bit 4 - for tables: has weak values
		 ** bit 5 - object is fixed (should not be collected)
		 ** bit 6 - object is "super" fixed (only the main thread)
		 */
		public const int WHITE0BIT = 0;
		public const int WHITE1BIT = 1;
		public const int BLACKBIT = 2;
		public const int FINALIZEDBIT = 3;
		public const int KEYWEAKBIT = 3;
		public const int VALUEWEAKBIT = 4;
		public const int FIXEDBIT = 5;
		public const int SFIXEDBIT = 6;
		public readonly static int WHITEBITS = bit2mask(WHITE0BIT, WHITE1BIT);

		public static bool iswhite(GCObject x) 
		{ 
			return test2bits(x.getGch().marked, WHITE0BIT, WHITE1BIT); 
		}
		
		public static bool isblack(GCObject x) 
		{ 
			return testbit(x.getGch().marked, BLACKBIT); 
		}
		
		public static bool isgray(GCObject x) 
		{ 
			return (!isblack(x) && !iswhite(x)); 
		}

		public static int otherwhite(global_State g) 
		{ 
			return g.currentwhite ^ WHITEBITS; 
		}
		
		public static bool isdead(global_State g, GCObject v) 
		{ 
			return (v.getGch().marked & otherwhite(g) & WHITEBITS) != 0; 
		}

		public static void changewhite(GCObject x) 
		{ 
			x.getGch().marked ^= (byte)WHITEBITS; 
		}
		
		public static void gray2black(GCObject x) 
		{ 
			Byte[] marked_ref = new Byte[1];
			GCheader gcheader = x.getGch();
			marked_ref[0] = gcheader.marked;
			l_setbit(/*ref*/ marked_ref, BLACKBIT); 
			gcheader.marked = marked_ref[0];
		}

		public static bool valiswhite(TValue x) 
		{ 
			return (LuaObject.iscollectable(x) && iswhite(LuaObject.gcvalue(x))); 
		}

		public static byte luaC_white(global_State g) 
		{ 
			return (byte)(g.currentwhite & WHITEBITS); 
		}

		public static void luaC_checkGC(lua_State L)
		{
			//condhardstacktests(luaD_reallocstack(L, L.stacksize - EXTRA_STACK - 1));
			//luaD_reallocstack(L, L.stacksize - EXTRA_STACK - 1);
			if (LuaState.G(L).totalbytes >= LuaState.G(L).GCthreshold)
			{
				luaC_step(L);
			}
		}

		public static void luaC_barrier(lua_State L, object p, TValue v)
		{
			if (valiswhite(v) && isblack(LuaState.obj2gco(p)))
			{
				luaC_barrierf(L, LuaState.obj2gco(p), LuaObject.gcvalue(v));
			}
		}

		public static void luaC_barriert(lua_State L, Table t, TValue v)
		{
			if (valiswhite(v) && isblack(LuaState.obj2gco(t)))
			{
				luaC_barrierback(L,t);
			}
		}

		public static void luaC_objbarrier(lua_State L, object p, object o)
		{
			if (iswhite(LuaState.obj2gco(o)) && isblack(LuaState.obj2gco(p)))
			{
				luaC_barrierf(L, LuaState.obj2gco(p), LuaState.obj2gco(o));
			}
		}

		public static void luaC_objbarriert(lua_State L, Table t, object o)
		{ 
			if (iswhite(LuaState.obj2gco(o)) && isblack(LuaState.obj2gco(t))) 
			{
				luaC_barrierback(L, t);
			}
		}

		public const int/*uint*/ GCSTEPSIZE	= 1024;
		public const int GCSWEEPMAX = 40;
		public const int GCSWEEPCOST = 10;
		public const int GCFINALIZECOST	= 100;

		public static byte maskmarks = (byte)(~(bitmask(BLACKBIT) | WHITEBITS));

		public static void makewhite(global_State g, GCObject x)
		{
			x.getGch().marked = (byte)(x.getGch().marked & maskmarks | luaC_white(g));
		}

		public static void white2gray(GCObject x) 
		{ 
			Byte[] marked_ref = new Byte[1];
			GCheader gcheader = x.getGch();
			marked_ref[0] = gcheader.marked;
			reset2bits(/*ref*/ marked_ref, WHITE0BIT, WHITE1BIT); 
			gcheader.marked = marked_ref[0];
		}
		
		public static void black2gray(GCObject x) 
		{ 
			Byte[] marked_ref = new Byte[1];
			GCheader gcheader = x.getGch();
			marked_ref[0] = gcheader.marked;
			resetbit(/*ref*/ marked_ref, BLACKBIT); 
			gcheader.marked = marked_ref[0];
		}

		public static void stringmark(TString s) 
		{
			Byte[] marked_ref = new Byte[1];
			GCheader gcheader = s.getGch();
			marked_ref[0] = gcheader.marked;
			reset2bits(/*ref*/ marked_ref, WHITE0BIT, WHITE1BIT);
			gcheader.marked = marked_ref[0];
		}

		public static bool isfinalized(Udata_uv u) 
		{ 
			return testbit(u.marked, FINALIZEDBIT); 
		}
		
		public static void markfinalized(Udata_uv u)
		{
			Byte/*lu_byte*/ marked = u.marked;	// can't pass properties in as ref
			Byte[] marked_ref = new Byte[1];
			marked_ref[0] = marked;
			l_setbit(/*ref*/ marked_ref, FINALIZEDBIT);
			marked = marked_ref[0];
			u.marked = marked;
		}

		public static int KEYWEAK = bitmask(KEYWEAKBIT);
		public static int VALUEWEAK = bitmask(VALUEWEAKBIT);

		public static void markvalue(global_State g, TValue o)
		{
			LuaObject.checkconsistency(o);
			if (LuaObject.iscollectable(o) && iswhite(LuaObject.gcvalue(o)))
			{
				reallymarkobject(g, LuaObject.gcvalue(o));
			}
		}

		public static void markobject(global_State g, object t)
		{
			if (iswhite(LuaState.obj2gco(t)))
			{
				reallymarkobject(g, LuaState.obj2gco(t));
			}
		}

		public static void setthreshold(global_State g)
		{
			g.GCthreshold = /*(uint)*/((g.estimate / 100) * g.gcpause);
		}

		private static void removeentry(Node n) 
		{
			LuaLimits.lua_assert(LuaObject.ttisnil(LuaTable.gval(n)));
			if (LuaObject.iscollectable(LuaTable.gkey(n)))
			{
				LuaObject.setttype(LuaTable.gkey(n), LuaObject.LUA_TDEADKEY);  /* dead key; remove it */
			}
		}

		private static void reallymarkobject(global_State g, GCObject o) 
		{
			LuaLimits.lua_assert(iswhite(o) && !isdead(g, o));
			white2gray(o);
			switch (o.getGch().tt) 
			{
				case Lua.LUA_TSTRING:
					{
						return;
					}
				case Lua.LUA_TUSERDATA:
					{
						Table mt = LuaState.gco2u(o).metatable;
						gray2black(o);  /* udata are never gray */
						if (mt != null) 
						{
							markobject(g, mt);
						}
						markobject(g, LuaState.gco2u(o).env);
						return;
					}
				case LuaObject.LUA_TUPVAL:
					{
						UpVal uv = LuaState.gco2uv(o);
						markvalue(g, uv.v);
						if (uv.v == uv.u.value)  /* closed? */
						{
							gray2black(o);  /* open upvalues are never black */
						}
						return;
					}
				case Lua.LUA_TFUNCTION:
					{
						LuaState.gco2cl(o).c.setGclist(g.gray);
						g.gray = o;
						break;
					}
				case Lua.LUA_TTABLE:
					{
						LuaState.gco2h(o).gclist = g.gray;
						g.gray = o;
						break;
					}
				case Lua.LUA_TTHREAD:
					{
						LuaState.gco2th(o).gclist = g.gray;
						g.gray = o;
						break;
					}
				case LuaObject.LUA_TPROTO:
					{
						LuaState.gco2p(o).gclist = g.gray;
						g.gray = o;
						break;
					}
				default: 
					{
						LuaLimits.lua_assert(0); 
						break;
					}
			}
		}

		private static void marktmu(global_State g) 
		{
			GCObject u = g.tmudata;
			if (u != null) 
			{
				do 
				{
					u = u.getGch().next;
					makewhite(g, u);  /* may be marked, if left from previous GC */
					reallymarkobject(g, u);
				} while (u != g.tmudata);
			}
		}

		/* move `dead' udata that need finalization to list `tmudata' */
		public static int/*uint*/ luaC_separateudata(lua_State L, int all) 
		{
			global_State g = LuaState.G(L);
			int/*uint*/ deadmem = 0;
			GCObjectRef p = new NextRef(g.mainthread);
			GCObject curr;
			while ((curr = p.get()) != null) 
			{
				if (!(iswhite(curr) || (all != 0)) || isfinalized(LuaState.gco2u(curr)))
				{
					p = new NextRef(curr.getGch());  /* don't bother with them */
				}
				else if (LuaTM.fasttm(L, LuaState.gco2u(curr).metatable, TMS.TM_GC) == null)
				{
					markfinalized(LuaState.gco2u(curr));  /* don't need finalization */
					p = new NextRef(curr.getGch());
				}
				else 
				{  
					/* must call its gc method */
					deadmem += /*(uint)*/LuaString.sizeudata(LuaState.gco2u(curr));
					markfinalized(LuaState.gco2u(curr));
					p.set(curr.getGch().next);
					/* link `curr' at the end of `tmudata' list */
					if (g.tmudata == null)  /* list is empty? */
					{
						g.tmudata = curr.getGch().next = curr;  /* creates a circular list */
					}
					else 
					{
						curr.getGch().next = g.tmudata.getGch().next;
						g.tmudata.getGch().next = curr;
						g.tmudata = curr;
					}
				}
			}
			return deadmem;
		}

		private static int traversetable(global_State g, Table h) 
		{
			int i;
			int weakkey = 0;
			int weakvalue = 0;
			/*const*/ TValue mode;
			if (h.metatable != null)
			{
				markobject(g, h.metatable);
			}
			mode = LuaTM.gfasttm(g, h.metatable, TMS.TM_MODE);
			if ((mode != null) && LuaObject.ttisstring(mode))
			{  
				/* is there a weak mode? */
				weakkey = (CharPtr.isNotEqual(LuaConf.strchr(LuaObject.svalue(mode), 'k'), null)) ? 1 : 0;
				weakvalue = (CharPtr.isNotEqual(LuaConf.strchr(LuaObject.svalue(mode), 'v'), null)) ? 1 : 0;
				if ((weakkey != 0) || (weakvalue != 0)) 
				{  
					/* is really weak? */
					h.marked &= (byte)~(KEYWEAK | VALUEWEAK);  /* clear bits */
					h.marked |= LuaLimits.cast_byte((weakkey << KEYWEAKBIT) |
						(weakvalue << VALUEWEAKBIT));
					h.gclist = g.weak;  /* must be cleared after GC, ... */
					g.weak = LuaState.obj2gco(h);  /* ... so put in the appropriate list */
				}
			}
			if ((weakkey != 0) && (weakvalue != 0)) 
			{
				return 1;
			}
			if (weakvalue == 0) 
			{
				i = h.sizearray;
				while ((i--) != 0)
				{
					markvalue(g, h.array[i]);
				}
			}
			i = LuaObject.sizenode(h);
			while ((i--) != 0) 
			{
				Node n = LuaTable.gnode(h, i);
				LuaLimits.lua_assert(LuaObject.ttype(LuaTable.gkey(n)) != LuaObject.LUA_TDEADKEY || LuaObject.ttisnil(LuaTable.gval(n)));
				if (LuaObject.ttisnil(LuaTable.gval(n)))
				{
					removeentry(n);  /* remove empty entries */
				}
				else 
				{
					LuaLimits.lua_assert(LuaObject.ttisnil(LuaTable.gkey(n)));
					if (weakkey == 0) 
					{
						markvalue(g, LuaTable.gkey(n));
					}
					if (weakvalue == 0) 
					{
						markvalue(g, LuaTable.gval(n));
					}
				}
			}
			return ((weakkey != 0) || (weakvalue != 0)) ? 1 : 0;
		}

		/*
		 ** All marks are conditional because a GC may happen while the
		 ** prototype is still being created
		 */
		private static void traverseproto(global_State g, Proto f) 
		{
			int i;
			if (f.source != null) 
			{
				stringmark(f.source);
			}
			for (i = 0; i < f.sizek; i++)  /* mark literals */
			{
				markvalue(g, f.k[i]);
			}
			for (i = 0; i < f.sizeupvalues; i++) 
			{  
				/* mark upvalue names */
				if (f.upvalues[i] != null)
				{
					stringmark(f.upvalues[i]);
				}
			}
			for (i = 0; i < f.sizep; i++) 
			{  
				/* mark nested protos */
				if (f.p[i] != null)
				{
					markobject(g, f.p[i]);
				}
			}
			for (i = 0; i < f.sizelocvars; i++) 
			{  
				/* mark local-variable names */
				if (f.locvars[i].varname != null)
				{
					stringmark(f.locvars[i].varname);
				}
			}
		}

		private static void traverseclosure(global_State g, Closure cl) 
		{
			markobject(g, cl.c.getEnv());
			if (cl.c.getIsC() != 0) 
			{
				int i;
				for (i = 0; i < cl.c.getNupvalues(); i++)  /* mark its upvalues */
				{
					markvalue(g, cl.c.upvalue[i]);
				}
			}
			else 
			{
				int i;
				LuaLimits.lua_assert(cl.l.getNupvalues() == cl.l.p.nups);
				markobject(g, cl.l.p);
				for (i = 0; i < cl.l.getNupvalues(); i++)  /* mark its upvalues */
				{
					markobject(g, cl.l.upvals[i]);
				}
			}
		}

		private static void checkstacksizes(lua_State L, TValue/*StkId*/ max)
		{
			int ci_used = LuaLimits.cast_int(CallInfo.minus(L.ci, L.base_ci[0]));  /* number of `ci' in use */
			int s_used = LuaLimits.cast_int(TValue.minus(max, L.stack));  /* part of stack in use */
			if (L.size_ci > LuaConf.LUAI_MAXCALLS)  /* handling overflow? */
			{
				return;  /* do not touch the stacks */
			}
			if (4*ci_used < L.size_ci && 2*LuaState.BASIC_CI_SIZE < L.size_ci)
			{
				LuaDo.luaD_reallocCI(L, L.size_ci / 2);  /* still big enough... */
			}
			//condhardstacktests(luaD_reallocCI(L, ci_used + 1));
			if (4*s_used < L.stacksize &&
			    2 * (LuaState.BASIC_STACK_SIZE + LuaState.EXTRA_STACK) < L.stacksize)
			{
				LuaDo.luaD_reallocstack(L, L.stacksize / 2);  /* still big enough... */
			}
			//condhardstacktests(luaD_reallocstack(L, s_used));
		}

		private static void traversestack (global_State g, lua_State l) 
		{
			TValue[]/*StkId*/ o = new TValue[1];
			o[0] = new TValue();
			TValue/*StkId*/ lim;
			CallInfo[] ci = new CallInfo[1];
			ci[0] = new CallInfo();
			markvalue(g, LuaState.gt(l));
			lim = l.top;
			for (ci[0] = l.base_ci[0]; CallInfo.lessEqual(ci[0], l.ci); CallInfo.inc(/*ref*/ ci))
			{
				LuaLimits.lua_assert(TValue.lessEqual(ci[0].top, l.stack_last));
				if (TValue.lessThan(lim, ci[0].top)) 
				{
					lim = ci[0].top;
				}
			}
			for (o[0] = l.stack[0]; TValue.lessThan(o[0], l.top); /*StkId*/TValue.inc(/*ref*/ o))
			{
				markvalue(g, o[0]);
			}
			for (; TValue.lessEqual(o[0], lim); /*StkId*/TValue.inc(/*ref*/ o))
			{
				LuaObject.setnilvalue(o[0]);
			}
			checkstacksizes(l, lim);
		}

		/*
		 ** traverse one gray object, turning it to black.
		 ** Returns `quantity' traversed.
		 */
		private static int/*Int32*//*l_mem*/ propagatemark(global_State g)
		{
			GCObject o = g.gray;
			LuaLimits.lua_assert(isgray(o));
			gray2black(o);
			switch (o.getGch().tt) 
			{
				case Lua.LUA_TTABLE:
					{
						Table h = LuaState.gco2h(o);
						g.gray = h.gclist;
						if (traversetable(g, h) != 0)  /* table is weak? */
						{
							black2gray(o);  /* keep it gray */
						}
                        return LuaConf.GetUnmanagedSize(new ClassType(ClassType.TYPE_TABLE)) + //typeof(Table)
                            LuaConf.GetUnmanagedSize(new ClassType(ClassType.TYPE_TVALUE)) * h.sizearray + //typeof(TValue)
                            LuaConf.GetUnmanagedSize(new ClassType(ClassType.TYPE_NODE)) * LuaObject.sizenode(h); //typeof(Node)
					}
				case Lua.LUA_TFUNCTION:
					{
						Closure cl = LuaState.gco2cl(o);
						g.gray = cl.c.getGclist();
						traverseclosure(g, cl);
						return (cl.c.getIsC() != 0) ? LuaFunc.sizeCclosure(cl.c.getNupvalues()) :
							LuaFunc.sizeLclosure(cl.l.getNupvalues());
					}
				case Lua.LUA_TTHREAD:
					{
						lua_State th = LuaState.gco2th(o);
						g.gray = th.gclist;
						th.gclist = g.grayagain;
						g.grayagain = o;
						black2gray(o);
						traversestack(g, th);
                        //typeof(lua_State)
                        //typeof(TValue)
                        //typeof(CallInfo)
                        return LuaConf.GetUnmanagedSize(new ClassType(ClassType.TYPE_LUA_STATE)) + 
                            LuaConf.GetUnmanagedSize(new ClassType(ClassType.TYPE_TVALUE)) * th.stacksize + 
                            LuaConf.GetUnmanagedSize(new ClassType(ClassType.TYPE_CALLINFO)) * th.size_ci; 
					}
				case LuaObject.LUA_TPROTO:
					{
						Proto p = LuaState.gco2p(o);
						g.gray = p.gclist;
						traverseproto(g, p);
                        //typeof(Proto)
                        //typeof(long/*UInt32*//*Instruction*/)
                        //typeof(Proto)
                        //typeof(TValue)
                        //typeof(int)
                        //typeof(LocVar)
                        //typeof(TString)
                        return LuaConf.GetUnmanagedSize(new ClassType(ClassType.TYPE_PROTO)) + 
                            LuaConf.GetUnmanagedSize(new ClassType(ClassType.TYPE_LONG)) * p.sizecode + 
                            LuaConf.GetUnmanagedSize(new ClassType(ClassType.TYPE_PROTO)) * p.sizep + 
                            LuaConf.GetUnmanagedSize(new ClassType(ClassType.TYPE_TVALUE)) * p.sizek + 
                            LuaConf.GetUnmanagedSize(new ClassType(ClassType.TYPE_INT)) * p.sizelineinfo + 
                            LuaConf.GetUnmanagedSize(new ClassType(ClassType.TYPE_LOCVAR)) * p.sizelocvars + 
                            LuaConf.GetUnmanagedSize(new ClassType(ClassType.TYPE_TSTRING)) * p.sizeupvalues; 
					}
				default: 
					{
						LuaLimits.lua_assert(0); 
						return 0;
					}
			}
		}

		private static int/*uint*/ propagateall(global_State g) 
		{
			int/*uint*/ m = 0;
			while (g.gray != null) 
			{
				m += /*(uint)*/propagatemark(g);
			}
			return m;
		}

		/*
		 ** The next function tells whether a key or value can be cleared from
		 ** a weak table. Non-collectable objects are never removed from weak
		 ** tables. Strings behave as `values', so are never removed too. for
		 ** other objects: if really collected, cannot keep them; for userdata
		 ** being finalized, keep them in keys, but not in values
		 */
		private static bool iscleared (TValue o, bool iskey) 
		{
			if (!LuaObject.iscollectable(o)) 
			{
				return false;
			}
			if (LuaObject.ttisstring(o))
			{
				stringmark(LuaObject.rawtsvalue(o));  /* strings are `values', so are never weak */
				return false;
			}
			return iswhite(LuaObject.gcvalue(o)) ||
				(LuaObject.ttisuserdata(o) && (!iskey && isfinalized(LuaObject.uvalue(o))));
		}


		/*
		 ** clear collected entries from weaktables
		 */
		private static void cleartable(GCObject l) 
		{
			while (l != null) 
			{
				Table h = LuaState.gco2h(l);
				int i = h.sizearray;
				LuaLimits.lua_assert(testbit(h.marked, VALUEWEAKBIT) ||
					testbit(h.marked, KEYWEAKBIT));
				if (testbit(h.marked, VALUEWEAKBIT)) 
				{
					while (i--!= 0) 
					{
						TValue o = h.array[i];
						if (iscleared(o, false))  /* value was collected? */
						{
							LuaObject.setnilvalue(o);  /* remove value */
						}
					}
				}
				i = LuaObject.sizenode(h);
				while (i-- != 0) 
				{
					Node n = LuaTable.gnode(h, i);
					if (!LuaObject.ttisnil(LuaTable.gval(n)) &&  /* non-empty entry? */
						(iscleared(LuaTable.key2tval(n), true) || iscleared(LuaTable.gval(n), false)))
					{
						LuaObject.setnilvalue(LuaTable.gval(n));  /* remove value ... */
						removeentry(n);  /* remove entry from Table */
					}
				}
				l = h.gclist;
			}
		}


		private static void freeobj(lua_State L, GCObject o) 
		{
			switch (o.getGch().tt) 
			{
				case LuaObject.LUA_TPROTO: 
					{
						LuaFunc.luaF_freeproto(L, LuaState.gco2p(o)); 
						break;
					}
				case Lua.LUA_TFUNCTION: 
					{
						LuaFunc.luaF_freeclosure(L, LuaState.gco2cl(o)); 
						break;
					}
				case LuaObject.LUA_TUPVAL: 
					{
						LuaFunc.luaF_freeupval(L, LuaState.gco2uv(o)); 
						break;
					}
				case Lua.LUA_TTABLE: 
					{
						LuaTable.luaH_free(L, LuaState.gco2h(o)); 
						break;
					}
				case Lua.LUA_TTHREAD:
					{
						LuaLimits.lua_assert(LuaState.gco2th(o) != L && LuaState.gco2th(o) != LuaState.G(L).mainthread);
						LuaState.luaE_freethread(L, LuaState.gco2th(o));
						break;
					}
				case Lua.LUA_TSTRING: 
					{
						LuaState.G(L).strt.nuse--;
						LuaMem.SubtractTotalBytes(L, LuaString.sizestring(LuaState.gco2ts(o)));
						LuaMem.luaM_freemem_TString(L, LuaState.gco2ts(o), new ClassType(ClassType.TYPE_TSTRING));
						break;
					}
				case Lua.LUA_TUSERDATA:
					{
						LuaMem.SubtractTotalBytes(L, LuaString.sizeudata(LuaState.gco2u(o)));
						LuaMem.luaM_freemem_Udata(L, LuaState.gco2u(o), new ClassType(ClassType.TYPE_UDATA));
						break;
					}
				default: 
					{
						LuaLimits.lua_assert(0); 
						break;
					}
			}
		}

		public static void sweepwholelist(lua_State L, GCObjectRef p) 
		{ 
			sweeplist(L, p, LuaLimits.MAX_LUMEM); 
		}

		private static GCObjectRef sweeplist(lua_State L, GCObjectRef p, long/*UInt32*//*lu_mem*/ count)
		{
			GCObject curr;
			global_State g = LuaState.G(L);
			int deadmask = otherwhite(g);
			while ((curr = p.get()) != null && count-- > 0) 
			{
				if (curr.getGch().tt == Lua.LUA_TTHREAD)  /* sweep open upvalues of each thread */
				{
					sweepwholelist(L, new OpenValRef(LuaState.gco2th(curr)));
				}
				if (((curr.getGch().marked ^ WHITEBITS) & deadmask) != 0) 
				{  
					/* not dead? */
					LuaLimits.lua_assert(isdead(g, curr) || testbit(curr.getGch().marked, FIXEDBIT));
					makewhite(g, curr);  /* make it white (for next cycle) */
					p = new NextRef(curr.getGch());
				}
				else 
				{  
					/* must erase `curr' */
					LuaLimits.lua_assert(isdead(g, curr) || deadmask == bitmask(SFIXEDBIT));
					p.set( curr.getGch().next );
					if (curr == g.rootgc)  /* is the first element of the list? */
					{
						g.rootgc = curr.getGch().next;  /* adjust first */
					}
					freeobj(L, curr);
				}
			}
			return p;
		}

		private static void checkSizes(lua_State L) 
		{
			global_State g = LuaState.G(L);
			/* check size of string hash */
			if (g.strt.nuse < (long/*UInt32*//*lu_int32*/)(g.strt.size / 4) &&
			    g.strt.size > LuaLimits.MINSTRTABSIZE * 2)
			{
				LuaString.luaS_resize(L, g.strt.size / 2);  /* table is too big */
			}
			/* check size of buffer */
			if (LuaZIO.luaZ_sizebuffer(g.buff) > LuaLimits.LUA_MINBUFFER * 2)
			{  
				/* buffer too big? */
				int/*uint*/ newsize = LuaZIO.luaZ_sizebuffer(g.buff) / 2;
				LuaZIO.luaZ_resizebuffer(L, g.buff, /*(int)*/newsize);
			}
		}

		private static void GCTM(lua_State L) 
		{
			global_State g = LuaState.G(L);
			GCObject o = g.tmudata.getGch().next;  /* get first element */
			Udata udata = LuaState.rawgco2u(o);
			TValue tm;
			/* remove udata from `tmudata' */
			if (o == g.tmudata)  /* last element? */
			{
				g.tmudata = null;
			}
			else
			{
				g.tmudata.getGch().next = udata.uv.next;
			}
			udata.uv.next = g.mainthread.next;  /* return it to `root' list */
			g.mainthread.next = o;
			makewhite(g, o);
			tm = LuaTM.fasttm(L, udata.uv.metatable, TMS.TM_GC);
			if (tm != null) 
			{
				Byte/*lu_byte*/ oldah = L.allowhook;
				long/*UInt32*//*lu_mem*/ oldt = (long/*UInt32*//*lu_mem*/)g.GCthreshold;
				L.allowhook = 0;  /* stop debug hooks during GC tag method */
				g.GCthreshold = 2*g.totalbytes;  /* avoid GC steps */
				LuaObject.setobj2s(L, L.top, tm);
				LuaObject.setuvalue(L, TValue.plus(L.top, 1), udata);
				L.top = TValue.plus(L.top, 2);
				LuaDo.luaD_call(L, TValue.minus(L.top, 2), 0);
				L.allowhook = oldah;  /* restore hooks */
				g.GCthreshold = /*(uint)*/oldt;  /* restore threshold */
			}
		}

		/*
		 ** Call all GC tag methods
		 */
		public static void luaC_callGCTM(lua_State L) 
		{
			while (LuaState.G(L).tmudata != null)
			{
				GCTM(L);
			}
		}

		public static void luaC_freeall(lua_State L) 
		{
			global_State g = LuaState.G(L);
			int i;
			g.currentwhite = (byte)(WHITEBITS | bitmask(SFIXEDBIT));  /* mask to collect all elements */
			sweepwholelist(L, new RootGCRef(g));
			for (i = 0; i < g.strt.size; i++)  /* free all string lists */
			{
				sweepwholelist(L, new ArrayRef(g.strt.hash, i));
			}
		}

		private static void markmt(global_State g) 
		{
			int i;
			for (i = 0; i < LuaObject.NUM_TAGS; i++)
			{
				if (g.mt[i] != null) 
				{
					markobject(g, g.mt[i]);
				}
			}
		}

		/* mark root set */
		private static void markroot(lua_State L) 
		{
			global_State g = LuaState.G(L);
			g.gray = null;
			g.grayagain = null;
			g.weak = null;
			markobject(g, g.mainthread);
			/* make global table be traversed before main stack */
			markvalue(g, LuaState.gt(g.mainthread));
			markvalue(g, LuaState.registry(L));
			markmt(g);
			g.gcstate = GCSpropagate;
		}

		private static void remarkupvals(global_State g) 
		{
			UpVal uv;
			for (uv = g.uvhead.u.l.next; uv != g.uvhead; uv = uv.u.l.next) 
			{
				LuaLimits.lua_assert(uv.u.l.next.u.l.prev == uv && uv.u.l.prev.u.l.next == uv);
				if (isgray(LuaState.obj2gco(uv)))
				{
					markvalue(g, uv.v);
				}
			}
		}

		private static void atomic(lua_State L) 
		{
			global_State g = LuaState.G(L);
			int/*uint*/ udsize;  /* total size of userdata to be finalized */
			/* remark occasional upvalues of (maybe) dead threads */
			remarkupvals(g);
			/* traverse objects cautch by write barrier and by 'remarkupvals' */
			propagateall(g);
			/* remark weak tables */
			g.gray = g.weak;
			g.weak = null;
			LuaLimits.lua_assert(!iswhite(LuaState.obj2gco(g.mainthread)));
			markobject(g, L);  /* mark running thread */
			markmt(g);  /* mark basic metatables (again) */
			propagateall(g);
			/* remark gray again */
			g.gray = g.grayagain;
			g.grayagain = null;
			propagateall(g);
			udsize = luaC_separateudata(L, 0);  /* separate userdata to be finalized */
			marktmu(g);  /* mark `preserved' userdata */
			udsize += propagateall(g);  /* remark, to propagate `preserveness' */
			cleartable(g.weak);  /* remove collected objects from weak tables */
			/* flip current white */
			g.currentwhite = LuaLimits.cast_byte(otherwhite(g));
			g.sweepstrgc = 0;
			g.sweepgc = new RootGCRef(g);
			g.gcstate = GCSsweepstring;
			g.estimate = g.totalbytes - udsize;  /* first estimate */
		}

		private static int/*Int32*//*l_mem*/ singlestep(lua_State L)
		{
			global_State g = LuaState.G(L);
			/*lua_checkmemory(L);*/
			switch (g.gcstate) 
			{
				case GCSpause: 
					{
						markroot(L);  /* start a new collection */
						return 0;
					}
				case GCSpropagate: 
					{
						if (g.gray != null)
						{
							return propagatemark(g);
						}
						else 
						{  
							/* no more `gray' objects */
							atomic(L);  /* finish mark phase */
							return 0;
						}
					}
				case GCSsweepstring: 
					{
						long/*UInt32*//*lu_mem*/ old = (long/*UInt32*//*lu_mem*/)g.totalbytes;
						sweepwholelist(L, new ArrayRef(g.strt.hash, g.sweepstrgc++));
						if (g.sweepstrgc >= g.strt.size)  /* nothing more to sweep? */
						{
							g.gcstate = GCSsweep;  /* end sweep-string phase */
						}
						LuaLimits.lua_assert(old >= g.totalbytes);
						g.estimate -= /*(uint)*/(old - g.totalbytes);
						return GCSWEEPCOST;
					}
				case GCSsweep: 
					{
						long/*UInt32*//*lu_mem*/ old = (long/*UInt32*//*lu_mem*/)g.totalbytes;
						g.sweepgc = sweeplist(L, g.sweepgc, GCSWEEPMAX);
						if (g.sweepgc.get() == null) 
						{  
							/* nothing more to sweep? */
							checkSizes(L);
							g.gcstate = GCSfinalize;  /* end sweep phase */
						}
						LuaLimits.lua_assert(old >= g.totalbytes);
						g.estimate -= /*(uint)*/(old - g.totalbytes);
						return GCSWEEPMAX*GCSWEEPCOST;
					}
				case GCSfinalize: 
					{
						if (g.tmudata != null) 
						{
							GCTM(L);
							if (g.estimate > GCFINALIZECOST)
							{
								g.estimate -= GCFINALIZECOST;
							}
							return GCFINALIZECOST;
						}
						else 
						{
							g.gcstate = GCSpause;  /* end collection */
							g.gcdept = 0;
							return 0;
						}
					}
				default: 
					{
						LuaLimits.lua_assert(0); 
						return 0;
					}
			}
		}

		public static void luaC_step (lua_State L) {
			global_State g = LuaState.G(L);
			int/*Int32*//*l_mem*/ lim = (int/*Int32*//*l_mem*/)((GCSTEPSIZE / 100) * g.gcstepmul);
			if (lim == 0)
			{
				lim = (int/*Int32*//*l_mem*/)((LuaLimits.MAX_LUMEM - 1) / 2);  /* no limit */
			}
			g.gcdept += g.totalbytes - g.GCthreshold;
			do 
			{
				lim -= singlestep(L);
				if (g.gcstate == GCSpause)
				{
					break;
				}
			} while (lim > 0);
			if (g.gcstate != GCSpause) 
			{
				if (g.gcdept < GCSTEPSIZE)
				{
					g.GCthreshold = g.totalbytes + GCSTEPSIZE;  /* - lim/g.gcstepmul;*/
				}
				else 
				{
					g.gcdept -= GCSTEPSIZE;
					g.GCthreshold = g.totalbytes;
				}
			}
			else 
			{
				LuaLimits.lua_assert(g.totalbytes >= g.estimate);
				setthreshold(g);
			}
		}

		public static void luaC_fullgc(lua_State L) 
		{
			global_State g = LuaState.G(L);
			if (g.gcstate <= GCSpropagate) 
			{
				/* reset sweep marks to sweep all elements (returning them to white) */
				g.sweepstrgc = 0;
				g.sweepgc = new RootGCRef(g);
				/* reset other collector lists */
				g.gray = null;
				g.grayagain = null;
				g.weak = null;
				g.gcstate = GCSsweepstring;
			}
			LuaLimits.lua_assert(g.gcstate != GCSpause && g.gcstate != GCSpropagate);
			/* finish any pending sweep phase */
			while (g.gcstate != GCSfinalize) 
			{
				LuaLimits.lua_assert(g.gcstate == GCSsweepstring || g.gcstate == GCSsweep);
				singlestep(L);
			}
			markroot(L);
			while (g.gcstate != GCSpause) 
			{
				singlestep(L);
			}
			setthreshold(g);
		}

		public static void luaC_barrierf(lua_State L, GCObject o, GCObject v) 
		{
			global_State g = LuaState.G(L);
			LuaLimits.lua_assert(isblack(o) && iswhite(v) && !isdead(g, v) && !isdead(g, o));
			LuaLimits.lua_assert(g.gcstate != GCSfinalize && g.gcstate != GCSpause);
			LuaLimits.lua_assert(LuaObject.ttype(o.getGch()) != Lua.LUA_TTABLE);
			/* must keep invariant? */
			if (g.gcstate == GCSpropagate)
			{
				reallymarkobject(g, v);  /* restore invariant */
			}
			else  /* don't mind */
			{
				makewhite(g, o);  /* mark as white just to avoid other barriers */
			}
		}


		public static void luaC_barrierback(lua_State L, Table t)
		{
			global_State g = LuaState.G(L);
			GCObject o = LuaState.obj2gco(t);
			LuaLimits.lua_assert(isblack(o) && !isdead(g, o));
			LuaLimits.lua_assert(g.gcstate != GCSfinalize && g.gcstate != GCSpause);
			black2gray(o);  /* make table gray (again) */
			t.gclist = g.grayagain;
			g.grayagain = o;
		}

		public static void luaC_link(lua_State L, GCObject o, Byte/*lu_byte*/ tt)
		{
			global_State g = LuaState.G(L);
			o.getGch().next = g.rootgc;
			g.rootgc = o;
			o.getGch().marked = luaC_white(g);
			o.getGch().tt = tt;
		}

		public static void luaC_linkupval(lua_State L, UpVal uv) 
		{
			global_State g = LuaState.G(L);
			GCObject o = LuaState.obj2gco(uv);
			o.getGch().next = g.rootgc;  /* link upvalue into `rootgc' list */
			g.rootgc = o;
			if (isgray(o)) 
			{
				if (g.gcstate == GCSpropagate) 
				{
					gray2black(o);  /* closed upvalues need barrier */
					luaC_barrier(L, uv, uv.v);
				}
				else 
				{  
					/* sweep phase: sweep it (turning it into white) */
					makewhite(g, o);
					LuaLimits.lua_assert(g.gcstate != GCSfinalize && g.gcstate != GCSpause);
				}
			}
		}
	}
}
