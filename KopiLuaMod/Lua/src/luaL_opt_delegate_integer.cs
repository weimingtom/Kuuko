namespace KopiLua
{
    public interface luaL_opt_delegate_integer
    {
        /*Int32*/
        /*lua_Integer*/
        int exec(lua_State L, int narg);
    }
}
