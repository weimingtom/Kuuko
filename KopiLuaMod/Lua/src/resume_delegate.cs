namespace KopiLua
{
	public class resume_delegate : Pfunc
	{
		public void exec(lua_State L, object ud)
		{
			LuaDo.resume(L, ud);
		}
	}
}
