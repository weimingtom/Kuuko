/*
 ** $Id: lobject.c,v 2.22.1.1 2007/12/27 13:02:25 roberto Exp $
 ** Some generic functions over Lua objects
 ** See Copyright Notice in lua.h
 */
namespace KopiLua
{
	//typedef TValue *StkId;  /* index to stack elements */

	/*
	 ** String headers for string table
	 */
	public class TString_tsv : GCObject
	{
		public byte reserved;  /*Byte*/ /*lu_byte*/
		/*FIXME:*/
		public long hash; /*int*//*uint*/
		public int len; /*uint*/
	}
}
