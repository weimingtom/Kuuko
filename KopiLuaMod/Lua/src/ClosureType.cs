/*
 ** $Id: lobject.c,v 2.22.1.1 2007/12/27 13:02:25 roberto Exp $
 ** Some generic functions over Lua objects
 ** See Copyright Notice in lua.h
 */
using System;

namespace KopiLua
{
	public class ClosureType
	{
		ClosureHeader header;

		public static implicit operator ClosureHeader(ClosureType ctype) 
		{ 
			return ctype.header; 
		}
		
		public ClosureType(ClosureHeader header) 
		{ 
			this.header = header; 
		}

		public Byte/*lu_byte*/ isC 
		{ 
			get 
			{ 
				return header.isC; 
			} 
			set 
			{ 
				header.isC = value; 
			} 
		}
		
		public Byte/*lu_byte*/ nupvalues 
		{ 
			get 
			{ 
				return header.nupvalues; 
			} 
			set 
			{ 
				header.nupvalues = value; 
			} 
		}
		
		public GCObject gclist 
		{ 
			get 
			{ 
				return header.gclist; 
			} 
			set 
			{ 
				header.gclist = value; 
			} 
		}
		
		public Table env 
		{ 
			get 
			{ 
				return header.env; 
			} 
			set 
			{ 
				header.env = value; 
			} 
		}
	}
}
