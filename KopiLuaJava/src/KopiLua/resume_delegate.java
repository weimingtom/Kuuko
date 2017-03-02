package KopiLua;

public class resume_delegate implements Pfunc {
	public final void exec(lua_State L, Object ud) {
		LuaDo.resume(L, ud);
	}
}