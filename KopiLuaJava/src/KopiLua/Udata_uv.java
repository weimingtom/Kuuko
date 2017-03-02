package KopiLua;

//
// ** $Id: lobject.c,v 2.22.1.1 2007/12/27 13:02:25 roberto Exp $
// ** Some generic functions over Lua objects
// ** See Copyright Notice in lua.h
// 
public class Udata_uv extends GCObject {
	public Table metatable;
	public Table env;
	public int len; //uint
}