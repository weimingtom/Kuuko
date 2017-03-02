/*
 ** $Id: lstate.c,v 2.36.1.2 2008/01/03 15:20:39 roberto Exp $
 ** Global State
 ** See Copyright Notice in lua.h
 */
using System;

namespace KopiLua
{
	//using lu_byte = System.Byte;
	//using lu_int32 = System.Int32;
	//using lu_mem = System.UInt32;
	//using TValue = Lua.TValue;
	//using StkId = TValue;
	//using ptrdiff_t = System.Int32;
	//using Instruction = System.UInt32;

	public class LuaState
	{
		/* table of globals */
		public static TValue gt(lua_State L)	
		{
			return L.l_gt;
		}

		/* registry */
		public static TValue registry(lua_State L)	
		{
			return G(L).l_registry;
		}

		/* extra stack space to handle TM calls and some other extras */
		public const int EXTRA_STACK = 5;

		public const int BASIC_CI_SIZE = 8;

		public const int BASIC_STACK_SIZE = (2*Lua.LUA_MINSTACK);

		public static Closure curr_func(lua_State L) 
		{ 
			return (LuaObject.clvalue(L.ci.func)); 
		}
		
		public static Closure ci_func(CallInfo ci) 
		{ 
			return (LuaObject.clvalue(ci.func)); 
		}
		
		public static bool f_isLua(CallInfo ci) 
		{ 
			return ci_func(ci).c.getIsC() == 0; 
		}
		
		public static bool isLua(CallInfo ci) 
		{ 
			return (LuaObject.ttisfunction((ci).func) && f_isLua(ci)); 
		}

		public static global_State G(lua_State L)	
		{
			return L.l_G;
		}
		
		public static void G_set(lua_State L, global_State s) 
		{ 
			L.l_G = s; 
		}

		/* macros to convert a GCObject into a specific value */
		public static TString rawgco2ts(GCObject o) 
		{ 
			return (TString)LuaLimits.check_exp(o.getGch().tt == Lua.LUA_TSTRING, o.getTs()); 
		}
		
		public static TString gco2ts(GCObject o) 
		{ 
			return (TString)(rawgco2ts(o).getTsv()); 
		}
		
		public static Udata rawgco2u(GCObject o) 
		{
            return (Udata)LuaLimits.check_exp(o.getGch().tt == Lua.LUA_TUSERDATA, o.getU()); 
		}
		
		public static Udata gco2u(GCObject o) 
		{ 
			return (Udata)(rawgco2u(o).uv); 
		}
		
		public static Closure gco2cl(GCObject o) 
		{
            return (Closure)LuaLimits.check_exp(o.getGch().tt == Lua.LUA_TFUNCTION, o.getCl()); 
		}
		
		public static Table gco2h(GCObject o) 
		{
            return (Table)LuaLimits.check_exp(o.getGch().tt == Lua.LUA_TTABLE, o.getH()); 
		}
		
		public static Proto gco2p(GCObject o) 
		{
            return (Proto)LuaLimits.check_exp(o.getGch().tt == LuaObject.LUA_TPROTO, o.getP()); 
		}
		
		public static UpVal gco2uv(GCObject o) 
		{
            return (UpVal)LuaLimits.check_exp(o.getGch().tt == LuaObject.LUA_TUPVAL, o.getUv()); 
		}
		
		public static UpVal ngcotouv(GCObject o) 
		{
            return (UpVal)LuaLimits.check_exp((o == null) || (o.getGch().tt == LuaObject.LUA_TUPVAL), o.getUv()); 
		}
		
		public static lua_State gco2th(GCObject o) 
		{
            return (lua_State)LuaLimits.check_exp(o.getGch().tt == Lua.LUA_TTHREAD, o.getTh()); 
		}

		/* macro to convert any Lua object into a GCObject */
		public static GCObject obj2gco(object v)	
		{
			return (GCObject)v;
		}

		public static int state_size(object x, ClassType t) 
		{
            return t.GetMarshalSizeOf() + LuaConf.LUAI_EXTRASPACE; //Marshal.SizeOf(x) 
		}
		
		/*
		public static lu_byte fromstate(object l)
		{
			return (lu_byte)(l - LUAI_EXTRASPACE);
		}
		*/
		
		public static lua_State tostate(object l)
		{
			ClassType.Assert(LuaConf.LUAI_EXTRASPACE == 0, "LUAI_EXTRASPACE not supported");
			return (lua_State)l;
		}

		private static void stack_init(lua_State L1, lua_State L) 
		{
			/* initialize CallInfo array */
			L1.base_ci = LuaMem.luaM_newvector_CallInfo(L, BASIC_CI_SIZE, new ClassType(ClassType.TYPE_CALLINFO));
			L1.ci = L1.base_ci[0];
			L1.size_ci = BASIC_CI_SIZE;
			L1.end_ci = L1.base_ci[L1.size_ci - 1];
			/* initialize stack array */
			L1.stack = LuaMem.luaM_newvector_TValue(L, BASIC_STACK_SIZE + EXTRA_STACK, new ClassType(ClassType.TYPE_TVALUE));
			L1.stacksize = BASIC_STACK_SIZE + EXTRA_STACK;
			L1.top = L1.stack[0];
			L1.stack_last = L1.stack[L1.stacksize - EXTRA_STACK - 1];
			/* initialize first ci */
			L1.ci.func = L1.top;
			TValue[] top = new TValue[1];
			top[0] = L1.top;
			TValue ret = /*StkId*/TValue.inc(/*ref*/ top);
			L1.top = top[0];
			LuaObject.setnilvalue(ret);  /* `function' entry for this `ci' */
			L1.base_ = L1.ci.base_ = L1.top;
			L1.ci.top = TValue.plus(L1.top, Lua.LUA_MINSTACK);
		}

		private static void freestack(lua_State L, lua_State L1) 
		{
			LuaMem.luaM_freearray_CallInfo(L, L1.base_ci, new ClassType(ClassType.TYPE_CALLINFO));
			LuaMem.luaM_freearray_TValue(L, L1.stack, new ClassType(ClassType.TYPE_TVALUE));
		}

		/*
		 ** open parts that may cause memory-allocation errors
		 */
		private static void f_luaopen(lua_State L, object ud) 
		{
			global_State g = G(L);
			//UNUSED(ud);
			stack_init(L, L);  /* init stack */
			LuaObject.sethvalue(L, gt(L), LuaTable.luaH_new(L, 0, 2));  /* table of globals */
			LuaObject.sethvalue(L, registry(L), LuaTable.luaH_new(L, 0, 2));  /* registry */
			LuaString.luaS_resize(L, LuaLimits.MINSTRTABSIZE);  /* initial size of string table */
			LuaTM.luaT_init(L);
			LuaLex.luaX_init(L);
			LuaString.luaS_fix(LuaString.luaS_newliteral(L, CharPtr.toCharPtr(LuaMem.MEMERRMSG)));
			g.GCthreshold = 4 * g.totalbytes;
		}
		
		public class f_luaopen_delegate : Pfunc
		{
			public void exec(lua_State L, object ud)
			{
				f_luaopen(L, ud);
			}
		}
		
		private static void preinit_state(lua_State L, global_State g) 
		{
			G_set(L, g);
			L.stack = null;
			L.stacksize = 0;
			L.errorJmp = null;
			L.hook = null;
			L.hookmask = 0;
			L.basehookcount = 0;
			L.allowhook = 1;
			LuaDebug.resethookcount(L);
			L.openupval = null;
			L.size_ci = 0;
			L.nCcalls = L.baseCcalls = 0;
			L.status = 0;
			L.base_ci = null;
			L.ci = null;
			L.savedpc = new InstructionPtr();
			L.errfunc = 0;
			LuaObject.setnilvalue(gt(L));
		}


		private static void close_state(lua_State L) 
		{
			global_State g = G(L);
			LuaFunc.luaF_close(L, L.stack[0]);  /* close all upvalues for this thread */
			LuaGC.luaC_freeall(L);  /* collect all objects */
			LuaLimits.lua_assert(g.rootgc == obj2gco(L));
			LuaLimits.lua_assert(g.strt.nuse == 0);
			LuaMem.luaM_freearray_GCObject(L, G(L).strt.hash, new ClassType(ClassType.TYPE_GCOBJECT));
			LuaZIO.luaZ_freebuffer(L, g.buff);
			freestack(L, L);
            LuaLimits.lua_assert(g.totalbytes == LuaConf.GetUnmanagedSize(new ClassType(ClassType.TYPE_LG))); //typeof(LG)
			//g.frealloc(g.ud, fromstate(L), (uint)state_size(typeof(LG)), 0);
		}

		//private
		public static lua_State luaE_newthread(lua_State L)
		{
			//lua_State L1 = tostate(luaM_malloc(L, state_size(typeof(lua_State))));
			lua_State L1 = LuaMem.luaM_new_lua_State(L, new ClassType(ClassType.TYPE_LUA_STATE));
			LuaGC.luaC_link(L, obj2gco(L1), (byte)Lua.LUA_TTHREAD);
			preinit_state(L1, G(L));
			stack_init(L1, L);  /* init stack */
			LuaObject.setobj2n(L, gt(L1), gt(L));  /* share table of globals */
			L1.hookmask = L.hookmask;
			L1.basehookcount = L.basehookcount;
			L1.hook = L.hook;
			LuaDebug.resethookcount(L1);
			LuaLimits.lua_assert(LuaGC.iswhite(obj2gco(L1)));
			return L1;
		}

		//private
		public static void luaE_freethread(lua_State L, lua_State L1) 
		{
			LuaFunc.luaF_close(L1, L1.stack[0]);  /* close all upvalues for this thread */
			LuaLimits.lua_assert(L1.openupval == null);
			LuaConf.luai_userstatefree(L1);
			freestack(L, L1);
			//luaM_freemem(L, fromstate(L1));
		}

		public static lua_State lua_newstate(lua_Alloc f, object ud) 
		{
			int i;
			lua_State L;
			global_State g;
			//object l = f(ud, null, 0, (uint)state_size(typeof(LG)));
            object l = f.exec(new ClassType(ClassType.TYPE_LG)); //typeof(LG)
			if (l == null) 
			{
				return null;
			}
			L = tostate(l);
			g = (L as LG).g;
			L.next = null;
			L.tt = Lua.LUA_TTHREAD;
			g.currentwhite = (Byte/*lu_byte*/)LuaGC.bit2mask(LuaGC.WHITE0BIT, LuaGC.FIXEDBIT);
			L.marked = LuaGC.luaC_white(g);
			Byte/*lu_byte*/ marked = L.marked;	// can't pass properties in as ref
			Byte[] marked_ref = new Byte[1];
			marked_ref[0] = marked;
			LuaGC.set2bits(/*ref*/ marked_ref, LuaGC.FIXEDBIT, LuaGC.SFIXEDBIT);
			marked = marked_ref[0];
			L.marked = marked;
			preinit_state(L, g);
			g.frealloc = f;
			g.ud = ud;
			g.mainthread = L;
			g.uvhead.u.l.prev = g.uvhead;
			g.uvhead.u.l.next = g.uvhead;
			g.GCthreshold = 0;  /* mark it as unfinished state */
			g.strt.size = 0;
			g.strt.nuse = 0;
			g.strt.hash = null;
			LuaObject.setnilvalue(registry(L));
			LuaZIO.luaZ_initbuffer(L, g.buff);
			g.panic = null;
			g.gcstate = LuaGC.GCSpause;
			g.rootgc = obj2gco(L);
			g.sweepstrgc = 0;
			g.sweepgc = new RootGCRef(g);
			g.gray = null;
			g.grayagain = null;
			g.weak = null;
			g.tmudata = null;
            g.totalbytes = (long/*uint*/)LuaConf.GetUnmanagedSize(new ClassType(ClassType.TYPE_LG));//typeof(LG)
			g.gcpause = LuaConf.LUAI_GCPAUSE;
			g.gcstepmul = LuaConf.LUAI_GCMUL;
			g.gcdept = 0;
			for (i = 0; i < LuaObject.NUM_TAGS; i++) 
			{
				g.mt[i] = null;
			}
			if (LuaDo.luaD_rawrunprotected(L, new f_luaopen_delegate(), null) != 0)
			{
				/* memory allocation error: free partial state */
				close_state(L);
				L = null;
			}
			else
			{
				LuaConf.luai_userstateopen(L);
			}
			return L;
		}

		private static void callallgcTM (lua_State L, object ud) 
		{
			//UNUSED(ud);
			LuaGC.luaC_callGCTM(L);  /* call GC metamethods for all udata */
		}
		
		public class callallgcTM_delegate : Pfunc
		{
			public void exec(lua_State L, object ud)
			{
				callallgcTM(L, ud);
			}
		}      

		public static void lua_close (lua_State L) 
		{
			L = G(L).mainthread;  /* only the main thread can be closed */
			LuaLimits.lua_lock(L);
			LuaFunc.luaF_close(L, L.stack[0]);  /* close all upvalues for this thread */
			LuaGC.luaC_separateudata(L, 1);  /* separate udata that have GC metamethods */
			L.errfunc = 0;  /* no error function during GC metamethods */
			do 
			{  
				/* repeat until no more errors */
				L.ci = L.base_ci[0];
				L.base_ = L.top = L.ci.base_;
				L.nCcalls = L.baseCcalls = 0;
			} while (LuaDo.luaD_rawrunprotected(L, new callallgcTM_delegate(), null) != 0);
			LuaLimits.lua_assert(G(L).tmudata == null);
			LuaConf.luai_userstateclose(L);
			close_state(L);
		}
	}
}
