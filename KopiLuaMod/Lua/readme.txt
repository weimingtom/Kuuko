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
(x) sizeof  ->see ClassType
(x) private readonly static Byte[] luaP_opmodes = {  //remove comment
(x) unchecked
(x) private static priority_[] priority =  //remove comment
(x) switch (argv[i][1]) //string switch
(x) using (MemoryStream stream = new MemoryStream())
(x) goto
(x) switch (m.Groups[2].Value) //string switch
(x) public struct Value //user-defined value types.
(x) out <var>
(x) Type t  ->see ClassType)
(x)(x?) uint (!!!remove not finish!!!)
(x) ref remove (!!!remove ref not finish, search 'ref<space key>' !!!)
(x) uint64
(x) int64
(x) Marshal->see ClassType
(x) typeof
(x) GetType()->see ClassType
(x) System.Activator->see ClassType
(x) Int32
(x) using System.Reflection;
(x) using System.Runtime.Serialization.Formatters.Binary; ->See ClassType
(x) using System.Runtime.InteropServices;->See ClassType
(x) ClassType move to no depend->remove typeof
(x) Convert.ToInt32->see ClassType
(x) params object[]->no problem, ignored
(x) Tools.sprintf->no problem, ignored
(x) embeded class(for example, f_call_delegate)->ignored
(x) Debug.Assert->ClassType.Assert
(x) using System.Diagnostics;->removed
(x) System.Diagnostics.Process
using System.IO;
System.OverflowException
System.Double.NegativeInfinity
System.Double.PositiveInfinity
(x) using System.Collections.Generic;
(x) using System.Collections;
(x) List(System.Collections) in LuaProgram.MainLua & LuacProgram.MainLuac
(x) newargs.RemoveRange(0, i);
(x) IntPtr->removed
(x) Stream->StreamProxy
(x) Array.IndexOf->removed
(x) public static Stream stdout = Console.OpenStandardOutput();->StreamProxy
(x) public static Stream stdin = Console.OpenStandardInput();->StreamProxy
(x) public static Stream stderr = Console.OpenStandardError();->StreamProxy
(x) Directory.GetCurrentDirectory()->StreamProxy
public static OpCode GET_OPCODE(long  ->change to table search
public static OpMode getOpMode(OpCode m) {  ->change to table search
public static OpArgMask getBMode(OpCode m) { ->change to table search
public static OpArgMask getCMode(OpCode m) { ->change to table search
(x) File.Delete ->StreamProxy.Delete
Console.Error.Write
Console.Error.Flush
BitConverter.GetBytes
Console.Write
Console.WriteLine
Console.ReadLine
