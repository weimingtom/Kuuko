/*
 ** $Id: llex.c,v 2.20.1.1 2007/12/27 13:02:25 roberto Exp $
 ** Lexical Analyzer
 ** See Copyright Notice in lua.h
 */
using System;

namespace KopiLua
{
	public class SemInfo
	{
		public SemInfo() 
		{
			
		}
		
		public SemInfo(SemInfo copy)
		{
			this.r = copy.r;
			this.ts = copy.ts;
		}
		
		public Double/*lua_Number*/ r;
		public TString ts;
	}  /* semantics information */
}
