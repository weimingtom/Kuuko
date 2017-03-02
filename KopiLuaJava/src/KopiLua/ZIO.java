package KopiLua;

//
// ** $Id: lzio.c,v 1.31.1.1 2007/12/27 13:02:25 roberto Exp $
// ** a generic input stream interface
// ** See Copyright Notice in lua.h
// 
public class ZIO { //Zio
	public int n; //uint
		// bytes still unread 
	public CharPtr p; // current position in buffer 
	public lua_Reader reader;
	public Object data; // additional data 
	public lua_State L; // Lua state (for reader) 

	//public class ZIO : Zio { };
}
 //Zio
