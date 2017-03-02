namespace KopiLua
{
    public interface lua_Reader
    {
        /*sz*/
        /*out*/
        /*uint*/
        CharPtr exec(lua_State L, object ud, int[] sz);
    }
}
