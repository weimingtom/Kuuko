/*
** $Id: lparser.c,v 2.42.1.3 2007/12/28 15:32:23 roberto Exp $
** Lua Parser
** See Copyright Notice in lua.h
*/
namespace KopiLua
{
    public class ConsControl
    {
        public expdesc v = new expdesc();  /* last list item read */
        public expdesc t;  /* table descriptor */
        public int nh;  /* total number of `record' elements */
        public int na;  /* total number of array elements */
        public int tostore;  /* number of array elements pending to be stored */
    }
}
