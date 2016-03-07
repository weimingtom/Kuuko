﻿/*
 ** $Id: lobject.c,v 2.22.1.1 2007/12/27 13:02:25 roberto Exp $
 ** Some generic functions over Lua objects
 ** See Copyright Notice in lua.h
 */
namespace KopiLua
{
	/*
	 ** Union of all Lua values
	 */
	public struct Value
	{
		public GCObject gc;
		public object p;
		public double n;  /*Double*/ /*lua_Number*/
		public int b;
	}
}
