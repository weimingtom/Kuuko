package KopiLua;

//
// ** $Id: lstate.c,v 2.36.1.2 2008/01/03 15:20:39 roberto Exp $
// ** Global State
// ** See Copyright Notice in lua.h
// 
public class stringtable {
	public GCObject[] hash;
	public long nuse; // number of elements  - lu_mem - UInt32
	public int size;
}