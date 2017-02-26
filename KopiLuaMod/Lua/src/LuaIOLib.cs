/*
 ** $Id: liolib.c,v 2.73.1.3 2008/01/18 17:47:43 roberto Exp $
 ** Standard I/O (and system) library
 ** See Copyright Notice in lua.h
 */
namespace KopiLua
{
	//using lua_Number = System.Double;
	//using lua_Integer = System.Int32;

	public class LuaIOLib
	{
		public const int IO_INPUT = 1;
		public const int IO_OUTPUT = 2;

		private static readonly string[] fnames = { 
			"input", 
			"output" 
		};

		private static int pushresult(lua_State L, int i, CharPtr filename) 
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
				if (CharPtr.isNotEqual(filename, null))
				{
					LuaAPI.lua_pushfstring(L, CharPtr.toCharPtr("%s: %s"), filename, LuaConf.strerror(en));
				}
				else
				{
					LuaAPI.lua_pushfstring(L, CharPtr.toCharPtr("%s"), LuaConf.strerror(en));
				}
				LuaAPI.lua_pushinteger(L, en);
				return 3;
			}
		}

		private static void fileerror(lua_State L, int arg, CharPtr filename) 
		{
			LuaAPI.lua_pushfstring(L, CharPtr.toCharPtr("%s: %s"), filename, LuaConf.strerror(LuaConf.errno()));
			LuaAuxLib.luaL_argerror(L, arg, Lua.lua_tostring(L, -1));
		}

		public static FilePtr tofilep(lua_State L) 
		{ 
			return (FilePtr)LuaAuxLib.luaL_checkudata(L, 1, CharPtr.toCharPtr(LuaLib.LUA_FILEHANDLE)); 
		}

		private static int io_type(lua_State L) 
		{
			object ud;
			LuaAuxLib.luaL_checkany(L, 1);
			ud = LuaAPI.lua_touserdata(L, 1);
			LuaAPI.lua_getfield(L, Lua.LUA_REGISTRYINDEX, CharPtr.toCharPtr(LuaLib.LUA_FILEHANDLE));
			if (ud == null || (LuaAPI.lua_getmetatable(L, 1) == 0) || (LuaAPI.lua_rawequal(L, -2, -1) == 0))
			{
				LuaAPI.lua_pushnil(L);  /* not a file */
			}
			else if ( (ud as FilePtr).file == null)
			{
				Lua.lua_pushliteral(L, CharPtr.toCharPtr("closed file"));
			}
			else
			{
				Lua.lua_pushliteral(L, CharPtr.toCharPtr("file"));
			}
			return 1;
		}

		private static StreamProxy tofile(lua_State L) 
		{
			FilePtr f = tofilep(L);
			if (f.file == null)
			{
				LuaAuxLib.luaL_error(L, CharPtr.toCharPtr("attempt to use a closed file"));
			}
			return f.file;
		}

		/*
		 ** When creating file files, always creates a `closed' file file
		 ** before opening the actual file; so, if there is a memory error, the
		 ** file is not left opened.
		 */
		private static FilePtr newfile(lua_State L) 
		{
			FilePtr pf = (FilePtr)LuaAPI.lua_newuserdata(L, new ClassType(ClassType.TYPE_FILEPTR)); //FilePtr
			pf.file = null;  /* file file is currently `closed' */
			LuaAuxLib.luaL_getmetatable(L, CharPtr.toCharPtr(LuaLib.LUA_FILEHANDLE));
			LuaAPI.lua_setmetatable(L, -2);
			return pf;
		}

		/*
		 ** function to (not) close the standard files stdin, stdout, and stderr
		 */
		private static int io_noclose(lua_State L) 
		{
			LuaAPI.lua_pushnil(L);
			Lua.lua_pushliteral(L, CharPtr.toCharPtr("cannot close standard file"));
			return 2;
		}

		/*
		 ** function to close 'popen' files
		 */
		private static int io_pclose(lua_State L) 
		{
			FilePtr p = tofilep(L);
			int ok = (LuaConf.lua_pclose(L, p.file) == 0) ? 1 : 0;
			p.file = null;
			return pushresult(L, ok, null);
		}

		/*
		 ** function to close regular files
		 */
		private static int io_fclose(lua_State L) 
		{
			FilePtr p = tofilep(L);
			int ok = (LuaConf.fclose(p.file) == 0) ? 1 : 0;
			p.file = null;
			return pushresult(L, ok, null);
		}

		private static int aux_close(lua_State L) 
		{
			LuaAPI.lua_getfenv(L, 1);
			LuaAPI.lua_getfield(L, -1, CharPtr.toCharPtr("__close"));
			return (LuaAPI.lua_tocfunction(L, -1)).exec(L);
		}

		private static int io_close(lua_State L) 
		{
			if (Lua.lua_isnone(L, 1))
			{
				LuaAPI.lua_rawgeti(L, Lua.LUA_ENVIRONINDEX, IO_OUTPUT);
			}
			tofile(L);  /* make sure argument is a file */
			return aux_close(L);
		}

		private static int io_gc(lua_State L) 
		{
			StreamProxy f = tofilep(L).file;
			/* ignore closed files */
			if (f != null)
				aux_close(L);
			return 0;
		}

		private static int io_tostring(lua_State L) 
		{
			StreamProxy f = tofilep(L).file;
			if (f == null)
			{
				Lua.lua_pushliteral(L, CharPtr.toCharPtr("file (closed)"));
			}
			else
			{
				LuaAPI.lua_pushfstring(L, CharPtr.toCharPtr("file (%p)"), f);
			}
			return 1;
		}

		private static int io_open(lua_State L) 
		{
			CharPtr filename = LuaAuxLib.luaL_checkstring(L, 1);
			CharPtr mode = LuaAuxLib.luaL_optstring(L, 2, CharPtr.toCharPtr("r"));
			FilePtr pf = newfile(L);
			pf.file = LuaConf.fopen(filename, mode);
			return (pf.file == null) ? pushresult(L, 0, filename) : 1;
		}

		/*
		 ** this function has a separated environment, which defines the
		 ** correct __close for 'popen' files
		 */
		private static int io_popen(lua_State L) 
		{
			CharPtr filename = LuaAuxLib.luaL_checkstring(L, 1);
			CharPtr mode = LuaAuxLib.luaL_optstring(L, 2, CharPtr.toCharPtr("r"));
			FilePtr pf = newfile(L);
			pf.file = LuaConf.lua_popen(L, filename, mode);
			return (pf.file == null) ? pushresult(L, 0, filename) : 1;
		}

		private static int io_tmpfile(lua_State L) 
		{
			FilePtr pf = newfile(L);
			pf.file = LuaConf.tmpfile();
			return (pf.file == null) ? pushresult(L, 0, null) : 1;
		}

		private static StreamProxy getiofile(lua_State L, int findex) 
		{
			StreamProxy f;
			LuaAPI.lua_rawgeti(L, Lua.LUA_ENVIRONINDEX, findex);
			f = (LuaAPI.lua_touserdata(L, -1) as FilePtr).file;
			if (f == null)
			{
				LuaAuxLib.luaL_error(L, CharPtr.toCharPtr("standard %s file is closed"), fnames[findex - 1]);
			}
			return f;
		}

		private static int g_iofile(lua_State L, int f, CharPtr mode) 
		{
			if (!Lua.lua_isnoneornil(L, 1))
			{
				CharPtr filename = Lua.lua_tostring(L, 1);
				if (CharPtr.isNotEqual(filename, null)) 
				{
					FilePtr pf = newfile(L);
					pf.file = LuaConf.fopen(filename, mode);
					if (pf.file == null)
					{
						fileerror(L, 1, filename);
					}
				}
				else 
				{
					tofile(L);  /* check that it's a valid file file */
					LuaAPI.lua_pushvalue(L, 1);
				}
				LuaAPI.lua_rawseti(L, Lua.LUA_ENVIRONINDEX, f);
			}
			/* return current value */
			LuaAPI.lua_rawgeti(L, Lua.LUA_ENVIRONINDEX, f);
			return 1;
		}

		private static int io_input(lua_State L) 
		{
			return g_iofile(L, IO_INPUT, CharPtr.toCharPtr("r"));
		}

		private static int io_output(lua_State L) 
		{
			return g_iofile(L, IO_OUTPUT, CharPtr.toCharPtr("w"));
		}

		private static void aux_lines(lua_State L, int idx, int toclose) 
		{
			LuaAPI.lua_pushvalue(L, idx);
			LuaAPI.lua_pushboolean(L, toclose);  /* close/not close file when finished */
			LuaAPI.lua_pushcclosure(L, new LuaIOLib_delegate("io_readline"), 2);
		}

		private static int f_lines(lua_State L) 
		{
			tofile(L);  /* check that it's a valid file file */
			aux_lines(L, 1, 0);
			return 1;
		}

		private static int io_lines(lua_State L) 
		{
			if (Lua.lua_isnoneornil(L, 1))
			{  
				/* no arguments? */
				/* will iterate over default input */
				LuaAPI.lua_rawgeti(L, Lua.LUA_ENVIRONINDEX, IO_INPUT);
				return f_lines(L);
			}
			else 
			{
				CharPtr filename = LuaAuxLib.luaL_checkstring(L, 1);
				FilePtr pf = newfile(L);
				pf.file = LuaConf.fopen(filename, CharPtr.toCharPtr("r"));
				if (pf.file == null)
				{
					fileerror(L, 1, filename);
				}
				aux_lines(L, LuaAPI.lua_gettop(L), 1);
				return 1;
			}
		}


		/*
		 ** {======================================================
		 ** READ
		 ** =======================================================
		 */

		private static int read_number(lua_State L, StreamProxy f) 
		{
			//lua_Number d;
			object[] parms = {(object)(double)0.0};
			if (LuaConf.fscanf(f, CharPtr.toCharPtr(LuaConf.LUA_NUMBER_SCAN), parms) == 1)
			{
				LuaAPI.lua_pushnumber(L, (double)parms[0]);
				return 1;
			}
			else 
			{
				return 0;  /* read fails */
			}
		}

		private static int test_eof(lua_State L, StreamProxy f) 
		{
			int c = LuaConf.getc(f);
			LuaConf.ungetc(c, f);
			LuaAPI.lua_pushlstring(L, null, 0);
			return (c != LuaConf.EOF) ? 1 : 0;
		}

		private static int read_line(lua_State L, StreamProxy f) 
		{
			luaL_Buffer b = new luaL_Buffer();
			LuaAuxLib.luaL_buffinit(L, b);
			for (;;) 
			{
				int/*uint*/ l;
				CharPtr p = LuaAuxLib.luaL_prepbuffer(b);
				if (CharPtr.isEqual(LuaConf.fgets(p, f), null))
				{  
					/* eof? */
					LuaAuxLib.luaL_pushresult(b);  /* close buffer */
					return (LuaAPI.lua_objlen(L, -1) > 0) ? 1 : 0;  /* check whether read something */
				}
				l = (int/*uint*/)LuaConf.strlen(p);
				if (l == 0 || p.get(l - 1) != '\n')
				{
					LuaAuxLib.luaL_addsize(b, (int)l);
				}
				else 
				{
					LuaAuxLib.luaL_addsize(b, (int)(l - 1));  /* do not include `eol' */
					LuaAuxLib.luaL_pushresult(b);  /* close buffer */
					return 1;  /* read at least an `eol' */
				}
			}
		}

		private static int read_chars(lua_State L, StreamProxy f, long/*uint*/ n) 
		{
			long/*uint*/ rlen;  /* how much to read */
			int/*uint*/ nr;  /* number of chars actually read */
			luaL_Buffer b = new luaL_Buffer();
			LuaAuxLib.luaL_buffinit(L, b);
			rlen = LuaConf.LUAL_BUFFERSIZE;  /* try to read that much each time */
			do 
			{
				CharPtr p = LuaAuxLib.luaL_prepbuffer(b);
				if (rlen > n) 
				{
					rlen = n;  /* cannot read more than asked */
				}
                nr = (int/*uint*/)LuaConf.fread(p, LuaConf.GetUnmanagedSize(new ClassType(ClassType.TYPE_CHAR)), (int)rlen, f); //typeof(char)
				LuaAuxLib.luaL_addsize(b, (int)nr);
				n -= nr;  /* still have to read `n' chars */
			} while (n > 0 && nr == rlen);  /* until end of count or eof */
			LuaAuxLib.luaL_pushresult(b);  /* close buffer */
			return (n == 0 || LuaAPI.lua_objlen(L, -1) > 0) ? 1 : 0;
		}

		private static int g_read(lua_State L, StreamProxy f, int first) 
		{
			int nargs = LuaAPI.lua_gettop(L) - 1;
			int success;
			int n;
			LuaConf.clearerr(f);
			if (nargs == 0) 
			{  
				/* no arguments? */
				success = read_line(L, f);
				n = first + 1;  /* to return 1 result */
			}
			else 
			{  
				/* ensure stack space for all results and for auxlib's buffer */
				LuaAuxLib.luaL_checkstack(L, nargs + Lua.LUA_MINSTACK, CharPtr.toCharPtr("too many arguments"));
				success = 1;
				for (n = first; (nargs-- != 0) && (success != 0); n++) 
				{
					if (LuaAPI.lua_type(L, n) == Lua.LUA_TNUMBER)
					{
						int/*uint*/ l = (int/*uint*/)LuaAPI.lua_tointeger(L, n);
						success = (l == 0) ? test_eof(L, f) : read_chars(L, f, l);
					}
					else 
					{
						CharPtr p = Lua.lua_tostring(L, n);
						LuaAuxLib.luaL_argcheck(L, (CharPtr.isNotEqual(p, null)) && (p.get(0) == '*'), n, "invalid option");
						switch (p.get(1)) 
						{
							case 'n':  /* number */
								{
									success = read_number(L, f);
									break;
								}
							case 'l':  /* line */
								{
									success = read_line(L, f);
									break;
								}
							case 'a':  /* file */
								{
								read_chars(L, f, /*~((uint)0*/ (long)((~(int)0) & 0xffffffff) );  /* read MAX_uint chars */
									success = 1; /* always success */
									break;
								}
							default:
								{
									return LuaAuxLib.luaL_argerror(L, n, CharPtr.toCharPtr("invalid format"));
								}
						}
					}
				}
			}
			if (LuaConf.ferror(f) != 0)
			{
				return pushresult(L, 0, null);
			}
			if (success == 0) 
			{
				Lua.lua_pop(L, 1);  /* remove last result */
				LuaAPI.lua_pushnil(L);  /* push nil instead */
			}
			return n - first;
		}

		private static int io_read(lua_State L) 
		{
			return g_read(L, getiofile(L, IO_INPUT), 1);
		}

		private static int f_read(lua_State L) 
		{
			return g_read(L, tofile(L), 2);
		}

		private static int io_readline(lua_State L) 
		{
			StreamProxy f = (LuaAPI.lua_touserdata(L, Lua.lua_upvalueindex(1)) as FilePtr).file;
			int sucess;
			if (f == null)  /* file is already closed? */
			{
				LuaAuxLib.luaL_error(L, CharPtr.toCharPtr("file is already closed"));
			}
			sucess = read_line(L, f);
			if (LuaConf.ferror(f) != 0)
			{
				return LuaAuxLib.luaL_error(L, CharPtr.toCharPtr("%s"), LuaConf.strerror(LuaConf.errno()));
			}
			if (sucess != 0) 
			{
				return 1;
			}
			else 
			{  
				/* EOF */
				if (LuaAPI.lua_toboolean(L, Lua.lua_upvalueindex(2)) != 0)
				{  
					/* generator created file? */
					LuaAPI.lua_settop(L, 0);
					LuaAPI.lua_pushvalue(L, Lua.lua_upvalueindex(1));
					aux_close(L);  /* close it */
				}
				return 0;
			}
		}

		/* }====================================================== */

		private static int g_write (lua_State L, StreamProxy f, int arg) 
		{
			int nargs = LuaAPI.lua_gettop(L) - 1;
			int status = 1;
			for (; (nargs--) != 0; arg++) 
			{
				if (LuaAPI.lua_type(L, arg) == Lua.LUA_TNUMBER)
				{
					/* optimization: could be done exactly as for strings */
					status = ((status!=0) &&
					          (LuaConf.fprintf(f, CharPtr.toCharPtr(LuaConf.LUA_NUMBER_FMT), LuaAPI.lua_tonumber(L, arg)) > 0)) ? 1 : 0;
				}
				else 
				{
					int[]/*uint*/ l = new int[1];
					CharPtr s = LuaAuxLib.luaL_checklstring(L, arg, /*out*/ l);
                    status = ((status != 0) && (LuaConf.fwrite(s, LuaConf.GetUnmanagedSize(new ClassType(ClassType.TYPE_CHAR)), (int)l[0], f) == l[0])) ? 1 : 0; //typeof(char)
				}
			}
			return pushresult(L, status, null);
		}

		private static int io_write(lua_State L) 
		{
			return g_write(L, getiofile(L, IO_OUTPUT), 1);
		}

		private static int f_write(lua_State L) 
		{
			return g_write(L, tofile(L), 2);
		}

		private static int f_seek(lua_State L) 
		{
			int[] mode = { 
				LuaConf.SEEK_SET, 
				LuaConf.SEEK_CUR, 
				LuaConf.SEEK_END 
			};
			CharPtr[] modenames = { 
				CharPtr.toCharPtr("set"), 
				CharPtr.toCharPtr("cur"), 
				CharPtr.toCharPtr("end"), 
				null
			};
			StreamProxy f = tofile(L);
			int op = LuaAuxLib.luaL_checkoption(L, 2, CharPtr.toCharPtr("cur"), modenames);
			long offset = LuaAuxLib.luaL_optlong(L, 3, 0);
			op = LuaConf.fseek(f, offset, mode[op]);
			if (op != 0)
			{
				return pushresult(L, 0, null);  /* error */
			}
			else 
			{
				LuaAPI.lua_pushinteger(L, LuaConf.ftell(f));
				return 1;
			}
		}

		private static int f_setvbuf(lua_State L) 
		{
			CharPtr[] modenames = { 
				CharPtr.toCharPtr("no"), 
				CharPtr.toCharPtr("full"), 
				CharPtr.toCharPtr("line"), 
				null 
			};
			int[] mode = { LuaConf._IONBF, LuaConf._IOFBF, LuaConf._IOLBF };
			StreamProxy f = tofile(L);
			int op = LuaAuxLib.luaL_checkoption(L, 2, null, modenames);
			int/*Int32*//*lua_Integer*/ sz = LuaAuxLib.luaL_optinteger(L, 3, LuaConf.LUAL_BUFFERSIZE);
			int res = LuaConf.setvbuf(f, null, mode[op], (/*uint*/int)sz);
			return pushresult(L, (res == 0) ? 1 : 0, null);
		}
		
		private static int io_flush(lua_State L) 
		{
			int result = 1;
			try 
			{
				getiofile(L, IO_OUTPUT).Flush();
			} 
			catch 
			{
				result = 0;
			}
			return pushresult(L, result, null);
		}

		private static int f_flush (lua_State L) 
		{
			int result = 1;
			try 
			{
				tofile(L).Flush();
			} 
			catch 
			{
				result = 0;
			}
			return pushresult(L, result, null);
		}


		private readonly static luaL_Reg[] iolib = {
			new luaL_Reg(CharPtr.toCharPtr("close"), new LuaIOLib_delegate("io_close")),
			new luaL_Reg(CharPtr.toCharPtr("flush"), new LuaIOLib_delegate("io_flush")),
			new luaL_Reg(CharPtr.toCharPtr("input"), new LuaIOLib_delegate("io_input")),
			new luaL_Reg(CharPtr.toCharPtr("lines"), new LuaIOLib_delegate("io_lines")),
			new luaL_Reg(CharPtr.toCharPtr("open"), new LuaIOLib_delegate("io_open")),
			new luaL_Reg(CharPtr.toCharPtr("output"), new LuaIOLib_delegate("io_output")),
			new luaL_Reg(CharPtr.toCharPtr("popen"), new LuaIOLib_delegate("io_popen")),
			new luaL_Reg(CharPtr.toCharPtr("read"), new LuaIOLib_delegate("io_read")),
			new luaL_Reg(CharPtr.toCharPtr("tmpfile"), new LuaIOLib_delegate("io_tmpfile")),
			new luaL_Reg(CharPtr.toCharPtr("type"), new LuaIOLib_delegate("io_type")),
			new luaL_Reg(CharPtr.toCharPtr("write"), new LuaIOLib_delegate("io_write")),
			new luaL_Reg(null, null)
		};

		private readonly static luaL_Reg[] flib = {
			new luaL_Reg(CharPtr.toCharPtr("close"), new LuaIOLib_delegate("io_close")),
			new luaL_Reg(CharPtr.toCharPtr("flush"), new LuaIOLib_delegate("f_flush")),
			new luaL_Reg(CharPtr.toCharPtr("lines"), new LuaIOLib_delegate("f_lines")),
			new luaL_Reg(CharPtr.toCharPtr("read"), new LuaIOLib_delegate("f_read")),
			new luaL_Reg(CharPtr.toCharPtr("seek"), new LuaIOLib_delegate("f_seek")),
			new luaL_Reg(CharPtr.toCharPtr("setvbuf"), new LuaIOLib_delegate("f_setvbuf")),
			new luaL_Reg(CharPtr.toCharPtr("write"), new LuaIOLib_delegate("f_write")),
			new luaL_Reg(CharPtr.toCharPtr("__gc"), new LuaIOLib_delegate("io_gc")),
			new luaL_Reg(CharPtr.toCharPtr("__tostring"), new LuaIOLib_delegate("io_tostring")),
			new luaL_Reg(null, null)
		};
		
		public class LuaIOLib_delegate : lua_CFunction
		{
			private string name;
			
			public LuaIOLib_delegate(string name)
			{
				this.name = name;
			}
			
			public int exec(lua_State L)
			{
				if ("io_close".Equals(name))
				{
					return io_close(L);
				} 
				else if ("io_flush".Equals(name)) 
				{
					return io_flush(L);
				} 
				else if ("io_input".Equals(name)) 
				{
					return io_input(L);
				} 
				else if ("io_lines".Equals(name)) 
				{
				    return io_lines(L);
				}
				else if ("io_open".Equals(name))
				{
				    return io_open(L);
				}
				else if ("io_output".Equals(name))
				{
					return io_output(L);
				}
				else if ("io_popen".Equals(name))
				{
					return io_popen(L);
				}
				else if ("io_read".Equals(name))
				{
					return io_read(L);
				}
				else if ("io_tmpfile".Equals(name))
				{
					return io_tmpfile(L);
				}
				else if ("io_type".Equals(name))
				{
					return io_type(L);
				}
				else if ("io_write".Equals(name))
				{
					return io_write(L);
				}
				else if ("f_flush".Equals(name))
				{
					return f_flush(L);
				}
				else if ("f_lines".Equals(name))
				{
					return f_lines(L);
				}
				else if ("f_read".Equals(name))
				{
					return f_read(L);
				}
				else if ("f_seek".Equals(name))
				{
					return f_seek(L);
				}
				else if ("f_setvbuf".Equals(name))
				{
					return f_setvbuf(L);
				}
				else if ("f_write".Equals(name))
				{
					return f_write(L);
				}
				else if ("io_gc".Equals(name))
				{
					return io_gc(L);
				}
				else if ("io_tostring".Equals(name))
		        {
		         	return io_tostring(L);
		        }
				else if ("io_fclose".Equals(name))
				{
					return io_fclose(L);
				}
				else if ("io_noclose".Equals(name))
				{
					return io_noclose(L);
				}
				else if ("io_pclose".Equals(name))
				{
					return io_pclose(L);
				}
				else if ("io_readline".Equals(name))
				{
					return io_readline(L);
				}
				else
				{
					return 0;
				}
			}
		}

		private static void createmeta(lua_State L) 
		{
			LuaAuxLib.luaL_newmetatable(L, CharPtr.toCharPtr(LuaLib.LUA_FILEHANDLE));  /* create metatable for file files */
			LuaAPI.lua_pushvalue(L, -1);  /* push metatable */
			LuaAPI.lua_setfield(L, -2, CharPtr.toCharPtr("__index"));  /* metatable.__index = metatable */
			LuaAuxLib.luaL_register(L, null, flib);  /* file methods */
		}

		private static void createstdfile(lua_State L, StreamProxy f, int k, CharPtr fname) 
		{
			newfile(L).file = f;
			if (k > 0) 
			{
				LuaAPI.lua_pushvalue(L, -1);
				LuaAPI.lua_rawseti(L, Lua.LUA_ENVIRONINDEX, k);
			}
			LuaAPI.lua_pushvalue(L, -2);  /* copy environment */
			LuaAPI.lua_setfenv(L, -2);  /* set it */
			LuaAPI.lua_setfield(L, -3, fname);
		}

		private static void newfenv(lua_State L, lua_CFunction cls) 
		{
			LuaAPI.lua_createtable(L, 0, 1);
			Lua.lua_pushcfunction(L, cls);
			LuaAPI.lua_setfield(L, -2, CharPtr.toCharPtr("__close"));
		}

		public static int luaopen_io(lua_State L) 
		{
			createmeta(L);
			/* create (private) environment (with fields IO_INPUT, IO_OUTPUT, __close) */
			newfenv(L, new LuaIOLib_delegate("io_fclose"));
			LuaAPI.lua_replace(L, Lua.LUA_ENVIRONINDEX);
			/* open library */
			LuaAuxLib.luaL_register(L, CharPtr.toCharPtr(LuaLib.LUA_IOLIBNAME), iolib);
			/* create (and set) default files */
			newfenv(L, new LuaIOLib_delegate("io_noclose"));  /* close function for default files */
			createstdfile(L, LuaConf.stdin, IO_INPUT, CharPtr.toCharPtr("stdin"));
			createstdfile(L, LuaConf.stdout, IO_OUTPUT, CharPtr.toCharPtr("stdout"));
			createstdfile(L, LuaConf.stderr, 0, CharPtr.toCharPtr("stderr"));
			Lua.lua_pop(L, 1);  /* pop environment for default files */
			LuaAPI.lua_getfield(L, -1, CharPtr.toCharPtr("popen"));
			newfenv(L, new LuaIOLib_delegate("io_pclose"));  /* create environment for 'popen' */
			LuaAPI.lua_setfenv(L, -2);  /* set fenv for 'popen' */
			Lua.lua_pop(L, 1);  /* pop 'popen' */
			return 1;
		}
	}
}
