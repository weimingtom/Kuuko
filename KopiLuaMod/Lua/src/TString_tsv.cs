/*
 ** $Id: lobject.c,v 2.22.1.1 2007/12/27 13:02:25 roberto Exp $
 ** Some generic functions over Lua objects
 ** See Copyright Notice in lua.h
 */
using System;

namespace KopiLua
{
	//typedef TValue *StkId;  /* index to stack elements */

	/*
	 ** String headers for string table
	 */
	public class TString_tsv : GCObject
	{
		public Byte/*lu_byte*/ reserved;
		/*FIXME:*/
		public long/*int*//*uint*/ hash;
		public int/*uint*/ len;
	}
}
