/*
 ** $Id: lobject.c,v 2.22.1.1 2007/12/27 13:02:25 roberto Exp $
 ** Some generic functions over Lua objects
 ** See Copyright Notice in lua.h
 */
using System;
using System.Diagnostics;

namespace KopiLua
{
	public class TValue : ArrayElement
	{
		private TValue[] values = null;
		private int index = -1;
		
		public void set_index(int index)
		{
			this.index = index;
		}
		
		public void set_array(object array)
		{
			this.values = (TValue[])array;
			Debug.Assert(this.values != null);
		}
		
		public TValue this[int offset]
		{
			get { return this.values[this.index + offset]; }
		}
		
		/*
		public TValue this[uint offset]
		{
			get { return this.values[this.index + (int)offset]; }
		}
		*/
		
		public static TValue operator +(TValue value, int offset)
		{
			return value.values[value.index + offset];
		}
		
		public static TValue operator +(int offset, TValue value)
		{
			return value.values[value.index + offset];
		}
		
		public static TValue operator -(TValue value, int offset)
		{
			return value.values[value.index - offset];
		}
		
		public static int operator -(TValue value, TValue[] array)
		{
			Debug.Assert(value.values == array);
			return value.index;
		}
		
		public static int operator -(TValue a, TValue b)
		{
			Debug.Assert(a.values == b.values);
			return a.index - b.index;
		}
		
		public static bool operator <(TValue a, TValue b)
		{
			Debug.Assert(a.values == b.values);
			return a.index < b.index;
		}
		
		public static bool operator <=(TValue a, TValue b)
		{
			Debug.Assert(a.values == b.values);
			return a.index <= b.index;
		}
		
		public static bool operator >(TValue a, TValue b)
		{
			Debug.Assert(a.values == b.values);
			return a.index > b.index;
		}
		
		public static bool operator >=(TValue a, TValue b)
		{
			Debug.Assert(a.values == b.values);
			return a.index >= b.index;
		}
		
		public static TValue inc(ref TValue value)
		{
			value = value[1];
			return value[-1];
		}
		
		public static TValue dec(ref TValue value)
		{
			value = value[-1];
			return value[1];
		}
		
		public static implicit operator int(TValue value)
		{
			return value.index;
		}
		
		public TValue()
		{
			this.values = null;
			this.index = 0;
			this.value = new Value();
			this.tt = 0;
		}
		
		public TValue(TValue value)
		{
			this.values = value.values;
			this.index = value.index;
			this.value = value.value; // todo: do a shallow copy here
			this.tt = value.tt;
		}
		
		public TValue(TValue[] values)
		{
			this.values = values;
			this.index = Array.IndexOf(values, this);
			this.value = new Value();
			this.tt = 0;
		}
		
		public TValue(Value value, int tt)
		{
			this.values = null;
			this.index = 0;
			this.value = value;
			this.tt = tt;
		}
		
		public TValue(TValue[] values, Value value, int tt)
		{
			this.values = values;
			this.index = Array.IndexOf(values, this);
			this.value = value;
			this.tt = tt;
		}
		
		public Value value = new Value();
		public int tt;
	}
}
