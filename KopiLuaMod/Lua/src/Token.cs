/*
 ** $Id: llex.c,v 2.20.1.1 2007/12/27 13:02:25 roberto Exp $
 ** Lexical Analyzer
 ** See Copyright Notice in lua.h
 */
using System;

namespace KopiLua
{
	public class Token
	{
		public Token()
		{
			
		}
		
		public Token(Token copy)
		{
			this.token = copy.token;
			this.seminfo = new SemInfo(copy.seminfo);
		}
		
		public int token;
		public SemInfo seminfo = new SemInfo();
	}
}
