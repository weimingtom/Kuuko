package KopiLua;

//
// ** $Id: lcode.c,v 2.25.1.3 2007/12/28 15:32:23 roberto Exp $
// ** Code generator for Lua
// ** See Copyright Notice in lua.h
// 
public enum UnOpr {
	OPR_MINUS,
	OPR_NOT,
	OPR_LEN,
	OPR_NOUNOPR;

	public int getValue() {
		return this.ordinal();
	}

	public static UnOpr forValue(int value) {
		return values()[value];
	}
}