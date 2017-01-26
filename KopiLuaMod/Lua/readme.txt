http://gerich-home.github.io/kopilua/

kopilua by gerich-home
C# port of Lua 5.1.4
Kopi Lua is a C# port of the Lua v.5.1.4 virtual machine, parser, libraries and command-line utilities. The latest version of Kopi Lua can be downloaded from the Kopi Lua web page: http://www.ppl-pilot.com/KopiLua

Install

Kopi Lua is implemented in C# and has been tested on Microsoft Visual C# 2008 Express Edition. In order for Kopi Lua to successfully compile the following conditional compilation symbols must be defined in the project settings: LUA_CORE;_WIN32;LUA_COMPAT_VARARG;LUA_COMPAT_MOD;LUA_COMPAT_GFIND;CATCH_EXCEPTIONS

License

MIT license

Authors

Mark Feldman (lua@ppl-pilot.com) 
gerich-home (gerich.home@gmail.com) 

Contact

Mark Feldman (lua@ppl-pilot.com) 
gerich-home (gerich.home@gmail.com) 
Download

You can download this project in either zip or tar formats.

You can also clone the project with Git by running:

$ git clone git://github.com/gerich-home/kopilua


-----------------------

mod

1. delete using lu_mem = System.UInt32;
2. lua_TValue -> TValue
3. delete partial class
4. move class/emum/struct out
5. uint->int
6. using



todo:
remove ref
CharPtr -> IntPtr
getter, setter
ushort
InstructionPtr Uint32
string equal

-----------------------

LuaAuxLib
LuaLoadLib
LuaObject
LuaParser
LuaPrint
LuaStrLib
LuaTableLib
LuaVM

-------------------------
operator

CharPtr
ClosureType
InstructionPtr
Node
TValue

public TValue this[int offset]

l.ToString() == "C"

----------------------

public static int GetUnmanagedSize(Type t)

LuaMem.luaM_freearray<Int32>(L, f.lineinfo);

public const uint MAX_SIZET	= uint.MaxValue - 2;
public const UInt32/*lu_mem*/ MAX_LUMEM = /*lu_mem*/UInt32.MaxValue - 2;
public const int MAX_INT = (/*Int32*/int.MaxValue - 2);  /* maximum value of an int (-2 for safety) */

-----------------------

public final class RefObject<T>
{
	public T argvalue;
	public RefObject(T refarg)
	{
		argvalue = refarg;
	}
}


------------------------------
input:
  =100 * -1

code:
public static TValue luaH_getnum(Table t, int key)
{
	/* (1 <= key && key <= t.sizearray) */
	if ((uint)(key-1) < (uint)t.sizearray)
	{
		return t.array[key-1];
	}
	else 
			
			
if key < 0, will be this

if ((uint)(key-1) < (uint)t.sizearray)
=>
if ((uint)(-100-1) < (uint)0)
=>
if (false)

so I modify it :

public static TValue luaH_getnum(Table t, int key)
{
	/* (1 <= key && key <= t.sizearray) */
	if ((long/*uint*/)((key - 1) & 0xffffffff) < (long/*uint*/)(t.sizearray & 0xffffffff))
	{
		return t.array[key - 1];
	}
	else 
	{

------------------------------

		public static bool CanIndex(Type t)
		{
			if (t == typeof(char))
			{
				return false;
			}
			if (t == typeof(byte))
			{
				return false;
			}
			if (t == typeof(int))
			{
				return false;
			}
			if (t == typeof(uint))
			{
				return false;
			}
			
------------------------------
			

(x) UInt32   -> long
(x) delegate
(x) #if #else #endif
(x) ulong
(x) [Conditional("DEBUG")]
sizeof
(x) private readonly static Byte[] luaP_opmodes = {  //remove comment
(x) unchecked
(x) private static priority_[] priority =  //remove comment
(x) switch (argv[i][1]) //string switch
(x) using (MemoryStream stream = new MemoryStream())
(x) goto
(x) switch (m.Groups[2].Value) //string switch
(x) public struct Value //user-defined value types.
(x) out <var>
Type t (?->see ClassType)
(x?) uint (!!!remove not finish!!!)
(x) ref remove (!!!remove ref not finish, search 'ref<space key>' !!!)
uint64
int64
Marshal
typeof
System.Activator
Int32

