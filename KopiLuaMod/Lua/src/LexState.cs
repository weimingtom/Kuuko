/*
 ** $Id: llex.c,v 2.20.1.1 2007/12/27 13:02:25 roberto Exp $
 ** Lexical Analyzer
 ** See Copyright Notice in lua.h
 */
namespace KopiLua
{
	public class LexState
	{
		public int current;  /* current character (charint) */
		public int linenumber;  /* input line counter */
		public int lastline;  /* line of last token `consumed' */
		public Token t = new Token();  /* current token */
		public Token lookahead = new Token();  /* look ahead token */
		public FuncState fs;  /* `FuncState' is private to the parser */
		public lua_State L;
		public ZIO z;  /* input stream */
		public Mbuffer buff;  /* buffer for tokens */
		public TString source;  /* current source name */
		public char decpoint;  /* locale decimal point */
	}
}
