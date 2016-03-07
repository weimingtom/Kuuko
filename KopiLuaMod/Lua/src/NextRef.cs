/*
 ** $Id: lstate.c,v 2.36.1.2 2008/01/03 15:20:39 roberto Exp $
 ** Global State
 ** See Copyright Notice in lua.h
 */
namespace KopiLua
{
	public class NextRef : GCObjectRef
	{
		private GCheader header;
		
		public NextRef(GCheader header) 
		{ 
			this.header = header; 
		}
		
		public void set(GCObject value) 
		{ 
			this.header.next = value; 
		}
		
		public GCObject get() 
		{ 
			return this.header.next; 
		}
	}
}
