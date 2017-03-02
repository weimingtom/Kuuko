package KopiLua;

//
// ** $Id: lopcodes.c,v 1.37.1.1 2007/12/27 13:02:25 roberto Exp $
// ** See Copyright Notice in lua.h
// 
public enum OpMode {
	// basic instruction format 
	iABC,
	iABx,
	iAsBx;

	public int getValue() {
		return this.ordinal();
	}

	public static OpMode forValue(int value) {
		return values()[value];
	}
}