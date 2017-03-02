package KopiLua;

//
// ** $Id: lundump.c,v 2.7.1.4 2008/04/04 19:51:41 roberto Exp $
// ** load precompiled Lua chunks
// ** See Copyright Notice in lua.h
// 
public class LoadState {
	public lua_State L;
	public ZIO Z;
	public Mbuffer b;
	public CharPtr name;
}