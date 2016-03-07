/*
 ** $Id: llex.c,v 2.20.1.1 2007/12/27 13:02:25 roberto Exp $
 ** Lexical Analyzer
 ** See Copyright Notice in lua.h
 */
namespace KopiLua
{
	/*
	 * WARNING: if you change the order of this enumeration,
	 * grep "ORDER RESERVED"
	 */
	public enum RESERVED
	{
		/* terminal symbols denoted by reserved words */
		TK_AND = LuaLex.FIRST_RESERVED, TK_BREAK,
		TK_DO, TK_ELSE, TK_ELSEIF, TK_END, TK_FALSE, TK_FOR, TK_FUNCTION,
		TK_IF, TK_IN, TK_LOCAL, TK_NIL, TK_NOT, TK_OR, TK_REPEAT,
		TK_RETURN, TK_THEN, TK_TRUE, TK_UNTIL, TK_WHILE,
		/* other terminal symbols */
		TK_CONCAT, TK_DOTS, TK_EQ, TK_GE, TK_LE, TK_NE, TK_NUMBER,
		TK_NAME, TK_STRING, TK_EOS
	}
}
