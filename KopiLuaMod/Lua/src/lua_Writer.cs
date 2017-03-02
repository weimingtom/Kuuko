namespace KopiLua
{
    // functions that read/write blocks when loading/dumping Lua chunks
	//public delegate int lua_Writer(lua_State L, CharPtr p, int//uint// sz, object ud);
	public interface lua_Writer
    {
        //uint sz
        int exec(lua_State L, CharPtr p, int sz, object ud);
    }
}
