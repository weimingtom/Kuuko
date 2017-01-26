/*
 ** $Id: lapi.c,v 2.55.1.5 2008/07/04 18:41:18 roberto Exp $
 ** Lua API
 ** See Copyright Notice in lua.h
 */
using System;

namespace KopiLua
{
	//using lu_mem = System.UInt32;
	//using TValue = Lua.TValue;
	//using StkId = Lua.TValue;
	//using lua_Integer = System.Int32;
	//using lua_Number = System.Double;
	//using ptrdiff_t = System.Int32;

	public class LuaAPI
	{
		public const string lua_ident =
			"$Lua: " + Lua.LUA_RELEASE + " " + Lua.LUA_COPYRIGHT + " $\n" +
			"$Authors: " + Lua.LUA_AUTHORS + " $\n" +
			"$URL: www.lua.org $\n";

		public static void api_checknelems(lua_State L, int n)
		{
			LuaLimits.api_check(L, n <= TValue.minus(L.top, L.base_));
		}

		public static void api_checkvalidindex(lua_State L, TValue/*StkId*/ i)
		{
			LuaLimits.api_check(L, i != LuaObject.luaO_nilobject);
		}

		public static void api_incr_top(lua_State L)
		{
			LuaLimits.api_check(L, TValue.lessThan(L.top, L.ci.top));
			TValue[] top = new TValue[1];
			top[0] = L.top;
			/*StkId*/TValue.inc(/*ref*/ top);
			L.top = top[0];
		}

		static TValue index2adr(lua_State L, int idx)
		{
			if (idx > 0)
			{
				TValue o = TValue.plus(L.base_, (idx - 1));
				LuaLimits.api_check(L, idx <= TValue.minus(L.ci.top, L.base_));
				if (TValue.greaterEqual(o, L.top))
				{
					return LuaObject.luaO_nilobject;
				}
				else
				{
					return o;
				}
			}
			else if (idx > Lua.LUA_REGISTRYINDEX)
			{
				LuaLimits.api_check(L, idx != 0 && -idx <= TValue.minus(L.top, L.base_));
				return TValue.plus(L.top, idx);
			}
			else
			{
				switch (idx)
				{
						/* pseudo-indices */
					case Lua.LUA_REGISTRYINDEX:
						{
							return LuaState.registry(L);
						}
					case Lua.LUA_ENVIRONINDEX:
						{
							Closure func = LuaState.curr_func(L);
							LuaObject.sethvalue(L, L.env, func.c.getEnv());
							return L.env;
						}
					case Lua.LUA_GLOBALSINDEX:
						{
							return LuaState.gt(L);
						}
					default:
						{
							Closure func = LuaState.curr_func(L);
							idx = Lua.LUA_GLOBALSINDEX - idx;
							return (idx <= func.c.getNupvalues())
								? func.c.upvalue[idx-1]
								: (TValue)LuaObject.luaO_nilobject;
						}
				}
			}
		}

		private static Table getcurrenv(lua_State L)
		{
			if (L.ci == L.base_ci[0])  /* no enclosing function? */
			{
				return LuaObject.hvalue(LuaState.gt(L));  /* use global table as environment */
			}
			else
			{
				Closure func = LuaState.curr_func(L);
				return func.c.getEnv();
			}
		}

		public static void luaA_pushobject(lua_State L, TValue o)
		{
			LuaObject.setobj2s(L, L.top, o);
			api_incr_top(L);
		}

		public static int lua_checkstack(lua_State L, int size)
		{
			int res = 1;
			LuaLimits.lua_lock(L);
			if (size > LuaConf.LUAI_MAXCSTACK || (TValue.minus(L.top, L.base_) + size) > LuaConf.LUAI_MAXCSTACK)
			{
				res = 0;  /* stack overflow */
			}
			else if (size > 0)
			{
				LuaDo.luaD_checkstack(L, size);
				if (TValue.lessThan(L.ci.top, TValue.plus(L.top, size)))
				{
					L.ci.top = TValue.plus(L.top, size);
				}
			}
			LuaLimits.lua_unlock(L);
			return res;
		}

		public static void lua_xmove(lua_State from, lua_State to, int n)
		{
			int i;
            if (from == to)
            {
                return;
            }
            LuaLimits.lua_lock(to);
			api_checknelems(from, n);
			LuaLimits.api_check(from, LuaState.G(from) == LuaState.G(to));
			LuaLimits.api_check(from, TValue.minus(to.ci.top, to.top) >= n);
			from.top = TValue.minus(from.top, n);
			for (i = 0; i < n; i++)
			{
				TValue[] top = new TValue[1];
				top[0] = to.top;
				TValue ret = /*StkId*/TValue.inc(/*ref*/ top);
				to.top = top[0];
				LuaObject.setobj2s(to, ret, TValue.plus(from.top, i));
			}
			LuaLimits.lua_unlock(to);
		}

		public static void lua_setlevel(lua_State from, lua_State to)
		{
			to.nCcalls = from.nCcalls;
		}

		public static lua_CFunction lua_atpanic(lua_State L, lua_CFunction panicf)
		{
			lua_CFunction old;
			LuaLimits.lua_lock(L);
			old = LuaState.G(L).panic;
			LuaState.G(L).panic = panicf;
			LuaLimits.lua_unlock(L);
			return old;
		}

		public static lua_State lua_newthread(lua_State L)
		{
			lua_State L1;
			LuaLimits.lua_lock(L);
			LuaGC.luaC_checkGC(L);
			L1 = LuaState.luaE_newthread(L);
			LuaObject.setthvalue(L, L.top, L1);
			api_incr_top(L);
			LuaLimits.lua_unlock(L);
			LuaConf.luai_userstatethread(L, L1);
			return L1;
		}

		/*
		 ** basic stack manipulation
		 */
		public static int lua_gettop(lua_State L)
		{
			return LuaLimits.cast_int(TValue.minus(L.top, L.base_));
		}

		public static void lua_settop(lua_State L, int idx)
		{
			LuaLimits.lua_lock(L);
			if (idx >= 0)
			{
				LuaLimits.api_check(L, idx <= TValue.minus(L.stack_last, L.base_));
				while (TValue.lessThan(L.top, TValue.plus(L.base_, idx)))
				{
					TValue[] top = new TValue[1];
					top[0] = L.top;
					LuaObject.setnilvalue(/*StkId*/TValue.inc(/*ref*/ top));
					L.top = top[0];
				}
				L.top = TValue.plus(L.base_, idx);
			}
			else
			{
				LuaLimits.api_check(L, -(idx + 1) <= TValue.minus(L.top, L.base_));
				L.top = TValue.plus(L.top, idx+1);  /* `subtract' index (index is negative) */
			}
			LuaLimits.lua_unlock(L);
		}

		public static void lua_remove(lua_State L, int idx)
		{
			TValue/*StkId*/ p;
			LuaLimits.lua_lock(L);
			p = index2adr(L, idx);
			api_checkvalidindex(L, p);
			while (TValue.lessThan((p = p.get(1)), L.top))
			{
				LuaObject.setobjs2s(L, TValue.minus(p, 1), p);
			}
			TValue[] top = new TValue[1];
			top[0] = L.top;
			/*StkId*/TValue.dec(/*ref*/ top);
			L.top = top[0];
			LuaLimits.lua_unlock(L);
		}

		public static void lua_insert(lua_State L, int idx)
		{
			TValue/*StkId*/ p;
			TValue[]/*StkId*/ q = new TValue[1];
			q[0] = new TValue();
			LuaLimits.lua_lock(L);
			p = index2adr(L, idx);
			api_checkvalidindex(L, p);
			for (q[0] = L.top; TValue.greaterThan(q[0], p); /*StkId*/TValue.dec(/*ref*/ q))
			{
				LuaObject.setobjs2s(L, q[0], TValue.minus(q[0], 1));
			}
			LuaObject.setobjs2s(L, p, L.top);
			LuaLimits.lua_unlock(L);
		}

		public static void lua_replace(lua_State L, int idx)
		{
			TValue/*StkId*/ o;
			LuaLimits.lua_lock(L);
			/* explicit test for incompatible code */
			if (idx == Lua.LUA_ENVIRONINDEX && L.ci == L.base_ci[0])
			{
				LuaDebug.luaG_runerror(L, CharPtr.toCharPtr("no calling environment"));
			}
			api_checknelems(L, 1);
			o = index2adr(L, idx);
			api_checkvalidindex(L, o);
			if (idx == Lua.LUA_ENVIRONINDEX)
			{
				Closure func = LuaState.curr_func(L);
				LuaLimits.api_check(L, LuaObject.ttistable(TValue.minus(L.top, 1)));
				func.c.setEnv(LuaObject.hvalue(TValue.minus(L.top, 1)));
				LuaGC.luaC_barrier(L, func, TValue.minus(L.top, 1));
			}
			else
			{
				LuaObject.setobj(L, o, TValue.minus(L.top, 1));
				if (idx < Lua.LUA_GLOBALSINDEX)  /* function upvalue? */
				{
					LuaGC.luaC_barrier(L, LuaState.curr_func(L), TValue.minus(L.top, 1));
				}
			}
			TValue[] top = new TValue[1];
			top[0] = L.top;
			/*StkId*/TValue.dec(/*ref*/ top);
			L.top = top[0];
			LuaLimits.lua_unlock(L);
		}

		public static void lua_pushvalue(lua_State L, int idx)
		{
			LuaLimits.lua_lock(L);
			LuaObject.setobj2s(L, L.top, index2adr(L, idx));
			api_incr_top(L);
			LuaLimits.lua_unlock(L);
		}

		/*
		 ** access functions (stack . C)
		 */
		public static int lua_type(lua_State L, int idx)
		{
			TValue/*StkId*/ o = index2adr(L, idx);
			return (o == LuaObject.luaO_nilobject) ? Lua.LUA_TNONE : LuaObject.ttype(o);
		}

		public static CharPtr lua_typename(lua_State L, int t)
		{
			//UNUSED(L);
			return (t == Lua.LUA_TNONE) ? CharPtr.toCharPtr("no value") : LuaTM.luaT_typenames[t];
		}

		public static bool lua_iscfunction(lua_State L, int idx)
		{
			TValue/*StkId*/ o = index2adr(L, idx);
			return LuaObject.iscfunction(o);
		}

		public static int lua_isnumber(lua_State L, int idx)
		{
			TValue n = new TValue();
			TValue o = index2adr(L, idx);
			TValue[] o_ref = new TValue[1];
			o_ref[0] = o;
			int ret = LuaVM.tonumber(/*ref*/ o_ref, n);
			o = o_ref[0];
			return ret;
		}

		public static int lua_isstring(lua_State L, int idx) 
		{
			int t = lua_type(L, idx);
			return (t == Lua.LUA_TSTRING || t == Lua.LUA_TNUMBER) ? 1 : 0;
		}

		public static int lua_isuserdata(lua_State L, int idx) 
		{
			TValue o = index2adr(L, idx);
			return (LuaObject.ttisuserdata(o) || LuaObject.ttislightuserdata(o)) ? 1 : 0;
		}

		public static int lua_rawequal(lua_State L, int index1, int index2) 
		{
			TValue/*StkId*/ o1 = index2adr(L, index1);
			TValue/*StkId*/ o2 = index2adr(L, index2);
			return (o1 == LuaObject.luaO_nilobject || o2 == LuaObject.luaO_nilobject) ? 0
				: LuaObject.luaO_rawequalObj(o1, o2);
		}

		public static int lua_equal(lua_State L, int index1, int index2) 
		{
			TValue/*StkId*/ o1, o2;
			int i;
			LuaLimits.lua_lock(L);  /* may call tag method */
			o1 = index2adr(L, index1);
			o2 = index2adr(L, index2);
			i = (o1 == LuaObject.luaO_nilobject || o2 == LuaObject.luaO_nilobject) ? 0 : LuaVM.equalobj(L, o1, o2);
			LuaLimits.lua_unlock(L);
			return i;
		}

		public static int lua_lessthan(lua_State L, int index1, int index2) 
		{
			TValue/*StkId*/ o1, o2;
			int i;
			LuaLimits.lua_lock(L);  /* may call tag method */
			o1 = index2adr(L, index1);
			o2 = index2adr(L, index2);
			i = (o1 == LuaObject.luaO_nilobject || o2 == LuaObject.luaO_nilobject) ? 0
				: LuaVM.luaV_lessthan(L, o1, o2);
			LuaLimits.lua_unlock(L);
			return i;
		}

		public static Double/*lua_Number*/ lua_tonumber(lua_State L, int idx) 
		{
			TValue n = new TValue();
			TValue o = index2adr(L, idx);
			TValue[] o_ref = new TValue[1];
			o_ref[0] = o;
			int ret = LuaVM.tonumber(/*ref*/ o_ref, n);
			o = o_ref[0];
			if (ret != 0)
			{
				return LuaObject.nvalue(o);
			}
			else
			{
				return 0;
			}
		}

		public static int/*Int32*//*lua_Integer*/ lua_tointeger(lua_State L, int idx) 
		{
			TValue n = new TValue();
			TValue o = index2adr(L, idx);
			TValue[] o_ref = new TValue[1];
			o_ref[0] = o;
			int ret = LuaVM.tonumber(/*ref*/ o_ref, n);
			o = o_ref[0];
			if (ret != 0)
			{
				int[]/*Int32*//*lua_Integer*/ res = new int[1];
				double/*Double*//*lua_Number*/ num = LuaObject.nvalue(o);
				LuaConf.lua_number2integer(/*out*/ res, num);
				return res[0];
			}
			else
			{
				return 0;
			}
		}

		public static int lua_toboolean(lua_State L, int idx) 
		{
			TValue o = index2adr(L, idx);
			return (LuaObject.l_isfalse(o) == 0) ? 1 : 0;
		}

		public static CharPtr lua_tolstring(lua_State L, int idx, /*out*/ int[]/*uint*/ len)
		{
			TValue/*StkId*/ o = index2adr(L, idx);
			if (!LuaObject.ttisstring(o))
			{
				LuaLimits.lua_lock(L);  /* `luaV_tostring' may create a new string */
				if (LuaVM.luaV_tostring(L, o) == 0)
				{  /* conversion failed? */
					len[0] = 0;
					LuaLimits.lua_unlock(L);
					return null;
				}
				LuaGC.luaC_checkGC(L);
				o = index2adr(L, idx);  /* previous call may reallocate the stack */
				LuaLimits.lua_unlock(L);
			}
			len[0] = LuaObject.tsvalue(o).len;
			return LuaObject.svalue(o);
		}

		public static int/*uint*/ lua_objlen (lua_State L, int idx) 
		{
			TValue/*StkId*/ o = index2adr(L, idx);
			switch (LuaObject.ttype(o))
			{
				case Lua.LUA_TSTRING:
					return LuaObject.tsvalue(o).len;
					
				case Lua.LUA_TUSERDATA:
					return LuaObject.uvalue(o).len;
					
				case Lua.LUA_TTABLE:
					return /*(uint)*/LuaTable.luaH_getn(LuaObject.hvalue(o));
					
				case Lua.LUA_TNUMBER:
					{
						int/*uint*/ l;
						LuaLimits.lua_lock(L);  /* `luaV_tostring' may create a new string */
						l = (LuaVM.luaV_tostring(L, o) != 0 ? LuaObject.tsvalue(o).len : 0);
						LuaLimits.lua_unlock(L);
						return l;
					}
					
				default:
					return 0;
			}
		}

		public static lua_CFunction lua_tocfunction(lua_State L, int idx) 
		{
			TValue/*StkId*/ o = index2adr(L, idx);
			return (!LuaObject.iscfunction(o)) ? null : LuaObject.clvalue(o).c.f;
		}

		public static object lua_touserdata(lua_State L, int idx) 
		{
			TValue/*StkId*/ o = index2adr(L, idx);
			switch (LuaObject.ttype(o))
			{
				case Lua.LUA_TUSERDATA: 
					{
						return (LuaObject.rawuvalue(o).user_data);
					}
				case Lua.LUA_TLIGHTUSERDATA: 
					{
						return LuaObject.pvalue(o);
					}
				default: 
					{
						return null;
					}
			}
		}

		public static lua_State lua_tothread(lua_State L, int idx) 
		{
			TValue/*StkId*/ o = index2adr(L, idx);
			return (!LuaObject.ttisthread(o)) ? null : LuaObject.thvalue(o);
		}

		public static object lua_topointer(lua_State L, int idx) 
		{
			TValue/*StkId*/ o = index2adr(L, idx);
			switch (LuaObject.ttype(o))
			{
				case Lua.LUA_TTABLE: 
					{
						return LuaObject.hvalue(o);
					}
				case Lua.LUA_TFUNCTION: 
					{
						return LuaObject.clvalue(o);
					}
				case Lua.LUA_TTHREAD: 
					{
						return LuaObject.thvalue(o);
					}
				case Lua.LUA_TUSERDATA:
				case Lua.LUA_TLIGHTUSERDATA:
					{
						return lua_touserdata(L, idx);
					}
				default: 
					{
						return null;
					}
			}
		}

		/*
		 ** push functions (C . stack)
		 */
		public static void lua_pushnil(lua_State L) 
		{
			LuaLimits.lua_lock(L);
			LuaObject.setnilvalue(L.top);
			api_incr_top(L);
			LuaLimits.lua_unlock(L);
		}

		public static void lua_pushnumber(lua_State L, double/*Double*//*lua_Number*/ n) 
		{
			LuaLimits.lua_lock(L);
			LuaObject.setnvalue(L.top, n);
			api_incr_top(L);
			LuaLimits.lua_unlock(L);
		}

		public static void lua_pushinteger(lua_State L, int/*Int32*//*lua_Integer*/ n) 
		{
			LuaLimits.lua_lock(L);
			LuaObject.setnvalue(L.top, LuaLimits.cast_num(n));
			api_incr_top(L);
			LuaLimits.lua_unlock(L);
		}

		public static void lua_pushlstring (lua_State L, CharPtr s, int/*uint*/ len) 
		{
			LuaLimits.lua_lock(L);
			LuaGC.luaC_checkGC(L);
			LuaObject.setsvalue2s(L, L.top, LuaString.luaS_newlstr(L, s, len));
			api_incr_top(L);
			LuaLimits.lua_unlock(L);
		}

		public static void lua_pushstring(lua_State L, CharPtr s) 
		{
			if (CharPtr.isEqual(s, null))
			{
				lua_pushnil(L);
			}
			else
			{
				lua_pushlstring(L, s, /*(uint)*/LuaConf.strlen(s));
			}
		}

		public static CharPtr lua_pushvfstring(lua_State L, CharPtr fmt, object[] argp) 
		{
			CharPtr ret;
			LuaLimits.lua_lock(L);
			LuaGC.luaC_checkGC(L);
			ret = LuaObject.luaO_pushvfstring(L, fmt, argp);
			LuaLimits.lua_unlock(L);
			return ret;
		}

		public static CharPtr lua_pushfstring(lua_State L, CharPtr fmt)
		{
			CharPtr ret;
			LuaLimits.lua_lock(L);
			LuaGC.luaC_checkGC(L);
			ret = LuaObject.luaO_pushvfstring(L, fmt, null);
			LuaLimits.lua_unlock(L);
			return ret;
		}

		public static CharPtr lua_pushfstring(lua_State L, CharPtr fmt, params object[] p)
		{
			CharPtr ret;
			LuaLimits.lua_lock(L);
			LuaGC.luaC_checkGC(L);
			ret = LuaObject.luaO_pushvfstring(L, fmt, p);
			LuaLimits.lua_unlock(L);
			return ret;
		}

		public static void lua_pushcclosure(lua_State L, lua_CFunction fn, int n)
		{
			Closure cl;
			LuaLimits.lua_lock(L);
			LuaGC.luaC_checkGC(L);
			api_checknelems(L, n);
			cl = LuaFunc.luaF_newCclosure(L, n, getcurrenv(L));
			cl.c.f = fn;
			L.top = TValue.minus(L.top, n);
			while (n-- != 0)
			{
				LuaObject.setobj2n(L, cl.c.upvalue[n], TValue.plus(L.top, n));
			}
			LuaObject.setclvalue(L, L.top, cl);
			LuaLimits.lua_assert(LuaGC.iswhite(LuaState.obj2gco(cl)));
			api_incr_top(L);
			LuaLimits.lua_unlock(L);
		}


		public static void lua_pushboolean(lua_State L, int b) 
		{
			LuaLimits.lua_lock(L);
			LuaObject.setbvalue(L.top, (b != 0) ? 1 : 0);  /* ensure that true is 1 */
			api_incr_top(L);
			LuaLimits.lua_unlock(L);
		}

		public static void lua_pushlightuserdata(lua_State L, object p) 
		{
			LuaLimits.lua_lock(L);
			LuaObject.setpvalue(L.top, p);
			api_incr_top(L);
			LuaLimits.lua_unlock(L);
		}

		public static int lua_pushthread (lua_State L) 
		{
			LuaLimits.lua_lock(L);
			LuaObject.setthvalue(L, L.top, L);
			api_incr_top(L);
			LuaLimits.lua_unlock(L);
			return (LuaState.G(L).mainthread == L) ? 1 : 0;
		}

		/*
		 ** get functions (Lua . stack)
		 */
		public static void lua_gettable (lua_State L, int idx) 
		{
			TValue/*StkId*/ t;
			LuaLimits.lua_lock(L);
			t = index2adr(L, idx);
			api_checkvalidindex(L, t);
			LuaVM.luaV_gettable(L, t, TValue.minus(L.top, 1), TValue.minus(L.top, 1));
			LuaLimits.lua_unlock(L);
		}

		public static void lua_getfield(lua_State L, int idx, CharPtr k)
		{
			TValue/*StkId*/ t;
			TValue key = new TValue();
			LuaLimits.lua_lock(L);
			t = index2adr(L, idx);
			api_checkvalidindex(L, t);
			LuaObject.setsvalue(L, key, LuaString.luaS_new(L, k));
			LuaVM.luaV_gettable(L, t, key, L.top);
			api_incr_top(L);
			LuaLimits.lua_unlock(L);
		}

		public static void lua_rawget(lua_State L, int idx) 
		{
			TValue/*StkId*/ t;
			LuaLimits.lua_lock(L);
			t = index2adr(L, idx);
			LuaLimits.api_check(L, LuaObject.ttistable(t));
			LuaObject.setobj2s(L, TValue.minus(L.top, 1), LuaTable.luaH_get(LuaObject.hvalue(t), TValue.minus(L.top, 1)));
			LuaLimits.lua_unlock(L);
		}

		public static void lua_rawgeti(lua_State L, int idx, int n) 
		{
			TValue/*StkId*/ o;
			LuaLimits.lua_lock(L);
			o = index2adr(L, idx);
			LuaLimits.api_check(L, LuaObject.ttistable(o));
			LuaObject.setobj2s(L, L.top, LuaTable.luaH_getnum(LuaObject.hvalue(o), n));
			api_incr_top(L);
			LuaLimits.lua_unlock(L);
		}

		public static void lua_createtable(lua_State L, int narray, int nrec) 
		{
			LuaLimits.lua_lock(L);
			LuaGC.luaC_checkGC(L);
			LuaObject.sethvalue(L, L.top, LuaTable.luaH_new(L, narray, nrec));
			api_incr_top(L);
			LuaLimits.lua_unlock(L);
		}

		public static int lua_getmetatable(lua_State L, int objindex) 
		{
			TValue obj;
			Table mt = null;
			int res;
			LuaLimits.lua_lock(L);
			obj = index2adr(L, objindex);
			switch (LuaObject.ttype(obj))
			{
				case Lua.LUA_TTABLE:
					{
						mt = LuaObject.hvalue(obj).metatable;
						break;
					}
				case Lua.LUA_TUSERDATA:
					{
						mt = LuaObject.uvalue(obj).metatable;
						break;
					}
				default:
					{
						mt = LuaState.G(L).mt[LuaObject.ttype(obj)];
						break;
					}
			}
			if (mt == null)
			{
				res = 0;
			}
			else 
			{
				LuaObject.sethvalue(L, L.top, mt);
				api_incr_top(L);
				res = 1;
			}
			LuaLimits.lua_unlock(L);
			return res;
		}

		public static void lua_getfenv(lua_State L, int idx) 
		{
			TValue/*StkId*/ o;
			LuaLimits.lua_lock(L);
			o = index2adr(L, idx);
			api_checkvalidindex(L, o);
			switch (LuaObject.ttype(o))
			{
				case Lua.LUA_TFUNCTION:
					{
						LuaObject.sethvalue(L, L.top, LuaObject.clvalue(o).c.getEnv());
						break;
					}
				case Lua.LUA_TUSERDATA:
					{
						LuaObject.sethvalue(L, L.top, LuaObject.uvalue(o).env);
						break;
					}
				case Lua.LUA_TTHREAD:
					{
						LuaObject.setobj2s(L, L.top, LuaState.gt(LuaObject.thvalue(o)));
						break;
					}
				default:
					{
						LuaObject.setnilvalue(L.top);
						break;
					}
			}
			api_incr_top(L);
			LuaLimits.lua_unlock(L);
		}


		/*
		 ** set functions (stack . Lua)
		 */
		public static void lua_settable(lua_State L, int idx) 
		{
			TValue/*StkId*/ t;
			LuaLimits.lua_lock(L);
			api_checknelems(L, 2);
			t = index2adr(L, idx);
			api_checkvalidindex(L, t);
			LuaVM.luaV_settable(L, t, TValue.minus(L.top, 2), TValue.minus(L.top, 1));
			L.top = TValue.minus(L.top, 2);  /* pop index and value */
			LuaLimits.lua_unlock(L);
		}

		public static void lua_setfield(lua_State L, int idx, CharPtr k) 
		{
			TValue/*StkId*/ t;
			TValue key = new TValue();
			LuaLimits.lua_lock(L);
			api_checknelems(L, 1);
			t = index2adr(L, idx);
			api_checkvalidindex(L, t);
			LuaObject.setsvalue(L, key, LuaString.luaS_new(L, k));
			LuaVM.luaV_settable(L, t, key, TValue.minus(L.top, 1));
			TValue[] top = new TValue[1];
			top[0] = L.top;
			/*StkId*/TValue.dec(/*ref*/ top);  /* pop value */
			L.top = top[0];
			LuaLimits.lua_unlock(L);
		}

		public static void lua_rawset(lua_State L, int idx) 
		{
			TValue/*StkId*/ t;
			LuaLimits.lua_lock(L);
			api_checknelems(L, 2);
			t = index2adr(L, idx);
			LuaLimits.api_check(L, LuaObject.ttistable(t));
			LuaObject.setobj2t(L, LuaTable.luaH_set(L, LuaObject.hvalue(t), TValue.minus(L.top, 2)), TValue.minus(L.top, 1));
			LuaGC.luaC_barriert(L, LuaObject.hvalue(t), TValue.minus(L.top, 1));
			L.top = TValue.minus(L.top, 2);
			LuaLimits.lua_unlock(L);
		}

		public static void lua_rawseti(lua_State L, int idx, int n) 
		{
			TValue/*StkId*/ o;
			LuaLimits.lua_lock(L);
			api_checknelems(L, 1);
			o = index2adr(L, idx);
			LuaLimits.api_check(L, LuaObject.ttistable(o));
			LuaObject.setobj2t(L, LuaTable.luaH_setnum(L, LuaObject.hvalue(o), n), TValue.minus(L.top, 1));
			LuaGC.luaC_barriert(L, LuaObject.hvalue(o), TValue.minus(L.top, 1));
			TValue[] top = new TValue[1];
			top[0] = L.top;
			/*StkId*/TValue.dec(/*ref*/ top);
			L.top = top[0];
			LuaLimits.lua_unlock(L);
		}

		public static int lua_setmetatable(lua_State L, int objindex) 
		{
			TValue obj;
			Table mt;
			LuaLimits.lua_lock(L);
			api_checknelems(L, 1);
			obj = index2adr(L, objindex);
			api_checkvalidindex(L, obj);
			if (LuaObject.ttisnil(TValue.minus(L.top, 1)))
			{
				mt = null;
			}
			else 
			{
				LuaLimits.api_check(L, LuaObject.ttistable(TValue.minus(L.top, 1)));
				mt = LuaObject.hvalue(TValue.minus(L.top, 1));
			}
			switch (LuaObject.ttype(obj))
			{
				case Lua.LUA_TTABLE:
					{
						LuaObject.hvalue(obj).metatable = mt;
						if (mt != null)
						{
							LuaGC.luaC_objbarriert(L, LuaObject.hvalue(obj), mt);
						}
						break;
					}
				case Lua.LUA_TUSERDATA:
					{
						LuaObject.uvalue(obj).metatable = mt;
						if (mt != null)
						{
							LuaGC.luaC_objbarrier(L, LuaObject.rawuvalue(obj), mt);
						}
						break;
					}
				default: 
					{
						LuaState.G(L).mt[LuaObject.ttype(obj)] = mt;
						break;
					}
			}
			TValue[] top = new TValue[1];
			top[0] = L.top;
			/*StkId*/TValue.dec(/*ref*/ top);
			L.top = top[0];
			LuaLimits.lua_unlock(L);
			return 1;
		}

		public static int lua_setfenv(lua_State L, int idx) 
		{
			TValue/*StkId*/ o;
			int res = 1;
			LuaLimits.lua_lock(L);
			api_checknelems(L, 1);
			o = index2adr(L, idx);
			api_checkvalidindex(L, o);
			LuaLimits.api_check(L, LuaObject.ttistable(TValue.minus(L.top, 1)));
			switch (LuaObject.ttype(o))
			{
				case Lua.LUA_TFUNCTION:
					{
						LuaObject.clvalue(o).c.setEnv(LuaObject.hvalue(TValue.minus(L.top, 1)));
						break;
					}
				case Lua.LUA_TUSERDATA:
					{
						LuaObject.uvalue(o).env = LuaObject.hvalue(TValue.minus(L.top, 1));
						break;
					}
				case Lua.LUA_TTHREAD:
					{
						LuaObject.sethvalue(L, LuaState.gt(LuaObject.thvalue(o)), LuaObject.hvalue(TValue.minus(L.top, 1)));
						break;
					}
				default:
					{
						res = 0;
						break;
					}
			}
			if (res != 0) 
			{
				LuaGC.luaC_objbarrier(L, LuaObject.gcvalue(o), LuaObject.hvalue(TValue.minus(L.top, 1)));
			}
			TValue[] top = new TValue[1];
			top[0] = L.top;
			/*StkId*/TValue.dec(/*ref*/ top);
			L.top = top[0];
			LuaLimits.lua_unlock(L);
			return res;
		}

		/*
		 ** `load' and `call' functions (run Lua code)
		 */
		public static void adjustresults(lua_State L, int nres) 
		{
			if (nres == Lua.LUA_MULTRET && TValue.greaterEqual(L.top, L.ci.top))
			{
				L.ci.top = L.top;
			}
		}

		public static void checkresults(lua_State L, int na, int nr) 
		{
			LuaLimits.api_check(L, (nr) == Lua.LUA_MULTRET || (TValue.minus(L.ci.top, L.top) >= (nr) - (na)));
		}
		
		public static void lua_call(lua_State L, int nargs, int nresults) 
		{
			TValue/*StkId*/ func;
			LuaLimits.lua_lock(L);
			api_checknelems(L, nargs+1);
			checkresults(L, nargs, nresults);
			func = TValue.minus(L.top, (nargs + 1));
			LuaDo.luaD_call(L, func, nresults);
			adjustresults(L, nresults);
			LuaLimits.lua_unlock(L);
		}

		static void f_call(lua_State L, object ud) 
		{
			CallS c = ud as CallS;
			LuaDo.luaD_call(L, c.func, c.nresults);
		}

		public class f_call_delegate : Pfunc
		{
			public void exec(lua_State L, object ud)
			{
				f_call(L, ud);
			}
		}       
		
		public static int lua_pcall(lua_State L, int nargs, int nresults, int errfunc) 
		{
			CallS c = new CallS();
			int status;
			int/*Int32*//*ptrdiff_t*/ func;
			LuaLimits.lua_lock(L);
			api_checknelems(L, nargs+1);
			checkresults(L, nargs, nresults);
			if (errfunc == 0)
			{
				func = 0;
			}
			else 
			{
				TValue/*StkId*/ o = index2adr(L, errfunc);
				api_checkvalidindex(L, o);
				func = LuaDo.savestack(L, o);
			}
			c.func = TValue.minus(L.top, (nargs + 1));  /* function to be called */
			c.nresults = nresults;
			status = LuaDo.luaD_pcall(L, new f_call_delegate(), c, LuaDo.savestack(L, c.func), func);
			adjustresults(L, nresults);
			LuaLimits.lua_unlock(L);
			return status;
		}

		static void f_Ccall(lua_State L, object ud) 
		{
			CCallS c = ud as CCallS;
			Closure cl;
			cl = LuaFunc.luaF_newCclosure(L, 0, getcurrenv(L));
			cl.c.f = c.func;
			LuaObject.setclvalue(L, L.top, cl);  /* push function */
			api_incr_top(L);
			LuaObject.setpvalue(L.top, c.ud);  /* push only argument */
			api_incr_top(L);
			LuaDo.luaD_call(L, TValue.minus(L.top, 2), 0);
		}

		public class f_Ccall_delegate : Pfunc
		{
			public void exec(lua_State L, object ud)
			{
				f_Ccall(L, ud);
			}
		}
		
		public static int lua_cpcall(lua_State L, lua_CFunction func, object ud) 
		{
			CCallS c = new CCallS();
			int status;
			LuaLimits.lua_lock(L);
			c.func = func;
			c.ud = ud;
			status = LuaDo.luaD_pcall(L, new f_Ccall_delegate(), c, LuaDo.savestack(L, L.top), 0);
			LuaLimits.lua_unlock(L);
			return status;
		}

		public static int lua_load(lua_State L, lua_Reader reader, object data, CharPtr chunkname) 
		{
			ZIO z = new ZIO();
			int status;
			LuaLimits.lua_lock(L);
			if (CharPtr.isEqual(chunkname, null)) 
			{
				chunkname = CharPtr.toCharPtr("?");
			}
			LuaZIO.luaZ_init(L, z, reader, data);
			status = LuaDo.luaD_protectedparser(L, z, chunkname);
			LuaLimits.lua_unlock(L);
			return status;
		}

		public static int lua_dump(lua_State L, lua_Writer writer, object data) 
		{
			int status;
			TValue o;
			LuaLimits.lua_lock(L);
			api_checknelems(L, 1);
			o = TValue.minus(L.top, 1);
			if (LuaObject.isLfunction(o))
			{
				status = LuaDump.luaU_dump(L, LuaObject.clvalue(o).l.p, writer, data, 0);
			}
			else
			{
				status = 1;
			}
			LuaLimits.lua_unlock(L);
			return status;
		}

		public static int lua_status(lua_State L)
		{
			return L.status;
		}

		/*
		 ** Garbage-collection function
		 */
		public static int lua_gc(lua_State L, int what, int data)
		{
			int res = 0;
			global_State g;
			LuaLimits.lua_lock(L);
			g = LuaState.G(L);
			switch (what)
			{
				case Lua.LUA_GCSTOP:
					{
						g.GCthreshold = LuaLimits.MAX_LUMEM;
						break;
					}
				case Lua.LUA_GCRESTART:
					{
						g.GCthreshold = g.totalbytes;
						break;
					}
				case Lua.LUA_GCCOLLECT:
					{
						LuaGC.luaC_fullgc(L);
						break;
					}
				case Lua.LUA_GCCOUNT:
					{
						/* GC values are expressed in Kbytes: #bytes/2^10 */
						res = LuaLimits.cast_int(g.totalbytes >> 10);
						break;
					}
				case Lua.LUA_GCCOUNTB:
					{
						res = LuaLimits.cast_int(g.totalbytes & 0x3ff);
						break;
					}
				case Lua.LUA_GCSTEP:
					{
						long/*UInt32*//*lu_mem*/ a = ((long/*UInt32*//*lu_mem*/)data << 10);
						if (a <= g.totalbytes)
						{
							g.GCthreshold = /*(uint)*/(g.totalbytes - a);
						}
						else
						{
							g.GCthreshold = 0;
						}
						while (g.GCthreshold <= g.totalbytes)
						{
							LuaGC.luaC_step(L);
							if (g.gcstate == LuaGC.GCSpause)
							{
								/* end of cycle? */
								res = 1;  /* signal it */
								break;
							}
						}
						break;
					}
				case Lua.LUA_GCSETPAUSE:
					{
						res = g.gcpause;
						g.gcpause = data;
						break;
					}
				case Lua.LUA_GCSETSTEPMUL:
					{
						res = g.gcstepmul;
						g.gcstepmul = data;
						break;
					}
				default:
					{
						res = -1;  /* invalid option */
						break;
					}
			}
			LuaLimits.lua_unlock(L);
			return res;
		}

		/*
		 ** miscellaneous functions
		 */
		public static int lua_error(lua_State L)
		{
			LuaLimits.lua_lock(L);
			api_checknelems(L, 1);
			LuaDebug.luaG_errormsg(L);
			LuaLimits.lua_unlock(L);
			return 0;  /* to avoid warnings */
		}

		public static int lua_next(lua_State L, int idx)
		{
			TValue/*StkId*/ t;
			int more;
			LuaLimits.lua_lock(L);
			t = index2adr(L, idx);
			LuaLimits.api_check(L, LuaObject.ttistable(t));
			more = LuaTable.luaH_next(L, LuaObject.hvalue(t), TValue.minus(L.top, 1));
			if (more != 0)
			{
				api_incr_top(L);
			}
			else  /* no more elements */
			{
				TValue[] top = new TValue[1];
				top[0] = L.top;
				/*StkId*/TValue.dec(/*ref*/ top);  /* remove key */
				L.top = top[0];
			}
			LuaLimits.lua_unlock(L);
			return more;
		}

		public static void lua_concat(lua_State L, int n)
		{
			LuaLimits.lua_lock(L);
			api_checknelems(L, n);
			if (n >= 2)
			{
				LuaGC.luaC_checkGC(L);
				LuaVM.luaV_concat(L, n, LuaLimits.cast_int(TValue.minus(L.top, L.base_)) - 1); //FIXME:
				L.top = TValue.minus(L.top, (n - 1));
			}
			else if (n == 0)
			{
				/* push empty string */
				LuaObject.setsvalue2s(L, L.top, LuaString.luaS_newlstr(L, CharPtr.toCharPtr(""), 0));
				api_incr_top(L);
			}
			/* else n == 1; nothing to do */
			LuaLimits.lua_unlock(L);
		}

		public static lua_Alloc lua_getallocf(lua_State L, /*ref*/ object[] ud)
		{
			lua_Alloc f;
			LuaLimits.lua_lock(L);
			if (ud[0] != null)
			{
				ud[0] = LuaState.G(L).ud;
			}
			f = LuaState.G(L).frealloc;
			LuaLimits.lua_unlock(L);
			return f;
		}

		public static void lua_setallocf(lua_State L, lua_Alloc f, object ud)
		{
			LuaLimits.lua_lock(L);
			LuaState.G(L).ud = ud;
			LuaState.G(L).frealloc = f;
			LuaLimits.lua_unlock(L);
		}

		public static object lua_newuserdata(lua_State L, int/*uint*/ size)
		{
			Udata u;
			LuaLimits.lua_lock(L);
			LuaGC.luaC_checkGC(L);
			u = LuaString.luaS_newudata(L, size, getcurrenv(L));
			LuaObject.setuvalue(L, L.top, u);
			api_incr_top(L);
			LuaLimits.lua_unlock(L);
			return u.user_data;
		}

		public static object lua_newuserdata(lua_State L, ClassType t)
		{
			Udata u;
			LuaLimits.lua_lock(L);
			LuaGC.luaC_checkGC(L);
			u = LuaString.luaS_newudata(L, t, getcurrenv(L));
			LuaObject.setuvalue(L, L.top, u);
			api_incr_top(L);
			LuaLimits.lua_unlock(L);
			return u.user_data;
		}

		static CharPtr aux_upvalue(TValue/*StkId*/ fi, int n, /*ref*/ TValue[] val)
		{
			Closure f;
			if (!LuaObject.ttisfunction(fi))
			{
				return null;
			}
			f = LuaObject.clvalue(fi);
			if (f.c.getIsC() != 0)
			{
				if (!(1 <= n && n <= f.c.getNupvalues()))
				{
					return null;
				}
				val[0] = f.c.upvalue[n - 1];
				return CharPtr.toCharPtr("");
			}
			else
			{
				Proto p = f.l.p;
				if (!(1 <= n && n <= p.sizeupvalues))
				{
					return null;
				}
				val[0] = f.l.upvals[n - 1].v;
				return LuaObject.getstr(p.upvalues[n - 1]);
			}
		}

		public static CharPtr lua_getupvalue(lua_State L, int funcindex, int n)
		{
			CharPtr name;
			TValue val = new TValue();
			LuaLimits.lua_lock(L);
			TValue[] val_ref = new TValue[1];
			val_ref[0] = val;
			name = aux_upvalue(index2adr(L, funcindex), n, /*ref*/ val_ref);
			val = val_ref[0];
			if (CharPtr.isNotEqual(name, null))
			{
				LuaObject.setobj2s(L, L.top, val);
				api_incr_top(L);
			}
			LuaLimits.lua_unlock(L);
			return name;
		}

		public static CharPtr lua_setupvalue(lua_State L, int funcindex, int n)
		{
			CharPtr name;
			TValue val = new TValue();
			TValue/*StkId*/ fi;
			LuaLimits.lua_lock(L);
			fi = index2adr(L, funcindex);
			api_checknelems(L, 1);
			TValue[] val_ref = new TValue[1];
			val_ref[0] = val;
			name = aux_upvalue(fi, n, /*ref*/ val_ref);
			val = val_ref[0];
			if (CharPtr.isNotEqual(name, null))
			{
				TValue[] top = new TValue[1];
				top[0] = L.top;
				/*StkId*/TValue.dec(/*ref*/ top);
				L.top = top[0];
				LuaObject.setobj(L, val, L.top);
				LuaGC.luaC_barrier(L, LuaObject.clvalue(fi), L.top);
			}
			LuaLimits.lua_unlock(L);
			return name;
		}
	}
}
