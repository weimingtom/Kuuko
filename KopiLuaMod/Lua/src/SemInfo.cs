/*
 ** $Id: llex.c,v 2.20.1.1 2007/12/27 13:02:25 roberto Exp $
 ** Lexical Analyzer
 ** See Copyright Notice in lua.h
 */
namespace KopiLua
{
	public class SemInfo
	{
		public double r;  /*Double*/ /*lua_Number*/
		public TString ts;
		
		public SemInfo() 
		{
			
		}
		
		public SemInfo(SemInfo copy)
		{
			this.r = copy.r;
			this.ts = copy.ts;
		}
	}  /* semantics information */
}
