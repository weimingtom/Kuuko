/*
 ** $Id: ldo.c,v 2.38.1.3 2008/01/18 22:31:22 roberto Exp $
 ** Stack and Call structure of Lua
 ** See Copyright Notice in lua.h
 */
namespace KopiLua
{
	/*
	 ** {======================================================
	 ** Error-recovery functions
	 ** =======================================================
	 */
	/* chain list of long jump buffers */
	public class lua_longjmp
	{
		public lua_longjmp previous;
		public luai_jmpbuf b;
		public volatile int status;  /* error code */
	}
}
