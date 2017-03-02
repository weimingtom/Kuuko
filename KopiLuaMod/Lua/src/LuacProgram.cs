/*
 ** $Id: luac.c,v 1.54 2006/06/02 17:37:11 lhf Exp $
 ** Lua compiler (saves bytecodes to files; also list bytecodes)
 ** See Copyright Notice in lua.h
 */
using System;

namespace KopiLua
{
	//using Instruction = System.UInt32;

	public class LuacProgram
	{
		//#include <errno.h>
		//#include <stdio.h>
		//#include <stdlib.h>
		//#include <string.h>

		//#define luac_c
		//#define LUA_CORE

		//#include "lua.h"
		//#include "lauxlib.h"

		//#include "ldo.h"
		//#include "lfunc.h"
		//#include "lmem.h"
		//#include "lobject.h"
		//#include "lopcodes.h"
		//#include "lstring.h"
		//#include "lundump.h"

		static CharPtr PROGNAME = CharPtr.toCharPtr("luac");		/* default program name */
		static CharPtr OUTPUT = CharPtr.toCharPtr(PROGNAME + ".out"); /* default output file */

		static int listing = 0;			/* list bytecodes? */
		static int dumping = 1;			/* dump bytecodes? */
		static int stripping = 0;			/* strip debug information? */
		static CharPtr Output = OUTPUT;	/* default output file name */
		static CharPtr output = Output;	/* actual output file name */
		static CharPtr progname = PROGNAME;	/* actual program name */

		static void fatal(CharPtr message)
		{
			LuaConf.fprintf(LuaConf.stderr, CharPtr.toCharPtr("%s: %s\n"), progname, message);
			Environment.Exit(LuaConf.EXIT_FAILURE);
		}

		static void cannot(CharPtr what)
		{
			LuaConf.fprintf(LuaConf.stderr, CharPtr.toCharPtr("%s: cannot %s %s: %s\n"), progname, what, output, LuaConf.strerror(LuaConf.errno()));
			Environment.Exit(LuaConf.EXIT_FAILURE);
		}

		static void usage(CharPtr message)
		{
			if (message.get(0) == '-')			
			{
				LuaConf.fprintf(LuaConf.stderr, 
					CharPtr.toCharPtr("%s: unrecognized option " + LuaConf.getLUA_QS() + "\n"), 
					progname, message);
			}
			else
			{
				LuaConf.fprintf(LuaConf.stderr, CharPtr.toCharPtr("%s: %s\n"), 
				    progname, message);
			}
			LuaConf.fprintf(LuaConf.stderr,
			                CharPtr.toCharPtr("usage: %s [options] [filenames].\n" +
			                "Available options are:\n" +
			                "  -        process stdin\n" +
			                "  -l       list\n" +
			                "  -o name  output to file " + LuaConf.LUA_QL("name") + " (default is \"%s\")\n" +
			                "  -p       parse only\n" +
			                "  -s       strip debug information\n" +
			                "  -v       show version information\n" +
			                "  --       stop handling options\n"),
			                progname, Output);
			Environment.Exit(LuaConf.EXIT_FAILURE);
		}

		//#define	IS(s)	(strcmp(argv[i],s)==0)

		static int doargs(int argc, string[] argv)
		{
			int i;
			int version = 0;
			if ((argv.Length > 0) && (!argv[0].Equals(""))) 
			{
				progname = CharPtr.toCharPtr(argv[0]);
			}
			for (i = 1; i < argc; i++)
			{
				if (argv[i][0] != '-')			/* end of options; keep it */
				{
					break;
				}
				else if (LuaConf.strcmp(CharPtr.toCharPtr(argv[i]), CharPtr.toCharPtr("--")) == 0)			/* end of options; skip it */
				{
					++i;
					if (version != 0) 
					{
						++version;
					}
					break;
				}
				else if (LuaConf.strcmp(CharPtr.toCharPtr(argv[i]), CharPtr.toCharPtr("-")) == 0)			/* end of options; use stdin */
				{
					break;
				}
				else if (LuaConf.strcmp(CharPtr.toCharPtr(argv[i]), CharPtr.toCharPtr("-l")) == 0)			/* list */
				{
					++listing;
				}
				else if (LuaConf.strcmp(CharPtr.toCharPtr(argv[i]), CharPtr.toCharPtr("-o")) == 0)			/* output file */
				{
					output = CharPtr.toCharPtr(argv[++i]);
					if (CharPtr.isEqual(output, null) || (output.get(0) == 0)) 
					{
						usage(CharPtr.toCharPtr(LuaConf.LUA_QL("-o") + " needs argument"));
					}
					if (LuaConf.strcmp(CharPtr.toCharPtr(argv[i]), CharPtr.toCharPtr("-")) == 0) 
					{
						output = null;
					}
				}
				else if (LuaConf.strcmp(CharPtr.toCharPtr(argv[i]), CharPtr.toCharPtr("-p")) == 0)			/* parse only */
				{
					dumping = 0;
				}
				else if (LuaConf.strcmp(CharPtr.toCharPtr(argv[i]), CharPtr.toCharPtr("-s")) == 0)			/* strip debug information */
				{
					stripping = 1;
				}
				else if (LuaConf.strcmp(CharPtr.toCharPtr(argv[i]), CharPtr.toCharPtr("-v")) == 0)			/* show version */
				{
					++version;
				}
				else					/* unknown option */
				{
					usage(CharPtr.toCharPtr(argv[i]));
				}
			}
			if (i == argc && ((listing != 0) || (dumping == 0)))
			{
				dumping = 0;
				argv[--i] = Output.ToString();
			}
			if (version != 0)
			{
				LuaConf.printf(CharPtr.toCharPtr("%s  %s\n"), Lua.LUA_RELEASE, Lua.LUA_COPYRIGHT);
				if (version == argc - 1) 
				{
					Environment.Exit(LuaConf.EXIT_SUCCESS);
				}
			}
			return i;
		}

		static Proto toproto(lua_State L, int i) 
		{
			return LuaObject.clvalue(TValue.plus(L.top, i)).l.p;
		}

		static Proto combine(lua_State L, int n)
		{
			if (n == 1)
			{
				return toproto(L, -1);
			}
			else
			{
				int i, pc;
				Proto f = LuaFunc.luaF_newproto(L);
				LuaObject.setptvalue2s(L, L.top,f); 
				LuaDo.incr_top(L);
				f.source = LuaString.luaS_newliteral(L, CharPtr.toCharPtr("=(" + PROGNAME + ")"));
				f.maxstacksize = 1;
				pc = 2 * n + 1;
                /*UInt32[]*/
                /*Instruction[]*/
                /*UInt32*/
                /*Instruction*/
				f.code = (long[])LuaMem.luaM_newvector_long(L, pc, new ClassType(ClassType.TYPE_LONG));
				f.sizecode = pc;
				f.p = LuaMem.luaM_newvector_Proto(L, n, new ClassType(ClassType.TYPE_PROTO));
				f.sizep = n;
				pc = 0;
				for (i = 0; i < n; i++)
				{
					f.p[i] = toproto(L,i-n-1);
					f.code[pc++] = (long/*uint*/)LuaOpCodes.CREATE_ABx(OpCode.OP_CLOSURE, 0, i);
					f.code[pc++] = (long/*uint*/)LuaOpCodes.CREATE_ABC(OpCode.OP_CALL, 0, 1, 1);
				}
				f.code[pc++] = (long/*uint*/)LuaOpCodes.CREATE_ABC(OpCode.OP_RETURN, 0, 1, 0);
				return f;
			}
		}

        //FIXME:StreamProxy/*object*/ u
		static int writer(lua_State L, CharPtr p, int/*uint*/ size, object u)
		{
			//UNUSED(L);
			return ((LuaConf.fwrite(p, (int)size, 1, (StreamProxy)u) != 1) && (size != 0)) ? 1 : 0;
		}
		
		public class writer_delegate : lua_Writer
		{
			public int exec(lua_State L, CharPtr p, int/*uint*/ sz, object ud)
			{
                //FIXME:StreamProxy/*object*/ u
				return writer(L, p, sz, ud);
			}
		}

		static int pmain(lua_State L)
		{
			Smain s = (Smain)LuaAPI.lua_touserdata(L, 1);
			int argc = s.argc;
			string[] argv = s.argv;
			Proto f;
			int i;
			if (LuaAPI.lua_checkstack(L,argc) == 0) 
			{
				fatal(CharPtr.toCharPtr("too many input files"));
			}
			for (i = 0; i < argc; i++)
			{
				CharPtr filename = (LuaConf.strcmp(CharPtr.toCharPtr(argv[i]), CharPtr.toCharPtr("-")) == 0) ? null : CharPtr.toCharPtr(argv[i]);
				if (LuaAuxLib.luaL_loadfile(L,filename) != 0) 
				{
					fatal(Lua.lua_tostring(L,-1));
				}
			}
			f = combine(L, argc);
			if (listing != 0) 
			{
				LuaPrint.luaU_print(f,(listing > 1) ? 1 : 0);
			}
			if (dumping!=0)
			{
				StreamProxy D = (CharPtr.isEqual(output, null)) ? LuaConf.stdout : LuaConf.fopen(output, CharPtr.toCharPtr("wb"));
                if (D == null)
                {
                    cannot(CharPtr.toCharPtr("open"));
                }
                LuaLimits.lua_lock(L);
                LuaDump.luaU_dump(L, f, new writer_delegate(), D, stripping);
				LuaLimits.lua_unlock(L);
                if (LuaConf.ferror(D) != 0)
                {
                    cannot(CharPtr.toCharPtr("write"));
                }
                if (LuaConf.fclose(D) != 0)
                {
                    cannot(CharPtr.toCharPtr("close"));
                }
			}
			return 0;
		}

		public static int MainLuac(string[] args)
		{
			// prepend the exe name to the arg list as it's done in C
			// so that we don't have to change any of the args indexing
			// code above
			string[] newargs = new string[(args != null ? args.Length : 0) + 1];
			newargs[0] = "luac";//Assembly.GetExecutingAssembly().Location);
			for (int idx = 0; idx < args.Length; idx++)
			{
				newargs[idx + 1] = args[idx];
			}
			args = newargs;

			lua_State L;
			Smain s = new Smain();
			int argc = args.Length;
			int i = doargs(argc,args);
			//newargs.RemoveRange(0, i);
			string[] newargs2 = new string[newargs.Length - i];
			for (int idx = newargs.Length - i; idx < newargs.Length; idx++)
			{
				newargs2[idx - (newargs.Length - i)] = newargs[idx];
			}
			argc -= i;
			args = newargs2;//(string[])newargs.ToArray();
			if (argc <= 0) 
			{
				usage(CharPtr.toCharPtr("no input files given"));
			}
			L = Lua.lua_open();
			if (L == null) 
			{
				fatal(CharPtr.toCharPtr("not enough memory for state"));
			}
			s.argc = argc;
			s.argv = args;
			if (LuaAPI.lua_cpcall(L, new pmain_delegate(), s) != 0)
			{
				fatal(Lua.lua_tostring(L,-1));
			}
			LuaState.lua_close(L);
			return LuaConf.EXIT_SUCCESS;
		}
		
		public class pmain_delegate : lua_CFunction
		{
			public int exec(lua_State L)
			{
				return pmain(L);
			}
		}
	}
}
