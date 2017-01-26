using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace KopiLua
{
    public class ClassType
    {
        public const int TYPE_CHAR = 1;//char //---
        public const int TYPE_INT = 2;//int //typeof(int/*uint*/)
        public const int TYPE_DOUBLE = 3; //Double, Lua_Number
        public const int TYPE_LONG = 4; ///*UInt32*//*Instruction*/ //---
        public const int TYPE_LG = 5;//LG
        public const int TYPE_FILEPTR = 6;//FilePtr
        public const int TYPE_TVALUE = 7;//TValue; //---
        public const int TYPE_CCLOSURE = 8;//CClosure
        public const int TYPE_LCLOSURE = 9;//LClosure
        public const int TYPE_TABLE = 10;//Table
        public const int TYPE_GCOBJECTREF = 11;//GCObjectRef
        public const int TYPE_TSTRING = 12;//TString
        public const int TYPE_NODE = 13;//Node
        public const int TYPE_UDATA = 14;//Udata
        public const int TYPE_LUA_STATE = 15;//lua_State
        public const int TYPE_CALLINFO = 16;//CallInfo //---
        public const int TYPE_PROTO = 17;//Proto //---
        public const int TYPE_LOCVAR = 18;//LocVar
        //---
        public const int TYPE_CLOSURE = 19;//Closure
        public const int TYPE_UPVAL = 20;//UpVal
        public const int TYPE_INT32 = 21;//Int32
        public const int TYPE_GCOBJECT = 22;//GCObject
        
        private int type = 0;

        public ClassType(int type)
        {
            this.type = type;
        }

        public Type GetOriginalType()
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
            return null;
        }

        public object Alloc()
        {
            return System.Activator.CreateInstance(this.GetOriginalType());
        }

        public bool CanIndex()
        {
            Type t = this.GetOriginalType();
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

        public int GetUnmanagedSize()
        {
            Type t = this.GetOriginalType();
            if (t == typeof(global_State))
            {
                return 228;
            }
            else if (t == typeof(LG))
            {
                return 376;
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
            Debug.Assert(false, "Trying to get unknown sized of unmanaged type " + t.ToString());
            return 0;
        }
    }
}
