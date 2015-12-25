/*
 ** $Id: lobject.c,v 2.22.1.1 2007/12/27 13:02:25 roberto Exp $
 ** Some generic functions over Lua objects
 ** See Copyright Notice in lua.h
 */
using System;

namespace KopiLua
{
	public class Closure : ClosureHeader
	{
		public Closure()
		{
			c = new CClosure(this);
			l = new LClosure(this);
		}
	
		public CClosure c;
		public LClosure l;
	}
}
