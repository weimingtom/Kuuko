/*
 ** $Id: lobject.c,v 2.22.1.1 2007/12/27 13:02:25 roberto Exp $
 ** Some generic functions over Lua objects
 ** See Copyright Notice in lua.h
 */
using System;

namespace KopiLua
{
	//using TValue = Lua.TValue;
	//using StkId = TValue;
	//using lu_byte = System.Byte;
	//using lua_Number = System.Double;
	//using l_uacNumber = System.Double;
	//using Instruction = System.UInt32;

	public class LuaObject
	{
		/* tags for values visible from Lua */
		public const int LAST_TAG = Lua.LUA_TTHREAD;

		public const int NUM_TAGS = (LAST_TAG + 1);

		/*
		 ** Extra tags for non-values
		 */
		public const int LUA_TPROTO	= (LAST_TAG + 1);
		public const int LUA_TUPVAL	= (LAST_TAG + 2);
		public const int LUA_TDEADKEY = (LAST_TAG + 3);

		/*
		 ** Tagged Values
		 */

		//#define TValuefields	Value value; int tt
		
		/* Macros to test type */
		public static bool ttisnil(TValue o) 
		{ 
			return (ttype(o) == Lua.LUA_TNIL); 
		}
		
		public static bool ttisnumber(TValue o) 
		{ 
			return (ttype(o) == Lua.LUA_TNUMBER); 
		}
		
		public static bool ttisstring(TValue o) 
		{ 
			return (ttype(o) == Lua.LUA_TSTRING); 
		}
		
		public static bool ttistable(TValue o) 
		{ 
			return (ttype(o) == Lua.LUA_TTABLE); 
		}
		
		public static bool ttisfunction(TValue o) 
		{
			return (ttype(o) == Lua.LUA_TFUNCTION); 
		}
		
		public static bool ttisboolean(TValue o)	
		{
			return (ttype(o) == Lua.LUA_TBOOLEAN);
		}
		
		public static bool ttisuserdata(TValue o) 
		{ 
			return (ttype(o) == Lua.LUA_TUSERDATA); 
		}
		
		public static bool ttisthread(TValue o) 
		{ 
			return (ttype(o) == Lua.LUA_TTHREAD); 
		}
		
		public static bool ttislightuserdata(TValue o) 
		{ 
			return (ttype(o) == Lua.LUA_TLIGHTUSERDATA); 
		}

		/* Macros to access values */
		public static int ttype(TValue o) 
		{ 
			return o.tt; 
		}
		
		public static int ttype(CommonHeader o) 
		{ 
			return o.tt; 
		}
		
		public static GCObject gcvalue(TValue o) 
		{ 
			return (GCObject)LuaLimits.check_exp(iscollectable(o), o.value.gc); 
		}
		
		public static object pvalue(TValue o) 
		{ 
			return (object)LuaLimits.check_exp(ttislightuserdata(o), o.value.p); 
		}
		
		public static Double/*lua_Number*/ nvalue(TValue o) 
		{ 
			return (Double/*lua_Number*/)LuaLimits.check_exp(ttisnumber(o), o.value.n); 
		}
		
		public static TString rawtsvalue(TValue o) 
		{ 
			return (TString)LuaLimits.check_exp(ttisstring(o), o.value.gc.getTs()); 
		}
		
		public static TString_tsv tsvalue(TValue o) 
		{ 
			return rawtsvalue(o).getTsv(); 
		}
		
		public static Udata rawuvalue(TValue o) 
		{ 
			return (Udata)LuaLimits.check_exp(ttisuserdata(o), o.value.gc.getU()); 
		}
		
		public static Udata_uv uvalue(TValue o) 
		{ 
			return rawuvalue(o).uv; 
		}
		
		public static Closure clvalue(TValue o) 
		{ 
			return (Closure)LuaLimits.check_exp(ttisfunction(o), o.value.gc.getCl()); 
		}
		
		public static Table hvalue(TValue o) 
		{ 
			return (Table)LuaLimits.check_exp(ttistable(o), o.value.gc.getH()); 
		}
		
		public static int bvalue(TValue o)	
		{
			return (int)LuaLimits.check_exp(ttisboolean(o), o.value.b);
		}
		
		public static lua_State thvalue(TValue o) 
		{ 
			return (lua_State)LuaLimits.check_exp(ttisthread(o), o.value.gc.getTh()); 
		}

		public static int l_isfalse(TValue o) 
		{ 
			return ((ttisnil(o) || (ttisboolean(o) && bvalue(o) == 0))) ? 1 : 0; 
		}

		/*
		 ** for internal debug only
		 */
		public static void checkconsistency(TValue obj)
		{
			LuaLimits.lua_assert(!iscollectable(obj) || (ttype(obj) == (obj).value.gc.getGch().tt));
		}

		public static void checkliveness(global_State g, TValue obj)
		{
			LuaLimits.lua_assert(!iscollectable(obj) ||
				((ttype(obj) == obj.value.gc.getGch().tt) && !LuaGC.isdead(g, obj.value.gc)));
		}


		/* Macros to set values */
		public static void setnilvalue(TValue obj) 
		{
			obj.tt = Lua.LUA_TNIL;
		}

		public static void setnvalue(TValue obj, Double/*lua_Number*/ x)
		{
			obj.value.n = x;
			obj.tt = Lua.LUA_TNUMBER;
		}

		public static void setpvalue(TValue obj, object x) 
		{
			obj.value.p = x;
			obj.tt = Lua.LUA_TLIGHTUSERDATA;
		}

		public static void setbvalue(TValue obj, int x) 
		{
            obj.value.b = x;
			obj.tt = Lua.LUA_TBOOLEAN;
		}

		public static void setsvalue(lua_State L, TValue obj, GCObject x) 
		{
			obj.value.gc = x;
			obj.tt = Lua.LUA_TSTRING;
			checkliveness(LuaState.G(L), obj);
		}

		public static void setuvalue(lua_State L, TValue obj, GCObject x) 
		{
            obj.value.gc = x;
			obj.tt = Lua.LUA_TUSERDATA;
			checkliveness(LuaState.G(L), obj);
		}

		public static void setthvalue(lua_State L, TValue obj, GCObject x) 
		{
			obj.value.gc = x;
			obj.tt = Lua.LUA_TTHREAD;
			checkliveness(LuaState.G(L), obj);
		}

		public static void setclvalue(lua_State L, TValue obj, Closure x) 
		{
			obj.value.gc = x;
			obj.tt = Lua.LUA_TFUNCTION;
			checkliveness(LuaState.G(L), obj);
		}

		public static void sethvalue(lua_State L, TValue obj, Table x) 
		{
			obj.value.gc = x;
			obj.tt = Lua.LUA_TTABLE;
			checkliveness(LuaState.G(L), obj);
		}

		public static void setptvalue(lua_State L, TValue obj, Proto x) 
		{
			obj.value.gc = x;
			obj.tt = LUA_TPROTO;
			checkliveness(LuaState.G(L), obj);
		}

		public static void setobj(lua_State L, TValue obj1, TValue obj2) 
		{
			obj1.value.copyFrom(obj2.value);
			obj1.tt = obj2.tt;
			checkliveness(LuaState.G(L), obj1);
		}


		/*
		 ** different types of sets, according to destination
		 */

		/* from stack to (same) stack */
		//#define setobjs2s	setobj
		public static void setobjs2s(lua_State L, TValue obj, TValue x) 
		{ 
			setobj(L, obj, x); 
		}
		//to stack (not from same stack)
		
		//#define setobj2s	setobj
		public static void setobj2s(lua_State L, TValue obj, TValue x) 
		{ 
			setobj(L, obj, x); 
		}

		//#define setsvalue2s	setsvalue
		public static void setsvalue2s(lua_State L, TValue obj, TString x) 
		{ 
			setsvalue(L, obj, x); 
		}

		//#define sethvalue2s	sethvalue
		public static void sethvalue2s(lua_State L, TValue obj, Table x) 
		{ 
			sethvalue(L, obj, x); 
		}

		//#define setptvalue2s	setptvalue
		public static void setptvalue2s(lua_State L, TValue obj, Proto x) 
		{ 
			setptvalue(L, obj, x); 
		}

		// from table to same table 
		//#define setobjt2t	setobj
		public static void setobjt2t(lua_State L, TValue obj, TValue x) 
		{ 
			setobj(L, obj, x); 
		}

		// to table 
		//#define setobj2t	setobj
		public static void setobj2t(lua_State L, TValue obj, TValue x) 
		{ 
			setobj(L, obj, x); 
		}

		// to new object 
		//#define setobj2n	setobj
		public static void setobj2n(lua_State L, TValue obj, TValue x) 
		{ 
			setobj(L, obj, x); 
		}

		//#define setsvalue2n	setsvalue
		public static void setsvalue2n(lua_State L, TValue obj, TString x) 
		{ 
			setsvalue(L, obj, x); 
		}

		public static void setttype(TValue obj, int tt) 
		{
			obj.tt = tt;
		}

		public static bool iscollectable(TValue o) 
		{ 
			return (ttype(o) >= Lua.LUA_TSTRING); 
		}

		public static CharPtr getstr(TString ts) 
		{ 
			return ts.str; 
		}
		
		public static CharPtr svalue(TValue/*StkId*/ o) 
		{ 
			return getstr(rawtsvalue(o)); 
		}

		/* masks for new-style vararg */
		public const int VARARG_HASARG = 1;
		public const int VARARG_ISVARARG = 2;
		public const int VARARG_NEEDSARG = 4;

		public static bool iscfunction(TValue o) 
		{ 
			return ((ttype(o) == Lua.LUA_TFUNCTION) && (clvalue(o).c.getIsC() != 0)); 
		}
		
		public static bool isLfunction(TValue o) 
		{ 
			return ((ttype(o) == Lua.LUA_TFUNCTION) && (clvalue(o).c.getIsC() == 0)); 
		}

		/*
		 ** `module' operation for hashing (size is always a power of 2)
		 */
		//#define lmod(s,size) \
		//    (check_exp((size&(size-1))==0, (cast(int, (s) & ((size)-1)))))

		public static int twoto(int x) 
		{ 
			return 1 << x; 
		}
		
		public static int sizenode(Table t)	
		{
			return twoto(t.lsizenode);
		}

		public static TValue luaO_nilobject_ = new TValue(new Value(), Lua.LUA_TNIL);
		public static TValue luaO_nilobject = luaO_nilobject_;

		public static int ceillog2(int x)	
		{
			return luaO_log2((/*uint*/int)(x - 1)) + 1;
		}
		
		/*
		 ** converts an integer to a "floating point byte", represented as
		 ** (eeeeexxx), where the real value is (1xxx) * 2^(eeeee - 1) if
		 ** eeeee != 0 and (xxx) otherwise.
		 */
		public static int luaO_int2fb(int/*uint*/ x) 
		{
			int e = 0;  /* expoent */
			while (x >= 16) 
			{
				x = (x+1) >> 1;
				e++;
			}
			if (x < 8) 
			{
				return (int)x;
			}
			else 
			{
				return ((e + 1) << 3) | (LuaLimits.cast_int(x) - 8);
			}
		}

		/* converts back */
		public static int luaO_fb2int(int x) 
		{
			int e = (x >> 3) & 31;
			if (e == 0) 
			{
				return x;
			}
			else 
			{
				return ((x & 7)+8) << (e - 1);
			}
		}

		private readonly static Byte/*lu_byte*/[] log_2 = {
			0,1,2,2,3,3,3,3,4,4,4,4,4,4,4,4,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,
			6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,
			7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,
			7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,
			8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,
			8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,
			8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,
			8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8
		};

		public static int luaO_log2(long/*uint*/ x) 
		{
			int l = -1;
			while (x >= 256) 
			{ 
				l += 8; 
				x >>= 8; 
			}
			return l + log_2[(int)x];
		}

		public static int luaO_rawequalObj(TValue t1, TValue t2) 
		{
			if (ttype(t1) != ttype(t2)) 
			{
				return 0;
			}
			else
			{
				switch (ttype(t1)) 
				{
					case Lua.LUA_TNIL:
						{
							return 1;
						}
					case Lua.LUA_TNUMBER:
						{
							return LuaConf.luai_numeq(nvalue(t1), nvalue(t2)) ? 1 : 0;
						}
					case Lua.LUA_TBOOLEAN:
						{
							return bvalue(t1) == bvalue(t2) ? 1 : 0;  /* boolean true must be 1....but not in C# !! */
						}
					case Lua.LUA_TLIGHTUSERDATA:
						{
							return pvalue(t1) == pvalue(t2) ? 1 : 0;
						}
					default:
						{
							LuaLimits.lua_assert(iscollectable(t1));
							return gcvalue(t1) == gcvalue(t2) ? 1 : 0;
						}
				}
			}
		}

		public static int luaO_str2d(CharPtr s, /*out*/ Double[]/*lua_Number*/ result)
		{
			CharPtr[] endptr = new CharPtr[1];
			endptr[0] = new CharPtr();
			result[0] = LuaConf.lua_str2number(s, /*out*/ endptr);
			if (CharPtr.isEqual(endptr[0], s))
			{
				return 0;  /* conversion failed */
			}
			if (endptr[0].get(0) == 'x' || endptr[0].get(0) == 'X')  /* maybe an hexadecimal constant? */
			{
				result[0] = LuaLimits.cast_num(LuaConf.strtoul(s, /*out*/ endptr, 16));
			}
			if (endptr[0].get(0) == '\0') 
			{
				return 1;  /* most common case */
			}
			while (LuaConf.isspace(endptr[0].get(0))) 
			{
				endptr[0] = endptr[0].next();
			}
			if (endptr[0].get(0) != '\0') 
			{
				return 0;  /* invalid trailing characters? */
			}
			return 1;
		}

		private static void pushstr(lua_State L, CharPtr str) 
		{
			setsvalue2s(L, L.top, LuaString.luaS_new(L, str));
			LuaDo.incr_top(L);
		}

		/* this function handles only `%d', `%c', %f, %p, and `%s' formats */
		public static CharPtr luaO_pushvfstring(lua_State L, CharPtr fmt, params object[] argp) 
		{
			int parm_index = 0;
			int n = 1;
			pushstr(L, CharPtr.toCharPtr(""));
			for (;;) 
			{
				CharPtr e = LuaConf.strchr(fmt, '%');
				if (CharPtr.isEqual(e, null)) 
				{
					break;
				}
				setsvalue2s(L, L.top, LuaString.luaS_newlstr(L, fmt, /*(uint)*/CharPtr.minus(e, fmt)));
				LuaDo.incr_top(L);
				switch (e.get(1)) 
				{
					case 's': 
						{
							object o = argp[parm_index++];
							CharPtr s = o as CharPtr;
							if (CharPtr.isEqual(s, null))
							{
								s = CharPtr.toCharPtr((string)o);
							}
							if (CharPtr.isEqual(s, null)) 
							{
								s = CharPtr.toCharPtr("(null)");
							}
							pushstr(L, s);
							break;
						}
					case 'c': 
						{
							CharPtr buff = CharPtr.toCharPtr(new char[2]);
							buff.set(0, (char)(int)argp[parm_index++]);
							buff.set(1, '\0');
							pushstr(L, buff);
							break;
						}
					case 'd': 
						{
							setnvalue(L.top, (int)argp[parm_index++]);
							LuaDo.incr_top(L);
							break;
						}
					case 'f': 
						{
							setnvalue(L.top, (Double/*l_uacNumber*/)argp[parm_index++]);
							LuaDo.incr_top(L);
							break;
						}
					case 'p': 
						{
							//CharPtr buff = new char[4*sizeof(void *) + 8]; /* should be enough space for a `%p' */
							CharPtr buff = CharPtr.toCharPtr(new char[32]);
							LuaConf.sprintf(buff, CharPtr.toCharPtr("0x%08x"), argp[parm_index++].GetHashCode());
							pushstr(L, buff);
							break;
						}
					case '%': 
						{
							pushstr(L, CharPtr.toCharPtr("%"));
							break;
						}
					default: 
						{
							CharPtr buff = CharPtr.toCharPtr(new char[3]);
							buff.set(0, '%');
							buff.set(1, e.get(1));
							buff.set(2, '\0');
							pushstr(L, buff);
							break;
						}
				}
				n += 2;
				fmt = CharPtr.plus(e, 2);
			}
			pushstr(L, fmt);
			LuaVM.luaV_concat(L, n + 1, LuaLimits.cast_int(TValue.minus(L.top, L.base_)) - 1);
			L.top = TValue.minus(L.top, n);
			return svalue(TValue.minus(L.top, 1));
		}

		public static CharPtr luaO_pushfstring(lua_State L, CharPtr fmt, params object[] args)
		{
			return luaO_pushvfstring(L, fmt, args);
		}

		public static void luaO_chunkid(CharPtr out_, CharPtr source, int/*uint*/ bufflen) 
		{
			//out_ = "";
			if (source.get(0) == '=') 
			{
				LuaConf.strncpy(out_, CharPtr.plus(source, 1), /*(int)*/bufflen);  /* remove first char */
				out_.set(bufflen - 1, '\0');  /* ensures null termination */
			}
			else
			{ 
				/* out = "source", or "...source" */
				if (source.get(0) == '@') 
				{
					int/*uint*/ l;
					source = source.next();  /* skip the `@' */
					bufflen -= /*(uint)*/(" '...' ".Length + 1); //FIXME:
					l = /*(uint)*/LuaConf.strlen(source);
					LuaConf.strcpy(out_, CharPtr.toCharPtr(""));
					if (l > bufflen) 
					{
						source = CharPtr.plus(source, (l - bufflen));  /* get last part of file name */
						LuaConf.strcat(out_, CharPtr.toCharPtr("..."));
					}
					LuaConf.strcat(out_, source);
				}
				else 
				{  
					/* out = [string "string"] */
					int/*uint*/ len = LuaConf.strcspn(source, CharPtr.toCharPtr("\n\r"));  /* stop at first newline */
					bufflen -= /*(uint)*/(" [string \"...\"] ".Length + 1);
					if (len > bufflen) 
					{
						len = bufflen;
					}
					LuaConf.strcpy(out_, CharPtr.toCharPtr("[string \""));
					if (source.get(len) != '\0') 
					{  
						/* must truncate? */
						LuaConf.strncat(out_, source, (int)len);
						LuaConf.strcat(out_, CharPtr.toCharPtr("..."));
					}
					else
					{
						LuaConf.strcat(out_, source);
					}
					LuaConf.strcat(out_, CharPtr.toCharPtr("\"]"));
				}
			}
		}
	}
}
