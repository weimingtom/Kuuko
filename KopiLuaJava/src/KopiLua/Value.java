package KopiLua;

//
// ** $Id: lobject.c,v 2.22.1.1 2007/12/27 13:02:25 roberto Exp $
// ** Some generic functions over Lua objects
// ** See Copyright Notice in lua.h
// 
//    
//	 ** Union of all Lua values
//	 
public class Value { //struct ValueCls
	public GCObject gc;
	public Object p;
	public double n; //Double
 //lua_Number
	public int b;

	public Value() {

	}

	public Value(Value copy) {
		this.gc = copy.gc;
		this.p = copy.p;
		this.n = copy.n;
		this.b = copy.b;
	}

	public final void copyFrom(Value copy) {
		this.gc = copy.gc;
		this.p = copy.p;
		this.n = copy.n;
		this.b = copy.b;
	}
}
 //struct ValueCls