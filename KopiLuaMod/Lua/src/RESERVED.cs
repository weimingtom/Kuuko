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
	public class RESERVED
	{
		/* terminal symbols denoted by reserved words */
		public const int TK_AND = LuaLex.FIRST_RESERVED;
        public const int TK_BREAK = LuaLex.FIRST_RESERVED + 1;
		public const int TK_DO = LuaLex.FIRST_RESERVED + 2; 
        public const int TK_ELSE = LuaLex.FIRST_RESERVED + 3;
        public const int TK_ELSEIF = LuaLex.FIRST_RESERVED + 4;
        public const int TK_END = LuaLex.FIRST_RESERVED + 5;
        public const int TK_FALSE = LuaLex.FIRST_RESERVED + 6;
        public const int TK_FOR = LuaLex.FIRST_RESERVED + 7;
        public const int TK_FUNCTION = LuaLex.FIRST_RESERVED + 8;
		public const int TK_IF = LuaLex.FIRST_RESERVED + 9;
        public const int TK_IN = LuaLex.FIRST_RESERVED + 10;
        public const int TK_LOCAL = LuaLex.FIRST_RESERVED + 11;
        public const int TK_NIL = LuaLex.FIRST_RESERVED + 12;
        public const int TK_NOT = LuaLex.FIRST_RESERVED + 13;
        public const int TK_OR = LuaLex.FIRST_RESERVED + 14;
        public const int TK_REPEAT = LuaLex.FIRST_RESERVED + 15;
		public const int TK_RETURN = LuaLex.FIRST_RESERVED + 16;
        public const int TK_THEN = LuaLex.FIRST_RESERVED + 17;
        public const int TK_TRUE = LuaLex.FIRST_RESERVED + 18;
        public const int TK_UNTIL = LuaLex.FIRST_RESERVED + 19;
        public const int TK_WHILE = LuaLex.FIRST_RESERVED + 20;
		/* other terminal symbols */
		public const int TK_CONCAT = LuaLex.FIRST_RESERVED + 21;
        public const int TK_DOTS = LuaLex.FIRST_RESERVED + 22;
        public const int TK_EQ = LuaLex.FIRST_RESERVED + 23;
        public const int TK_GE = LuaLex.FIRST_RESERVED + 24;
        public const int TK_LE = LuaLex.FIRST_RESERVED + 25;
        public const int TK_NE = LuaLex.FIRST_RESERVED + 26;
        public const int TK_NUMBER = LuaLex.FIRST_RESERVED + 27;
		public const int TK_NAME = LuaLex.FIRST_RESERVED + 28;
        public const int TK_STRING = LuaLex.FIRST_RESERVED + 29;
        public const int TK_EOS = LuaLex.FIRST_RESERVED + 30;
	}
}
