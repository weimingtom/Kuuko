/*
 ** $Id: lobject.c,v 2.22.1.1 2007/12/27 13:02:25 roberto Exp $
 ** Some generic functions over Lua objects
 ** See Copyright Notice in lua.h
 */
namespace KopiLua
{
	/*
	 ** Upvalues
	 */
	public class UpVal : GCObject
	{
		public class _u
		{			
			public class _l
			{  
				/* double linked list (when open) */
				public UpVal prev;
				public UpVal next;
			}
			
			public TValue value = new TValue();  /* the value (when closed) */
			public _l l = new _l();
		}
		public /*new*/ _u u = new _u();
		
		public TValue v;  /* points to stack or to its own value */
	}
}
