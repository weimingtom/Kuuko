package KopiLua;

//
// ** $Id: lstate.c,v 2.36.1.2 2008/01/03 15:20:39 roberto Exp $
// ** Global State
// ** See Copyright Notice in lua.h
// 
//    
//	 ** Union of all collectable objects (not a union anymore in the C# port)
//	 
public class GCObject extends GCheader implements ArrayElement {
	// todo: remove this?
	//private GCObject[] values = null;
	//private int index = -1;

	public final void set_index(int index) {
		//this.index = index;
	}

	public final void set_array(Object array) {
		//this.values = (GCObject[])array;
		//ClassType.Assert(this.values != null);
	}

	public final GCheader getGch() {
		return (GCheader)this;
	}

	public final TString getTs() {
		return (TString)this;
	}

	public final Udata getU() {
		return (Udata)this;
	}

	public final Closure getCl() {
		return (Closure)this;
	}

	public final Table getH() {
		return (Table)this;
	}

	public final Proto getP() {
		return (Proto)this;
	}

	public final UpVal getUv() {
		return (UpVal)this;
	}

	public final lua_State getTh() {
		return (lua_State)this;
	}
}