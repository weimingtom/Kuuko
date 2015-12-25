/*
 ** $Id: lobject.c,v 2.22.1.1 2007/12/27 13:02:25 roberto Exp $
 ** Some generic functions over Lua objects
 ** See Copyright Notice in lua.h
 */
using System;

namespace KopiLua
{
	public class TKey
	{
		public TKey()
		{
			this.nk = new TKey_nk();
		}
		
		public TKey(TKey copy)
		{
			this.nk = new TKey_nk(copy.nk.value, copy.nk.tt, copy.nk.next);
		}
		
		public TKey(Value value, int tt, Node next)
		{
			this.nk = new TKey_nk(value, tt, next);
		}
		
		public TKey_nk nk = new TKey_nk();
		
		public TValue tvk 
		{ 
			get 
			{ 
				return this.nk; 
			}
		}
	}
}
