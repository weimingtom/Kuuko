/*
 ** $Id: lparser.c,v 2.42.1.3 2007/12/28 15:32:23 roberto Exp $
 ** Lua Parser
 ** See Copyright Notice in lua.h
 */
using System;

namespace KopiLua
{
	public class priority_
	{
		public priority_(Byte/*lu_byte*/ left, Byte/*lu_byte*/ right)
		{
			this.left = left;
			this.right = right;
		}
		
		public Byte/*lu_byte*/ left;  /* left priority for each binary operator */
		public Byte/*lu_byte*/ right; /* right priority */
	}
}
