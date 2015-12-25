/*
 ** $Id: lcode.c,v 2.25.1.3 2007/12/28 15:32:23 roberto Exp $
 ** Code generator for Lua
 ** See Copyright Notice in lua.h
 */
namespace KopiLua
{
	/*
	 ** grep "ORDER OPR" if you change these enums
	 */
	public enum BinOpr
	{
		OPR_ADD, 
		OPR_SUB, 
		OPR_MUL, 
		OPR_DIV, 
		OPR_MOD, 
		OPR_POW,
		OPR_CONCAT,
		OPR_NE, 
		OPR_EQ,
		OPR_LT, 
		OPR_LE, 
		OPR_GT, 
		OPR_GE,
		OPR_AND, 
		OPR_OR,
		OPR_NOBINOPR
	}
}
