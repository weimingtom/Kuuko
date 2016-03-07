/*
** $Id: ldump.c,v 2.8.1.1 2007/12/27 13:02:25 roberto Exp $
** save precompiled Lua chunks
** See Copyright Notice in lua.h
*/
namespace KopiLua
{
    public class DumpState
    {
        public lua_State L;
        public lua_Writer writer;
        public object data;
        public int strip;
        public int status;
    }
}
