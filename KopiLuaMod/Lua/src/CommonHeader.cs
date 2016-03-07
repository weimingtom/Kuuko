/*
 ** $Id: lobject.c,v 2.22.1.1 2007/12/27 13:02:25 roberto Exp $
 ** Some generic functions over Lua objects
 ** See Copyright Notice in lua.h
 */
namespace KopiLua
{
    /*
     ** Common Header for all collectable objects (in macro form, to be
     ** included in other objects)
     */
    public class CommonHeader
    {
    	public GCObject next;
    	public byte tt; /*Byte*/ /*lu_byte*/
    	public byte marked; /*Byte*/ /*lu_byte*/
    }
}
