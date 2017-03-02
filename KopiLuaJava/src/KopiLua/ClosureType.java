package KopiLua;

//
// ** $Id: lobject.c,v 2.22.1.1 2007/12/27 13:02:25 roberto Exp $
// ** Some generic functions over Lua objects
// ** See Copyright Notice in lua.h
// 
public class ClosureType {
	private ClosureHeader header;

	//implicit operator ClosureHeader
	public static ClosureHeader toClosureHeader(ClosureType ctype) {
		return ctype.header;
	}

	public ClosureType(ClosureHeader header) {
		this.header = header;
	}

	//Byte
 //lu_byte
	public final byte getIsC() {
		return header.isC;
	}

	//Byte
 //lu_byte
	public final void setIsC(byte val) {
		header.isC = val;
	}

	//Byte
	//lu_byte
	public final byte getNupvalues() {
		return header.nupvalues;
	}

	//Byte
	//lu_byte
	public final void setNupvalues(byte val) {
		header.nupvalues = val;
	}

	public final GCObject getGclist() {
		return header.gclist;
	}

	public final void setGclist(GCObject val) {
		header.gclist = val;
	}

	public final Table getEnv() {
		return header.env;
	}

	public final void setEnv(Table val) {
		header.env = val;
	}
}