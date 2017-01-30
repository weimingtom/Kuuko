/*
 ** $Id: lobject.c,v 2.22.1.1 2007/12/27 13:02:25 roberto Exp $
 ** Some generic functions over Lua objects
 ** See Copyright Notice in lua.h
 */
namespace KopiLua
{
	public class Closure : ClosureHeader
	{	
		public CClosure c;
		public LClosure l;
		
		public Closure()
		{
			c = new CClosure(this);
			l = new LClosure(this);
		}
	}
}
