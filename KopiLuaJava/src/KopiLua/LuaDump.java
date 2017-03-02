package KopiLua;

//
// ** $Id: ldump.c,v 2.8.1.1 2007/12/27 13:02:25 roberto Exp $
// ** save precompiled Lua chunks
// ** See Copyright Notice in lua.h
// 

//using lua_Number = System.Double;
//using TValue = Lua.TValue;

public class LuaDump {
	public static void DumpMem(Object b, DumpState D, ClassType t) {
		byte[] bytes = t.ObjToBytes(b);
		char[] ch = new char[bytes.length];
		for (int i = 0; i < bytes.length; i++) {
			ch[i] = (char)bytes[i];
		}
		CharPtr str = CharPtr.toCharPtr(ch);
		DumpBlock(str, str.chars.length, D); //(uint)
	}

	public static void DumpMem_int(int[] b, int n, DumpState D, ClassType t) {
		ClassType.Assert(b.length == n);
		for (int i = 0; i < n; i++) {
			DumpMem(b[i], D, t);
		}
	}

	public static void DumpMem_long(long[] b, int n, DumpState D, ClassType t) {
		ClassType.Assert(b.length == n);
		for (int i = 0; i < n; i++) {
			DumpMem(b[i], D, t);
		}
	}

	public static void DumpVar(Object x, DumpState D, ClassType t) {
		DumpMem(x, D, t);
	}

	private static void DumpBlock(CharPtr b, int size, DumpState D) { //uint
		if (D.status == 0) {
			LuaLimits.lua_unlock(D.L);
			D.status = D.writer.exec(D.L, b, size, D.data);
			LuaLimits.lua_lock(D.L);
		}
	}

	private static void DumpChar(int y, DumpState D) {
		char x = (char)y;
		DumpVar(x, D, new ClassType(ClassType.TYPE_CHAR));
	}

	private static void DumpInt(int x, DumpState D) {
		DumpVar(x, D, new ClassType(ClassType.TYPE_INT));
	}

	private static void DumpNumber(double x, DumpState D) { //lua_Number
		DumpVar(x, D, new ClassType(ClassType.TYPE_DOUBLE));
	}

	private static void DumpVector_int(int[] b, int n, DumpState D, ClassType t) {
		DumpInt(n, D);
		DumpMem_int(b, n, D, t);
	}

	private static void DumpVector_long(long[] b, int n, DumpState D, ClassType t) {
		DumpInt(n, D);
		DumpMem_long(b, n, D, t);
	}

	private static void DumpString(TString s, DumpState D) {
		if (s == null || CharPtr.isEqual(LuaObject.getstr(s), '\0')) {
			int size = 0; //uint
			DumpVar(size, D, new ClassType(ClassType.TYPE_INT));
		}
		else {
			int size = s.getTsv().len + 1; // include trailing '\0'  - uint
			DumpVar(size, D, new ClassType(ClassType.TYPE_INT));
			DumpBlock(LuaObject.getstr(s), size, D);
		}
	}

	private static void DumpCode(Proto f, DumpState D) {
		DumpVector_long(f.code, f.sizecode, D, new ClassType(ClassType.TYPE_LONG));
	}

	private static void DumpConstants(Proto f, DumpState D) {
		int i, n = f.sizek;
		DumpInt(n, D);
		for (i = 0; i < n; i++) {
			//const
 TValue o = f.k[i];
			DumpChar(LuaObject.ttype(o), D);
			switch (LuaObject.ttype(o)) {
				case Lua.LUA_TNIL: {
						break;
					}
				case Lua.LUA_TBOOLEAN: {
						DumpChar(LuaObject.bvalue(o), D);
						break;
					}
				case Lua.LUA_TNUMBER: {
						DumpNumber(LuaObject.nvalue(o), D);
						break;
					}
				case Lua.LUA_TSTRING: {
						DumpString(LuaObject.rawtsvalue(o), D);
						break;
					}
				default: {
						LuaLimits.lua_assert(0); // cannot happen 
						break;
					}
			}
		}
		n = f.sizep;
		DumpInt(n, D);
		for (i = 0; i < n; i++) {
			DumpFunction(f.p[i], f.source, D);
		}
	}

	private static void DumpDebug(Proto f, DumpState D) {
		int i,n;
		n = (D.strip != 0) ? 0 : f.sizelineinfo;
		DumpVector_int(f.lineinfo, n, D, new ClassType(ClassType.TYPE_INT));
		n = (D.strip != 0) ? 0 : f.sizelocvars;
		DumpInt(n, D);
		for (i = 0; i < n; i++) {
			DumpString(f.locvars[i].varname, D);
			DumpInt(f.locvars[i].startpc, D);
			DumpInt(f.locvars[i].endpc, D);
		}
		n = (D.strip != 0) ? 0 : f.sizeupvalues;
		DumpInt(n, D);
		for (i = 0; i < n; i++) {
			DumpString(f.upvalues[i], D);
		}
	}

	private static void DumpFunction(Proto f, TString p, DumpState D) {
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

	private static void DumpHeader(DumpState D) {
		CharPtr h = CharPtr.toCharPtr(new char[LuaUndump.LUAC_HEADERSIZE]);
		LuaUndump.luaU_header(h);
		DumpBlock(h, LuaUndump.LUAC_HEADERSIZE, D);
	}

//        
//		 ** dump Lua function as precompiled chunk
//		 
	public static int luaU_dump(lua_State L, Proto f, lua_Writer w, Object data, int strip) {
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