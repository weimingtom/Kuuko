/*
 ** $Id: lundump.c,v 2.7.1.4 2008/04/04 19:51:41 roberto Exp $
 ** load precompiled Lua chunks
 ** See Copyright Notice in lua.h
 */
using System;

namespace KopiLua
{
	//using TValue = Lua.TValue;
	//using lua_Number = System.Double;
	//using lu_byte = System.Byte;
	//using StkId = TValue;
	//using Instruction = System.UInt32;

	public class LuaUndump
	{
		/* for header of binary files -- this is Lua 5.1 */
		public const int LUAC_VERSION = 0x51;

		/* for header of binary files -- this is the official format */
		public const int LUAC_FORMAT = 0;

		/* size of header of binary files */
		public const int LUAC_HEADERSIZE = 12;

		//#ifdef LUAC_TRUST_BINARIES
		//#define IF(c,s)
		//#define error(S,s)
		//#else
		//#define IF(c,s)		if (c) error(S,s)

		public static void IF(int c, string s) 
		{
			
		}

		public static void IF(bool c, string s)
		{
			
		}

		static void error(LoadState S, CharPtr why)
		{
			LuaObject.luaO_pushfstring(S.L, CharPtr.toCharPtr("%s: %s in precompiled chunk"), S.name, why);
			LuaDo.luaD_throw(S.L, Lua.LUA_ERRSYNTAX);
		}
		//#endif

		public static object LoadMem(LoadState S, ClassType t)
		{
            int size = t.GetMarshalSizeOf();
			CharPtr str = CharPtr.toCharPtr(new char[size]);
			LoadBlock(S, str, size);
			byte[] bytes = new byte[str.chars.Length];
			for (int i = 0; i < str.chars.Length; i++)
			{
				bytes[i] = (byte)str.chars[i];
			}
            object b = t.bytesToObj(bytes);
            return b;
		}

		public static object LoadMem(LoadState S, ClassType t, int n)
		{
			//ArrayList array = new ArrayList();
			object[] array = new object[n];
			for (int i = 0; i < n; i++)
			{
				array[i] = LoadMem(S, t);
			}
			return t.ToArray(array);
        }
		
		public static Byte/*lu_byte*/ LoadByte(LoadState S) 
		{ 
			return (Byte/*lu_byte*/)LoadChar(S); 
		}
		
		public static object LoadVar(LoadState S, ClassType t) 
		{ 
			return LoadMem(S, t); 
		}
		
		public static object LoadVector(LoadState S, ClassType t, int n) 
		{
			return LoadMem(S, t, n);
		}

		private static void LoadBlock(LoadState S, CharPtr b, int size)
		{
			int/*uint*/ r = LuaZIO.luaZ_read(S.Z, b, /*(uint)*/size);
			IF (r != 0, "unexpected end");
		}

		private static int LoadChar(LoadState S)
		{
			return (char)LoadVar(S, new ClassType(ClassType.TYPE_CHAR));
		}

		private static int LoadInt(LoadState S)
		{
            int x = (int)LoadVar(S, new ClassType(ClassType.TYPE_INT));
			IF (x < 0, "bad integer");
			return x;
		}

		private static Double/*lua_Number*/ LoadNumber(LoadState S)
		{
            return (Double/*lua_Number*/)LoadVar(S, new ClassType(ClassType.TYPE_DOUBLE));/*lua_Number*/
		}

		private static TString LoadString(LoadState S)
		{
			//typeof(int/*uint*/)
			int/*uint*/ size = (int/*uint*/)LoadVar(S, new ClassType(ClassType.TYPE_INT)); 
			if (size == 0)
			{
				return null;
			}
			else
			{
				CharPtr s = LuaZIO.luaZ_openspace(S.L, S.b, size);
				LoadBlock(S, s, (int)size);
				return LuaString.luaS_newlstr(S.L, s, size - 1);/* remove trailing '\0' */
			}
		}

		private static void LoadCode(LoadState S, Proto f)
		{
			int n = LoadInt(S);
            /*UInt32*/
            /*Instruction*/
			f.code = LuaMem.luaM_newvector_long(S.L, n, new ClassType(ClassType.TYPE_LONG));
			f.sizecode = n;
			f.code = (long[]/*UInt32[]*//*Instruction[]*/)LoadVector(S, new ClassType(ClassType.TYPE_LONG), n);
		}

		private static void LoadConstants(LoadState S, Proto f)
		{
			int i,n;
			n = LoadInt(S);
			f.k = LuaMem.luaM_newvector_TValue(S.L, n, new ClassType(ClassType.TYPE_TVALUE));
			f.sizek = n;
			for (i = 0; i < n; i++) 
			{
				LuaObject.setnilvalue(f.k[i]);
			}
			for (i = 0; i < n; i++)
			{
				TValue o = f.k[i];
				int t = LoadChar(S);
				switch (t)
				{
					case Lua.LUA_TNIL:
						{
							LuaObject.setnilvalue(o);
							break;
						}
					case Lua.LUA_TBOOLEAN:
						{
							LuaObject.setbvalue(o, LoadChar(S));
							break;
						}
					case Lua.LUA_TNUMBER:
						{
							LuaObject.setnvalue(o, LoadNumber(S));
							break;
						}
					case Lua.LUA_TSTRING:
						{
							LuaObject.setsvalue2n(S.L, o, LoadString(S));
							break;
						}
					default:
						{
							error(S, CharPtr.toCharPtr("bad constant"));
							break;
						}
				}
			}
			n = LoadInt(S);
			f.p = LuaMem.luaM_newvector_Proto(S.L, n, new ClassType(ClassType.TYPE_PROTO));
			f.sizep = n;
			for (i = 0; i < n; i++) 
			{
				f.p[i] = null;
			}
			for (i = 0; i < n; i++) 
			{
				f.p[i] = LoadFunction(S,f.source);
			}
		}

		private static void LoadDebug(LoadState S, Proto f)
		{
			int i, n;
			n = LoadInt(S);
			f.lineinfo = LuaMem.luaM_newvector_int(S.L, n, new ClassType(ClassType.TYPE_INT));
			f.sizelineinfo = n;
            f.lineinfo = (int[])LoadVector(S, new ClassType(ClassType.TYPE_INT), n); //typeof(int)
			n = LoadInt(S);
			f.locvars = LuaMem.luaM_newvector_LocVar(S.L, n, new ClassType(ClassType.TYPE_LOCVAR));
			f.sizelocvars = n;
			for (i = 0; i < n; i++) 
			{
				f.locvars[i].varname = null;
			}
			for (i = 0; i < n; i++)
			{
				f.locvars[i].varname = LoadString(S);
				f.locvars[i].startpc = LoadInt(S);
				f.locvars[i].endpc = LoadInt(S);
			}
			n = LoadInt(S);
			f.upvalues = LuaMem.luaM_newvector_TString(S.L, n, new ClassType(ClassType.TYPE_TSTRING));
			f.sizeupvalues = n;
			for (i = 0; i < n; i++) 
			{
				f.upvalues[i] = null;
			}
			for (i = 0; i < n; i++) 
			{
				f.upvalues[i] = LoadString(S);
			}
		}

		private static Proto LoadFunction(LoadState S, TString p)
		{
			Proto f;
			if (++S.L.nCcalls > LuaConf.LUAI_MAXCCALLS) 
			{
				error(S, CharPtr.toCharPtr("code too deep"));
			}
			f = LuaFunc.luaF_newproto(S.L);
			LuaObject.setptvalue2s(S.L, S.L.top, f); 
			LuaDo.incr_top(S.L);
			f.source = LoadString(S); 
			if (f.source == null) 
			{
				f.source = p;
			}
			f.linedefined = LoadInt(S);
			f.lastlinedefined = LoadInt(S);
			f.nups = LoadByte(S);
			f.numparams = LoadByte(S);
			f.is_vararg = LoadByte(S);
			f.maxstacksize = LoadByte(S);
			LoadCode(S,f);
			LoadConstants(S,f);
			LoadDebug(S,f);
			IF(LuaDebug.luaG_checkcode(f) == 0 ? 1 : 0, "bad code");
			TValue[] top = new TValue[1];
			top[0] = S.L.top;
			/*StkId*/TValue.dec(/*ref*/ top);
			S.L.top = top[0];
			S.L.nCcalls--;
			return f;
		}

		private static void LoadHeader(LoadState S)
		{
			CharPtr h = CharPtr.toCharPtr(new char[LUAC_HEADERSIZE]);
			CharPtr s = CharPtr.toCharPtr(new char[LUAC_HEADERSIZE]);
			luaU_header(h);
			LoadBlock(S, s, LUAC_HEADERSIZE);
			IF(LuaConf.memcmp(h, s, LUAC_HEADERSIZE) != 0, "bad header");
		}

		/*
		 ** load precompiled chunk
		 */
		public static Proto luaU_undump(lua_State L, ZIO Z, Mbuffer buff, CharPtr name)
		{
			LoadState S = new LoadState();
			if (name.get(0) == '@' || name.get(0) == '=')
			{
				S.name = CharPtr.plus(name, 1);
			}
			else if (name.get(0) == Lua.LUA_SIGNATURE[0])
			{
				S.name = CharPtr.toCharPtr("binary string");
			}
			else
			{
				S.name = name;
			}
			S.L = L;
			S.Z = Z;
			S.b = buff;
			LoadHeader(S);
			return LoadFunction(S, LuaString.luaS_newliteral(L, CharPtr.toCharPtr("=?")));
		}

		/*
		 * make header
		 */
		public static void luaU_header(CharPtr h)
		{
			h = new CharPtr(h);
			int x = 1;
			LuaConf.memcpy(h, CharPtr.toCharPtr(Lua.LUA_SIGNATURE), Lua.LUA_SIGNATURE.Length);
			h = h.add(Lua.LUA_SIGNATURE.Length);
			h.set(0, (char)LUAC_VERSION);
			h.inc();
			h.set(0, (char)LUAC_FORMAT);
			h.inc();
			//*h++=(char)*(char*)&x;				/* endianness */
			h.set(0, (char)x);						/* endianness */
			h.inc();
			h.set(0, (char)ClassType.SizeOfInt());
			h.inc();
            //FIXME:
			h.set(0, (char)ClassType.SizeOfLong());
			//sizeof(long/*uint*/)
			h.inc();
            h.set(0, (char)ClassType.SizeOfLong());
            //sizeof(long/*UInt32*//*Instruction*/));
			h.inc();
			h.set(0, (char)ClassType.SizeOfDouble());
			//sizeof(Double/*lua_Number*/)
			h.inc();
			//(h++)[0] = ((lua_Number)0.5 == 0) ? 0 : 1;		/* is lua_Number integral? */
			h.set(0, (char)0);	// always 0 on this build
		}
	}
}
