/*
 ** $Id: ldo.c,v 2.38.1.3 2008/01/18 22:31:22 roberto Exp $
 ** Stack and Call structure of Lua
 ** See Copyright Notice in lua.h
 */
namespace KopiLua
{
	/*
	 ** Execute a protected parser.
	 */
	public class SParser
	{
		/* data to `f_parser' */
		public ZIO z;
		public Mbuffer buff = new Mbuffer();  /* buffer to be used by the scanner */
		public CharPtr name;
	}
}
