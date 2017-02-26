/*
 ** $Id: loadlib.c,v 1.52.1.3 2008/08/06 13:29:28 roberto Exp $
 ** Dynamic library loader for Lua
 ** See Copyright Notice in lua.h
 **
 ** This module contains an implementation of loadlib for Unix systems
 ** that have dlfcn, an implementation for Darwin (Mac OS X), an
 ** implementation for Windows, and a stub for other systems.
 */
using System;

namespace KopiLua
{
	public class LuaLoadLib
	{
		/* prefix for open functions in C libraries */
		public const string LUA_POF = "luaopen_";

		/* separator for open functions in C libraries */
		public const string LUA_OFSEP = "_";

		public const string LIBPREFIX = "LOADLIB: ";

		public const string POF = LUA_POF;
		public const string LIB_FAIL = "open";

		/* error codes for ll_loadfunc */
		public const int ERRLIB			= 1;
		public const int ERRFUNC		= 2;

		//public static void setprogdir(lua_State L) { }

		public static void setprogdir(lua_State L)
		{
			CharPtr buff = CharPtr.toCharPtr(StreamProxy.GetCurrentDirectory());
			LuaAuxLib.luaL_gsub(L, Lua.lua_tostring(L, -1), CharPtr.toCharPtr(LuaConf.LUA_EXECDIR), buff);
			LuaAPI.lua_remove(L, -2);  /* remove original string */
		}


		//#if LUA_DL_DLOPEN		
//		/*
//		 ** {========================================================================
//		 ** This is an implementation of loadlib based on the dlfcn interface.
//		 ** The dlfcn interface is available in Linux, SunOS, Solaris, IRIX, FreeBSD,
//		 ** NetBSD, AIX 4.2, HPUX 11, and  probably most other Unix flavors, at least
//		 ** as an emulation layer on top of native functions.
//		 ** =========================================================================
//		 */
//
//		//#include <dlfcn.h>
//
//		static void ll_unloadlib (void *lib)
//		{
//			dlclose(lib);
//		}
//
//		static void *ll_load (lua_State L, readonly CharPtr path)
//		{
//			void *lib = dlopen(path, RTLD_NOW);
//			if (lib == null)
//			{
//				lua_pushstring(L, dlerror());
//			}
//			return lib;
//		}
//
//		static lua_CFunction ll_sym (lua_State L, void *lib, readonly CharPtr sym)
//		{
//			lua_CFunction f = (lua_CFunction)dlsym(lib, sym);
//			if (f == null)
//			{
//				lua_pushstring(L, dlerror());
//			}
//			return f;
//		}
//
//		/* }====================================================== */
//
//
//
//		//#elif defined(LUA_DL_DLL)
//		/*
//		 ** {======================================================================
//		 ** This is an implementation of loadlib for Windows using native functions.
//		 ** =======================================================================
//		 */
//
//		//#include <windows.h>
//
//
//		//#undef setprogdir
//
//		static void setprogdir (lua_State L)
//		{
//			char buff[MAX_PATH + 1];
//			char *lb;
//			DWORD nsize = sizeof(buff)/GetUnmanagedSize(typeof(char));
//			DWORD n = GetModuleFileNameA(null, buff, nsize);
//			if (n == 0 || n == nsize || (lb = strrchr(buff, '\\')) == null)
//			{
//				luaL_error(L, "unable to get ModuleFileName");
//			}
//			else
//			{
//				*lb = '\0';
//				luaL_gsub(L, lua_tostring(L, -1), LUA_EXECDIR, buff);
//				lua_remove(L, -2);  /* remove original string */
//			}
//		}
//
//		static void pusherror (lua_State L)
//		{
//			int error = GetLastError();
//			char buffer[128];
//			if (FormatMessageA(FORMAT_MESSAGE_IGNORE_INSERTS | FORMAT_MESSAGE_FROM_SYSTEM,
//			                   null, error, 0, buffer, sizeof(buffer), null))
//			{
//				lua_pushstring(L, buffer);
//			}
//			else
//			{
//				lua_pushfstring(L, "system error %d\n", error);
//			}
//		}
//
//		static void ll_unloadlib(void *lib)
//		{
//			FreeLibrary((HINSTANCE)lib);
//		}
//
//		static void *ll_load (lua_State L, readonly CharPtr path)
//		{
//			HINSTANCE lib = LoadLibraryA(path);
//			if (lib == null)
//			{
//				pusherror(L);
//			}
//			return lib;
//		}
//
//		static lua_CFunction ll_sym (lua_State L, void *lib, readonly CharPtr sym)
//		{
//			lua_CFunction f = (lua_CFunction)GetProcAddress((HINSTANCE)lib, sym);
//			if (f == null)
//			{
//				pusherror(L);
//			}
//			return f;
//		}
//
//		/* }====================================================== */
//
//		#elif LUA_DL_DYLD
//		/*
//		 ** {======================================================================
//		 ** Native Mac OS X / Darwin Implementation
//		 ** =======================================================================
//		 */
//
//		//#include <mach-o/dyld.h>
//
//
//		/* Mac appends a `_' before C function names */
//		//#undef POF
//		//#define POF	"_" LUA_POF
//
//		static void pusherror (lua_State L)
//		{
//			CharPtr err_str;
//			CharPtr err_file;
//			NSLinkEditErrors err;
//			int err_num;
//			NSLinkEditError(err, err_num, err_file, err_str);
//			lua_pushstring(L, err_str);
//		}
//
//
//		static CharPtr errorfromcode (NSObjectFileImageReturnCode ret)
//		{
//			switch (ret)
//			{
//				case NSObjectFileImageInappropriateFile:
//					{
//						return "file is not a bundle";
//					}
//				case NSObjectFileImageArch:
//					{
//						return "library is for wrong CPU type";
//					}
//				case NSObjectFileImageFormat:
//					{
//						return "bad format";
//					}
//				case NSObjectFileImageAccess:
//					{
//						return "cannot access file";
//					}
//				case NSObjectFileImageFailure:
//				default:
//					{
//						return "unable to load library";
//					}
//			}
//		}
//
//		static void ll_unloadlib (void *lib)
//		{
//			NSUnLinkModule((NSModule)lib, NSUNLINKMODULE_OPTION_RESET_LAZY_REFERENCES);
//		}
//
//		static void *ll_load (lua_State L, readonly CharPtr path)
//		{
//			NSObjectFileImage img;
//			NSObjectFileImageReturnCode ret;
//			/* this would be a rare case, but prevents crashing if it happens */
//			if(!_dyld_present()) {
//				lua_pushliteral(L, "dyld not present");
//				return null;
//			}
//			ret = NSCreateObjectFileImageFromFile(path, img);
//			if (ret == NSObjectFileImageSuccess) {
//				NSModule mod = NSLinkModule(img, path, NSLINKMODULE_OPTION_PRIVATE |
//				                            NSLINKMODULE_OPTION_RETURN_ON_ERROR);
//				NSDestroyObjectFileImage(img);
//				if (mod == null) pusherror(L);
//				return mod;
//			}
//			lua_pushstring(L, errorfromcode(ret));
//			return null;
//		}
//
//		static lua_CFunction ll_sym (lua_State L, void *lib, readonly CharPtr sym)
//		{
//			NSSymbol nss = NSLookupSymbolInModule((NSModule)lib, sym);
//			if (nss == null)
//			{
//				lua_pushfstring(L, "symbol " + LUA_QS + " not found", sym);
//				return null;
//			}
//			return (lua_CFunction)NSAddressOfSymbol(nss);
//		}
//
//		/* }====================================================== */

		//#else
		/*
		 ** {======================================================
		 ** Fallback for other systems
		 ** =======================================================
		 */

		//#undef LIB_FAIL
		//#define LIB_FAIL	"absent"

		public const string DLMSG = "dynamic libraries not enabled; check your Lua installation";

		public static void ll_unloadlib (object lib)
		{
			//(void)lib;  /* to avoid warnings */
		}

		public static object ll_load(lua_State L, CharPtr path)
		{
			//(void)path;  /* to avoid warnings */
			Lua.lua_pushliteral(L, CharPtr.toCharPtr(DLMSG));
			return null;
		}

		public static lua_CFunction ll_sym(lua_State L, object lib, CharPtr sym)
		{
			//(void)lib; (void)sym;  /* to avoid warnings */
			Lua.lua_pushliteral(L, CharPtr.toCharPtr(DLMSG));
			return null;
		}

		/* }====================================================== */
		//#endif

		private static object ll_register (lua_State L, CharPtr path)
		{
			// todo: the whole usage of plib here is wrong, fix it - mjf
			//void **plib;
			object plib = null;
			LuaAPI.lua_pushfstring(L, CharPtr.toCharPtr("%s%s"), LIBPREFIX, path);
			LuaAPI.lua_gettable(L, Lua.LUA_REGISTRYINDEX);  /* check library in registry? */
			if (!Lua.lua_isnil(L, -1))  /* is there an entry? */
			{
				plib = LuaAPI.lua_touserdata(L, -1);
			}
			else
			{
				/* no entry yet; create one */
				Lua.lua_pop(L, 1);
				//plib = lua_newuserdata(L, (uint)Marshal.SizeOf(plib));
				//plib[0] = null;
				LuaAuxLib.luaL_getmetatable(L, CharPtr.toCharPtr("_LOADLIB"));
				LuaAPI.lua_setmetatable(L, -2);
				LuaAPI.lua_pushfstring(L, CharPtr.toCharPtr("%s%s"), LIBPREFIX, path);
				LuaAPI.lua_pushvalue(L, -2);
				LuaAPI.lua_settable(L, Lua.LUA_REGISTRYINDEX);
			}
			return plib;
		}

		/*
		 ** __gc tag method: calls library's `ll_unloadlib' function with the lib
		 ** handle
		 */
		private static int gctm(lua_State L)
		{
			object lib = LuaAuxLib.luaL_checkudata(L, 1, CharPtr.toCharPtr("_LOADLIB"));
			if (lib != null)
			{
				ll_unloadlib(lib);
			}
			lib = null;  /* mark library as closed */
			return 0;
		}

		private static int ll_loadfunc(lua_State L, CharPtr path, CharPtr sym)
		{
			object reg = ll_register(L, path);
			if (reg == null)
			{
				reg = ll_load(L, path);
			}
			if (reg == null)
			{
				return ERRLIB;  /* unable to load library */
			}
			else
			{
				lua_CFunction f = ll_sym(L, reg, sym);
				if (f == null)
				{
					return ERRFUNC;  /* unable to find function */
				}
				Lua.lua_pushcfunction(L, f);
				return 0;  /* return function */
			}
		}

		private static int ll_loadlib(lua_State L)
		{
			CharPtr path = LuaAuxLib.luaL_checkstring(L, 1);
			CharPtr init = LuaAuxLib.luaL_checkstring(L, 2);
			int stat = ll_loadfunc(L, path, init);
			if (stat == 0)  /* no errors? */
			{
				return 1;  /* return the loaded function */
			}
			else
			{  /* error; error message is on stack top */
				LuaAPI.lua_pushnil(L);
				LuaAPI.lua_insert(L, -2);
				LuaAPI.lua_pushstring(L, (stat == ERRLIB) ? CharPtr.toCharPtr(LIB_FAIL) : CharPtr.toCharPtr("init"));
				return 3;  /* return nil, error message, and where */
			}
		}


		/*
		 ** {======================================================
		 ** 'require' function
		 ** =======================================================
		 */
		private static int readable(CharPtr filename)
		{
			StreamProxy f = LuaConf.fopen(filename, CharPtr.toCharPtr("r"));  /* try to open file */
			if (f == null) return 0;  /* open failed */
			LuaConf.fclose(f);
			return 1;
		}

		private static CharPtr pushnexttemplate(lua_State L, CharPtr path)
		{
			CharPtr l;
			while (path.get(0) == LuaConf.LUA_PATHSEP[0])
			{
				path = path.next();  /* skip separators */
			}
			if (path.get(0) == '\0')
			{
				return null;  /* no more templates */
			}
			l = LuaConf.strchr(path, LuaConf.LUA_PATHSEP[0]);  /* find next separator */
			if (CharPtr.isEqual(l, null))
			{
				l = CharPtr.plus(path, LuaConf.strlen(path));
			}
			LuaAPI.lua_pushlstring(L, path, /*(uint)*/CharPtr.minus(l, path));  /* template */
			return l;
		}

		private static CharPtr findfile(lua_State L, CharPtr name, CharPtr pname) 
		{
			CharPtr path;
			name = LuaAuxLib.luaL_gsub(L, name, CharPtr.toCharPtr("."), CharPtr.toCharPtr(LuaConf.LUA_DIRSEP));
			LuaAPI.lua_getfield(L, Lua.LUA_ENVIRONINDEX, pname);
			path = Lua.lua_tostring(L, -1);
			if (CharPtr.isEqual(path, null))
			{
				LuaAuxLib.luaL_error(L, CharPtr.toCharPtr(LuaConf.LUA_QL("package.%s") + " must be a string"), pname);
			}
			Lua.lua_pushliteral(L, CharPtr.toCharPtr(""));  /* error accumulator */
			while (CharPtr.isNotEqual((path = pushnexttemplate(L, path)), null))
			{
				CharPtr filename;
				filename = LuaAuxLib.luaL_gsub(L, Lua.lua_tostring(L, -1), CharPtr.toCharPtr(LuaConf.LUA_PATH_MARK), name);
				LuaAPI.lua_remove(L, -2);  /* remove path template */
				if (readable(filename) != 0)  /* does file exist and is readable? */
				{
					return filename;  /* return that file name */
				}
				LuaAPI.lua_pushfstring(L, CharPtr.toCharPtr("\n\tno file " + LuaConf.getLUA_QS()), filename);
				LuaAPI.lua_remove(L, -2);  /* remove file name */
				LuaAPI.lua_concat(L, 2);  /* add entry to possible error message */
			}
			return null;  /* not found */
		}

		private static void loaderror(lua_State L, CharPtr filename)
		{
			LuaAuxLib.luaL_error(L, CharPtr.toCharPtr("error loading module " + LuaConf.getLUA_QS() + " from file " + LuaConf.getLUA_QS() + ":\n\t%s"),
				Lua.lua_tostring(L, 1), filename, Lua.lua_tostring(L, -1));
		}

		private static int loader_Lua(lua_State L)
		{
			CharPtr filename;
			CharPtr name = LuaAuxLib.luaL_checkstring(L, 1);
			filename = findfile(L, name, CharPtr.toCharPtr("path"));
			if (CharPtr.isEqual(filename, null))
			{
				return 1;  /* library not found in this path */
			}
			if (LuaAuxLib.luaL_loadfile(L, filename) != 0)
			{
				loaderror(L, filename);
			}
			return 1;  /* library loaded successfully */
		}

		private static CharPtr mkfuncname(lua_State L, CharPtr modname)
		{
			CharPtr funcname;
			CharPtr mark = LuaConf.strchr(modname, LuaConf.LUA_IGMARK[0]);
			if (CharPtr.isNotEqual(mark, null))
			{
				modname = CharPtr.plus(mark, 1);
			}
			funcname = LuaAuxLib.luaL_gsub(L, modname, CharPtr.toCharPtr("."), CharPtr.toCharPtr(LUA_OFSEP));
			funcname = LuaAPI.lua_pushfstring(L, CharPtr.toCharPtr(POF + "%s"), funcname);
			LuaAPI.lua_remove(L, -2);  /* remove 'gsub' result */
			return funcname;
		}

		private static int loader_C(lua_State L)
		{
			CharPtr funcname;
			CharPtr name = LuaAuxLib.luaL_checkstring(L, 1);
			CharPtr filename = findfile(L, name, CharPtr.toCharPtr("cpath"));
			if (CharPtr.isEqual(filename, null))
			{
				return 1;  /* library not found in this path */
			}
			funcname = mkfuncname(L, name);
			if (ll_loadfunc(L, filename, funcname) != 0)
			{
				loaderror(L, filename);
			}
			return 1;  /* library loaded successfully */
		}

		private static int loader_Croot(lua_State L)
		{
			CharPtr funcname;
			CharPtr filename;
			CharPtr name = LuaAuxLib.luaL_checkstring(L, 1);
			CharPtr p = LuaConf.strchr(name, '.');
			int stat;
			if (CharPtr.isEqual(p, null))
			{
				return 0;  /* is root */
			}
			LuaAPI.lua_pushlstring(L, name, /*(uint)*/CharPtr.minus(p, name));
			filename = findfile(L, Lua.lua_tostring(L, -1), CharPtr.toCharPtr("cpath"));
			if (CharPtr.isEqual(filename, null))
			{
				return 1;  /* root not found */
			}
			funcname = mkfuncname(L, name);
			if ((stat = ll_loadfunc(L, filename, funcname)) != 0)
			{
				if (stat != ERRFUNC)
				{
					loaderror(L, filename);  /* real error */
				}
				LuaAPI.lua_pushfstring(L, CharPtr.toCharPtr("\n\tno module " + LuaConf.getLUA_QS() + " in file " + LuaConf.getLUA_QS()),
				                       name, filename);
				return 1;  /* function not found */
			}
			return 1;
		}

		private static int loader_preload(lua_State L)
		{
			CharPtr name = LuaAuxLib.luaL_checkstring(L, 1);
			LuaAPI.lua_getfield(L, Lua.LUA_ENVIRONINDEX, CharPtr.toCharPtr("preload"));
			if (!Lua.lua_istable(L, -1))
			{
				LuaAuxLib.luaL_error(L, CharPtr.toCharPtr(LuaConf.LUA_QL("package.preload") + " must be a table"));
			}
			LuaAPI.lua_getfield(L, -1, name);
			if (Lua.lua_isnil(L, -1))  /* not found? */
			{
				LuaAPI.lua_pushfstring(L, CharPtr.toCharPtr("\n\tno field package.preload['%s']"), name);
			}
			return 1;
		}

		public static object sentinel = new object();

		public static int ll_require (lua_State L)
		{
			CharPtr name = LuaAuxLib.luaL_checkstring(L, 1);
			int i;
			LuaAPI.lua_settop(L, 1);  /* _LOADED table will be at index 2 */
			LuaAPI.lua_getfield(L, Lua.LUA_REGISTRYINDEX, CharPtr.toCharPtr("_LOADED"));
			LuaAPI.lua_getfield(L, 2, name);
			if (LuaAPI.lua_toboolean(L, -1) != 0)
			{
				/* is it there? */
				if (LuaAPI.lua_touserdata(L, -1) == sentinel)  /* check loops */
				{
					LuaAuxLib.luaL_error(L, CharPtr.toCharPtr("loop or previous error loading module " + LuaConf.getLUA_QS()), name);
				}
				return 1;  /* package is already loaded */
			}
			/* else must load it; iterate over available loaders */
			LuaAPI.lua_getfield(L, Lua.LUA_ENVIRONINDEX, CharPtr.toCharPtr("loaders"));
			if (!Lua.lua_istable(L, -1))
			{
				LuaAuxLib.luaL_error(L, CharPtr.toCharPtr(LuaConf.LUA_QL("package.loaders") + " must be a table"));
			}
			Lua.lua_pushliteral(L, CharPtr.toCharPtr(""));  /* error message accumulator */
			for (i = 1; ; i++)
			{
				LuaAPI.lua_rawgeti(L, -2, i);  /* get a loader */
				if (Lua.lua_isnil(L, -1))
				{
					LuaAuxLib.luaL_error(L, CharPtr.toCharPtr("module " + LuaConf.getLUA_QS() + " not found:%s"),
					                     name, Lua.lua_tostring(L, -2));
				}
				LuaAPI.lua_pushstring(L, name);
				LuaAPI.lua_call(L, 1, 1);  /* call it */
				if (Lua.lua_isfunction(L, -1))  /* did it find module? */
				{
					break;  /* module loaded successfully */
				}
				else if (LuaAPI.lua_isstring(L, -1) != 0)  /* loader returned error message? */
				{
					LuaAPI.lua_concat(L, 2);  /* accumulate it */
				}
				else
				{
					Lua.lua_pop(L, 1);
				}
			}
			LuaAPI.lua_pushlightuserdata(L, sentinel);
			LuaAPI.lua_setfield(L, 2, name);  /* _LOADED[name] = sentinel */
			LuaAPI.lua_pushstring(L, name);  /* pass name as argument to module */
			LuaAPI.lua_call(L, 1, 1);  /* run loaded module */
			if (!Lua.lua_isnil(L, -1))  /* non-nil return? */
			{
				LuaAPI.lua_setfield(L, 2, name);  /* _LOADED[name] = returned value */
			}
			LuaAPI.lua_getfield(L, 2, name);
			if (LuaAPI.lua_touserdata(L, -1) == sentinel)
			{
				/* module did not set a value? */
				LuaAPI.lua_pushboolean(L, 1);  /* use true as result */
				LuaAPI.lua_pushvalue(L, -1);  /* extra copy to be returned */
				LuaAPI.lua_setfield(L, 2, name);  /* _LOADED[name] = true */
			}
			return 1;
		}

		/* }====================================================== */

		/*
		 ** {======================================================
		 ** 'module' function
		 ** =======================================================
		 */
		

		private static void setfenv(lua_State L)
		{
			lua_Debug ar = new lua_Debug();
			if (LuaDebug.lua_getstack(L, 1, ar) == 0 ||
			    LuaDebug.lua_getinfo(L, CharPtr.toCharPtr("f"), ar) == 0 ||  /* get calling function */
			    LuaAPI.lua_iscfunction(L, -1))
			{
				LuaAuxLib.luaL_error(L, CharPtr.toCharPtr(LuaConf.LUA_QL("module") + " not called from a Lua function"));
			}
			LuaAPI.lua_pushvalue(L, -2);
			LuaAPI.lua_setfenv(L, -2);
			Lua.lua_pop(L, 1);
		}

		private static void dooptions(lua_State L, int n)
		{
			int i;
			for (i = 2; i <= n; i++)
			{
				LuaAPI.lua_pushvalue(L, i);  /* get option (a function) */
				LuaAPI.lua_pushvalue(L, -2);  /* module */
				LuaAPI.lua_call(L, 1, 0);
			}
		}

		private static void modinit(lua_State L, CharPtr modname)
		{
			CharPtr dot;
			LuaAPI.lua_pushvalue(L, -1);
			LuaAPI.lua_setfield(L, -2, CharPtr.toCharPtr("_M"));  /* module._M = module */
			LuaAPI.lua_pushstring(L, modname);
			LuaAPI.lua_setfield(L, -2, CharPtr.toCharPtr("_NAME"));
			dot = LuaConf.strrchr(modname, '.');  /* look for last dot in module name */
			if (CharPtr.isEqual(dot, null))
			{
				dot = modname;
			}
			else
			{
				dot = dot.next();
			}
			/* set _PACKAGE as package name (full module name minus last part) */
			LuaAPI.lua_pushlstring(L, modname, /*(uint)*/CharPtr.minus(dot, modname));
			LuaAPI.lua_setfield(L, -2, CharPtr.toCharPtr("_PACKAGE"));
		}

		private static int ll_module(lua_State L)
		{
			CharPtr modname = LuaAuxLib.luaL_checkstring(L, 1);
			int loaded = LuaAPI.lua_gettop(L) + 1;  /* index of _LOADED table */
			LuaAPI.lua_getfield(L, Lua.LUA_REGISTRYINDEX, CharPtr.toCharPtr("_LOADED"));
			LuaAPI.lua_getfield(L, loaded, modname);  /* get _LOADED[modname] */
			if (!Lua.lua_istable(L, -1))
			{
				/* not found? */
				Lua.lua_pop(L, 1);  /* remove previous result */
				/* try global variable (and create one if it does not exist) */
				if (CharPtr.isNotEqual(LuaAuxLib.luaL_findtable(L, Lua.LUA_GLOBALSINDEX, modname, 1), null))
				{
					return LuaAuxLib.luaL_error(L, CharPtr.toCharPtr("name conflict for module " + LuaConf.getLUA_QS()), modname);
				}
				LuaAPI.lua_pushvalue(L, -1);
				LuaAPI.lua_setfield(L, loaded, modname);  /* _LOADED[modname] = new table */
			}
			/* check whether table already has a _NAME field */
			LuaAPI.lua_getfield(L, -1, CharPtr.toCharPtr("_NAME"));
			if (!Lua.lua_isnil(L, -1))  /* is table an initialized module? */
			{
				Lua.lua_pop(L, 1);
			}
			else
			{
				/* no; initialize it */
				Lua.lua_pop(L, 1);
				modinit(L, modname);
			}
			LuaAPI.lua_pushvalue(L, -1);
			setfenv(L);
			dooptions(L, loaded - 1);
			return 0;
		}

		private static int ll_seeall(lua_State L)
		{
			LuaAuxLib.luaL_checktype(L, 1, Lua.LUA_TTABLE);
			if (LuaAPI.lua_getmetatable(L, 1) == 0)
			{
				LuaAPI.lua_createtable(L, 0, 1); /* create new metatable */
				LuaAPI.lua_pushvalue(L, -1);
				LuaAPI.lua_setmetatable(L, 1);
			}
			LuaAPI.lua_pushvalue(L, Lua.LUA_GLOBALSINDEX);
			LuaAPI.lua_setfield(L, -2, CharPtr.toCharPtr("__index"));  /* mt.__index = _G */
			return 0;
		}

		/* }====================================================== */

		/* auxiliary mark (for internal use) */
		public readonly static string AUXMARK = String.Format("{0}", (char)1);

		private static void setpath(lua_State L, CharPtr fieldname, CharPtr envname, CharPtr def)
		{
			CharPtr path = LuaConf.getenv(envname);
			if (CharPtr.isEqual(path, null))  /* no environment variable? */
			{
				LuaAPI.lua_pushstring(L, def);  /* use default */
			}
			else
			{
				/* replace ";;" by ";AUXMARK;" and then AUXMARK by default path */
				path = LuaAuxLib.luaL_gsub(L, path, CharPtr.toCharPtr(LuaConf.LUA_PATHSEP + LuaConf.LUA_PATHSEP),
					CharPtr.toCharPtr(LuaConf.LUA_PATHSEP + AUXMARK + LuaConf.LUA_PATHSEP));
				LuaAuxLib.luaL_gsub(L, path, CharPtr.toCharPtr(AUXMARK), def);
				LuaAPI.lua_remove(L, -2);
			}
			setprogdir(L);
			LuaAPI.lua_setfield(L, -2, fieldname);
		}

		private readonly static luaL_Reg[] pk_funcs = {
			new luaL_Reg(CharPtr.toCharPtr("loadlib"), new LuaLoadLib_delegate("ll_loadlib")),
			new luaL_Reg(CharPtr.toCharPtr("seeall"), new LuaLoadLib_delegate("ll_seeall")),
			new luaL_Reg(null, null)
		};

		private readonly static luaL_Reg[] ll_funcs = {
			new luaL_Reg(CharPtr.toCharPtr("module"), new LuaLoadLib_delegate("ll_module")),
			new luaL_Reg(CharPtr.toCharPtr("require"), new LuaLoadLib_delegate("ll_require")),
			new luaL_Reg(null, null)
		};

		public readonly static lua_CFunction[] loaders = {
			new LuaLoadLib_delegate("loader_preload"),
			new LuaLoadLib_delegate("loader_Lua"),
			new LuaLoadLib_delegate("loader_C"),
			new LuaLoadLib_delegate("loader_Croot"),
			null
		};
		
		public class LuaLoadLib_delegate : lua_CFunction
		{
			private string name;
			
			public LuaLoadLib_delegate(string name)
			{
				this.name = name;
			}
			
			public int exec(lua_State L)
			{
				if ("ll_loadlib".Equals(name))
				{
					return ll_loadlib(L);
				} 
				else if ("ll_seeall".Equals(name)) 
				{
					return ll_seeall(L);
				} 
				else if ("ll_module".Equals(name)) 
				{
					return ll_module(L);
				} 
				else if ("ll_require".Equals(name)) 
				{
				    return ll_require(L);
				}
				else if ("loader_preload".Equals(name))
				{
				    return loader_preload(L);
				}
				else if ("loader_Lua".Equals(name))
				{
					return loader_Lua(L);
				}
				else if ("loader_C".Equals(name))
				{
					return loader_C(L);
				}
				else if ("loader_Croot".Equals(name))
				{
					return loader_Croot(L);
				}
				else if ("gctm".Equals(name))
				{
					return gctm(L);
				}
				else
				{
					return 0;
				}
			}
		}
		
		public static int luaopen_package(lua_State L)
		{
			int i;
			/* create new type _LOADLIB */
			LuaAuxLib.luaL_newmetatable(L, CharPtr.toCharPtr("_LOADLIB"));
			Lua.lua_pushcfunction(L, new LuaLoadLib_delegate("gctm"));
			LuaAPI.lua_setfield(L, -2, CharPtr.toCharPtr("__gc"));
			/* create `package' table */
			LuaAuxLib.luaL_register(L, CharPtr.toCharPtr(LuaLib.LUA_LOADLIBNAME), pk_funcs);
			//#if LUA_COMPAT_LOADLIB
//			lua_getfield(L, -1, "loadlib");
//			lua_setfield(L, LUA_GLOBALSINDEX, "loadlib");
			//#endif
			LuaAPI.lua_pushvalue(L, -1);
			LuaAPI.lua_replace(L, Lua.LUA_ENVIRONINDEX);
			/* create `loaders' table */
			LuaAPI.lua_createtable(L, 0, loaders.Length - 1);
			/* fill it with pre-defined loaders */
			for (i = 0; loaders[i] != null; i++)
			{
				Lua.lua_pushcfunction(L, loaders[i]);
				LuaAPI.lua_rawseti(L, -2, i + 1);
			}
			LuaAPI.lua_setfield(L, -2, CharPtr.toCharPtr("loaders"));  /* put it in field `loaders' */
			setpath(L, CharPtr.toCharPtr("path"), CharPtr.toCharPtr(LuaConf.LUA_PATH), CharPtr.toCharPtr(LuaConf.LUA_PATH_DEFAULT));  /* set field `path' */
			setpath(L, CharPtr.toCharPtr("cpath"), CharPtr.toCharPtr(LuaConf.LUA_CPATH), CharPtr.toCharPtr(LuaConf.LUA_CPATH_DEFAULT)); /* set field `cpath' */
			/* store config information */
			Lua.lua_pushliteral(L, CharPtr.toCharPtr(LuaConf.LUA_DIRSEP + "\n" + LuaConf.LUA_PATHSEP + "\n" + LuaConf.LUA_PATH_MARK + "\n" +
				LuaConf.LUA_EXECDIR + "\n" + LuaConf.LUA_IGMARK));
			LuaAPI.lua_setfield(L, -2, CharPtr.toCharPtr("config"));
			/* set field `loaded' */
			LuaAuxLib.luaL_findtable(L, Lua.LUA_REGISTRYINDEX, CharPtr.toCharPtr("_LOADED"), 2);
			LuaAPI.lua_setfield(L, -2, CharPtr.toCharPtr("loaded"));
			/* set field `preload' */
			Lua.lua_newtable(L);
			LuaAPI.lua_setfield(L, -2, CharPtr.toCharPtr("preload"));
			LuaAPI.lua_pushvalue(L, Lua.LUA_GLOBALSINDEX);
			LuaAuxLib.luaL_register(L, null, ll_funcs);  /* open lib into global table */
			Lua.lua_pop(L, 1);
			return 1;  /* return 'package' table */
		}
	}
}
