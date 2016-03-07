/*
 ** $Id: lauxlib.c,v 1.159.1.3 2008/01/21 13:20:51 roberto Exp $
 ** Auxiliary functions for building Lua libraries
 ** See Copyright Notice in lua.h
 */
/**
 ** #define lauxlib_c
 ** #define LUA_LIB
 */
namespace KopiLua
{
	public class luaL_Buffer
	{
		public int p; /* current position in buffer */
		public int lvl; /* number of strings in the stack (level) */
		public lua_State L;
		public CharPtr buffer = CharPtr.toCharPtr(new char[LuaConf.LUAL_BUFFERSIZE]);
	}
}
