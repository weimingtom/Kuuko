/*
 ** $Id: lstate.c,v 2.36.1.2 2008/01/03 15:20:39 roberto Exp $
 ** Global State
 ** See Copyright Notice in lua.h
 */
using System;

namespace KopiLua
{
	/*
	 ** Main thread combines a thread state and the global state
	 */
	public class LG : lua_State
	{
		public lua_State l 
		{ 
			get 
			{ 
				return this; 
			} 
		}
		
		public global_State g = new global_State();
	}
}
