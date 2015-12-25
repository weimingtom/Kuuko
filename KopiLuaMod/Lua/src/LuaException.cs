/*
 ** $Id: luaconf.h,v 1.82.1.7 2008/02/11 16:25:08 roberto Exp $
 ** Configuration file for Lua
 ** See Copyright Notice in lua.h
 */
using System;

namespace KopiLua
{
	public class LuaException : Exception
	{
		public lua_State L;
		public lua_longjmp c;
	
		public LuaException(lua_State L, lua_longjmp c) 
		{ 
			this.L = L; 
			this.c = c;
		}
	}
}
