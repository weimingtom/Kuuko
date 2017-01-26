/*
 ** $Id: ltablib.c,v 1.38.1.3 2008/02/14 16:46:58 roberto Exp $
 ** Library for Table Manipulation
 ** See Copyright Notice in lua.h
 */
using System;

namespace KopiLua
{
	//using lua_Number = System.Double;

	public class LuaTableLib
	{
		private static int aux_getn(lua_State L, int n) 
		{ 
			LuaAuxLib.luaL_checktype(L, n, Lua.LUA_TTABLE); 
			return LuaAuxLib.luaL_getn(L, n);
		}

		private static int foreachi(lua_State L) 
		{
			int i;
			int n = aux_getn(L, 1);
			LuaAuxLib.luaL_checktype(L, 2, Lua.LUA_TFUNCTION);
			for (i = 1; i <= n; i++) 
			{
				LuaAPI.lua_pushvalue(L, 2);  /* function */
				LuaAPI.lua_pushinteger(L, i);  /* 1st argument */
				LuaAPI.lua_rawgeti(L, 1, i);  /* 2nd argument */
				LuaAPI.lua_call(L, 2, 1);
				if (!Lua.lua_isnil(L, -1))
				{
					return 1;
				}
				Lua.lua_pop(L, 1);  /* remove nil result */
			}
			return 0;
		}
		
		private static int _foreach(lua_State L) 
		{
			LuaAuxLib.luaL_checktype(L, 1, Lua.LUA_TTABLE);
			LuaAuxLib.luaL_checktype(L, 2, Lua.LUA_TFUNCTION);
			LuaAPI.lua_pushnil(L);  /* first key */
			while (LuaAPI.lua_next(L, 1) != 0)
			{
				LuaAPI.lua_pushvalue(L, 2);  /* function */
				LuaAPI.lua_pushvalue(L, -3);  /* key */
				LuaAPI.lua_pushvalue(L, -3);  /* value */
				LuaAPI.lua_call(L, 2, 1);
				if (!Lua.lua_isnil(L, -1))
				{
					return 1;
				}
				Lua.lua_pop(L, 2);  /* remove value and result */
			}
			return 0;
		}

		private static int maxn(lua_State L) 
		{
			Double/*lua_Number*/ max = 0;
			LuaAuxLib.luaL_checktype(L, 1, Lua.LUA_TTABLE);
			LuaAPI.lua_pushnil(L);  /* first key */
			while (LuaAPI.lua_next(L, 1) != 0)
			{
				Lua.lua_pop(L, 1);  /* remove value */
				if (LuaAPI.lua_type(L, -1) == Lua.LUA_TNUMBER)
				{
					Double/*lua_Number*/ v = LuaAPI.lua_tonumber(L, -1);
					if (v > max) 
					{
						max = v;
					}
				}
			}
			LuaAPI.lua_pushnumber(L, max);
			return 1;
		}

		private static int getn(lua_State L) 
		{
			LuaAPI.lua_pushinteger(L, aux_getn(L, 1));
			return 1;
		}

		private static int setn(lua_State L) 
		{
			LuaAuxLib.luaL_checktype(L, 1, Lua.LUA_TTABLE);
			//#ifndef luaL_setn
			//luaL_setn(L, 1, luaL_checkint(L, 2));
			//#else
			LuaAuxLib.luaL_error(L, CharPtr.toCharPtr(LuaConf.LUA_QL("setn") + " is obsolete"));
			//#endif
			LuaAPI.lua_pushvalue(L, 1);
			return 1;
		}

		private static int tinsert(lua_State L) 
		{
			int e = aux_getn(L, 1) + 1;  /* first empty element */
			int pos;  /* where to insert new element */
			switch (LuaAPI.lua_gettop(L))
			{
				case 2: 
					{  
						/* called with only 2 arguments */
						pos = e;  /* insert new element at the end */
						break;
					}
				case 3: 
					{
						int i;
						pos = LuaAuxLib.luaL_checkint(L, 2);  /* 2nd argument is the position */
						if (pos > e) 
						{
							e = pos;  /* `grow' array if necessary */
						}
						for (i = e; i > pos; i--) 
						{  
							/* move up elements */
							LuaAPI.lua_rawgeti(L, 1, i - 1);
							LuaAPI.lua_rawseti(L, 1, i);  /* t[i] = t[i-1] */
						}
						break;
					}
				default: 
					{
						return LuaAuxLib.luaL_error(L, CharPtr.toCharPtr("wrong number of arguments to " + LuaConf.LUA_QL("insert")));
					}
			}
			LuaAuxLib.luaL_setn(L, 1, e);  /* new size */
			LuaAPI.lua_rawseti(L, 1, pos);  /* t[pos] = v */
			return 0;
		}

		private static int tremove(lua_State L) 
		{
			int e = aux_getn(L, 1);
			int pos = LuaAuxLib.luaL_optint(L, 2, e);
			if (!(1 <= pos && pos <= e))  /* position is outside bounds? */
			{
				return 0;  /* nothing to remove */
			}
			LuaAuxLib.luaL_setn(L, 1, e - 1);  /* t.n = n-1 */
			LuaAPI.lua_rawgeti(L, 1, pos);  /* result = t[pos] */
			for (;pos<e; pos++) 
			{
				LuaAPI.lua_rawgeti(L, 1, pos + 1);
				LuaAPI.lua_rawseti(L, 1, pos);  /* t[pos] = t[pos+1] */
			}
			LuaAPI.lua_pushnil(L);
			LuaAPI.lua_rawseti(L, 1, e);  /* t[e] = nil */
			return 1;
		}

		private static void addfield(lua_State L, luaL_Buffer b, int i) 
		{
			LuaAPI.lua_rawgeti(L, 1, i);
			if (LuaAPI.lua_isstring(L, -1) == 0)
			{
				LuaAuxLib.luaL_error(L, CharPtr.toCharPtr("invalid value (%s) at index %d in table for " +
					LuaConf.LUA_QL("concat")), LuaAuxLib.luaL_typename(L, -1), i);
			}
			LuaAuxLib.luaL_addvalue(b);
		}


		private static int tconcat(lua_State L) 
		{
			luaL_Buffer b = new luaL_Buffer();
			int[]/*uint*/ lsep = new int[1];
			int i, last;
			CharPtr sep = LuaAuxLib.luaL_optlstring(L, 2, CharPtr.toCharPtr(""), /*out*/ lsep);
			LuaAuxLib.luaL_checktype(L, 1, Lua.LUA_TTABLE);
			i = LuaAuxLib.luaL_optint(L, 3, 1);
			last = LuaAuxLib.luaL_opt_integer(L, new LuaAuxLib.luaL_checkint_delegate(), 4, LuaAuxLib.luaL_getn(L, 1));
			LuaAuxLib.luaL_buffinit(L, b);
			for (; i < last; i++) 
			{
				addfield(L, b, i);
				LuaAuxLib.luaL_addlstring(b, sep, lsep[0]);
			}
			if (i == last)  /* add last value (if interval was not empty) */
			{
				addfield(L, b, i);
			}
			LuaAuxLib.luaL_pushresult(b);
			return 1;
		}

		/*
		 ** {======================================================
		 ** Quicksort
		 ** (based on `Algorithms in MODULA-3', Robert Sedgewick;
		 **  Addison-Wesley, 1993.)
		 */
		
		private static void set2(lua_State L, int i, int j) 
		{
			LuaAPI.lua_rawseti(L, 1, i);
			LuaAPI.lua_rawseti(L, 1, j);
		}

		private static int sort_comp(lua_State L, int a, int b) 
		{
			if (!Lua.lua_isnil(L, 2))
			{  
				/* function? */
				int res;
				LuaAPI.lua_pushvalue(L, 2);
				LuaAPI.lua_pushvalue(L, a - 1);  /* -1 to compensate function */
				LuaAPI.lua_pushvalue(L, b - 2);  /* -2 to compensate function and `a' */
				LuaAPI.lua_call(L, 2, 1);
				res = LuaAPI.lua_toboolean(L, -1);
				Lua.lua_pop(L, 1);
				return res;
			}
			else  /* a < b? */
			{
				return LuaAPI.lua_lessthan(L, a, b);
			}
		}

		private static int auxsort_loop1(lua_State L, /*ref*/ int[] i)
		{
			LuaAPI.lua_rawgeti(L, 1, ++i[0]);
			return sort_comp(L, -1, -2);
		}

		private static int auxsort_loop2(lua_State L, /*ref*/ int[] j)
		{
			LuaAPI.lua_rawgeti(L, 1, --j[0]);
			return sort_comp(L, -3, -1);
		}

		private static void auxsort(lua_State L, int l, int u) 
		{
			while (l < u) 
			{  
				/* for tail recursion */
				int i, j;
				/* sort elements a[l], a[(l+u)/2] and a[u] */
				LuaAPI.lua_rawgeti(L, 1, l);
				LuaAPI.lua_rawgeti(L, 1, u);
				if (sort_comp(L, -1, -2) != 0)  /* a[u] < a[l]? */
				{
					set2(L, l, u);  /* swap a[l] - a[u] */
				}
				else
				{
					Lua.lua_pop(L, 2);
				}
				if (u - l == 1) 
				{
					break;  /* only 2 elements */
				}
				i = (l + u) / 2;
				LuaAPI.lua_rawgeti(L, 1, i);
				LuaAPI.lua_rawgeti(L, 1, l);
				if (sort_comp(L, -2, -1) != 0)  /* a[i]<a[l]? */
				{
					set2(L, i, l);
				}
				else 
				{
					Lua.lua_pop(L, 1);  /* remove a[l] */
					LuaAPI.lua_rawgeti(L, 1, u);
					if (sort_comp(L, -1, -2) != 0)  /* a[u]<a[i]? */
					{
						set2(L, i, u);
					}
					else
					{
						Lua.lua_pop(L, 2);
					}
				}
				if (u - l == 2) 
				{
					break;  /* only 3 elements */
				}
				LuaAPI.lua_rawgeti(L, 1, i);  /* Pivot */
				LuaAPI.lua_pushvalue(L, -1);
				LuaAPI.lua_rawgeti(L, 1, u - 1);
				set2(L, i, u - 1);
				/* a[l] <= P == a[u-1] <= a[u], only need to sort from l+1 to u-2 */
				i = l; 
				j = u - 1;
				for (;;) 
				{  
					/* invariant: a[l..i] <= P <= a[j..u] */
					/* repeat ++i until a[i] >= P */
					while (true) 
					{
						int[] i_ref = new int[1];
						i_ref[0] = i;
						int ret_1 = auxsort_loop1(L, /*ref*/ i_ref);
						i = i_ref[0];
						if (!(ret_1 != 0))
						{
							break;
						}
						if (i > u) 
						{
							LuaAuxLib.luaL_error(L, CharPtr.toCharPtr("invalid order function for sorting"));
						}
						Lua.lua_pop(L, 1);  /* remove a[i] */
					}
					/* repeat --j until a[j] <= P */
					while (true) 
					{
						int[] j_ref = new int[1];
						j_ref[0] = i;
						int ret_2 = auxsort_loop2(L, /*ref*/ j_ref);
						j = j_ref[0];
						if (!(ret_2 != 0))
						{
							break;
						}
						if (j < l) 
						{
							LuaAuxLib.luaL_error(L, CharPtr.toCharPtr("invalid order function for sorting"));
						}
						Lua.lua_pop(L, 1);  /* remove a[j] */
					}
					if (j < i) 
					{
						Lua.lua_pop(L, 3);  /* pop pivot, a[i], a[j] */
						break;
					}
					set2(L, i, j);
				}
				LuaAPI.lua_rawgeti(L, 1, u - 1);
				LuaAPI.lua_rawgeti(L, 1, i);
				set2(L, u-1, i);  /* swap pivot (a[u-1]) with a[i] */
				/* a[l..i-1] <= a[i] == P <= a[i+1..u] */
				/* adjust so that smaller half is in [j..i] and larger one in [l..u] */
				if (i - l < u - i) 
				{
					j = l; 
					i = i - 1; 
					l = i + 2;
				}
				else 
				{
					j = i + 1; 
					i = u; 
					u = j - 2;
				}
				auxsort(L, j, i);  /* call recursively the smaller one */
			}  /* repeat the routine for the larger one */
		}

		private static int sort(lua_State L) 
		{
			int n = aux_getn(L, 1);
			LuaAuxLib.luaL_checkstack(L, 40, CharPtr.toCharPtr(""));  /* assume array is smaller than 2^40 */
			if (!Lua.lua_isnoneornil(L, 2))  /* is there a 2nd argument? */
			{
				LuaAuxLib.luaL_checktype(L, 2, Lua.LUA_TFUNCTION);
			}
			LuaAPI.lua_settop(L, 2);  /* make sure there is two arguments */
			auxsort(L, 1, n);
			return 0;
		}

		/* }====================================================== */

		private readonly static luaL_Reg[] tab_funcs = {
			new luaL_Reg(CharPtr.toCharPtr("concat"), new LuaTableLib_delegate("tconcat")),
			new luaL_Reg(CharPtr.toCharPtr("foreach"), new LuaTableLib_delegate("_foreach")),
			new luaL_Reg(CharPtr.toCharPtr("foreachi"), new LuaTableLib_delegate("foreachi")),
			new luaL_Reg(CharPtr.toCharPtr("getn"), new LuaTableLib_delegate("getn")),
			new luaL_Reg(CharPtr.toCharPtr("maxn"), new LuaTableLib_delegate("maxn")),
			new luaL_Reg(CharPtr.toCharPtr("insert"), new LuaTableLib_delegate("tinsert")),
			new luaL_Reg(CharPtr.toCharPtr("remove"), new LuaTableLib_delegate("tremove")),
			new luaL_Reg(CharPtr.toCharPtr("setn"), new LuaTableLib_delegate("setn")),
			new luaL_Reg(CharPtr.toCharPtr("sort"), new LuaTableLib_delegate("sort")),
			new luaL_Reg(null, null)
		};

		public static int luaopen_table(lua_State L) 
		{
			LuaAuxLib.luaL_register(L, CharPtr.toCharPtr(LuaLib.LUA_TABLIBNAME), tab_funcs);
			return 1;
		}
		
		public class LuaTableLib_delegate : lua_CFunction
		{
			private string name;
			
			public LuaTableLib_delegate(string name)
			{
				this.name = name;
			}
			
			public int exec(lua_State L)
			{
				if ("tconcat".Equals(name))
				{
					return tconcat(L);
				} 
				else if ("_foreach".Equals(name)) 
				{
					return _foreach(L);
				} 
				else if ("foreachi".Equals(name)) 
				{
					return foreachi(L);
				} 
				else if ("getn".Equals(name)) 
				{
				    return getn(L);
				}
				else if ("maxn".Equals(name))
				{
				    return maxn(L);
				}
				else if ("tinsert".Equals(name))
				{
					return tinsert(L);
				}
				else if ("tremove".Equals(name))
				{
					return tremove(L);
				}
				else if ("setn".Equals(name))
				{
					return setn(L);
				}
				else if ("sort".Equals(name))
				{
					return sort(L);
				}
				else
				{
					return 0;
				}
			}
		}
	}
}
