package KopiLua;

//
// ** $Id: lua.h,v 1.218.1.5 2008/08/06 13:30:12 roberto Exp $
// ** Lua - An Extensible Extension Language
// ** Lua.org, PUC-Rio, Brazil (http://www.lua.org)
// ** See Copyright Notice at the end of this file
// 

//using lua_Number = Double;
//using lua_Integer = System.Int32;

public class Lua {
	public static final String LUA_VERSION = "Lua 5.1";
	public static final String LUA_RELEASE = "Lua 5.1.4";
	public static final int LUA_VERSION_NUM = 501;
	public static final String LUA_COPYRIGHT = "Copyright (C) 1994-2008 Lua.org, PUC-Rio";
	public static final String LUA_AUTHORS = "R. Ierusalimschy, L. H. de Figueiredo & W. Celes";

	// mark for precompiled code (`<esc>Lua') 
	public static final String LUA_SIGNATURE = "\u001bLua";

	// option for multiple returns in `lua_pcall' and `lua_call' 
	public static final int LUA_MULTRET = (-1);

//        
//		 ** pseudo-indices
//		 
	public static final int LUA_REGISTRYINDEX = (-10000);
	public static final int LUA_ENVIRONINDEX = (-10001);
	public static final int LUA_GLOBALSINDEX = (-10002);
	public static int lua_upvalueindex(int i) {
		return LUA_GLOBALSINDEX - i;
	}

	// thread status; 0 is OK 
	public static final int LUA_YIELD = 1;
	public static final int LUA_ERRRUN = 2;
	public static final int LUA_ERRSYNTAX = 3;
	public static final int LUA_ERRMEM = 4;
	public static final int LUA_ERRERR = 5;

//        
//		 ** basic types
//		 
	public static final int LUA_TNONE = -1;

	public static final int LUA_TNIL = 0;
	public static final int LUA_TBOOLEAN = 1;
	public static final int LUA_TLIGHTUSERDATA = 2;
	public static final int LUA_TNUMBER = 3;
	public static final int LUA_TSTRING = 4;
	public static final int LUA_TTABLE = 5;
	public static final int LUA_TFUNCTION = 6;
	public static final int LUA_TUSERDATA = 7;
	public static final int LUA_TTHREAD = 8;

	// minimum Lua stack available to a C function 
	public static final int LUA_MINSTACK = 20;

	// type of numbers in Lua 
	//typedef LUA_NUMBER lua_Number;

	// type for integer functions 
	//typedef LUA_INTEGER lua_Integer;

//        
//		 ** garbage-collection function and options
//		 
	public static final int LUA_GCSTOP = 0;
	public static final int LUA_GCRESTART = 1;
	public static final int LUA_GCCOLLECT = 2;
	public static final int LUA_GCCOUNT = 3;
	public static final int LUA_GCCOUNTB = 4;
	public static final int LUA_GCSTEP = 5;
	public static final int LUA_GCSETPAUSE = 6;
	public static final int LUA_GCSETSTEPMUL = 7;

//         
//		 ** ===============================================================
//		 ** some useful macros
//		 ** ===============================================================
//		 
	public static void lua_pop(lua_State L, int n) {
		LuaAPI.lua_settop(L, -(n) - 1);
	}

	public static void lua_newtable(lua_State L) {
		LuaAPI.lua_createtable(L, 0, 0);
	}

	public static void lua_register(lua_State L, CharPtr n, lua_CFunction f) {
		lua_pushcfunction(L, f);
		lua_setglobal(L, n);
	}

	public static void lua_pushcfunction(lua_State L, lua_CFunction f) {
		LuaAPI.lua_pushcclosure(L, f, 0);
	}

	public static int lua_strlen(lua_State L, int i) { //uint
		return LuaAPI.lua_objlen(L, i);
	}

	public static boolean lua_isfunction(lua_State L, int n) {
		return LuaAPI.lua_type(L, n) == LUA_TFUNCTION;
	}

	public static boolean lua_istable(lua_State L, int n) {
		return LuaAPI.lua_type(L, n) == LUA_TTABLE;
	}

	public static boolean lua_islightuserdata(lua_State L, int n) {
		return LuaAPI.lua_type(L, n) == LUA_TLIGHTUSERDATA;
	}

	public static boolean lua_isnil(lua_State L, int n) {
		return LuaAPI.lua_type(L, n) == LUA_TNIL;
	}

	public static boolean lua_isboolean(lua_State L, int n) {
		return LuaAPI.lua_type(L, n) == LUA_TBOOLEAN;
	}

	public static boolean lua_isthread(lua_State L, int n) {
		return LuaAPI.lua_type(L, n) == LUA_TTHREAD;
	}

	public static boolean lua_isnone(lua_State L, int n) {
		return LuaAPI.lua_type(L, n) == LUA_TNONE;
	}

	public static boolean lua_isnoneornil(lua_State L, double n) { //lua_Number
		return LuaAPI.lua_type(L, (int)n) <= 0;
	}

	public static void lua_pushliteral(lua_State L, CharPtr s) {
		//TODO: Implement use using lua_pushlstring instead of lua_pushstring
		//lua_pushlstring(L, "" s, (sizeof(s)/GetUnmanagedSize(typeof(char)))-1)
		LuaAPI.lua_pushstring(L, s);
	}

	public static void lua_setglobal(lua_State L, CharPtr s) {
		LuaAPI.lua_setfield(L, LUA_GLOBALSINDEX, s);
	}

	public static void lua_getglobal(lua_State L, CharPtr s) {
		LuaAPI.lua_getfield(L, LUA_GLOBALSINDEX, s);
	}

	public static CharPtr lua_tostring(lua_State L, int i) {
		int[] blah = new int[1]; //uint
		return LuaAPI.lua_tolstring(L, i, blah); //out
	}

	////#define lua_open()	luaL_newstate()
	public static lua_State lua_open() {
		return LuaAuxLib.luaL_newstate();
	}

	////#define lua_getregistry(L)	lua_pushvalue(L, LUA_REGISTRYINDEX)
	public static void lua_getregistry(lua_State L) {
		LuaAPI.lua_pushvalue(L, LUA_REGISTRYINDEX);
	}

	////#define lua_getgccount(L)	lua_gc(L, LUA_GCCOUNT, 0)
	public static int lua_getgccount(lua_State L) {
		return LuaAPI.lua_gc(L, LUA_GCCOUNT, 0);
	}

	///#define lua_Chunkreader		lua_Reader
	///#define lua_Chunkwriter		lua_Writer


//        
//		 ** {======================================================================
//		 ** Debug API
//		 ** =======================================================================
//		 


//        
//		 ** Event codes
//		 
	public static final int LUA_HOOKCALL = 0;
	public static final int LUA_HOOKRET = 1;
	public static final int LUA_HOOKLINE = 2;
	public static final int LUA_HOOKCOUNT = 3;
	public static final int LUA_HOOKTAILRET = 4;


//        
//		 ** Event masks
//		 
	public static final int LUA_MASKCALL = (1 << LUA_HOOKCALL);
	public static final int LUA_MASKRET = (1 << LUA_HOOKRET);
	public static final int LUA_MASKLINE = (1 << LUA_HOOKLINE);
	public static final int LUA_MASKCOUNT = (1 << LUA_HOOKCOUNT);

	// }====================================================================== 

//        *****************************************************************************
//		 * Copyright (C) 1994-2008 Lua.org, PUC-Rio.  All rights reserved.
//		 *
//		 * Permission is hereby granted, free of charge, to any person obtaining
//		 * a copy of this software and associated documentation files (the
//		 * "Software"), to deal in the Software without restriction, including
//		 * without limitation the rights to use, copy, modify, merge, publish,
//		 * distribute, sublicense, and/or sell copies of the Software, and to
//		 * permit persons to whom the Software is furnished to do so, subject to
//		 * the following conditions:
//		 *
//		 * The above copyright notice and this permission notice shall be
//		 * included in all copies or substantial portions of the Software.
//		 *
//		 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//		 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//		 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//		 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
//		 * CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
//		 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
//		 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//		 *****************************************************************************
}
//public delegate int lua_CFunction(lua_State L);
//public delegate CharPtr lua_Reader(lua_State L, object ud, out int/*uint*/ sz);

//public interface lua_CFunction
//{
//	int exec(lua_State L);
//}
//public interface lua_Reader
//{
//	CharPtr exec(lua_State L, object ud, /*out*/ int[]/*uint*/ sz);
//}

//    
//	 ** prototype for memory-allocation functions
//	 
//public delegate object lua_Alloc(object ud, object ptr, uint osize, uint nsize);

//public delegate object lua_Alloc(Type t);
//public interface lua_Alloc
//{
//	object exec(ClassType t);
//}


//    
//	 ** functions that read/write blocks when loading/dumping Lua chunks
//	 
//public delegate int lua_Writer(lua_State L, CharPtr p, int/*uint*/ sz, object ud);
//public interface lua_Writer
//{
//	int exec(lua_State L, CharPtr p, int/*uint*/ sz, object ud);
//}


// Functions to be called by the debuger in specific events 
//public delegate void lua_Hook(lua_State L, lua_Debug ar);
//public interface lua_Hook
//{
//	void exec(lua_State L, lua_Debug ar);
//}
