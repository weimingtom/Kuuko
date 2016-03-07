﻿/*
 ** $Id: ltm.c,v 2.8.1.1 2007/12/27 13:02:25 roberto Exp $
 ** Tag methods
 ** See Copyright Notice in lua.h
 */
namespace KopiLua
{
	/*
	 * WARNING: if you change the order of this enumeration,
	 * grep "ORDER TM"
	 */
	public enum TMS
	{
		TM_INDEX,
		TM_NEWINDEX,
		TM_GC,
		TM_MODE,
		TM_EQ,  /* last tag method with `fast' access */
		TM_ADD,
		TM_SUB,
		TM_MUL,
		TM_DIV,
		TM_MOD,
		TM_POW,
		TM_UNM,
		TM_LEN,
		TM_LT,
		TM_LE,
		TM_CONCAT,
		TM_CALL,
		TM_N		/* number of elements in the enum */
	}
}
