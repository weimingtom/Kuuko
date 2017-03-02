package KopiLua;

public class ClassType {
	//FIXME:remove typeof
	//TODO:need reimplementation->search for stub replacement
	//TODO:not implemented->search for empty stub
	//TODO:not sync
	private static final boolean DONNOT_USE_REIMPLEMENT = false;

	//char //---
	public static final int TYPE_CHAR = 1;
	//FIXME:TYPE_INT equal TYPE_INT32
	//int //typeof(int/*uint*/) 
	public static final int TYPE_INT = 2;
	//Double, Lua_Number
	public static final int TYPE_DOUBLE = 3;
	// UInt32 Instruction //---        
	public static final int TYPE_LONG = 4;
	//LG        
	public static final int TYPE_LG = 5;
	//FilePtr
	public static final int TYPE_FILEPTR = 6;
	//TValue; //---
	public static final int TYPE_TVALUE = 7;
	//CClosure
	public static final int TYPE_CCLOSURE = 8;
	//LClosure
	public static final int TYPE_LCLOSURE = 9;
	//Table
	public static final int TYPE_TABLE = 10;
	//GCObjectRef
	public static final int TYPE_GCOBJECTREF = 11;
	//TString
	public static final int TYPE_TSTRING = 12;
	//Node
	public static final int TYPE_NODE = 13;
	//Udata
	public static final int TYPE_UDATA = 14;
	//lua_State
	public static final int TYPE_LUA_STATE = 15;
	//CallInfo //---
	public static final int TYPE_CALLINFO = 16;
	//Proto //---
	public static final int TYPE_PROTO = 17;
	//LocVar
	public static final int TYPE_LOCVAR = 18;
	//---
	//Closure
	public static final int TYPE_CLOSURE = 19;
	//UpVal
	public static final int TYPE_UPVAL = 20;
	//Int32
	public static final int TYPE_INT32 = 21;
	//GCObject
	public static final int TYPE_GCOBJECT = 22;
	//---
	public static final int TYPE_CHARPTR = 23;

	private int type = 0;

	public ClassType(int type) {
		this.type = type;
	}

	public final int GetTypeID() {
		return this.type;
	}

	public final String GetTypeString() {
		if (DONNOT_USE_REIMPLEMENT) {
			return GetTypeString_csharp();
		}
		else {
			//TODO:not sync
			String result = null;
			if (type == TYPE_CHAR) {
				result = "Char";
			}
			else if (type == TYPE_INT) {
				result = "Int";
			}
			else if (type == TYPE_DOUBLE) {
				result = "Double";
			}
			else if (type == TYPE_LONG) {
				result = "Int64"; //FIXME:
			}
			else if (type == TYPE_LG) {
				result = "LG";
			}
			else if (type == TYPE_FILEPTR) {
				result = "FilePtr";
			}
			else if (type == TYPE_TVALUE) {
				result = "TValue";
			}
			else if (type == TYPE_CCLOSURE) {
				result = "CClosure";
			}
			else if (type == TYPE_LCLOSURE) {
				result = "LClosure";
			}
			else if (type == TYPE_TABLE) {
				result = "Table";
			}
			else if (type == TYPE_GCOBJECTREF) {
				result = "GCObjectRef";
			}
			else if (type == TYPE_TSTRING) {
				result = "TString";
			}
			else if (type == TYPE_NODE) {
				result = "Node";
			}
			else if (type == TYPE_UDATA) {
				result = "Udata";
			}
			else if (type == TYPE_LUA_STATE) {
				result = "lua_State";
			}
			else if (type == TYPE_CALLINFO) {
				result = "CallInfo";
			}
			else if (type == TYPE_PROTO) {
				result = "Proto";
			}
			else if (type == TYPE_LOCVAR) {
				result = "LocVar";
			}
			else if (type == TYPE_CLOSURE) {
				result = "Closure";
			}
			else if (type == TYPE_UPVAL) {
				result = "UpVal";
			}
			else if (type == TYPE_INT32) {
				result = "Int32"; //FIXME:
			}
			else if (type == TYPE_GCOBJECT) {
				result = "GCObject";
			}
			else if (type == TYPE_CHARPTR) {
				result = "CharPtr";
			}
			//return null;
			if (result == null) {
				return "unknown type";
			}
			else {
				return result;
			}
		}
	}

	public final Object Alloc() {
		if (DONNOT_USE_REIMPLEMENT) {
			return Alloc_csharp();
		}
		else {
			Object result = null;
			//FIXME:
			//return System.Activator.CreateInstance(this.GetOriginalType());
			if (type == TYPE_CHAR) {
				result = new Character('\0');
			}
			else if (type == TYPE_INT) {
				result = new Integer(0);
			}
			else if (type == TYPE_DOUBLE) {
				result = new Double(0);
			}
			else if (type == TYPE_LONG) {
				result = new Long(0); //FIXME:
			}
			else if (type == TYPE_LG) {
				result = new LG();
			}
			else if (type == TYPE_FILEPTR) {
				result = new FilePtr();
			}
			else if (type == TYPE_TVALUE) {
				result = new TValue();
			}
			else if (type == TYPE_CCLOSURE) {
				throw new RuntimeException("alloc CClosure error");
				//return new CClosure(null);
			}
			else if (type == TYPE_LCLOSURE) {
				throw new RuntimeException("alloc LClosure error");
				//return new LClosure(null);
			}
			else if (type == TYPE_TABLE) {
				result = new Table();
			}
			else if (type == TYPE_GCOBJECTREF) {
				//return null; //FIXME:interface!!!
				throw new RuntimeException("alloc GCObjectRef error");
			}
			else if (type == TYPE_TSTRING) {
				result = new TString();
			}
			else if (type == TYPE_NODE) {
				result = new Node();
			}
			else if (type == TYPE_UDATA) {
				result = new Udata();
			}
			else if (type == TYPE_LUA_STATE) {
				result = new lua_State();
			}
			else if (type == TYPE_CALLINFO) {
				result = new CallInfo();
			}
			else if (type == TYPE_PROTO) {
				result = new Proto();
			}
			else if (type == TYPE_LOCVAR) {
				result = new LocVar();
			}
			else if (type == TYPE_CLOSURE) {
				result = new Closure();
			}
			else if (type == TYPE_UPVAL) {
				result = new UpVal();
			}
			else if (type == TYPE_INT32) {
				result = new Integer(0); //FIXME:
			}
			else if (type == TYPE_GCOBJECT) {
				result = new GCObject();
			}
			else if (type == TYPE_CHARPTR) {
				result = new CharPtr();
			}
			//return null;
			if (result == null) {
				throw new RuntimeException("alloc unknown type error");
			}
			else {
				//Debug.WriteLine("alloc " + result.GetType().ToString());
				return result;
			}
		}
	}

	public final boolean CanIndex() {
		if (DONNOT_USE_REIMPLEMENT) {
			return CanIndex_csharp();
		}
		else {
			if (type == TYPE_CHAR) {
				return false;
			}
			//else if (type == TYPE_BYTE)
			//{
			//    return false;
			//}
			else if (type == TYPE_INT) {
				return false;
			}
			else if (type == TYPE_LOCVAR) {
				return false;
			}
			else if (type == TYPE_LONG) {
				return false;
			}
			else {
				return true;
			}
		}
	}

	public final int GetUnmanagedSize() {
		if (DONNOT_USE_REIMPLEMENT) {
			return GetUnmanagedSize_csharp();
		}
		else {
			int result = -1;
			if (type == TYPE_LG) {
				result = 376;
			}
			//else if (type == TYPE_GLOBAL_STATE)
			//{
			//    result = 228;
			//}
			else if (type == TYPE_CALLINFO) {
				result = 24;
			}
			else if (type == TYPE_TVALUE) {
				result = 16;
			}
			else if (type == TYPE_TABLE) {
				result = 32;
			}
			else if (type == TYPE_NODE) {
				result = 32;
			}
			else if (type == TYPE_GCOBJECT) {
				result = 120;
			}
			else if (type == TYPE_GCOBJECTREF) {
				result = 4;
			}
			//else if (type == TYPE_ARRAYREF)
			//{
			//    result = 4;
			//}
			else if (type == TYPE_CLOSURE) {
				//FIXME: this is zero
				result = 0; // handle this one manually in the code
			}
			else if (type == TYPE_PROTO) {
				result = 76;
			}
			//else if (type == TYPE_LUAL_REG)
			//{
			//    result = 8;
			//}
			//else if (type == TYPE_LUAL_BUFFER)
			//{
			//    result = 524;
			//}
			else if (type == TYPE_LUA_STATE) {
				result = 120;
			}
			//else if (type == TYPE_LUA_DEBUG)
			//{
			//    result = 100;
			//}
			//else if (type == TYPE_CALLS)
			//{
			//    result = 8;
			//}
			//else if (type == TYPE_LOADF)
			//{
			//    result = 520;
			//}
			//else if (type == TYPE_LOADS)
			//{
			//    result = 8;
			//}
			//else if (type == TYPE_LUA_LONGJMP)
			//{
			//   result = 72;
			//}
			//else if (type == TYPE_SPARSER)
			//{
			//    result = 20;
			//}
			//else if (type == TYPE_TOKEN)
			//{
			//    result = 16;
			//}
			//else if (type == TYPE_LEXSTATE)
			//{
			//    result = 52;
			//}
			//else if (type == TYPE_FUNCSTATE)
			//{
			//    result = 572;
			//}
			//else if (type == TYPE_GCHEADER)
			//{
			//    result = 8;
			//}
			else if (type == TYPE_TVALUE) {
				result = 16;
			}
			else if (type == TYPE_TSTRING) {
				result = 16;
			}
			else if (type == TYPE_LOCVAR) {
				result = 12;
			}
			else if (type == TYPE_UPVAL) {
				result = 32;
			}
			else if (type == TYPE_CCLOSURE) {
				result = 40;
			}
			else if (type == TYPE_LCLOSURE) {
				result = 24;
			}
			//else if (type == TYPE_TKEY)
			//{
			//    result = 16;
			//}
			//else if (type == TYPE_CONSCONTROL)
			//{
			//    result = 40;
			//}
			//else if (type == TYPE_LHS_ASSIGN)
			//{
			//    result = 32;
			//}
			//else if (type == TYPE_EXPDESC)
			//{
			//    result = 24;
			//}
			//else if (type == TYPE_UPVALDESC)
			//{
			//    result = 2;
			//}
			//else if (type == TYPE_BLOCKCNT)
			//{
			//    result = 12;
			//}
			//else if (type == TYPE_ZIO)
			//{
			//    result = 20;
			//}
			//else if (type == TYPE_MBUFFER)
			//{
			//    result = 12;
			//}
			//else if (type == TYPE_LOADSTATE)
			//{
			//    result = 16;
			//}
			//else if (type == TYPE_MATCHSTATE)
			//{
			//    result = 272;
			//}
			//else if (type == TYPE_STRINGTABLE)
			//{
			//    result = 12;
			//}
			else if (type == TYPE_FILEPTR) {
				result = 4;
			}
			else if (type == TYPE_UDATA) {
				result = 24;
			}
			else if (type == TYPE_CHAR) {
				result = 1;
			}
			//else if (type == TYPE_UINT16)
			//{
			//    result = 2;
			//}
			//else if (type == TYPE_INT16)
			//{
			//    result = 2;
			//}
			//else if (type == TYPE_UINT32)
			//{
			//    result = 4;
			//}
			else if (type == TYPE_INT32) {
				result = 4;
			}
			else if (type == TYPE_INT) {
				//FIXME: added, equal to TYPE_INT32 
				result = 4;
			}
			//else if (type == TYPE_SINGLE)
			//{
			//    result = 4;
			//}
			else if (type == TYPE_LONG) {
				result = 8;
			}
			if (result < 0) {
				throw new RuntimeException("Trying to get unknown sized of unmanaged type " + GetTypeString());
			}
			else {
				return result;
			}
		}
	}

	//TODO:need reimplementation
	public final int GetMarshalSizeOf() {
		if (DONNOT_USE_REIMPLEMENT) {
			return GetMarshalSizeOf_csharp();
		}
		else {
			//new method
			return GetUnmanagedSize();
		}
	}

	//only byValue type
	public final byte[] ObjToBytes(Object b) {
		if (DONNOT_USE_REIMPLEMENT) {
			return ObjToBytes_csharp(b);
		}
		else {
			//TODO:not implemented
			return null;
			//LuaDump.DumpMem not work
			//LuaStrLib.writer not work
		}
	}

	//TODO:need reimplementation
	public final byte[] ObjToBytes2(Object b) {
		if (DONNOT_USE_REIMPLEMENT) {
			return ObjToBytes2_csharp(b);
		}
		else {
			return ObjToBytes(b);
		}
	}

	//TODO:need reimplementation
	public final Object bytesToObj(byte[] bytes) {
		if (DONNOT_USE_REIMPLEMENT) {
			return bytesToObj_csharp(bytes);
		}
		else {
			//TODO:not implemented
			return null;
			//LuaUndump.LoadMem not work
		}
	}

	//number of ints inside a lua_Number
	public static int GetNumInts() {
		//return sizeof(Double/*lua_Number*/) / sizeof(int); //FIXME:
		return 8 / 4;
	}

	public static int SizeOfInt() {
		//return sizeof(int); //FIXME:
		return 4;
	}

	public static int SizeOfLong() {
		//sizeof(long/*uint*/)
		//sizeof(long/*UInt32*//*Instruction*/));
		//return sizeof(long); //FIXME:
		return 8;
	}

	public static int SizeOfDouble() {
		//sizeof(Double/*lua_Number*/)
		//return sizeof(double);//FIXME:
		return 8;
	}

	public static double ConvertToSingle(Object o) {
		//return Convert.ToSingle(o); //FIXME:
		return Float.parseFloat(o.toString());
	}

	public static char ConvertToChar(String str) {
		return str.length() > 0 ? str.charAt(0) : '\0';
	}

	public static int ConvertToInt32(String str) {
		return Integer.parseInt(str);
	}

	public static int ConvertToInt32(long i) {
		//return Convert.ToInt32(i); //FIXME:
		return (int)i;
	}

	public static int ConvertToInt32_object(Object i) {
		//return Convert.ToInt32(i);//FIXME:
		return Integer.parseInt(i.toString());
	}

	public static double ConvertToDouble(String str, boolean[] isSuccess) {
		if (isSuccess != null) {
			isSuccess[0] = true;
		}
		try {
			return Double.parseDouble(str);
		}
		catch (java.lang.Exception e2) {
			if (isSuccess != null) {
				isSuccess[0] = false;
			}
			return 0;
		}
	}

	public static boolean isNaN(double d) {
		return Double.isNaN(d);
	}

	public static int log2(double x) {
		if (DONNOT_USE_REIMPLEMENT) {
			return log2_csharp(x);
		}
		else {
			return (int)(Math.log(x) / Math.log(2));
		}
	}

	public static double ConvertToInt32(Object obj) {
		//return Convert.ToInt32(obj);//FIXME:
		return Integer.parseInt(obj.toString());
	}

	public static boolean IsPunctuation(char c) {
		if (c == ',' ||
			c == '.' ||
			c == ';' ||
			c == ':' ||
			c == '!' ||
			c == '?' ||
			c == '/' ||
			c == '\\' ||
			c == '\'' ||
			c == '\"'){
			return true;
		} else {
			return false;
		}
	}

	public static int IndexOfAny(String str, char[] anyOf) {
		if (DONNOT_USE_REIMPLEMENT) {
			return IndexOfAny_csharp(str, anyOf);
		}
		else {
			int index = -1;
			for (int i = 0; i < anyOf.length; i++) {
				int index2 = str.indexOf(anyOf[i]);
				if (index2 >= 0) {
					if (index == -1) {
						index = index2;
					}
					else {
						if (index2 < index) {
							index = index2;
						}
					}
				}
			}
			return index;
		}
	}

	public static void Assert(boolean condition) {
		if (DONNOT_USE_REIMPLEMENT) {
			Assert_csharp(condition);
		}
		else {
			if (!condition) {
				throw new RuntimeException("Assert");
			}
		}
	}

	public static void Assert(boolean condition, String message) {
		if (DONNOT_USE_REIMPLEMENT) {
			Assert_csharp(condition, message);
		}
		else {
			if (!condition) {
				throw new RuntimeException(message);
			}
		}
	}

	public static int processExec(String strCmdLine) {
		if (DONNOT_USE_REIMPLEMENT) {
			return processExec_csharp(strCmdLine);
		}
		else {
			//TODO:not implemented
			return 0;
			//LuaOSLib.os_execute
		}
	}

	//object[] to T[]
	public final Object ToArray(Object[] arr) {
		if (DONNOT_USE_REIMPLEMENT) {
			return ToArray_csharp(arr);
		}
		else {
			//TODO:not implemented
			return null;
			//LuaUndump
		}
	}

	public static byte[] GetBytes(double d) {
		//FIXME:
		long value = Double.doubleToRawLongBits(d);  
		byte[] byteRet = new byte[8];  
		for (int i = 0; i < 8; i++) {  
		    byteRet[i] = (byte) ((value >> 8 * i) & 0xff);  
		}
		return byteRet;  
	}

	//--------------------------------
	//csharp only implementations
	//--------------------------------

	//using System.Runtime.Serialization.Formatters.Binary;
	private byte[] ObjToBytes2_csharp(Object b) {
		return null;
	}

	private int GetMarshalSizeOf_csharp() {
		return 0;
	}

	private Object bytesToObj_csharp(byte[] bytes) {
		return null;
	}

	private byte[] ObjToBytes_csharp(Object b) {
		return null;
	}

	private static int processExec_csharp(String strCmdLine) {
		return 0;
	}

	private Object Alloc_csharp() {
		return null;
	}

	private static void Assert_csharp(boolean condition) {
	
	}

	private static void Assert_csharp(boolean condition, String message) {
		
	}

	private java.lang.Class GetOriginalType_csharp() {
		return null;
	}

	private Object ToArray_csharp(Object[] arr) {
		return null;
	}

	private boolean CanIndex_csharp() {
		return false;
	}

	private String GetTypeString_csharp() {
		return null;
	}

	private int GetUnmanagedSize_csharp() {
		return 0;
	}

	public static int IndexOfAny_csharp(String str, char[] anyOf) {
		return 0;
	}

	public static int log2_csharp(double x) {
		return 0;
	}
}