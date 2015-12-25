/*
 ** $Id: lapi.c,v 2.55.1.5 2008/07/04 18:41:18 roberto Exp $
 ** Lua API
 ** See Copyright Notice in lua.h
 */
namespace KopiLua
{
	/*
	 ** Execute a protected call.
	 */
	public class CallS
	{
		/* data to `f_call' */
		public TValue func; /*StkId*/
		public int nresults;
	}
}
