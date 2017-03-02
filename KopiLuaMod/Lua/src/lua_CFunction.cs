namespace KopiLua
{
    public interface lua_CFunction
    {
        int exec(lua_State L);
    }
}
