/*
 ** $Id: lapi.c,v 2.55.1.5 2008/07/04 18:41:18 roberto Exp $
 ** Lua API
 ** See Copyright Notice in lua.h
 */
namespace KopiLua
{
	/*
	 ** Execute a protected C call.
	 */
	public class CCallS
	{
		/* data to `f_Ccall' */
		public lua_CFunction func;
		public object ud;
	}
}
