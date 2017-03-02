package KopiLua;

//
// ** $Id: lobject.c,v 2.22.1.1 2007/12/27 13:02:25 roberto Exp $
// ** Some generic functions over Lua objects
// ** See Copyright Notice in lua.h
// 
public class TString extends TString_tsv {
	public CharPtr str;

	//public L_Umaxalign dummy;  /* ensures maximum alignment for strings */

	public final TString_tsv getTsv() {
		return this;
	}

	public TString() {

	}

	public TString(CharPtr str) {
		this.str = str;
	}

	@Override
	public String toString() {
		return str.toString();
	} // for debugging
}