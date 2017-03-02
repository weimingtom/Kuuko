package KopiLua;

//
// ** $Id: lobject.c,v 2.22.1.1 2007/12/27 13:02:25 roberto Exp $
// ** Some generic functions over Lua objects
// ** See Copyright Notice in lua.h
// 
//    
//	 ** Upvalues
//	 
public class UpVal extends GCObject {
	public static class _u {
		public static class _l {
			// double linked list (when open) 
			public UpVal prev;
			public UpVal next;
		}

		public TValue value = new TValue(); // the value (when closed) 
		public _l l = new _l();
	}
	public _u u = new _u(); //new

	public TValue v; // points to stack or to its own value 
}