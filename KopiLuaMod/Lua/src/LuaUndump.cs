/*
 ** $Id: lundump.c,v 2.7.1.4 2008/04/04 19:51:41 roberto Exp $
 ** load precompiled Lua chunks
 ** See Copyright Notice in lua.h
 */
using System;
using System.Collections;
using System.Runtime.InteropServices;

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
			LuaObject.luaO_pushfstring(S.L, "%s: %s in precompiled chunk", S.name, why);
			LuaDo.luaD_throw(S.L, Lua.LUA_ERRSYNTAX);
		}
		//#endif

		public static object LoadMem(LoadState S, Type t)
		{
			int size = Marshal.SizeOf(t);
			CharPtr str = new char[size];
			LoadBlock(S, str, size);
			byte[] bytes = new byte[str.chars.Length];
			for (int i = 0; i < str.chars.Length; i++)
			{
				bytes[i] = (byte)str.chars[i];
			}
			GCHandle pinnedPacket = GCHandle.Alloc(bytes, GCHandleType.Pinned);
			object b = Marshal.PtrToStructure(pinnedPacket.AddrOfPinnedObject(), t);
			pinnedPacket.Free();
			return b;
		}

		public static object LoadMem(LoadState S, Type t, int n)
		{
			ArrayList array = new ArrayList();
			for (int i = 0; i < n; i++)
			{
				array.Add(LoadMem(S, t));
			}
			return array.ToArray(t);
		}
		
		public static Byte/*lu_byte*/ LoadByte(LoadState S) 
		{ 
			return (Byte/*lu_byte*/)LoadChar(S); 
		}
		
		public static object LoadVar(LoadState S, Type t) 
		{ 
			return LoadMem(S, t); 
		}
		
		public static object LoadVector(LoadState S, Type t, int n) 
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
			return (char)LoadVar(S, typeof(char));
		}

		private static int LoadInt(LoadState S)
		{
			int x = (int)LoadVar(S, typeof(int));
			IF (x < 0, "bad integer");
			return x;
		}

		private static Double/*lua_Number*/ LoadNumber(LoadState S)
		{
			return (Double/*lua_Number*/)LoadVar(S, typeof(Double/*lua_Number*/));
		}

		private static TString LoadString(LoadState S)
		{
			int/*uint*/ size = (int/*uint*/)LoadVar(S, typeof(uint));
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
			f.code = LuaMem.luaM_newvector<UInt32/*Instruction*/>(S.L, n);
			f.sizecode = n;
			f.code = (UInt32[]/*Instruction[]*/)LoadVector(S, typeof(UInt32/*Instruction*/), n);
		}

		private static void LoadConstants(LoadState S, Proto f)
		{
			int i,n;
			n = LoadInt(S);
			f.k = LuaMem.luaM_newvector<TValue>(S.L, n);
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
							error(S, "bad constant");
							break;
						}
				}
			}
			n = LoadInt(S);
			f.p = LuaMem.luaM_newvector<Proto>(S.L, n);
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
			f.lineinfo = LuaMem.luaM_newvector<int>(S.L, n);
			f.sizelineinfo = n;
			f.lineinfo = (int[])LoadVector(S, typeof(int), n);
			n = LoadInt(S);
			f.locvars = LuaMem.luaM_newvector<LocVar>(S.L, n);
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
			f.upvalues = LuaMem.luaM_newvector<TString>(S.L, n);
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
				error(S, "code too deep");
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
			/*StkId*/TValue.dec(ref S.L.top);
			S.L.nCcalls--;
			return f;
		}

		private static void LoadHeader(LoadState S)
		{
			CharPtr h = new char[LUAC_HEADERSIZE];
			CharPtr s = new char[LUAC_HEADERSIZE];
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
			if (name[0] == '@' || name[0] == '=')
			{
				S.name = CharPtr.plus(name, 1);
			}
			else if (name[0] == Lua.LUA_SIGNATURE[0])
			{
				S.name = "binary string";
			}
			else
			{
				S.name = name;
			}
			S.L = L;
			S.Z = Z;
			S.b = buff;
			LoadHeader(S);
			return LoadFunction(S, LuaString.luaS_newliteral(L, "=?"));
		}

		/*
		 * make header
		 */
		public static void luaU_header(CharPtr h)
		{
			h = new CharPtr(h);
			int x = 1;
			LuaConf.memcpy(h, Lua.LUA_SIGNATURE, Lua.LUA_SIGNATURE.Length);
			h = h.add(Lua.LUA_SIGNATURE.Length);
			h[0] = (char)LUAC_VERSION;
			h.inc();
			h[0] = (char)LUAC_FORMAT;
			h.inc();
			//*h++=(char)*(char*)&x;				/* endianness */
			h[0] = (char)x;						/* endianness */
			h.inc();
			h[0] = (char)sizeof(int);
			h.inc();
			h[0] = (char)sizeof(uint);
			h.inc();
			h[0] = (char)sizeof(UInt32/*Instruction*/);
			h.inc();
			h[0] = (char)sizeof(Double/*lua_Number*/);
			h.inc();
			//(h++)[0] = ((lua_Number)0.5 == 0) ? 0 : 1;		/* is lua_Number integral? */
			h[0] = (char)0;	// always 0 on this build
		}
	}
}
