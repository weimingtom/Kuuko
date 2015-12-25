/*
 ** $Id: lzio.c,v 1.31.1.1 2007/12/27 13:02:25 roberto Exp $
 ** a generic input stream interface
 ** See Copyright Notice in lua.h
 */
using System;

namespace KopiLua
{
	public class ZIO//Zio
	{
		public int/*uint*/ n;			/* bytes still unread */
		public CharPtr p;			/* current position in buffer */
		public lua_Reader reader;
		public object data;			/* additional data */
		public lua_State L;			/* Lua state (for reader) */
		
		//public class ZIO : Zio { };
	}
}

