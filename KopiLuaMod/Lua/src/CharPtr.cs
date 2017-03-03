/*
 ** $Id: luaconf.h,v 1.82.1.7 2008/02/11 16:25:08 roberto Exp $
 ** Configuration file for Lua
 ** See Copyright Notice in lua.h
 */
namespace KopiLua
{
	public class CharPtr
	{
		public char[] chars;
		public int index;

        //public char this[int offset] get
        public char get(int offset)
        {
            return chars[index + offset];
        }

        //public char this[int offset] set
        public void set(int offset, char val)
        {
            chars[index + offset] = val;
        }

        //public char this[long offset] get
        public char get(long offset)
        {
            return chars[index + (int)offset];
        }

        //public char this[long offset] set
        public void set(long offset, char val)
        {
            chars[index + (int)offset] = val;
        }
		
        //implicit operator CharPtr
        public static CharPtr toCharPtr(string str) 
		{
			return new CharPtr(str); 
		}

		//implicit operator CharPtr
		public static CharPtr toCharPtr(char[] chars) 
		{ 
			return new CharPtr(chars); 
		}
		
		public CharPtr()
		{
			this.chars = null;
			this.index = 0;
		}
		
		public CharPtr(string str)
		{
			this.chars = (str + '\0').ToCharArray();
			this.index = 0;
		}
		
		public CharPtr(CharPtr ptr)
		{
			this.chars = ptr.chars;
			this.index = ptr.index;
		}
		
		public CharPtr(CharPtr ptr, int index)
		{
			this.chars = ptr.chars;
			this.index = index;
		}
		
		public CharPtr(char[] chars)
		{
			this.chars = chars;
			this.index = 0;
		}
		
		public CharPtr(char[] chars, int index)
		{
			this.chars = chars;
			this.index = index;
		}
		
		//public CharPtr(IntPtr ptr)
		//{
		//	this.chars = new char[0];
		//	this.index = 0;
		//}
		
		public static CharPtr plus(CharPtr ptr, int offset) 
		{ 
			return new CharPtr(ptr.chars, ptr.index + offset); 
		}
		
		public static CharPtr minus(CharPtr ptr, int offset) 
		{
			return new CharPtr(ptr.chars, ptr.index - offset); 
		}
		
		public void inc() 
		{
			this.index++; 
		}
		
		public void dec() 
		{ 
			this.index--; 
		}
		
		public CharPtr next() 
		{ 
			return new CharPtr(this.chars, this.index + 1); 
		}
		
		public CharPtr prev() 
		{ 
			return new CharPtr(this.chars, this.index - 1); 
		}
		
		public CharPtr add(int ofs) 
		{ 
			return new CharPtr(this.chars, this.index + ofs); 
		}
		
		public CharPtr sub(int ofs) 
		{ 
			return new CharPtr(this.chars, this.index - ofs); 
		}

        //operator ==
        public static bool isEqualChar(CharPtr ptr, char ch)
        {
            return ptr.get(0) == ch;
        }

        //operator ==
        public static bool isEqualChar(char ch, CharPtr ptr) 
		{ 
			return ptr.get(0) == ch; 
		}
		
        //operator !=
		public static bool isNotEqualChar(CharPtr ptr, char ch) 
		{ 
			return ptr.get(0) != ch; 
		}
		
        //operator !=
        public static bool isNotEqualChar(char ch, CharPtr ptr) 
		{ 
			return ptr.get(0) != ch; 
		}
		
		public static CharPtr plus(CharPtr ptr1, CharPtr ptr2)
		{
			string result = "";
			for (int i = 0; ptr1.get(i) != '\0'; i++)
			{
				result += ptr1.get(i);
			}
			for (int i = 0; ptr2.get(i) != '\0'; i++)
			{
				result += ptr2.get(i);
			}
			return new CharPtr(result);
		}
		
		public static int minus(CharPtr ptr1, CharPtr ptr2)
		{
			ClassType.Assert(ptr1.chars == ptr2.chars); return ptr1.index - ptr2.index;
		}
		
        //operator <
		public static bool lessThan(CharPtr ptr1, CharPtr ptr2)
		{
			ClassType.Assert(ptr1.chars == ptr2.chars); return ptr1.index < ptr2.index;
		}
        //operator <=
		public static bool lessEqual(CharPtr ptr1, CharPtr ptr2)
		{
			ClassType.Assert(ptr1.chars == ptr2.chars); return ptr1.index <= ptr2.index;
		}
        public static bool greaterThan(CharPtr ptr1, CharPtr ptr2)
		{
			ClassType.Assert(ptr1.chars == ptr2.chars); return ptr1.index > ptr2.index;
		}
        //operator >=
		public static bool greaterEqual(CharPtr ptr1, CharPtr ptr2)
		{
			ClassType.Assert(ptr1.chars == ptr2.chars); return ptr1.index >= ptr2.index;
		}

        //operator ==
        public static bool isEqual(CharPtr ptr1, CharPtr ptr2)
        {
            object o1 = ptr1 as CharPtr;
            object o2 = ptr2 as CharPtr;
            if ((o1 == null) && (o2 == null))
            {
                return true;
            }
            if (o1 == null)
            {
                return false;
            }
            if (o2 == null)
            {
                return false;
            }
            return (ptr1.chars == ptr2.chars) && (ptr1.index == ptr2.index);
        }

        //operator !=
        public static bool isNotEqual(CharPtr ptr1, CharPtr ptr2)
        {
            return !(CharPtr.isEqual(ptr1, ptr2));
        }
		
		public override bool Equals(object o)
		{
			return CharPtr.isEqual(this, (o as CharPtr));
		}
		
		public override int GetHashCode()
		{
			return 0;
		}
		
		public override string ToString()
		{
			string result = "";
			for (int i = index; (i < chars.Length) && (chars[i] != '\0'); i++)
			{
				result += chars[i];
			}
			return result;
		}
	}
}
