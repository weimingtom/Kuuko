/*
 ** $Id: lobject.c,v 2.22.1.1 2007/12/27 13:02:25 roberto Exp $
 ** Some generic functions over Lua objects
 ** See Copyright Notice in lua.h
 */
namespace KopiLua
{
    /*
     ** Closures
     */ 
    public class ClosureHeader : GCObject
    {
    	public byte isC; /*Byte*/ /*lu_byte*/
    	public byte nupvalues; /*Byte*/ /*lu_byte*/
    	public GCObject gclist;
    	public Table env;
    }
}
