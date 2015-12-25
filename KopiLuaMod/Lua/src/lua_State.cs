﻿/*
 ** $Id: lstate.c,v 2.36.1.2 2008/01/03 15:20:39 roberto Exp $
 ** Global State
 ** See Copyright Notice in lua.h
 */
using System;

namespace KopiLua
{
	/*
	 ** `per thread' state
	 */
	public class lua_State : GCObject
	{
		public Byte/*lu_byte*/ status;
		public TValue/*StkId*/ top;  /* first free slot in the stack */
		public TValue/*StkId*/ base_;  /* base of current function */
		public global_State l_G;
		public CallInfo ci;  /* call info for current function */
		public InstructionPtr savedpc = new InstructionPtr();  /* `savedpc' of current function */
		public TValue/*StkId*/ stack_last;  /* last free slot in the stack */
		public TValue[]/*StkId[]*/ stack;  /* stack base */
		public CallInfo end_ci;  /* points after end of ci array*/
		public CallInfo[] base_ci;  /* array of CallInfo's */
		public int stacksize;
		public int size_ci;  /* size of array `base_ci' */
		public ushort nCcalls;  /* number of nested C calls */
		public ushort baseCcalls;  /* nested C calls when resuming coroutine */
		public Byte/*lu_byte*/ hookmask;
		public Byte/*lu_byte*/ allowhook;
		public int basehookcount;
		public int hookcount;
		public lua_Hook hook;
		public TValue l_gt = new TValue();  /* table of globals */
		public TValue env = new TValue();  /* temporary place for environments */
		public GCObject openupval;  /* list of open upvalues in this stack */
		public GCObject gclist;
		public lua_longjmp errorJmp;  /* current error recover point */
		public Int32/*ptrdiff_t*/ errfunc;  /* current error handling function (stack index) */
	}
}
