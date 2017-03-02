/*
 ** $Id: loslib.c,v 1.19.1.3 2008/01/18 16:38:18 roberto Exp $
 ** Standard Operating System library
 ** See Copyright Notice in lua.h
 */
using System;

namespace KopiLua
{
	//using TValue = Lua.TValue;
	//using StkId = TValue;
	//using lua_Integer = System.Int32;
	//using lua_Number = System.Double;

	public class LuaOSLib
	{
		private static int os_pushresult(lua_State L, int i, CharPtr filename) 
		{
			int en = LuaConf.errno();  /* calls to Lua API may change this value */
			if (i != 0) 
			{
				LuaAPI.lua_pushboolean(L, 1);
				return 1;
			}
			else 
			{
				LuaAPI.lua_pushnil(L);
				LuaAPI.lua_pushfstring(L, CharPtr.toCharPtr("%s: %s"), filename, LuaConf.strerror(en));
				LuaAPI.lua_pushinteger(L, en);
				return 3;
			}
		}

		private static int os_execute(lua_State L) 
		{
			CharPtr strCmdLine = CharPtr.toCharPtr("" + LuaAuxLib.luaL_optstring(L, 1, null));
			LuaAPI.lua_pushinteger(L, ClassType.processExec(strCmdLine.ToString()));
			return 1;
		}

		private static int os_remove(lua_State L) 
		{
			CharPtr filename = LuaAuxLib.luaL_checkstring(L, 1);
			int result = 1;
			try 
			{
				StreamProxy.Delete(filename.ToString());
			} 
			catch 
			{
				result = 0;
			}
			return os_pushresult(L, result, filename);
		}

		private static int os_rename(lua_State L) 
		{
			CharPtr fromname = LuaAuxLib.luaL_checkstring(L, 1);
			CharPtr toname = LuaAuxLib.luaL_checkstring(L, 2);
			int result;
			try
			{
				StreamProxy.Move(fromname.ToString(), toname.ToString());
				result = 0;
			}
			catch
			{
				result = 1; // todo: this should be a proper error code
			}
			return os_pushresult(L, result, fromname);
		}

		private static int os_tmpname(lua_State L) 
		{
			LuaAPI.lua_pushstring(L, CharPtr.toCharPtr(StreamProxy.GetTempFileName()));
			return 1;
		}


		private static int os_getenv(lua_State L) 
		{
			LuaAPI.lua_pushstring(L, LuaConf.getenv(LuaAuxLib.luaL_checkstring(L, 1)));  /* if null push nil */
			return 1;
		}

		private static int os_clock(lua_State L) 
		{
            LuaAPI.lua_pushnumber(L, DateTimeProxy.getClock());
			return 1;
		}

		/*
		 ** {======================================================
		 ** Time/Date operations
		 ** { year=%Y, month=%m, day=%d, hour=%H, min=%M, sec=%S,
		 **   wday=%w+1, yday=%j, isdst=? }
		 ** =======================================================
		 */
		private static void setfield(lua_State L, CharPtr key, int value) 
		{
			LuaAPI.lua_pushinteger(L, value);
			LuaAPI.lua_setfield(L, -2, key);
		}

		private static void setboolfield(lua_State L, CharPtr key, int value) 
		{
			if (value < 0)  /* undefined? */
			{
				return;  /* does not set field */
			}
			LuaAPI.lua_pushboolean(L, value);
			LuaAPI.lua_setfield(L, -2, key);
		}

		private static int getboolfield(lua_State L, CharPtr key) 
		{
			int res;
			LuaAPI.lua_getfield(L, -1, key);
			res = Lua.lua_isnil(L, -1) ? -1 : LuaAPI.lua_toboolean(L, -1);
			Lua.lua_pop(L, 1);
			return res;
		}

		private static int getfield(lua_State L, CharPtr key, int d) 
		{
			int res;
			LuaAPI.lua_getfield(L, -1, key);
			if (LuaAPI.lua_isnumber(L, -1) != 0)
			{
				res = (int)LuaAPI.lua_tointeger(L, -1);
			}
			else 
			{
				if (d < 0)
				{
					return LuaAuxLib.luaL_error(L, CharPtr.toCharPtr("field " + LuaConf.getLUA_QS() + " missing in date table"), key);
				}
				res = d;
			}
			Lua.lua_pop(L, 1);
			return res;
		}

		private static int os_date(lua_State L) 
		{
			CharPtr s = LuaAuxLib.luaL_optstring(L, 1, CharPtr.toCharPtr("%c"));
			DateTimeProxy stm = new DateTimeProxy();
			if (s.get(0) == '!') 
			{  
				/* UTC? */
				stm.setUTCNow();
				s.inc();  /* skip `!' */
			}
			else
			{
				stm.setNow();
			}
			if (LuaConf.strcmp(s, CharPtr.toCharPtr("*t")) == 0)
			{
				LuaAPI.lua_createtable(L, 0, 9);  /* 9 = number of fields */
				setfield(L, CharPtr.toCharPtr("sec"), stm.getSecond());
				setfield(L, CharPtr.toCharPtr("min"), stm.getMinute());
				setfield(L, CharPtr.toCharPtr("hour"), stm.getHour());
				setfield(L, CharPtr.toCharPtr("day"), stm.getDay());
				setfield(L, CharPtr.toCharPtr("month"), stm.getMonth());
				setfield(L, CharPtr.toCharPtr("year"), stm.getYear());
				setfield(L, CharPtr.toCharPtr("wday"), (int)stm.getDayOfWeek());
				setfield(L, CharPtr.toCharPtr("yday"), stm.getDayOfYear());
				setboolfield(L, CharPtr.toCharPtr("isdst"), stm.IsDaylightSavingTime() ? 1 : 0);
			}
			else 
			{
				LuaAuxLib.luaL_error(L, CharPtr.toCharPtr("strftime not implemented yet")); // todo: implement this - mjf
				//#if false
//				CharPtr cc = new char[3];
//				luaL_Buffer b;
//				cc[0] = '%'; 
//				cc[2] = '\0';
//				luaL_buffinit(L, b);
//				for (; s[0] != 0; s.inc()) 
//				{
//					if (s[0] != '%' || s[1] == '\0')  /* no conversion specifier? */
//					{
//						luaL_addchar(b, s[0]);
//					}
//					else 
//					{
//						uint reslen;
//						CharPtr buff = new char[200];  /* should be big enough for any conversion result */
//						s.inc();
//						cc[1] = s[0];
//						reslen = strftime(buff, buff.Length, cc, stm);
//						luaL_addlstring(b, buff, reslen);
//					}
//				}
//				luaL_pushresult(b);
				//#endif // #if 0
			}
			return 1;
		}

		private static int os_time(lua_State L) 
		{
			DateTimeProxy t = new DateTimeProxy();
			if (Lua.lua_isnoneornil(L, 1))  /* called without args? */
			{
				t.setNow();  /* get current time */
			}
			else 
			{
				LuaAuxLib.luaL_checktype(L, 1, Lua.LUA_TTABLE);
				LuaAPI.lua_settop(L, 1);  /* make sure table is at the top */
				int sec = getfield(L, CharPtr.toCharPtr("sec"), 0);
				int min = getfield(L, CharPtr.toCharPtr("min"), 0);
				int hour = getfield(L, CharPtr.toCharPtr("hour"), 12);
				int day = getfield(L, CharPtr.toCharPtr("day"), -1);
				int month = getfield(L, CharPtr.toCharPtr("month"), -1) - 1;
				int year = getfield(L, CharPtr.toCharPtr("year"), -1) - 1900;
				int isdst = getboolfield(L, CharPtr.toCharPtr("isdst"));	// todo: implement this - mjf
				t = new DateTimeProxy(year, month, day, hour, min, sec);
			}
			LuaAPI.lua_pushnumber(L, t.getTicks());
			return 1;
		}

		private static int os_difftime(lua_State L) 
		{
			long ticks = (long)LuaAuxLib.luaL_checknumber(L, 1) - (long)LuaAuxLib.luaL_optnumber(L, 2, 0);
            LuaAPI.lua_pushnumber(L, ticks / 10000000); //FIXME: ticks / TimeSpan.TicksPerSecond
			return 1;
		}

		/* }====================================================== */

		// locale not supported yet
		private static int os_setlocale(lua_State L)
		{
			/*
		  static string[] cat = {LC_ALL, LC_COLLATE, LC_CTYPE, LC_MONETARY,
							  LC_NUMERIC, LC_TIME};
		  static string[] catnames[] = {"all", "collate", "ctype", "monetary",
			 "numeric", "time", null};
		  CharPtr l = luaL_optstring(L, 1, null);
		  int op = luaL_checkoption(L, 2, "all", catnames);
		  lua_pushstring(L, setlocale(cat[op], l));
			 */
			CharPtr l = LuaAuxLib.luaL_optstring(L, 1, null);
			LuaAPI.lua_pushstring(L, CharPtr.toCharPtr("C"));
			return (l.ToString().Equals("C")) ? 1 : 0;
		}

		private static int os_exit(lua_State L) 
		{
			Environment.Exit(LuaConf.EXIT_SUCCESS);
			return 0;
		}

		private readonly static luaL_Reg[] syslib = {
			new luaL_Reg(CharPtr.toCharPtr("clock"), new LuaOSLib_delegate("os_clock")),
			new luaL_Reg(CharPtr.toCharPtr("date"), new LuaOSLib_delegate("os_date")),
			new luaL_Reg(CharPtr.toCharPtr("difftime"), new LuaOSLib_delegate("os_difftime")),
			new luaL_Reg(CharPtr.toCharPtr("execute"), new LuaOSLib_delegate("os_execute")),
			new luaL_Reg(CharPtr.toCharPtr("exit"), new LuaOSLib_delegate("os_exit")),
			new luaL_Reg(CharPtr.toCharPtr("getenv"), new LuaOSLib_delegate("os_getenv")),
			new luaL_Reg(CharPtr.toCharPtr("remove"), new LuaOSLib_delegate("os_remove")),
			new luaL_Reg(CharPtr.toCharPtr("rename"), new LuaOSLib_delegate("os_rename")),
			new luaL_Reg(CharPtr.toCharPtr("setlocale"), new LuaOSLib_delegate("os_setlocale")),
			new luaL_Reg(CharPtr.toCharPtr("time"), new LuaOSLib_delegate("os_time")),
			new luaL_Reg(CharPtr.toCharPtr("tmpname"), new LuaOSLib_delegate("os_tmpname")),
			new luaL_Reg(null, null)
		};

		public class LuaOSLib_delegate : lua_CFunction
		{
			private string name;
			
			public LuaOSLib_delegate(string name)
			{
				this.name = name;
			}
			
			public int exec(lua_State L)
			{
				if ("os_clock".Equals(name))
				{
					return os_clock(L);
				} 
				else if ("os_date".Equals(name)) 
				{
					return os_date(L);
				} 
				else if ("os_difftime".Equals(name)) 
				{
					return os_difftime(L);
				} 
				else if ("os_execute".Equals(name)) 
				{
				    return os_execute(L);
				}
				else if ("os_exit".Equals(name))
				{
				    return os_exit(L);
				}
				else if ("os_getenv".Equals(name))
				{
					return os_getenv(L);
				}
				else if ("os_remove".Equals(name))
				{
					return os_remove(L);
				}
				else if ("os_rename".Equals(name))
				{
					return os_rename(L);
				}
				else if ("os_setlocale".Equals(name))
				{
					return os_setlocale(L);
				}
				else if ("os_time".Equals(name))
				{
					return os_time(L);
				}
				else if ("os_tmpname".Equals(name))
				{
					return os_tmpname(L);
				}
				else
				{
					return 0;
				}
			}
		}
		
		
		/* }====================================================== */

		public static int luaopen_os (lua_State L) 
		{
			LuaAuxLib.luaL_register(L, CharPtr.toCharPtr(LuaLib.LUA_OSLIBNAME), syslib);
			return 1;
		}
	}
}
