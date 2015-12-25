/*
 ** $Id: linit.c,v 1.14.1.1 2007/12/27 13:02:25 roberto Exp $
 ** Initialization of libraries for lua.c
 ** See Copyright Notice in lua.h
 */
using System;

namespace KopiLua
{
	public class LuaInit
	{
		private readonly static luaL_Reg[] lualibs = {
			new luaL_Reg("", LuaBaseLib.luaopen_base),
			new luaL_Reg(LuaLib.LUA_LOADLIBNAME, LuaLoadLib.luaopen_package),
			new luaL_Reg(LuaLib.LUA_TABLIBNAME, LuaTableLib.luaopen_table),
			new luaL_Reg(LuaLib.LUA_IOLIBNAME, LuaIOLib.luaopen_io),
			new luaL_Reg(LuaLib.LUA_OSLIBNAME, LuaOSLib.luaopen_os),
			new luaL_Reg(LuaLib.LUA_STRLIBNAME, LuaStrLib.luaopen_string),
			new luaL_Reg(LuaLib.LUA_MATHLIBNAME, LuaMathLib.luaopen_math),
			new luaL_Reg(LuaLib.LUA_DBLIBNAME, LuaDebugLib.luaopen_debug),
			new luaL_Reg(null, null)
		};

		public static void luaL_openlibs(lua_State L) 
		{
			for (int i = 0; i < lualibs.Length - 1; i++)
			{
				luaL_Reg lib = lualibs[i];
				Lua.lua_pushcfunction(L, lib.func);
				LuaAPI.lua_pushstring(L, lib.name);
				LuaAPI.lua_call(L, 1, 0);
			}
		}
	}
}
