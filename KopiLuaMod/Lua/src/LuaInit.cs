/*
 ** $Id: linit.c,v 1.14.1.1 2007/12/27 13:02:25 roberto Exp $
 ** Initialization of libraries for lua.c
 ** See Copyright Notice in lua.h
 */
namespace KopiLua
{
	public class LuaInit
	{
		private readonly static luaL_Reg[] lualibs = {
			new luaL_Reg(CharPtr.toCharPtr(""), new LuaInit_delegate("LuaBaseLib.luaopen_base")),
			new luaL_Reg(CharPtr.toCharPtr(LuaLib.LUA_LOADLIBNAME), new LuaInit_delegate("LuaLoadLib.luaopen_package")),
			new luaL_Reg(CharPtr.toCharPtr(LuaLib.LUA_TABLIBNAME), new LuaInit_delegate("LuaTableLib.luaopen_table")),
			new luaL_Reg(CharPtr.toCharPtr(LuaLib.LUA_IOLIBNAME), new LuaInit_delegate("LuaIOLib.luaopen_io")),
			new luaL_Reg(CharPtr.toCharPtr(LuaLib.LUA_OSLIBNAME), new LuaInit_delegate("LuaOSLib.luaopen_os")),
			new luaL_Reg(CharPtr.toCharPtr(LuaLib.LUA_STRLIBNAME), new LuaInit_delegate("LuaStrLib.luaopen_string")),
			new luaL_Reg(CharPtr.toCharPtr(LuaLib.LUA_MATHLIBNAME), new LuaInit_delegate("LuaMathLib.luaopen_math")),
			new luaL_Reg(CharPtr.toCharPtr(LuaLib.LUA_DBLIBNAME), new LuaInit_delegate("LuaDebugLib.luaopen_debug")),
			new luaL_Reg(null, null)
		};
			
		public class LuaInit_delegate : lua_CFunction
		{
			private string name;
			
			public LuaInit_delegate(string name)
			{
				this.name = name;
			}
			
			public int exec(lua_State L)
			{
				if ("LuaBaseLib.luaopen_base".Equals(name))
				{
					return LuaBaseLib.luaopen_base(L);
				}
				else if ("LuaLoadLib.luaopen_package".Equals(name))
				{
					return LuaLoadLib.luaopen_package(L);
				}
				else if ("LuaTableLib.luaopen_table".Equals(name))
				{
					return LuaTableLib.luaopen_table(L);
				}
				else if ("LuaIOLib.luaopen_io".Equals(name))
				{
					return LuaIOLib.luaopen_io(L);
				}
				else if ("LuaOSLib.luaopen_os".Equals(name))
				{
					return LuaOSLib.luaopen_os(L);
				}
				else if ("LuaStrLib.luaopen_string".Equals(name))
				{
					return LuaStrLib.luaopen_string(L);
				}
				else if ("LuaMathLib.luaopen_math".Equals(name))
				{
					return LuaMathLib.luaopen_math(L);
				}
				else if ("LuaDebugLib.luaopen_debug".Equals(name))
				{
					return LuaDebugLib.luaopen_debug(L);
				}
				else
				{
					return 0;
				}
			}
		}
		

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
