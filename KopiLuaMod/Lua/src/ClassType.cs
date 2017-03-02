using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;

namespace KopiLua
{
    public class ClassType
    {
    	//FIXME:remove typeof
    	//TODO:need reimplementation->search for stub replacement
    	//TODO:not implemented->search for empty stub
    	//TODO:not sync
    	private const bool DONNOT_USE_REIMPLEMENT = false;
    	
    	//char //---
        public const int TYPE_CHAR = 1;
        //FIXME:TYPE_INT equal TYPE_INT32
        //int //typeof(int/*uint*/) 
        public const int TYPE_INT = 2;
        //Double, Lua_Number
        public const int TYPE_DOUBLE = 3;
		// UInt32 Instruction //---        
        public const int TYPE_LONG = 4;
		//LG        
        public const int TYPE_LG = 5;
        //FilePtr
        public const int TYPE_FILEPTR = 6;
        //TValue; //---
        public const int TYPE_TVALUE = 7;
        //CClosure
        public const int TYPE_CCLOSURE = 8;
        //LClosure
        public const int TYPE_LCLOSURE = 9;
        //Table
        public const int TYPE_TABLE = 10;
        //GCObjectRef
        public const int TYPE_GCOBJECTREF = 11;
        //TString
        public const int TYPE_TSTRING = 12;
        //Node
        public const int TYPE_NODE = 13;
        //Udata
        public const int TYPE_UDATA = 14;
        //lua_State
        public const int TYPE_LUA_STATE = 15;
        //CallInfo //---
        public const int TYPE_CALLINFO = 16;
        //Proto //---
        public const int TYPE_PROTO = 17;
        //LocVar
        public const int TYPE_LOCVAR = 18;
        //---
        //Closure
        public const int TYPE_CLOSURE = 19;
        //UpVal
        public const int TYPE_UPVAL = 20;
        //Int32
        public const int TYPE_INT32 = 21;
        //GCObject
        public const int TYPE_GCOBJECT = 22;
        //---
        public const int TYPE_CHARPTR = 23;
        
        private int type = 0;

        public ClassType(int type)
        {
            this.type = type;
        }

        public int GetTypeID()
        {
            return this.type;
        }

        public string GetTypeString()
        {
        	if (DONNOT_USE_REIMPLEMENT)
        	{
        		return GetTypeString_csharp();
        	}
        	else
        	{
        		//TODO:not sync
        		string result = null;
        		if (type == TYPE_CHAR)
	            {
        	    	result = "Char";
	            }
	            else if (type == TYPE_INT)
	            {
	            	result = "Int";
	            }
	            else if (type == TYPE_DOUBLE)
	            {
	            	result = "Double";
	            }
	            else if (type == TYPE_LONG)
	            {
	            	result = "Int64"; //FIXME:
	            }
	            else if (type == TYPE_LG)
	            {
	                result = "LG";
	            }
	            else if (type == TYPE_FILEPTR)
	            {
	                result = "FilePtr";
	            }
	            else if (type == TYPE_TVALUE)
	            {
	                result = "TValue";
	            }
	            else if (type == TYPE_CCLOSURE)
	            {
	            	result = "CClosure";
	            }
	            else if (type == TYPE_LCLOSURE)
	            {
	            	result = "LClosure";
	            }
	            else if (type == TYPE_TABLE)
	            {
	                result = "Table";
	            }
	            else if (type == TYPE_GCOBJECTREF)
	            {
	                result = "GCObjectRef";
	            }
	            else if (type == TYPE_TSTRING)
	            {
	                result = "TString";
	            }
	            else if (type == TYPE_NODE)
	            {
	                result = "Node";
	            }
	            else if (type == TYPE_UDATA)
	            {
	                result = "Udata";
	            }
	            else if (type == TYPE_LUA_STATE)
	            {
	                result = "lua_State";
	            }
	            else if (type == TYPE_CALLINFO)
	            {
	                result = "CallInfo";
	            }
	            else if (type == TYPE_PROTO)
	            {
	                result = "Proto";
	            }
	            else if (type == TYPE_LOCVAR)
	            {
	                result = "LocVar";
	            }
	            else if (type == TYPE_CLOSURE)
	            {
	                result = "Closure";
	            }
	            else if (type == TYPE_UPVAL)
	            {
	                result = "UpVal";
	            }
	            else if (type == TYPE_INT32)
	            {
	                result = "Int32";//FIXME:
	            }
	            else if (type == TYPE_GCOBJECT)
	            {
	                result = "GCObject";
	            }
	            else if (type == TYPE_CHARPTR)
	            {
	                result = "CharPtr";
	            }
	            //return null;
	            if (result == null)
	            {
        			return "unknown type";
	            }
	            else
	            {
	            	return result;
	            }
        	}
        }

        public object Alloc()
        {
        	if (DONNOT_USE_REIMPLEMENT)
        	{
        		return Alloc_csharp();
        	}
        	else
        	{
        		object result = null;
        		//FIXME:
        		//return System.Activator.CreateInstance(this.GetOriginalType());
        	    if (type == TYPE_CHAR)
	            {
        	    	result = new Char();
	            }
	            else if (type == TYPE_INT)
	            {
	            	result = new Int32();
	            }
	            else if (type == TYPE_DOUBLE)
	            {
	            	result = new Double();
	            }
	            else if (type == TYPE_LONG)
	            {
	            	result = new Int64(); //FIXME:
	            }
	            else if (type == TYPE_LG)
	            {
	                result = new LG();
	            }
	            else if (type == TYPE_FILEPTR)
	            {
	                result = new FilePtr();
	            }
	            else if (type == TYPE_TVALUE)
	            {
	                result = new TValue();
	            }
	            else if (type == TYPE_CCLOSURE)
	            {
	            	throw new Exception("alloc CClosure error");
	                //return new CClosure(null);
	            }
	            else if (type == TYPE_LCLOSURE)
	            {
	            	throw new Exception("alloc LClosure error");
	                //return new LClosure(null);
	            }
	            else if (type == TYPE_TABLE)
	            {
	                result = new Table();
	            }
	            else if (type == TYPE_GCOBJECTREF)
	            {
	                //return null; //FIXME:interface!!!
	                throw new Exception("alloc GCObjectRef error");
	            }
	            else if (type == TYPE_TSTRING)
	            {
	                result = new TString();
	            }
	            else if (type == TYPE_NODE)
	            {
	                result = new Node();
	            }
	            else if (type == TYPE_UDATA)
	            {
	                result = new Udata();
	            }
	            else if (type == TYPE_LUA_STATE)
	            {
	                result = new lua_State();
	            }
	            else if (type == TYPE_CALLINFO)
	            {
	                result = new CallInfo();
	            }
	            else if (type == TYPE_PROTO)
	            {
	                result = new Proto();
	            }
	            else if (type == TYPE_LOCVAR)
	            {
	                result = new LocVar();
	            }
	            else if (type == TYPE_CLOSURE)
	            {
	                result = new Closure();
	            }
	            else if (type == TYPE_UPVAL)
	            {
	                result = new UpVal();
	            }
	            else if (type == TYPE_INT32)
	            {
	                result = new Int32();//FIXME:
	            }
	            else if (type == TYPE_GCOBJECT)
	            {
	                result = new GCObject();
	            }
	            else if (type == TYPE_CHARPTR)
	            {
	                result = new CharPtr();
	            }
	            //return null;
	            if (result == null)
	            {
        			throw new Exception("alloc unknown type error");
	            }
	            else
	            {
	            	//Debug.WriteLine("alloc " + result.GetType().ToString());
	            	return result;
	            }
        	}
        }

        public bool CanIndex()
        {
        	if (DONNOT_USE_REIMPLEMENT)
        	{
        		return CanIndex_csharp();
        	}
        	else
        	{
	            if (type == TYPE_CHAR)
	            {
	                return false;
	            }	            
	            //else if (type == TYPE_BYTE)
	            //{
	            //    return false;
	            //}
	            else if (type == TYPE_INT)
	            {
	                return false;
	            }
	            else if (type == TYPE_LOCVAR)
	            {
	                return false;
	            }
	            else if (type == TYPE_LONG)
	            {
	                return false;
	            }
	            else
	            {
	            	return true;
	            }
        	}
        }

        public int GetUnmanagedSize()
        {
        	if (DONNOT_USE_REIMPLEMENT)
        	{
        		return GetUnmanagedSize_csharp();
        	}
        	else 
        	{
        		int result = -1;
	            if (type == TYPE_LG)
	            {
	                result = 376;
	            }
	            //else if (type == TYPE_GLOBAL_STATE)
	            //{
	            //    result = 228;
	            //}
	            else if (type == TYPE_CALLINFO)
	            {
	                result = 24;
	            }
	            else if (type == TYPE_TVALUE)
	            {
	                result = 16;
	            }
	            else if (type == TYPE_TABLE)
	            {
	                result = 32;
	            }
	            else if (type == TYPE_NODE)
	            {
	                result = 32;
	            }
	            else if (type == TYPE_GCOBJECT)
	            {
	                result = 120;
	            }
	            else if (type == TYPE_GCOBJECTREF)
	            {
	                result = 4;
	            }
	            //else if (type == TYPE_ARRAYREF)
	            //{
	            //    result = 4;
	            //}
	            else if (type == TYPE_CLOSURE)
	            {
	            	//FIXME: this is zero
	                result = 0;	// handle this one manually in the code
	            }
	            else if (type == TYPE_PROTO)
	            {
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
	            else if (type == TYPE_LUA_STATE)
	            {
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
	            else if (type == TYPE_TVALUE)
	            {
	                result = 16;
	            }
	            else if (type == TYPE_TSTRING)
	            {
	                result = 16;
	            }
	            else if (type == TYPE_LOCVAR)
	            {
	                result = 12;
	            }
	            else if (type == TYPE_UPVAL)
	            {
	                result = 32;
	            }
	            else if (type == TYPE_CCLOSURE)
	            {
	                result = 40;
	            }
	            else if (type == TYPE_LCLOSURE)
	            {
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
	            else if (type == TYPE_FILEPTR)
	            {
	                result = 4;
	            }
	            else if (type == TYPE_UDATA)
	            {
	                result = 24;
	            }
	            else if (type == TYPE_CHAR)
	            {
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
	            else if (type == TYPE_INT32)
	            {
	                result = 4;
	            }
	            else if (type == TYPE_INT)
	            {
	            	//FIXME: added, equal to TYPE_INT32 
	            	result = 4;
	            }
	            //else if (type == TYPE_SINGLE)
	            //{
	            //    result = 4;
	            //}
	            else if (type == TYPE_LONG)
	            {
	                result = 8;
	            }
	            if (result < 0)
	            {
	            	throw new Exception("Trying to get unknown sized of unmanaged type " + GetTypeString());
	            }
	            else
	            {
	            	return result;
	            }
	        }
        }

        //TODO:need reimplementation
        public int GetMarshalSizeOf()
        {
            if (DONNOT_USE_REIMPLEMENT) 
            {
            	return GetMarshalSizeOf_csharp();
            }
            else
            {
                //new method
                return GetUnmanagedSize();
            }
        }

        //only byValue type
        public byte[] ObjToBytes(object b)
        {
        	if (DONNOT_USE_REIMPLEMENT) 
        	{
        		return ObjToBytes_csharp(b);
        	}
        	else
        	{
        		//TODO:not implemented
        		return null;
        		//LuaDump.DumpMem not work
        		//LuaStrLib.writer not work
        	}
        }

        //TODO:need reimplementation
        public byte[] ObjToBytes2(object b)
        {
        	if (DONNOT_USE_REIMPLEMENT) 
        	{
        		return ObjToBytes2_csharp(b);
        	} 
        	else 
        	{
        		return ObjToBytes(b);
        	}
        }

        //TODO:need reimplementation
        public object bytesToObj(byte[] bytes)
        {
        	if (DONNOT_USE_REIMPLEMENT) 
        	{
        		return bytesToObj_csharp(bytes);
	        }
        	else
        	{
        		//TODO:not implemented
        		return null;
        		//LuaUndump.LoadMem not work
        	}
        }

        //number of ints inside a lua_Number
        public static int GetNumInts()
        {
            //return sizeof(Double/*lua_Number*/) / sizeof(int); //FIXME:
        	return 8 / 4;
        }

        public static int SizeOfInt()
        {
            //return sizeof(int); //FIXME:
            return 4;
        }

        public static int SizeOfLong()
        {
            //sizeof(long/*uint*/)
            //sizeof(long/*UInt32*//*Instruction*/));
            //return sizeof(long); //FIXME:
            return 8;
        }

        public static int SizeOfDouble()
        {
            //sizeof(Double/*lua_Number*/)
            //return sizeof(double);//FIXME:
            return 8;
        }

        public static Double ConvertToSingle(object o)
        {
            //return Convert.ToSingle(o); //FIXME:
            return Convert.ToSingle(o.ToString());
        }

        public static char ConvertToChar(String str)
        {
            return Convert.ToChar(str);
        }

        public static int ConvertToInt32(String str)
        {
            return Convert.ToInt32(str);
        }

        public static int ConvertToInt32(long i)
        {
            //return Convert.ToInt32(i); //FIXME:
            return (int)i;
        }

        public static int ConvertToInt32_object(object i)
        {
            //return Convert.ToInt32(i);//FIXME:
            return Convert.ToInt32(i.ToString());
        }

        public static double ConvertToDouble(String str, bool[] isSuccess)
        {
            if (isSuccess != null)
            {
                isSuccess[0] = true;
            }
            try
            {
                return Convert.ToDouble(str);
            }
            catch (System.OverflowException)
            {
                // this is a hack, fix it - mjf
                if (str[0] == '-')
                {
                    return System.Double.NegativeInfinity;
                }
                else
                {
                    return System.Double.PositiveInfinity;
                }
            }
            catch
			{
                if (isSuccess != null)
                {
                    isSuccess[0] = false;
                }
                return 0;
			}
        }

        public static bool isNaN(double d)
        {
            return Double.IsNaN(d);
        }

        public static int log2(double x)
        {
            if (DONNOT_USE_REIMPLEMENT)
            {
                return log2_csharp(x);
            }
            else
            {
                return (int)(Math.Log(x) / Math.Log(2));
            }
        }

        public static double ConvertToInt32(object obj)
        {
        	//return Convert.ToInt32(obj);//FIXME:
        	return Convert.ToInt32(obj.ToString());
        }

        public static bool IsPunctuation(char c)
        {
            return Char.IsPunctuation(c);
        }

        public static int IndexOfAny(string str, char[] anyOf)
        {
            if (DONNOT_USE_REIMPLEMENT)
            {
                return IndexOfAny_csharp(str, anyOf);
            }
            else
            {
                int index = -1;
                for (int i = 0; i < anyOf.Length; i++)
                {
                    int index2 = str.IndexOf(anyOf[i]);
                    if (index2 >= 0)
                    {
                        if (index == -1)
                        {
                            index = index2;
                        }
                        else
                        {
                            if (index2 < index)
                            {
                                index = index2;
                            }
                        }
                    }
                }
                return index;
            }
        }

        public static void Assert(bool condition)
        {
        	if (DONNOT_USE_REIMPLEMENT)
        	{
        		Assert_csharp(condition);
        	}
        	else
        	{
        		if (!condition)
        		{
        			throw new Exception("Assert");
        		}
        	}
        }
        
        public static void Assert(bool condition, string message)
        {
        	if (DONNOT_USE_REIMPLEMENT)
        	{
        		Assert_csharp(condition, message);
        	}
        	else
        	{
        		if (!condition)
        		{
        			throw new Exception(message);
        		}
        	}
        }
        
        public static int processExec(string strCmdLine)
        {
        	if (DONNOT_USE_REIMPLEMENT)
        	{
        		return processExec_csharp(strCmdLine);
        	}
        	else
        	{
        		//TODO:not implemented
        		return 0;
        		//LuaOSLib.os_execute
        	}
        }
        
        //object[] to T[]
        public object ToArray(object[] arr)
        {
        	if (DONNOT_USE_REIMPLEMENT)
        	{
        		return ToArray_csharp(arr);
        	}
        	else
        	{
        		//TODO:not implemented
        		return null;
        		//LuaUndump
        	}
        }

        public static byte[] GetBytes(double d)
        {
            //FIXME:
            //long value = Double.doubleToRawLongBits(d);  
            //byte[] byteRet = new byte[8];  
            //for (int i = 0; i < 8; i++) {  
            //    byteRet[i] = (byte) ((value >> 8 * i) & 0xff);  
            //}
            //return byteRet;  
            return BitConverter.GetBytes(d);
        }

        //--------------------------------
        //csharp only implementations
        //--------------------------------
        
		//using System.Runtime.Serialization.Formatters.Binary;
        private byte[] ObjToBytes2_csharp(object b)
        {
            byte[] bytes = new byte[0];
            MemoryStream stream = new MemoryStream();
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, b);
                stream.Flush();
                bytes = stream.GetBuffer();
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
            }
            return bytes;
        }
        
        private int GetMarshalSizeOf_csharp()
        {
        	return Marshal.SizeOf(this.GetOriginalType_csharp());
        }
        
        private object bytesToObj_csharp(byte[] bytes)
        {
        	GCHandle pinnedPacket = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            object b = Marshal.PtrToStructure(pinnedPacket.AddrOfPinnedObject(), GetOriginalType_csharp());
            pinnedPacket.Free();
            return b;  
        }
        
        private byte[] ObjToBytes_csharp(object b)
        {
    		int size = Marshal.SizeOf(b);
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(b, ptr, false);
            byte[] bytes = new byte[size];
            Marshal.Copy(ptr, bytes, 0, size);
            return bytes;
        }
        
        private static int processExec_csharp(string strCmdLine)
        {
    		Process proc = new Process();
			proc.EnableRaisingEvents = false;
			proc.StartInfo.FileName = "CMD.exe";
			proc.StartInfo.Arguments = "/C regenresx " + strCmdLine;
			proc.Start();
			proc.WaitForExit();
			return proc.ExitCode;
        }
        
        private object Alloc_csharp()
        {
        	return Activator.CreateInstance(this.GetOriginalType_csharp());
        }
        
        private static void Assert_csharp(bool condition)
        {
        	Debug.Assert(condition);
        }
        
        private static void Assert_csharp(bool condition, string message)
        {
        	Debug.Assert(condition, message);
        }
        
        private Type GetOriginalType_csharp()
        {
            if (type == TYPE_CHAR)
            {
                return typeof(char);
            }
            else if (type == TYPE_INT)
            {
                return typeof(int);
            }
            else if (type == TYPE_DOUBLE)
            {
                return typeof(Double);
            }
            else if (type == TYPE_LONG)
            {
                return typeof(long);
            }
            else if (type == TYPE_LG)
            {
                return typeof(LG);
            }
            else if (type == TYPE_FILEPTR)
            {
                return typeof(FilePtr);
            }
            else if (type == TYPE_TVALUE)
            {
                return typeof(TValue);
            }
            else if (type == TYPE_CCLOSURE)
            {
                return typeof(CClosure);
            }
            else if (type == TYPE_LCLOSURE)
            {
                return typeof(LClosure);
            }
            else if (type == TYPE_TABLE)
            {
                return typeof(Table);
            }
            else if (type == TYPE_GCOBJECTREF)
            {
                return typeof(GCObjectRef); //FIXME:interface!!!
            }
            else if (type == TYPE_TSTRING)
            {
                return typeof(TString);
            }
            else if (type == TYPE_NODE)
            {
                return typeof(Node);
            }
            else if (type == TYPE_UDATA)
            {
                return typeof(Udata);
            }
            else if (type == TYPE_LUA_STATE)
            {
                return typeof(lua_State);
            }
            else if (type == TYPE_CALLINFO)
            {
                return typeof(CallInfo);
            }
            else if (type == TYPE_PROTO)
            {
                return typeof(Proto);
            }
            else if (type == TYPE_LOCVAR)
            {
                return typeof(LocVar);
            }
            else if (type == TYPE_CLOSURE)
            {
                return typeof(Closure);
            }
            else if (type == TYPE_UPVAL)
            {
                return typeof(UpVal);
            }
            else if (type == TYPE_INT32)
            {
                return typeof(Int32);//FIXME:
            }
            else if (type == TYPE_GCOBJECT)
            {
                return typeof(GCObject);
            }
            else if (type == TYPE_CHARPTR)
            {
                return typeof(CharPtr);
            }
            return null;
        }
        
        private object ToArray_csharp(object[] arr)
        {
        	ArrayList array = new ArrayList();
			for (int i = 0; i < arr.Length; i++)
			{
				array.Add(array[i]);
			}
			return array.ToArray(this.GetOriginalType_csharp());
        }
        
        private bool CanIndex_csharp()
        {
        	Type t = this.GetOriginalType_csharp();
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
            /*
            if (t == typeof(uint))
            {
                return false;
            }*/
            if (t == typeof(LocVar))
            {
                return false;
            }
            if (t == typeof(long))
            {
                return false;
            }
            return true;
        }
        
        private string GetTypeString_csharp()
        {
        	return this.GetOriginalType_csharp().GetType().ToString();
        }
        
        private int GetUnmanagedSize_csharp()
        {
        	Type t = this.GetOriginalType_csharp();
            if (t == typeof(LG))
            {
                return 376;
            }
           	else if (t == typeof(global_State))
            {
                return 228;
            }
            else if (t == typeof(CallInfo))
            {
                return 24;
            }
            else if (t == typeof(TValue))
            {
                return 16;
            }
            else if (t == typeof(Table))
            {
                return 32;
            }
            else if (t == typeof(Node))
            {
                return 32;
            }
            else if (t == typeof(GCObject))
            {
                return 120;
            }
            else if (t == typeof(GCObjectRef))
            {
                return 4;
            }
            else if (t == typeof(ArrayRef))
            {
                return 4;
            }
            else if (t == typeof(Closure))
            {
                return 0;	// handle this one manually in the code
            }
            else if (t == typeof(Proto))
            {
                return 76;
            }
            else if (t == typeof(luaL_Reg))
            {
                return 8;
            }
            else if (t == typeof(luaL_Buffer))
            {
                return 524;
            }
            else if (t == typeof(lua_State))
            {
                return 120;
            }
            else if (t == typeof(lua_Debug))
            {
                return 100;
            }
            else if (t == typeof(CallS))
            {
                return 8;
            }
            else if (t == typeof(LoadF))
            {
                return 520;
            }
            else if (t == typeof(LoadS))
            {
                return 8;
            }
            else if (t == typeof(lua_longjmp))
            {
                return 72;
            }
            else if (t == typeof(SParser))
            {
                return 20;
            }
            else if (t == typeof(Token))
            {
                return 16;
            }
            else if (t == typeof(LexState))
            {
                return 52;
            }
            else if (t == typeof(FuncState))
            {
                return 572;
            }
            else if (t == typeof(GCheader))
            {
                return 8;
            }
            else if (t == typeof(TValue))
            {
                return 16;
            }
            else if (t == typeof(TString))
            {
                return 16;
            }
            else if (t == typeof(LocVar))
            {
                return 12;
            }
            else if (t == typeof(UpVal))
            {
                return 32;
            }
            else if (t == typeof(CClosure))
            {
                return 40;
            }
            else if (t == typeof(LClosure))
            {
                return 24;
            }
            else if (t == typeof(TKey))
            {
                return 16;
            }
            else if (t == typeof(ConsControl))
            {
                return 40;
            }
            else if (t == typeof(LHS_assign))
            {
                return 32;
            }
            else if (t == typeof(expdesc))
            {
                return 24;
            }
            else if (t == typeof(upvaldesc))
            {
                return 2;
            }
            else if (t == typeof(BlockCnt))
            {
                return 12;
            }
            else if (t == typeof(ZIO/*Zio*/))
            {
                return 20;
            }
            else if (t == typeof(Mbuffer))
            {
                return 12;
            }
            else if (t == typeof(LoadState))
            {
                return 16;
            }
            else if (t == typeof(MatchState))
            {
                return 272;
            }
            else if (t == typeof(stringtable))
            {
                return 12;
            }
            else if (t == typeof(FilePtr))
            {
                return 4;
            }
            else if (t == typeof(Udata))
            {
                return 24;
            }
            else if (t == typeof(Char))
            {
                return 1;
            }
            else if (t == typeof(UInt16))
            {
                return 2;
            }
            else if (t == typeof(Int16))
            {
                return 2;
            }
            else if (t == typeof(UInt32))
            {
                return 4;
            }
            else if (t == typeof(Int32))
            {
                return 4;
            }
            else if (t == typeof(Single))
            {
                return 4;
            }
            else if (t == typeof(long))
            {
                return 8;
            }
            Assert_csharp(false, "Trying to get unknown sized of unmanaged type " + t.ToString());
            return 0;
        }

        public static int IndexOfAny_csharp(string str, char[] anyOf)
        {
            return str.IndexOfAny(anyOf);
        }

        public static int log2_csharp(double x)
        {
            return (int)Math.Log(x, 2);
        }
    }
}
