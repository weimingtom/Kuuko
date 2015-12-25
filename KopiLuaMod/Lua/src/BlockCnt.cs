/*
 ** $Id: lparser.c,v 2.42.1.3 2007/12/28 15:32:23 roberto Exp $
 ** Lua Parser
 ** See Copyright Notice in lua.h
 */
namespace KopiLua
{
    /*
    ** nodes for block list (list of active blocks)
    */
    public class BlockCnt
    {
        public BlockCnt previous;  /* chain */
        public int breaklist;  /* list of jumps out of this loop */
        public byte nactvar;  /*Byte lu_byte*/ /* # active locals outside the breakable structure */
        public byte upval;  /*Byte lu_byte*/ /* true if some variable in the block is an upvalue */
        public byte isbreakable;  /*Bytelu_byte*/ /* true if `block' is a loop */
    }
}
