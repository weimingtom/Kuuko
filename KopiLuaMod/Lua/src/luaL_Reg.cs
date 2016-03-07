/*
 ** $Id: lauxlib.c,v 1.159.1.3 2008/01/21 13:20:51 roberto Exp $
 ** Auxiliary functions for building Lua libraries
 ** See Copyright Notice in lua.h
 */
/*
 ** #define lauxlib_c
 ** #define LUA_LIB
 */
namespace KopiLua
{
	public class luaL_Reg
	{
		public CharPtr name;
		public lua_CFunction func;
		
		public luaL_Reg(CharPtr name, lua_CFunction func)
		{
			this.name = name;
			this.func = func;
		}
	}
}
