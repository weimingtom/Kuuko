/*
 ** $Id: lobject.c,v 2.22.1.1 2007/12/27 13:02:25 roberto Exp $
 ** Some generic functions over Lua objects
 ** See Copyright Notice in lua.h
 */
namespace KopiLua
{
	public class Udata : Udata_uv
	{
		public /*new*/ Udata_uv uv;
		
		//public L_Umaxalign dummy;  /* ensures maximum alignment for `local' udata */
		
		// in the original C code this was allocated alongside the structure memory. it would probably
		// be possible to still do that by allocating memory and pinning it down, but we can do the
		// same thing just as easily by allocating a seperate byte array for it instead.
		public object user_data;
		
		public Udata() 
		{ 
			this.uv = this; 
		}
	}
}
