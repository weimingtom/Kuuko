/*
 ** $Id: lstate.c,v 2.36.1.2 2008/01/03 15:20:39 roberto Exp $
 ** Global State
 ** See Copyright Notice in lua.h
 */
namespace KopiLua
{
	/*
	 ** this interface and is used for implementing GCObject references,
	 ** it's used to emulate the behaviour of a C-style GCObject 
	 */
	public interface GCObjectRef
	{
		void set(GCObject value);
		GCObject get();
	}
}
