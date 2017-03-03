/*
 ** $Id: lauxlib.c,v 1.159.1.3 2008/01/21 13:20:51 roberto Exp $
 ** Auxiliary functions for building Lua libraries
 ** See Copyright Notice in lua.h
 */
using System;

/*
 ** #define lauxlib_c
 ** #define LUA_LIB
 */
namespace KopiLua
{
	//using lua_Number = System.Double;
	//using lua_Integer = System.Int32;

	public class LuaAuxLib
	{
		//#if LUA_COMPAT_GETN
//		public static int luaL_getn(lua_State L, int t);
//		public static void luaL_setn(lua_State L, int t, int n);
		//#else
		public static int luaL_getn(lua_State L, int i) {return (int)LuaAPI.lua_objlen(L, i);}
		public static void luaL_setn(lua_State L, int i, int j) {} /* no op! */
		//#endif

		//#if LUA_COMPAT_OPENLIB
		//#define luaI_openlib	luaL_openlib
		//#endif

		/* extra error code for `luaL_load' */
		public const int LUA_ERRFILE = (Lua.LUA_ERRERR+1);

		/*
		 ** ===============================================================
		 ** some useful macros
		 ** ===============================================================
		 */
		public static void luaL_argcheck(lua_State L, bool cond, int numarg, string extramsg) 
		{
			if (!cond)
			{
				luaL_argerror(L, numarg, CharPtr.toCharPtr(extramsg));
			}
		}
		
		public static CharPtr luaL_checkstring(lua_State L, int n) 
		{
			return luaL_checklstring(L, n); 
		}
		
		public static CharPtr luaL_optstring(lua_State L, int n, CharPtr d)
		{
			int[]/*uint*/ len = new int[1];
			return luaL_optlstring(L, n, d, /*out*/ len);
		}
		
		public static int luaL_checkint(lua_State L, int n)	
		{
			return /*(int)*/luaL_checkinteger(L, n);
		}
		
		public class luaL_checkint_delegate : luaL_opt_delegate_integer
		{
			public int/*Int32*//*lua_Integer*/ exec(lua_State L, int narg)
			{
				return luaL_checkint(L, narg);
			}
		}
			
		public static int luaL_optint(lua_State L, int n, int/*Int32*//*lua_Integer*/ d) 
		{
			return /*(int)*/luaL_optinteger(L, n, d); 
		}
		
		public static long luaL_checklong(lua_State L, int n)	
		{
			return luaL_checkinteger(L, n);
		}
		
		public static long luaL_optlong(lua_State L, int n, int/*Int32*//*lua_Integer*/ d) 
		{ 
			return luaL_optinteger(L, n, d); 
		}

		public static CharPtr luaL_typename(lua_State L, int i) 
		{ 
			return LuaAPI.lua_typename(L, LuaAPI.lua_type(L, i)); 
		}

		//#define luaL_dofile(L, fn) \
		//    (luaL_loadfile(L, fn) || lua_pcall(L, 0, LUA_MULTRET, 0))

		//#define luaL_dostring(L, s) \
		//    (luaL_loadstring(L, s) || lua_pcall(L, 0, LUA_MULTRET, 0))

		public static void luaL_getmetatable(lua_State L, CharPtr n) 
		{ 
			LuaAPI.lua_getfield(L, Lua.LUA_REGISTRYINDEX, n); 
		}

		public static double/*Double*//*lua_Number*/ luaL_opt(lua_State L, luaL_opt_delegate f, int n, Double/*lua_Number*/ d)
		{
			return Lua.lua_isnoneornil(L, (n != 0) ? d : f.exec(L, n)) ? 1 : 0;
		}

		public static int/*Int32*//*lua_Integer*/ luaL_opt_integer(lua_State L, luaL_opt_delegate_integer f, int n, Double/*lua_Number*/ d)
		{
			return (int/*Int32*//*lua_Integer*/)(Lua.lua_isnoneornil(L, n) ? d : f.exec(L, (n)));
		}

		/*
		 ** {======================================================
		 ** Generic Buffer manipulation
		 ** =======================================================
		 */
		public static void luaL_addchar(luaL_Buffer B, char c) 
		{
			if (B.p >= LuaConf.LUAL_BUFFERSIZE)
			{
				luaL_prepbuffer(B);
			}
			B.buffer.set(B.p++, c);
		}

		// compatibility only
		public static void luaL_putchar(luaL_Buffer B, char c)	
		{
			luaL_addchar(B,c);
		}

		public static void luaL_addsize(luaL_Buffer B, int n)	
		{
			B.p += n;
		}

		/* }====================================================== */


		/* compatibility with ref system */

		/* pre-defined references */
		public const int LUA_NOREF = (-2);
		public const int LUA_REFNIL = (-1);

		//#define lua_ref(L,lock) ((lock) ? luaL_ref(L, LUA_REGISTRYINDEX) : \
		//      (lua_pushstring(L, "unlocked references are obsolete"), lua_error(L), 0))

		//#define lua_unref(L,ref)        luaL_unref(L, LUA_REGISTRYINDEX, (ref))

		//#define lua_getref(L,ref)       lua_rawgeti(L, LUA_REGISTRYINDEX, (ref))


		//#define luaL_reg	luaL_Reg


		/* This file uses only the official API of Lua.
		 ** Any function declared here could be written as an application function.
		 */

		//#define lauxlib_c
		//#define LUA_LIB
		public const int FREELIST_REF = 0;	/* free list of references */

		/* convert a stack index to positive */
		public static int abs_index(lua_State L, int i)
		{
			return ((i) > 0 || (i) <= Lua.LUA_REGISTRYINDEX ? (i) : LuaAPI.lua_gettop(L) + (i) + 1);
		}


		/*
		 ** {======================================================
		 ** Error-report functions
		 ** =======================================================
		 */
		public static int luaL_argerror(lua_State L, int narg, CharPtr extramsg) 
		{
			lua_Debug ar = new lua_Debug();
			if (LuaDebug.lua_getstack(L, 0, ar) == 0)  /* no stack frame? */
			{
				return luaL_error(L, CharPtr.toCharPtr("bad argument #%d (%s)"), narg, extramsg);
			}
			LuaDebug.lua_getinfo(L, CharPtr.toCharPtr("n"), ar);
			if (LuaConf.strcmp(ar.namewhat, CharPtr.toCharPtr("method")) == 0)
			{
				narg--;  /* do not count `self' */
				if (narg == 0)  /* error is in the self argument itself? */
				{
					return luaL_error(L, CharPtr.toCharPtr("calling " + LuaConf.getLUA_QS() + " on bad self ({1})"),
					              ar.name, extramsg); //FIXME:
				}
			}
			if (CharPtr.isEqual(ar.name, null))
			{
				ar.name = CharPtr.toCharPtr("?");
			}
			return luaL_error(L, CharPtr.toCharPtr("bad argument #%d to " + LuaConf.getLUA_QS() + " (%s)"),
				narg, ar.name, extramsg);
		}

		public static int luaL_typerror(lua_State L, int narg, CharPtr tname) 
		{
			CharPtr msg = LuaAPI.lua_pushfstring(L, CharPtr.toCharPtr("%s expected, got %s"),
				tname, luaL_typename(L, narg));
			return luaL_argerror(L, narg, msg);
		}

		private static void tag_error(lua_State L, int narg, int tag) 
		{
			luaL_typerror(L, narg, LuaAPI.lua_typename(L, tag));
		}

		public static void luaL_where(lua_State L, int level) 
		{
			lua_Debug ar = new lua_Debug();
			if (LuaDebug.lua_getstack(L, level, ar) != 0) 
			{  
				/* check function at level */
				LuaDebug.lua_getinfo(L, CharPtr.toCharPtr("Sl"), ar);  /* get info about it */
				if (ar.currentline > 0) 
				{  
					/* is there info? */
					LuaAPI.lua_pushfstring(L, CharPtr.toCharPtr("%s:%d: "), ar.short_src, ar.currentline);
					return;
				}
			}
			Lua.lua_pushliteral(L, CharPtr.toCharPtr(""));  /* else, no information available... */
		}

		public static int luaL_error(lua_State L, CharPtr fmt, params object[] p)
		{
			luaL_where(L, 1);
			LuaAPI.lua_pushvfstring(L, fmt, p);
			LuaAPI.lua_concat(L, 2);
			return LuaAPI.lua_error(L);
		}

		/* }====================================================== */

		public static int luaL_checkoption(lua_State L, int narg, CharPtr def, CharPtr [] lst) 
		{
			CharPtr name = (CharPtr.isNotEqual(def, null)) ? luaL_optstring(L, narg, def) :
				luaL_checkstring(L, narg);
			int i;
			for (i = 0; i < lst.Length; i++)
			{	
				if (LuaConf.strcmp(lst[i], name) == 0)
				{
					return i;
				}
			}
			return luaL_argerror(L, narg, LuaAPI.lua_pushfstring(L, CharPtr.toCharPtr("invalid option " + LuaConf.getLUA_QS()), name));
		}

		public static int luaL_newmetatable(lua_State L, CharPtr tname) 
		{
			LuaAPI.lua_getfield(L, Lua.LUA_REGISTRYINDEX, tname);  /* get registry.name */
			if (!Lua.lua_isnil(L, -1))  /* name already in use? */
			{
				return 0;  /* leave previous value on top, but return 0 */
			}
			Lua.lua_pop(L, 1);
			Lua.lua_newtable(L);  /* create metatable */
			LuaAPI.lua_pushvalue(L, -1);
			LuaAPI.lua_setfield(L, Lua.LUA_REGISTRYINDEX, tname);  /* registry.name = metatable */
			return 1;
		}

		public static object luaL_checkudata(lua_State L, int ud, CharPtr tname) 
		{
			object p = LuaAPI.lua_touserdata(L, ud);
			if (p != null) 
			{  
				/* value is a userdata? */
				if (LuaAPI.lua_getmetatable(L, ud) != 0)
				{  
					/* does it have a metatable? */
					LuaAPI.lua_getfield(L, Lua.LUA_REGISTRYINDEX, tname);  /* get correct metatable */
					if (LuaAPI.lua_rawequal(L, -1, -2) != 0)
					{  
						/* does it have the correct mt? */
						Lua.lua_pop(L, 2);  /* remove both metatables */
						return p;
					}
				}
			}
			luaL_typerror(L, ud, tname);  /* else error */
			return null;  /* to avoid warnings */
		}

		public static void luaL_checkstack(lua_State L, int space, CharPtr mes) 
		{
			if (LuaAPI.lua_checkstack(L, space) == 0)
			{
				luaL_error(L, CharPtr.toCharPtr("stack overflow (%s)"), mes);
			}
		}

		public static void luaL_checktype(lua_State L, int narg, int t) 
		{
			if (LuaAPI.lua_type(L, narg) != t)
			{
				tag_error(L, narg, t);
			}
		}


		public static void luaL_checkany(lua_State L, int narg) 
		{
			if (LuaAPI.lua_type(L, narg) == Lua.LUA_TNONE)
			{
				luaL_argerror(L, narg, CharPtr.toCharPtr("value expected"));
			}
		}

		public static CharPtr luaL_checklstring(lua_State L, int narg)
		{
			int[]/*uint*/ len = new int[1];
			return luaL_checklstring(L, narg, /*out*/ len);
		}
		
		public static CharPtr luaL_checklstring(lua_State L, int narg, /*out*/ int[]/*uint*/ len)
		{
			CharPtr s = LuaAPI.lua_tolstring(L, narg, /*out*/ len);
			if (CharPtr.isEqual(s, null)) 
			{
				tag_error(L, narg, Lua.LUA_TSTRING);
			}
			return s;
		}

		public static CharPtr luaL_optlstring(lua_State L, int narg, CharPtr def)
		{
			int[]/*uint*/ len = new int[1];
			return luaL_optlstring(L, narg, def, /*out*/ len);
		}
		
		public static CharPtr luaL_optlstring(lua_State L, int narg, CharPtr def, /*out*/ int[]/*uint*/ len)
		{
			if (Lua.lua_isnoneornil(L, narg))
			{
				len[0] = /*(uint)*/((CharPtr.isNotEqual(def, null)) ? LuaConf.strlen(def) : 0);
				return def;
			}
			else
			{
				return luaL_checklstring(L, narg, /*out*/ len);
			}
		}

		public static double/*Double*//*lua_Number*/ luaL_checknumber(lua_State L, int narg)
		{
			double/*Double*//*lua_Number*/ d = LuaAPI.lua_tonumber(L, narg);
			if ((d == 0) && (LuaAPI.lua_isnumber(L, narg) == 0))  /* avoid extra test when d is not 0 */
			{
				tag_error(L, narg, Lua.LUA_TNUMBER);
			}
			return d;
		}

		public class luaL_checknumber_delegate : luaL_opt_delegate
		{
			public double/*Double*//*lua_Number*/ exec(lua_State L, int narg) 
			{
				return luaL_checknumber(L, narg);
			}
		}
		
		public static double/*Double*//*lua_Number*/ luaL_optnumber(lua_State L, int narg, Double/*lua_Number*/ def)
		{
			return luaL_opt(L, new luaL_checknumber_delegate(), narg, def);
		}

		public static int/*Int32*//*lua_Integer*/ luaL_checkinteger(lua_State L, int narg)
		{
			int/*Int32*//*lua_Integer*/ d = LuaAPI.lua_tointeger(L, narg);
			if (d == 0 && LuaAPI.lua_isnumber(L, narg) == 0)  /* avoid extra test when d is not 0 */
			tag_error(L, narg, Lua.LUA_TNUMBER);
			return d;
		}
		
		public class luaL_checkinteger_delegate : luaL_opt_delegate_integer
		{
			public int/*Int32*//*lua_Integer*/ exec(lua_State L, int narg)
			{
				return luaL_checkinteger(L, narg);
			}
		}

		public static int/*Int32*//*lua_Integer*/ luaL_optinteger(lua_State L, int narg, int/*Int32*//*lua_Integer*/ def)
		{
			return luaL_opt_integer(L, new luaL_checkinteger_delegate(), narg, def);
		}

		public static int luaL_getmetafield(lua_State L, int obj, CharPtr event_) 
		{
			if (LuaAPI.lua_getmetatable(L, obj) == 0)  /* no metatable? */
			{
				return 0;
			}
			LuaAPI.lua_pushstring(L, event_);
			LuaAPI.lua_rawget(L, -2);
			if (Lua.lua_isnil(L, -1))
			{
				Lua.lua_pop(L, 2);  /* remove metatable and metafield */
				return 0;
			}
			else 
			{
				LuaAPI.lua_remove(L, -2);  /* remove only metatable */
				return 1;
			}
		}

		public static int luaL_callmeta (lua_State L, int obj, CharPtr event_) 
		{
			obj = abs_index(L, obj);
			if (luaL_getmetafield(L, obj, event_)==0)  /* no metafield? */
			{
				return 0;
			}
			LuaAPI.lua_pushvalue(L, obj);
			LuaAPI.lua_call(L, 1, 1);
			return 1;
		}

		public static void luaL_register(lua_State L, CharPtr libname, luaL_Reg[] l) 
		{
			luaI_openlib(L, libname, l, 0);
		}

		// we could just take the .Length member here, but let's try
		// to keep it as close to the C implementation as possible.
		private static int libsize(luaL_Reg[] l) 
		{
			int size = 0;
			for (; CharPtr.isNotEqual(l[size].name, null); size++)
			{
				;
			}
			return size;
		}

		public static void luaI_openlib (lua_State L, CharPtr libname, luaL_Reg[] l, int nup) 
		{
			if (CharPtr.isNotEqual(libname, null)) 
			{
				int size = libsize(l);
				/* check whether lib already exists */
				luaL_findtable(L, Lua.LUA_REGISTRYINDEX, CharPtr.toCharPtr("_LOADED"), 1);
				LuaAPI.lua_getfield(L, -1, libname);  /* get _LOADED[libname] */
				if (!Lua.lua_istable(L, -1))
				{  
					/* not found? */
					Lua.lua_pop(L, 1);  /* remove previous result */
					/* try global variable (and create one if it does not exist) */
					if (CharPtr.isNotEqual(luaL_findtable(L, Lua.LUA_GLOBALSINDEX, libname, size), null))
					{
						luaL_error(L, CharPtr.toCharPtr("name conflict for module " + LuaConf.getLUA_QS()), libname);
					}
					LuaAPI.lua_pushvalue(L, -1);
					LuaAPI.lua_setfield(L, -3, libname);  /* _LOADED[libname] = new table */
				}
				LuaAPI.lua_remove(L, -2);  /* remove _LOADED table */
				LuaAPI.lua_insert(L, -(nup + 1));  /* move library table to below upvalues */
			}
			int reg_num = 0;
			for (; CharPtr.isNotEqual(l[reg_num].name, null); reg_num++) 
			{
				int i;
				for (i = 0; i < nup; i++)  /* copy upvalues to the top */
				{
					LuaAPI.lua_pushvalue(L, -nup);
				}
				LuaAPI.lua_pushcclosure(L, l[reg_num].func, nup);
				LuaAPI.lua_setfield(L, -(nup + 2), l[reg_num].name);
			}
			Lua.lua_pop(L, nup);  /* remove upvalues */
		}

		/*
		 ** {======================================================
		 ** getn-setn: size for arrays
		 ** =======================================================
		 */
		
		//#if LUA_COMPAT_GETN

//		static int checkint(lua_State L, int topop) 
//		{
//			int n = (lua_type(L, -1) == LUA_TNUMBER) ? lua_tointeger(L, -1) : -1;
//			lua_pop(L, topop);
//			return n;
//		}
//
//		static void getsizes(lua_State L) 
//		{
//			lua_getfield(L, LUA_REGISTRYINDEX, "LUA_SIZES");
//			if (lua_isnil(L, -1)) 
//			{  
//				/* no `size' table? */
//				lua_pop(L, 1);  /* remove nil */
//				lua_newtable(L);  /* create it */
//				lua_pushvalue(L, -1);  /* `size' will be its own metatable */
//				lua_setmetatable(L, -2);
//				lua_pushliteral(L, "kv");
//				lua_setfield(L, -2, "__mode");  /* metatable(N).__mode = "kv" */
//				lua_pushvalue(L, -1);
//				lua_setfield(L, LUA_REGISTRYINDEX, "LUA_SIZES");  /* store in register */
//			}
//		}
//
//		public static void luaL_setn(lua_State L, int t, int n) 
//		{
//			t = abs_index(L, t);
//			lua_pushliteral(L, "n");
//			lua_rawget(L, t);
//			if (checkint(L, 1) >= 0) 
//			{  
//				/* is there a numeric field `n'? */
//				lua_pushliteral(L, "n");  /* use it */
//				lua_pushinteger(L, n);
//				lua_rawset(L, t);
//			}
//			else 
//			{  
//				/* use `sizes' */
//				getsizes(L);
//				lua_pushvalue(L, t);
//				lua_pushinteger(L, n);
//				lua_rawset(L, -3);  /* sizes[t] = n */
//				lua_pop(L, 1);  /* remove `sizes' */
//			}
//		}
//
//		public static int luaL_getn(lua_State L, int t) 
//		{
//			int n;
//			t = abs_index(L, t);
//			lua_pushliteral(L, "n");  /* try t.n */
//			lua_rawget(L, t);
//			if ((n = checkint(L, 1)) >= 0) 
//			{
//				return n;
//			}
//			getsizes(L);  /* else try sizes[t] */
//			lua_pushvalue(L, t);
//			lua_rawget(L, -2);
//			if ((n = checkint(L, 2)) >= 0) 
//			{
//				return n;
//			}
//			return (int)lua_objlen(L, t);
//		}

		//#endif

		/* }====================================================== */

		public static CharPtr luaL_gsub(lua_State L, CharPtr s, CharPtr p, CharPtr r) 
		{
			CharPtr wild;
			int/*uint*/ l = /*(uint)*/LuaConf.strlen(p);
			luaL_Buffer b = new luaL_Buffer();
			luaL_buffinit(L, b);
			while (CharPtr.isNotEqual((wild = LuaConf.strstr(s, p)), null))
			{
				luaL_addlstring(b, s, /*(uint)*/CharPtr.minus(wild, s));  /* push prefix */
				luaL_addstring(b, r);  /* push replacement in place of pattern */
				s = CharPtr.plus(wild, l);  /* continue after `p' */
			}
			luaL_addstring(b, s);  /* push last suffix */
			luaL_pushresult(b);
			return Lua.lua_tostring(L, -1);
		}

		public static CharPtr luaL_findtable(lua_State L, int idx, CharPtr fname, int szhint) 
		{
			CharPtr e;
			LuaAPI.lua_pushvalue(L, idx);
			do 
			{
				e = LuaConf.strchr(fname, '.');
				if (CharPtr.isEqual(e, null)) 
				{
					e = CharPtr.plus(fname, LuaConf.strlen(fname));
				}
				LuaAPI.lua_pushlstring(L, fname, /*(uint)*/CharPtr.minus(e, fname));
				LuaAPI.lua_rawget(L, -2);
				if (Lua.lua_isnil(L, -1))
				{
					/* no such field? */
					Lua.lua_pop(L, 1);  /* remove this nil */
					LuaAPI.lua_createtable(L, 0, (CharPtr.isEqualChar(e, '.') ? 1 : szhint)); /* new table for field */
					LuaAPI.lua_pushlstring(L, fname, /*(uint)*/CharPtr.minus(e, fname));
					LuaAPI.lua_pushvalue(L, -2);
					LuaAPI.lua_settable(L, -4);  /* set new table into field */
				}
				else if (!Lua.lua_istable(L, -1))
				{
					/* field has a non-table value? */
					Lua.lua_pop(L, 2);  /* remove table and value */
					return fname;  /* return problematic part of the name */
				}
				LuaAPI.lua_remove(L, -2);  /* remove previous table */
				fname = CharPtr.plus(e, 1);
            } while (CharPtr.isEqualChar(e, '.'));
			return null;
		}

		/*
		 ** {======================================================
		 ** Generic Buffer manipulation
		 ** =======================================================
		 */
		private static int bufflen(luaL_Buffer B)	
		{
			return B.p;
		}
		
		private static int bufffree(luaL_Buffer B) 
		{ 
			return LuaConf.LUAL_BUFFERSIZE - bufflen(B); 
		}

		public const int LIMIT = Lua.LUA_MINSTACK / 2;

		private static int emptybuffer(luaL_Buffer B)
		{
			int/*uint*/ l = /*(uint)*/bufflen(B);
			if (l == 0)
			{
				return 0;  /* put nothing on stack */
			}
			else
			{
				LuaAPI.lua_pushlstring(B.L, B.buffer, l);
				B.p = 0;
				B.lvl++;
				return 1;
			}
		}

		private static void adjuststack(luaL_Buffer B) 
		{	
			if (B.lvl > 1) 
			{
				lua_State L = B.L;
				int toget = 1;  /* number of levels to concat */
				int/*uint*/ toplen = Lua.lua_strlen(L, -1);
				do 
				{
					int/*uint*/ l = Lua.lua_strlen(L, -(toget + 1));
					if (B.lvl - toget + 1 >= LIMIT || toplen > l) 
					{
						toplen += l;
						toget++;
					}
					else 
					{
						break;
					}
				} while (toget < B.lvl);
				LuaAPI.lua_concat(L, toget);
				B.lvl = B.lvl - toget + 1;
			}
		}

		public static CharPtr luaL_prepbuffer(luaL_Buffer B) 
		{
			if (emptybuffer(B) != 0)
			{
				adjuststack(B);
			}
			return new CharPtr(B.buffer, B.p);
		}

		public static void luaL_addlstring(luaL_Buffer B, CharPtr s, int/*uint*/ l)
		{
			while (l-- != 0)
			{
				char c = s.get(0);
				s = s.next();
				luaL_addchar(B, c);
			}
		}

		public static void luaL_addstring(luaL_Buffer B, CharPtr s)
		{
			luaL_addlstring(B, s, /*(uint)*/LuaConf.strlen(s));
		}

		public static void luaL_pushresult(luaL_Buffer B)
		{
			emptybuffer(B);
			LuaAPI.lua_concat(B.L, B.lvl);
			B.lvl = 1;
		}

		public static void luaL_addvalue(luaL_Buffer B) 
		{
			lua_State L = B.L;
			int[]/*uint*/ vl = new int[1];
			CharPtr s = LuaAPI.lua_tolstring(L, -1, /*out*/ vl);
			if (vl[0] <= bufffree(B))
			{
				/* fit into buffer? */
				CharPtr dst = new CharPtr(B.buffer.chars, B.buffer.index + B.p);
				CharPtr src = new CharPtr(s.chars, s.index);
				for (int/*uint*/ i = 0; i < vl[0]; i++)
				{
					dst.set(i, src.get(i));
				}
				B.p += /*(int)*/vl[0];
				Lua.lua_pop(L, 1);  /* remove from stack */
			}
			else
			{
				if (emptybuffer(B) != 0)
				{
					LuaAPI.lua_insert(L, -2);  /* put buffer before new value */
				}
				B.lvl++;  /* add new value into B stack */
				adjuststack(B);
			}
		}

		public static void luaL_buffinit(lua_State L, luaL_Buffer B)
		{
			B.L = L;
			B.p = /*B.buffer*/ 0;
			B.lvl = 0;
		}

		/* }====================================================== */

		public static int luaL_ref(lua_State L, int t)
		{
			int ref_;
			t = abs_index(L, t);
			if (Lua.lua_isnil(L, -1))
			{
				Lua.lua_pop(L, 1);  /* remove from stack */
				return LUA_REFNIL;  /* `nil' has a unique fixed reference */
			}
			LuaAPI.lua_rawgeti(L, t, FREELIST_REF);  /* get first free element */
			ref_ = /*(int)*/LuaAPI.lua_tointeger(L, -1);  /* ref = t[FREELIST_REF] */
			Lua.lua_pop(L, 1);  /* remove it from stack */
			if (ref_ != 0)
			{
				/* any free element? */
				LuaAPI.lua_rawgeti(L, t, ref_);  /* remove it from list */
				LuaAPI.lua_rawseti(L, t, FREELIST_REF);  /* (t[FREELIST_REF] = t[ref]) */
			}
			else
			{
				/* no free elements */
				ref_ = /*(int)*/LuaAPI.lua_objlen(L, t);
				ref_++;  /* create new reference */
			}
			LuaAPI.lua_rawseti(L, t, ref_);
			return ref_;
		}

		public static void luaL_unref (lua_State L, int t, int ref_)
		{
			if (ref_ >= 0)
			{
				t = abs_index(L, t);
				LuaAPI.lua_rawgeti(L, t, FREELIST_REF);
				LuaAPI.lua_rawseti(L, t, ref_);  /* t[ref] = t[FREELIST_REF] */
				LuaAPI.lua_pushinteger(L, ref_);
				LuaAPI.lua_rawseti(L, t, FREELIST_REF);  /* t[FREELIST_REF] = ref */
			}
		}

		/*
		 ** {======================================================
		 ** Load functions
		 ** =======================================================
		 */
		public static CharPtr getF(lua_State L, object ud, /*out*/ int[]/*uint*/ size)
		{
			size[0] = 0;
			LoadF lf = (LoadF)ud;
			//(void)L;
			if (lf.extraline != 0) 
			{
				lf.extraline = 0;
				size[0] = 1;
				return CharPtr.toCharPtr("\n");
			}
			if (LuaConf.feof(lf.f) != 0) 
			{
				return null;
			}
			size[0] = /*(uint)*/LuaConf.fread(lf.buff, 1, lf.buff.chars.Length, lf.f);
			return (size[0] > 0) ? new CharPtr(lf.buff) : null;
		}

		private static int errfile(lua_State L, CharPtr what, int fnameindex) 
		{
			CharPtr serr = LuaConf.strerror(LuaConf.errno());
			CharPtr filename = CharPtr.plus(Lua.lua_tostring(L, fnameindex), 1);
			LuaAPI.lua_pushfstring(L, CharPtr.toCharPtr("cannot %s %s: %s"), what, filename, serr);
			LuaAPI.lua_remove(L, fnameindex);
			return LUA_ERRFILE;
		}

		public static int luaL_loadfile(lua_State L, CharPtr filename) 
		{
			LoadF lf = new LoadF();
			int status, readstatus;
			int c;
			int fnameindex = LuaAPI.lua_gettop(L) + 1;  /* index of filename on the stack */
			lf.extraline = 0;
			if (CharPtr.isEqual(filename, null)) 
			{
				Lua.lua_pushliteral(L, CharPtr.toCharPtr("=stdin"));
				lf.f = LuaConf.stdin;
			}
			else 
			{
				LuaAPI.lua_pushfstring(L, CharPtr.toCharPtr("@%s"), filename);
				lf.f = LuaConf.fopen(filename, CharPtr.toCharPtr("r"));
				if (lf.f == null) 
				{
					return errfile(L, CharPtr.toCharPtr("open"), fnameindex);
				}
			}
			c = LuaConf.getc(lf.f);
			if (c == '#') 
			{  
				/* Unix exec. file? */
				lf.extraline = 1;
				while ((c = LuaConf.getc(lf.f)) != LuaConf.EOF && c != '\n') 
				{
					;  /* skip first line */
				}
				if (c == '\n') 
				{
					c = LuaConf.getc(lf.f);
				}
			}
			if (c == Lua.LUA_SIGNATURE[0] && (CharPtr.isNotEqual(filename, null)))
			{
				/* binary file? */
				lf.f = LuaConf.freopen(filename, CharPtr.toCharPtr("rb"), lf.f);  /* reopen in binary mode */
				if (lf.f == null) 
				{
					return errfile(L, CharPtr.toCharPtr("reopen"), fnameindex);
				}
				/* skip eventual `#!...' */
				while ((c = LuaConf.getc(lf.f)) != LuaConf.EOF && c != Lua.LUA_SIGNATURE[0]) 
				{
					;
				}
				lf.extraline = 0;
			}
			LuaConf.ungetc(c, lf.f);
			status = LuaAPI.lua_load(L, new getF_delegate(), lf, Lua.lua_tostring(L, -1));
			readstatus = LuaConf.ferror(lf.f);
			if (CharPtr.isNotEqual(filename, null)) 
			{
				LuaConf.fclose(lf.f);  /* close file (even in case of errors) */
			}
			if (readstatus != 0) 
			{
				LuaAPI.lua_settop(L, fnameindex);  /* ignore results from `lua_load' */
				return errfile(L, CharPtr.toCharPtr("read"), fnameindex);
			}
			LuaAPI.lua_remove(L, fnameindex);
			return status;
		}

		static CharPtr getS(lua_State L, object ud, /*out*/ int[]/*uint*/ size)
		{
			LoadS ls = (LoadS)ud;
			//(void)L;
			//if (ls.size == 0) return null;
			size[0] = ls.size;
			ls.size = 0;
			return ls.s;
		}

		public static int luaL_loadbuffer(lua_State L, CharPtr buff, int/*uint*/ size, CharPtr name) 
		{
			LoadS ls = new LoadS();
			ls.s = new CharPtr(buff);
			ls.size = size;
			return LuaAPI.lua_load(L, new getS_delegate(), ls, name);
		}

		public static int luaL_loadstring(lua_State L, CharPtr s) 
		{
			return luaL_loadbuffer(L, s, /*(uint)*/LuaConf.strlen(s), s);
		}

		/* }====================================================== */

		private static object l_alloc(ClassType t) 
		{
			return t.Alloc();
		}
		
		public class l_alloc_delegate : lua_Alloc
		{
			public object exec(ClassType t)
			{
				return l_alloc(t);
			}
		}

		private static int panic(lua_State L) 
		{
			//(void)L;  /* to avoid warnings */
			LuaConf.fprintf(LuaConf.stderr, CharPtr.toCharPtr("PANIC: unprotected error in call to Lua API (%s)\n"),
				Lua.lua_tostring(L, -1));
			return 0;
		}

		public static lua_State luaL_newstate()
		{
			lua_State L = LuaState.lua_newstate(new l_alloc_delegate(), null);
			if (L != null) LuaAPI.lua_atpanic(L, new LuaAuxLib_delegate("panic"));
			return L;
		}
		
		
		public class LuaAuxLib_delegate : lua_CFunction
		{
			private string name;
			
			public LuaAuxLib_delegate(string name)
			{
				this.name = name;
			}
			
			public int exec(lua_State L)
			{
				if ("panic".Equals(name))
				{
					return panic(L);
				}
				else
				{
					return 0;
				}
			}
		}
		
		public class getF_delegate : lua_Reader
		{
			public CharPtr exec(lua_State L, object ud, /*out*/ int[]/*uint*/ sz)
			{
				return getF(L, ud, /*out*/ sz);
			}
		}
		
		public class getS_delegate : lua_Reader
		{
			public CharPtr exec(lua_State L, object ud, /*out*/ int[]/*uint*/ sz)
			{
				return getS(L, ud, /*out*/ sz);
			}
		}
	}

	//public delegate double/*Double*//*lua_Number*/ luaL_opt_delegate(lua_State L, int narg);
	//public delegate int/*Int32*//*lua_Integer*/ luaL_opt_delegate_integer(lua_State L, int narg);

	//public interface luaL_opt_delegate
	//{
	//	double/*Double*//*lua_Number*/ exec(lua_State L, int narg);
	//}

	//public interface luaL_opt_delegate_integer
	//{
	//	int/*Int32*//*lua_Integer*/ exec(lua_State L, int narg);
	//}
}
