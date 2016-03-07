/*
 ** $Id: lobject.c,v 2.22.1.1 2007/12/27 13:02:25 roberto Exp $
 ** Some generic functions over Lua objects
 ** See Copyright Notice in lua.h
 */
namespace KopiLua
{
	public class LocVar
	{
		public TString varname;
		public int startpc;  /* first point where variable is active */
		public int endpc;    /* first point where variable is dead */
	}
}
