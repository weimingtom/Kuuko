package KopiLua;

//
// ** $Id: lstate.c,v 2.36.1.2 2008/01/03 15:20:39 roberto Exp $
// ** Global State
// ** See Copyright Notice in lua.h
// 
//    
//	 ** Main thread combines a thread state and the global state
//	 
public class LG extends lua_State {
	public global_State g = new global_State();

	public final lua_State getL() {
		return this;
	}
}