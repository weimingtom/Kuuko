namespace KopiLua
{
    public interface luaL_opt_delegate
    {
        /*Double*/
        /*lua_Number*/
        double exec(lua_State L, int narg);
    }
}
