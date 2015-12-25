/*
 ** $Id: ldump.c,v 2.8.1.1 2007/12/27 13:02:25 roberto Exp $
 ** save precompiled Lua chunks
 ** See Copyright Notice in lua.h
 */
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace KopiLua
{
	//using lua_Number = System.Double;
	//using TValue = Lua.TValue;

	public class LuaDump
	{
		public static void DumpMem(object b, DumpState D)
		{
			int size = Marshal.SizeOf(b);
			IntPtr ptr = Marshal.AllocHGlobal(size);
			Marshal.StructureToPtr(b, ptr, false);
			byte[] bytes = new byte[size];
			Marshal.Copy(ptr, bytes, 0, size);
			char[] ch = new char[bytes.Length];
			for (int i = 0; i < bytes.Length; i++)
			{
				ch[i] = (char)bytes[i];
			}
			CharPtr str = ch;
			DumpBlock(str, /*(uint)*/str.chars.Length, D);
		}

		public static void DumpMem(object b, int n, DumpState D)
		{
			Array array = b as Array;
			Debug.Assert(array.Length == n);
			for (int i = 0; i < n; i++)
			{
				DumpMem(array.GetValue(i), D);
			}
		}

		public static void DumpVar(object x, DumpState D)
		{
			DumpMem(x, D);
		}

		private static void DumpBlock(CharPtr b, int/*uint*/ size, DumpState D)
		{
			if (D.status == 0)
			{
				LuaLimits.lua_unlock(D.L);
				D.status = D.writer(D. L, b, size, D.data);
				LuaLimits.lua_lock(D.L);
			}
		}

		private static void DumpChar(int y, DumpState D)
		{
			char x = (char)y;
			DumpVar(x,D);
		}

		private static void DumpInt(int x, DumpState D)
		{
			DumpVar(x,D);
		}

		private static void DumpNumber(Double/*lua_Number*/ x, DumpState D)
		{
			DumpVar(x,D);
		}

		static void DumpVector(object b, int n, DumpState D)
		{
			DumpInt(n,D);
			DumpMem(b, n, D);
		}

		private static void DumpString(TString s, DumpState D)
		{
			if (s == null || LuaObject.getstr(s) == null)
			{
				int/*uint*/ size = 0;
				DumpVar(size,D);
			}
			else
			{
				int/*uint*/ size = s.tsv.len + 1;		/* include trailing '\0' */
				DumpVar(size,D);
				DumpBlock(LuaObject.getstr(s), size, D);
			}
		}

		private static void DumpCode(Proto f,DumpState D)
		{
			DumpVector(f.code, f.sizecode, D);
		}

		private static void DumpConstants(Proto f, DumpState D)
		{
			int i, n = f.sizek;
			DumpInt(n, D);
			for (i = 0; i < n; i++)
			{
				/*const*/ TValue o=f.k[i];
				DumpChar(LuaObject.ttype(o), D);
				switch (LuaObject.ttype(o))
				{
					case Lua.LUA_TNIL:
						{
							break;
						}
					case Lua.LUA_TBOOLEAN:
						{
							DumpChar(LuaObject.bvalue(o), D);
							break;
						}
					case Lua.LUA_TNUMBER:
						{
							DumpNumber(LuaObject.nvalue(o), D);
							break;
						}
					case Lua.LUA_TSTRING:
						{
							DumpString(LuaObject.rawtsvalue(o), D);
							break;
						}
					default:
						{
							LuaLimits.lua_assert(0);			/* cannot happen */
							break;
						}
				}
			}
			n = f.sizep;
			DumpInt(n, D);
			for (i = 0; i < n; i++) 
			{
				DumpFunction(f.p[i],f.source,D);
			}
		}

		private static void DumpDebug(Proto f, DumpState D)
		{
			int i,n;
			n = (D.strip != 0) ? 0 : f.sizelineinfo;
			DumpVector(f.lineinfo, n, D);
			n = (D.strip != 0) ? 0 : f.sizelocvars;
			DumpInt(n, D);
			for (i = 0; i < n; i++)
			{
				DumpString(f.locvars[i].varname, D);
				DumpInt(f.locvars[i].startpc, D);
				DumpInt(f.locvars[i].endpc, D);
			}
			n = (D.strip != 0) ? 0 : f.sizeupvalues;
			DumpInt(n, D);
			for (i = 0; i < n; i++) 
			{
				DumpString(f.upvalues[i], D);
			}
		}

		private static void DumpFunction(Proto f, TString p, DumpState D)
		{
			DumpString(((f.source == p) || (D.strip != 0)) ? null : f.source, D);
			DumpInt(f.linedefined, D);
			DumpInt(f.lastlinedefined, D);
			DumpChar(f.nups, D);
			DumpChar(f.numparams, D);
			DumpChar(f.is_vararg, D);
			DumpChar(f.maxstacksize, D);
			DumpCode(f, D);
			DumpConstants(f, D);
			DumpDebug(f, D);
		}

		private static void DumpHeader(DumpState D)
		{
			CharPtr h = new char[LuaUndump.LUAC_HEADERSIZE];
			LuaUndump.luaU_header(h);
			DumpBlock(h, LuaUndump.LUAC_HEADERSIZE, D);
		}

		/*
		 ** dump Lua function as precompiled chunk
		 */
		public static int luaU_dump (lua_State L, Proto f, lua_Writer w, object data, int strip)
		{
			DumpState D = new DumpState();
			D.L = L;
			D.writer = w;
			D.data = data;
			D.strip = strip;
			D.status = 0;
			DumpHeader(D);
			DumpFunction(f, null, D);
			return D.status;
		}
	}
}
