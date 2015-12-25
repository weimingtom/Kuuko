/*
 ** $Id: lstate.c,v 2.36.1.2 2008/01/03 15:20:39 roberto Exp $
 ** Global State
 ** See Copyright Notice in lua.h
 */
using System;

namespace KopiLua
{
	/*
	 ** Union of all collectable objects (not a union anymore in the C# port)
	 */
	public class GCObject : GCheader, ArrayElement
	{
		// todo: remove this?
		//private GCObject[] values = null;
		//private int index = -1;
		
		public void set_index(int index)
		{
			//this.index = index;
		}
		
		public void set_array(object array)
		{
			//this.values = (GCObject[])array;
			//Debug.Assert(this.values != null);
		}
		
		public GCheader gch
		{
			get
			{
				return (GCheader)this;
			}
		}
		
		public TString ts
		{
			get
			{
				return (TString)this;
			}
		}
		
		public Udata u
		{
			get
			{
				return (Udata)this;
			}
		}
		
		public Closure cl
		{
			get
			{
				return (Closure)this;
			}
		}
		
		public Table h
		{
			get
			{
				return (Table)this;
			}
		}
		
		public Proto p
		{
			get
			{
				return (Proto)this;
			}
		}
		
		public UpVal uv
		{
			get
			{
				return (UpVal)this;
			}
		}
		
		public lua_State th
		{
			get
			{
				return (lua_State)this;
			}
		}
	}
}
