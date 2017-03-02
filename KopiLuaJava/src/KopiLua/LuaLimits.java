package KopiLua;

//
// ** $Id: llimits.h,v 1.69.1.1 2007/12/27 13:02:25 roberto Exp $
// ** Limits, basic types, and some other `installation-dependent' definitions
// ** See Copyright Notice in lua.h
// 

//
// ** #define lua_assert
// 
//using lu_int32 = System.UInt32;
//using lu_mem = System.UInt32;
//using l_mem = System.Int32;
//using lu_byte = System.Byte;
//using l_uacNumber = System.Double;
//using lua_Number = System.Double;
//using Instruction = System.UInt32;

public class LuaLimits {
	//typedef LUAI_UINT32 lu_int32;

	//typedef LUAI_UMEM lu_mem;

	//typedef LUAI_MEM l_mem;



	// chars used as small naturals (so that `char' is reserved for characters) 
	//typedef unsigned char lu_byte;


	public static final int MAX_SIZET = Integer.MAX_VALUE - 2; //uint - uint

	public static final int MAX_LUMEM = Integer.MAX_VALUE - 2; //UInt32 - lu_mem - lu_mem - UInt32

	public static final int MAX_INT = (Integer.MAX_VALUE - 2); // maximum value of an int (-2 for safety)  - Int32

//        
//		 ** conversion of pointer to integer
//		 ** this is for hashing only; there is no problem if the integer
//		 ** cannot hold the whole pointer value
//		 
	///#define IntPoint(p)  ((uint)(lu_mem)(p))



	// type to ensure maximum alignment 
	//typedef LUAI_USER_ALIGNMENT_T L_Umaxalign;


	// result of a `usual argument conversion' over lua_Number 
	//typedef LUAI_UACNUMBER l_uacNumber;


	// internal assertions for in-house debugging 

	///#if lua_assert

//		[Conditional("DEBUG")]
//		public static void lua_assert(bool c) 
//		{
//			Debug.Assert(c);
//		}
//
//		[Conditional("DEBUG")]
//		public static void lua_assert(int c) 
//		{ 
//			Debug.Assert(c != 0); 
//		}
//
//		public static object check_exp(bool c, object e)		
//		{
//			lua_assert(c); 
//			return e;
//		}
//		public static object check_exp(int c, object e) 
//		{ 
//			lua_assert(c != 0); 
//			return e; 
//		}

	///#else

	//[Conditional("DEBUG")]
	public static void lua_assert(boolean c) {

	}

	//[Conditional("DEBUG")]
	public static void lua_assert(int c) {

	}

	public static Object check_exp(boolean c, Object e) {
		return e;
	}
	public static Object check_exp(int c, Object e) {
		return e;
	}

	///#endif

	//[Conditional("DEBUG")]
	public static void api_check(Object o, boolean e) {
		lua_assert(e);
	}

	public static void api_check(Object o, int e) {
		lua_assert(e != 0);
	}

	///#define UNUSED(x)	((void)(x))	/* to avoid warnings */

	public static byte cast_byte(int i) { //lu_byte
		return (byte)i; //lu_byte
	}

	public static byte cast_byte(long i) { //lu_byte
		return (byte)(int)i; //lu_byte
	}

	public static byte cast_byte(boolean i) { //lu_byte
		return i ? (byte)1 : (byte)0; //lu_byte - lu_byte
	}

	public static byte cast_byte(double i) { //lua_Number - lu_byte
		return (byte)i; //lu_byte
	}

	//public static Byte/*lu_byte*/ cast_byte(object i) 
	//{ 
	//	return (Byte/*lu_byte*/)(int)(Int32)(i); 
	//}

	public static int cast_int(int i) {
		return (int)i;
	}

	public static int cast_int(long i) {
		return (int)(int)i;
	}

	public static int cast_int(boolean i) {
		return i ? (int)1 : (int)0;
	}

	public static int cast_int(double i) { //lua_Number
		return (int)i;
	}

	public static int cast_int_instruction(long i) { //Instruction - UInt32
		return ClassType.ConvertToInt32(i);
	}

	public static int cast_int(Object i) {
		ClassType.Assert(false, "Can't convert int.");
		return ClassType.ConvertToInt32_object(i);
	}

	public static double cast_num(int i) { //lua_Number
		return (double)i; //lua_Number
	}

	public static double cast_num(long i) { //lua_Number
		return (double)i; //lua_Number
	}

	public static double cast_num(boolean i) { //lua_Number
		return i ? (double)1 : (double)0; //lua_Number - lua_Number
	}

	public static double cast_num(Object i) { //lua_Number
		//FIXME:
		ClassType.Assert(false, "Can't convert number.");
		return ClassType.ConvertToSingle(i);
	}

//        
//		 ** type for virtual-machine instructions
//		 ** must be an unsigned with (at least) 4 bytes (see details in lopcodes.h)
//		 
	//typedef lu_int32 Instruction;

	// maximum stack for a Lua function 
	public static final int MAXSTACK = 250;

	// minimum size for the string table (must be power of 2) 
	public static final int MINSTRTABSIZE = 32;

	// minimum size for string buffer 
	public static final int LUA_MINBUFFER = 32;

	///#if !lua_lock
	public static void lua_lock(lua_State L) {

	}
	public static void lua_unlock(lua_State L) {

	}
	///#endif

	///#if !luai_threadyield
	public static void luai_threadyield(lua_State L) {
		lua_unlock(L);
		lua_lock(L);
	}
	///#endif

//        
//		 ** macro to control inclusion of some hard tests on stack reallocation
//		 
	///#ifndef HARDSTACKTESTS
	///#define condhardstacktests(x)	((void)0)
	///#else
	///#define condhardstacktests(x)	x
	///#endif
}