namespace KopiLua
{
	public class f_parser_delegate : Pfunc
	{
		public f_parser_delegate()
		{
			
		}
		
		public void exec(lua_State L, object ud)
		{
			LuaDo.f_parser(L, ud);
		}
	}
}
