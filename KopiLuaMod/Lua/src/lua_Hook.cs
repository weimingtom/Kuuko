namespace KopiLua
{
    /* Functions to be called by the debuger in specific events */
    //public delegate void lua_Hook(lua_State L, lua_Debug ar);
    public interface lua_Hook
    {
        void exec(lua_State L, lua_Debug ar);
    }
}
