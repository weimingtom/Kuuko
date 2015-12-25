/*
 ** $Id: lobject.c,v 2.22.1.1 2007/12/27 13:02:25 roberto Exp $
 ** Some generic functions over Lua objects
 ** See Copyright Notice in lua.h
 */
using System;

namespace KopiLua
{
	public class TString : TString_tsv
	{
		//public L_Umaxalign dummy;  /* ensures maximum alignment for strings */
		public TString_tsv tsv 
		{ 
			get 
			{ 
				return this; 
			}
		}
		
		public TString()
		{
			
		}
		
		public TString(CharPtr str) 
		{ 
			this.str = str; 
		}
		
		public CharPtr str;
		
		public override string ToString() 
		{ 
			return str.ToString(); 
		} // for debugging
	}
}
