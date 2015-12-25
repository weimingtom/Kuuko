/*
 ** $Id: lobject.c,v 2.22.1.1 2007/12/27 13:02:25 roberto Exp $
 ** Some generic functions over Lua objects
 ** See Copyright Notice in lua.h
 */
using System;

namespace KopiLua
{
	public class Table : GCObject
	{
		public Byte/*lu_byte*/ flags;  /* 1<<p means tagmethod(p) is not present */
		public Byte/*lu_byte*/ lsizenode;  /* log2 of size of `node' array */
		public Table metatable;
		public TValue[] array;  /* array part */
		public Node[] node;
		public int lastfree;  /* any free position is before this position */
		public GCObject gclist;
		public int sizearray;  /* size of `array' array */
	}
}
