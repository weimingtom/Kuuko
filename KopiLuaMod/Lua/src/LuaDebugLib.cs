/*
 ** $Id: ldblib.c,v 1.104.1.3 2008/01/21 13:11:21 roberto Exp $
 ** Interface from Lua to its debug API
 ** See Copyright Notice in lua.h
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace KopiLua
{
	public class LuaDebugLib
	{
		private static int db_getregistry(lua_State L) 
		{
			LuaAPI.lua_pushvalue(L, Lua.LUA_REGISTRYINDEX);
			return 1;
		}

		private static int db_getmetatable(lua_State L) 
		{
			LuaAuxLib.luaL_checkany(L, 1);
			if (LuaAPI.lua_getmetatable(L, 1) == 0)
			{
				LuaAPI.lua_pushnil(L);  /* no metatable */
			}
			return 1;
		}

		private static int db_setmetatable(lua_State L) 
		{
			int t = LuaAPI.lua_type(L, 2);
			LuaAuxLib.luaL_argcheck(L, t == Lua.LUA_TNIL || t == Lua.LUA_TTABLE, 2,
				"nil or table expected");
			LuaAPI.lua_settop(L, 2);
			LuaAPI.lua_pushboolean(L, LuaAPI.lua_setmetatable(L, 1));
			return 1;
		}

		private static int db_getfenv(lua_State L) 
		{
			LuaAPI.lua_getfenv(L, 1);
			return 1;
		}


		private static int db_setfenv(lua_State L) 
		{
			LuaAuxLib.luaL_checktype(L, 2, Lua.LUA_TTABLE);
			LuaAPI.lua_settop(L, 2);
			if (LuaAPI.lua_setfenv(L, 1) == 0)
			{	
				LuaAuxLib.luaL_error(L, LuaConf.LUA_QL("setfenv") +
					" cannot change environment of given object");
			}
			return 1;
		}

		private static void settabss(lua_State L, CharPtr i, CharPtr v) 
		{
			LuaAPI.lua_pushstring(L, v);
			LuaAPI.lua_setfield(L, -2, i);
		}

		private static void settabsi(lua_State L, CharPtr i, int v) 
		{
			LuaAPI.lua_pushinteger(L, v);
			LuaAPI.lua_setfield(L, -2, i);
		}

		private static lua_State getthread(lua_State L, out int arg) 
		{
			if (Lua.lua_isthread(L, 1)) 
			{
				arg = 1;
				return LuaAPI.lua_tothread(L, 1);
			}
			else 
			{
				arg = 0;
				return L;
			}
		}

		private static void treatstackoption(lua_State L, lua_State L1, CharPtr fname) 
		{
			if (L == L1) 
			{
				LuaAPI.lua_pushvalue(L, -2);
				LuaAPI.lua_remove(L, -3);
			}
			else
			{
				LuaAPI.lua_xmove(L1, L, 1);
			}
			LuaAPI.lua_setfield(L, -2, fname);
		}

		private static int db_getinfo(lua_State L) 
		{
			lua_Debug ar = new lua_Debug();
			int arg;
			lua_State L1 = getthread(L, out arg);
			CharPtr options = LuaAuxLib.luaL_optstring(L, arg + 2, "flnSu");
			if (LuaAPI.lua_isnumber(L, arg + 1) != 0)
			{
				if (LuaDebug.lua_getstack(L1, (int)LuaAPI.lua_tointeger(L, arg + 1), ar) == 0)
				{
					LuaAPI.lua_pushnil(L);  /* level out of range */
					return 1;
				}
			}
			else if (Lua.lua_isfunction(L, arg + 1)) 
			{
				LuaAPI.lua_pushfstring(L, ">%s", options);
				options = Lua.lua_tostring(L, -1);
				LuaAPI.lua_pushvalue(L, arg + 1);
				LuaAPI.lua_xmove(L, L1, 1);
			}
			else
			{
				return LuaAuxLib.luaL_argerror(L, arg + 1, "function or level expected");
			}
			if (LuaDebug.lua_getinfo(L1, options, ar) == 0)
			{
				return LuaAuxLib.luaL_argerror(L, arg + 2, "invalid option");
			}
			LuaAPI.lua_createtable(L, 0, 2);
			if (LuaConf.strchr(options, 'S') != null)
			{
				settabss(L, "source", ar.source);
				settabss(L, "short_src", ar.short_src);
				settabsi(L, "linedefined", ar.linedefined);
				settabsi(L, "lastlinedefined", ar.lastlinedefined);
				settabss(L, "what", ar.what);
			}
			if (LuaConf.strchr(options, 'l') != null)
			{
				settabsi(L, "currentline", ar.currentline);
			}
			if (LuaConf.strchr(options, 'u') != null)
			{
				settabsi(L, "nups", ar.nups);
			}
			if (LuaConf.strchr(options, 'n') != null)
			{
				settabss(L, "name", ar.name);
				settabss(L, "namewhat", ar.namewhat);
			}
			if (LuaConf.strchr(options, 'L') != null)
			{
				treatstackoption(L, L1, "activelines");
			}
			if (LuaConf.strchr(options, 'f') != null)
			{
				treatstackoption(L, L1, "func");
			}
			return 1;  /* return table */
		}
		
		private static int db_getlocal(lua_State L) 
		{
			int arg;
			lua_State L1 = getthread(L, out arg);
			lua_Debug ar = new lua_Debug();
			CharPtr name;
			if (LuaDebug.lua_getstack(L1, LuaAuxLib.luaL_checkint(L, arg + 1), ar) == 0)  /* out of range? */
			{
				return LuaAuxLib.luaL_argerror(L, arg + 1, "level out of range");
			}
			name = LuaDebug.lua_getlocal(L1, ar, LuaAuxLib.luaL_checkint(L, arg + 2));
			if (name != null) 
			{
				LuaAPI.lua_xmove(L1, L, 1);
				LuaAPI.lua_pushstring(L, name);
				LuaAPI.lua_pushvalue(L, -2);
				return 2;
			}
			else 
			{
				LuaAPI.lua_pushnil(L);
				return 1;
			}
		}

		private static int db_setlocal(lua_State L) 
		{
			int arg;
			lua_State L1 = getthread(L, out arg);
			lua_Debug ar = new lua_Debug();
			if (LuaDebug.lua_getstack(L1, LuaAuxLib.luaL_checkint(L, arg + 1), ar) == 0)  /* out of range? */
			{
				return LuaAuxLib.luaL_argerror(L, arg + 1, "level out of range");
			}
			LuaAuxLib.luaL_checkany(L, arg + 3);
			LuaAPI.lua_settop(L, arg + 3);
			LuaAPI.lua_xmove(L, L1, 1);
			LuaAPI.lua_pushstring(L, LuaDebug.lua_setlocal(L1, ar, LuaAuxLib.luaL_checkint(L, arg + 2)));
			return 1;
		}

		private static int auxupvalue(lua_State L, int get) 
		{
			CharPtr name;
			int n = LuaAuxLib.luaL_checkint(L, 2);
			LuaAuxLib.luaL_checktype(L, 1, Lua.LUA_TFUNCTION);
			if (LuaAPI.lua_iscfunction(L, 1)) return 0;  /* cannot touch C upvalues from Lua */
			{
				name = (get != 0) ? LuaAPI.lua_getupvalue(L, 1, n) : LuaAPI.lua_setupvalue(L, 1, n);
			}
			if (name == null) 
			{
				return 0;
			}
			LuaAPI.lua_pushstring(L, name);
			LuaAPI.lua_insert(L, -(get + 1));
			return get + 1;
		}

		private static int db_getupvalue(lua_State L) 
		{
			return auxupvalue(L, 1);
		}

		private static int db_setupvalue(lua_State L) 
		{
			LuaAuxLib.luaL_checkany(L, 3);
			return auxupvalue(L, 0);
		}
		
		private const string KEY_HOOK = "h";

		private static readonly string[] hooknames = {
			"call", 
			"return", 
			"line", 
			"count", 
			"tail return"
		};

		private static void hookf(lua_State L, lua_Debug ar) 
		{
			LuaAPI.lua_pushlightuserdata(L, KEY_HOOK);
			LuaAPI.lua_rawget(L, Lua.LUA_REGISTRYINDEX);
			LuaAPI.lua_pushlightuserdata(L, L);
			LuaAPI.lua_rawget(L, -2);
			if (Lua.lua_isfunction(L, -1))
			{
				LuaAPI.lua_pushstring(L, hooknames[(int)ar.event_]);
				if (ar.currentline >= 0)
				{
					LuaAPI.lua_pushinteger(L, ar.currentline);
				}
				else
				{
					LuaAPI.lua_pushnil(L);
				}
				LuaLimits.lua_assert(LuaDebug.lua_getinfo(L, "lS", ar));
				LuaAPI.lua_call(L, 2, 0);
			}
		}

		private static int makemask(CharPtr smask, int count) 
		{
			int mask = 0;
			if (LuaConf.strchr(smask, 'c') != null) 
			{
				mask |= Lua.LUA_MASKCALL;
			}
			if (LuaConf.strchr(smask, 'r') != null) 
			{
				mask |= Lua.LUA_MASKRET;
			}
			if (LuaConf.strchr(smask, 'l') != null) 
			{
				mask |= Lua.LUA_MASKLINE;
			}
			if (count > 0) 
			{
				mask |= Lua.LUA_MASKCOUNT;
			}
			return mask;
		}

		private static CharPtr unmakemask (int mask, CharPtr smask) 
		{
			int i = 0;
			if ((mask & Lua.LUA_MASKCALL) != 0) 
			{
				smask[i++] = 'c';
			}
			if ((mask & Lua.LUA_MASKRET) != 0) 
			{
				smask[i++] = 'r';
			}
			if ((mask & Lua.LUA_MASKLINE) != 0) 
			{
				smask[i++] = 'l';
			}
			smask[i] = '\0';
			return smask;
		}

		private static void gethooktable (lua_State L) 
		{
			LuaAPI.lua_pushlightuserdata(L, KEY_HOOK);
			LuaAPI.lua_rawget(L, Lua.LUA_REGISTRYINDEX);
			if (!Lua.lua_istable(L, -1))
			{
				Lua.lua_pop(L, 1);
				LuaAPI.lua_createtable(L, 0, 1);
				LuaAPI.lua_pushlightuserdata(L, KEY_HOOK);
				LuaAPI.lua_pushvalue(L, -2);
				LuaAPI.lua_rawset(L, Lua.LUA_REGISTRYINDEX);
			}
		}

		private static int db_sethook(lua_State L) 
		{
			int arg, mask, count;
			lua_Hook func;
			lua_State L1 = getthread(L, out arg);
			if (Lua.lua_isnoneornil(L, arg + 1)) 
			{
				LuaAPI.lua_settop(L, arg + 1);
				func = null; 
				mask = 0; 
				count = 0;  /* turn off hooks */
			}
			else 
			{
				CharPtr smask = LuaAuxLib.luaL_checkstring(L, arg + 2);
				LuaAuxLib.luaL_checktype(L, arg + 1, Lua.LUA_TFUNCTION);
				count = LuaAuxLib.luaL_optint(L, arg + 3, 0);
				func = hookf; mask = makemask(smask, count);
			}
			gethooktable(L);
			LuaAPI.lua_pushlightuserdata(L, L1);
			LuaAPI.lua_pushvalue(L, arg + 1);
			LuaAPI.lua_rawset(L, -3);  /* set new hook */
			Lua.lua_pop(L, 1);  /* remove hook table */
			LuaDebug.lua_sethook(L1, func, mask, count);  /* set hooks */
			return 0;
		}

		private static int db_gethook(lua_State L) 
		{
			int arg;
			lua_State L1 = getthread(L, out arg);
			CharPtr buff = new char[5];
			int mask = LuaDebug.lua_gethookmask(L1);
			lua_Hook hook = LuaDebug.lua_gethook(L1);
			if (hook != null && hook != hookf)  /* external hook? */
			{
				Lua.lua_pushliteral(L, "external hook");
			}
			else 
			{
				gethooktable(L);
				LuaAPI.lua_pushlightuserdata(L, L1);
				LuaAPI.lua_rawget(L, -2);   /* get hook */
				LuaAPI.lua_remove(L, -2);  /* remove hook table */
			}
			LuaAPI.lua_pushstring(L, unmakemask(mask, buff));
			LuaAPI.lua_pushinteger(L, LuaDebug.lua_gethookcount(L1));
			return 3;
		}

		private static int db_debug(lua_State L) 
		{
			for (;;) 
			{
				CharPtr buffer = new char[250];
				LuaConf.fputs("lua_debug> ", LuaConf.stderr);
				if (LuaConf.fgets(buffer, LuaConf.stdin) == null ||
				    LuaConf.strcmp(buffer, "cont\n") == 0)
				{
					return 0;
				}
				if (LuaAuxLib.luaL_loadbuffer(L, buffer, /*(uint)*/LuaConf.strlen(buffer), "=(debug command)") != 0 ||
				    LuaAPI.lua_pcall(L, 0, 0, 0) != 0)
				{
					LuaConf.fputs(Lua.lua_tostring(L, -1), LuaConf.stderr);
					LuaConf.fputs("\n", LuaConf.stderr);
				}
				LuaAPI.lua_settop(L, 0);  /* remove eventual returns */
			}
		}

		public const int LEVELS1 = 12;	/* size of the first part of the stack */
		public const int LEVELS2 = 10;	/* size of the second part of the stack */

		private static int db_errorfb(lua_State L) 
		{
			int level;
			bool firstpart = true;  /* still before eventual `...' */
			int arg;
			lua_State L1 = getthread(L, out arg);
			lua_Debug ar = new lua_Debug();
			if (LuaAPI.lua_isnumber(L, arg + 2) != 0)
			{
				level = (int)LuaAPI.lua_tointeger(L, arg + 2);
				Lua.lua_pop(L, 1);
			}
			else
			{
				level = (L == L1) ? 1 : 0;  /* level 0 may be this own function */
			}
			if (LuaAPI.lua_gettop(L) == arg)
			{
				Lua.lua_pushliteral(L, "");
			}
			else if (LuaAPI.lua_isstring(L, arg + 1) == 0) 
			{
				return 1;  /* message is not a string */
			}
			else 
			{
				Lua.lua_pushliteral(L, "\n");
			}
			Lua.lua_pushliteral(L, "stack traceback:");
			while (LuaDebug.lua_getstack(L1, level++, ar) != 0)
			{
				if (level > LEVELS1 && firstpart) 
				{
					/* no more than `LEVELS2' more levels? */
					if (LuaDebug.lua_getstack(L1, level + LEVELS2, ar) == 0)
					{
						level--;  /* keep going */
					}
					else 
					{
						Lua.lua_pushliteral(L, "\n\t...");  /* too many levels */
						while (LuaDebug.lua_getstack(L1, level + LEVELS2, ar) != 0)  /* find last levels */
						{
							level++;
						}
					}
					firstpart = false;
					continue;
				}
				Lua.lua_pushliteral(L, "\n\t");
				LuaDebug.lua_getinfo(L1, "Snl", ar);
				LuaAPI.lua_pushfstring(L, "%s:", ar.short_src);
				if (ar.currentline > 0)
				{
					LuaAPI.lua_pushfstring(L, "%d:", ar.currentline);
				}
				if (ar.namewhat != '\0')  /* is there a name? */
				{
					LuaAPI.lua_pushfstring(L, " in function " + LuaConf.LUA_QS, ar.name);
				}
				else 
				{
					if (ar.what == 'm')  /* main? */
					{
						LuaAPI.lua_pushfstring(L, " in main chunk");
					}
					else if (ar.what == 'C' || ar.what == 't')
					{
						Lua.lua_pushliteral(L, " ?");  /* C function or tail call */
					}
					else
					{
						LuaAPI.lua_pushfstring(L, " in function <%s:%d>",
							ar.short_src, ar.linedefined);
					}
				}
				LuaAPI.lua_concat(L, LuaAPI.lua_gettop(L) - arg);
			}
			LuaAPI.lua_concat(L, LuaAPI.lua_gettop(L) - arg);
			return 1;
		}

		private readonly static luaL_Reg[] dblib = {
			new luaL_Reg("debug", db_debug),
			new luaL_Reg("getfenv", db_getfenv),
			new luaL_Reg("gethook", db_gethook),
			new luaL_Reg("getinfo", db_getinfo),
			new luaL_Reg("getlocal", db_getlocal),
			new luaL_Reg("getregistry", db_getregistry),
			new luaL_Reg("getmetatable", db_getmetatable),
			new luaL_Reg("getupvalue", db_getupvalue),
			new luaL_Reg("setfenv", db_setfenv),
			new luaL_Reg("sethook", db_sethook),
			new luaL_Reg("setlocal", db_setlocal),
			new luaL_Reg("setmetatable", db_setmetatable),
			new luaL_Reg("setupvalue", db_setupvalue),
			new luaL_Reg("traceback", db_errorfb),
			new luaL_Reg(null, null)
		};


		public static int luaopen_debug (lua_State L) {
			LuaAuxLib.luaL_register(L, LuaLib.LUA_DBLIBNAME, dblib);
			return 1;
		}

	}
}
