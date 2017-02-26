/*
 ** $Id: lstate.c,v 2.36.1.2 2008/01/03 15:20:39 roberto Exp $
 ** Global State
 ** See Copyright Notice in lua.h
 */
namespace KopiLua
{
	public class stringtable
	{
		public GCObject[] hash;
		public long/*UInt32*//*lu_mem*/ nuse;  /* number of elements */
		public int size;
	}
}
