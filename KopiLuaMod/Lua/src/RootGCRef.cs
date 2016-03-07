/*
 ** $Id: lstate.c,v 2.36.1.2 2008/01/03 15:20:39 roberto Exp $
 ** Global State
 ** See Copyright Notice in lua.h
 */
namespace KopiLua
{
	public class RootGCRef : GCObjectRef
	{
		private global_State g;
		
		public RootGCRef(global_State g) 
		{ 
			this.g = g; 
		}
		
		public void set(GCObject value) 
		{ 
			this.g.rootgc = value; 
		}
		
		public GCObject get() 
		{ 
			return this.g.rootgc; 
		}
	}
}
