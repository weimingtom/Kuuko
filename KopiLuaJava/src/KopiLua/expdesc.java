package KopiLua;

//
// ** $Id: lparser.c,v 2.42.1.3 2007/12/28 15:32:23 roberto Exp $
// ** Lua Parser
// ** See Copyright Notice in lua.h
// 
public class expdesc {
	public static class _u {
		public _s s = new _s();
		public double nval; //Double
 //lua_Number

		public final void Copy(_u u) {
			this.s.Copy(u.s);
			this.nval = u.nval;
		}

		public static class _s {
			public int info, aux;

			public final void Copy(_s s) {
				this.info = s.info;
				this.aux = s.aux;
			}
		}
	}

	public _u u = new _u();

	public int t; // patch list of `exit when true' 
	public int f; // patch list of `exit when false' 

	public expkind k = expkind.forValue(0);

	public final void Copy(expdesc e) {
		this.k = e.k;
		this.u.Copy(e.u);
		this.t = e.t;
		this.f = e.f;
	}
}