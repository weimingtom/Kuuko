/*
 ** $Id: lopcodes.c,v 1.37.1.1 2007/12/27 13:02:25 roberto Exp $
 ** See Copyright Notice in lua.h
 */
namespace KopiLua
{
	/*===========================================================================
      Notes:
      (*) In OP_CALL, if (B == 0) then B = top. C is the number of returns - 1,
    	  and can be 0: OP_CALL then sets `top' to last_result+1, so
    	  next open instruction (OP_CALL, OP_RETURN, OP_SETLIST) may use `top'.
    
      (*) In OP_VARARG, if (B == 0) then use actual number of varargs and
    	  set top (like in OP_CALL with C == 0).
    
      (*) In OP_RETURN, if (B == 0) then return up to `top'
    
      (*) In OP_SETLIST, if (B == 0) then B = `top';
    	  if (C == 0) then next `instruction' is real C
    
      (*) For comparisons, A specifies what condition the test should accept
    	  (true or false).
    
      (*) All `skips' (pc++) assume that next instruction is a jump
    ===========================================================================*/
	
	
	/*
	 ** masks for instruction properties. The format is:
	 ** bits 0-1: op mode
	 ** bits 2-3: C arg mode
	 ** bits 4-5: B arg mode
	 ** bit 6: instruction set register A
	 ** bit 7: operator is a test
	 */
	
	public enum OpArgMask 
	{
		OpArgN,  /* argument is not used */
		OpArgU,  /* argument is used */
		OpArgR,  /* argument is a register or a jump offset */
		OpArgK   /* argument is a constant or register/constant */
	}
}
