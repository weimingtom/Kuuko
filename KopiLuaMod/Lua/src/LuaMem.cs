/*
 ** $Id: lmem.c,v 1.70.1.1 2007/12/27 13:02:25 roberto Exp $
 ** Interface to Memory Manager
 ** See Copyright Notice in lua.h
 */
using System;

namespace KopiLua
{
	public class LuaMem
	{
		public const string MEMERRMSG = "not enough memory";

        //-------------------------------

        public static char[] luaM_reallocv_char(lua_State L, char[] block, int new_size, ClassType t)
        {
            return (char[])luaM_realloc__char(L, block, new_size, t);
        }

        public static TValue[] luaM_reallocv_TValue(lua_State L, TValue[] block, int new_size, ClassType t)
        {
            return (TValue[])luaM_realloc__TValue(L, block, new_size, t);
        }

        public static TString[] luaM_reallocv_TString(lua_State L, TString[] block, int new_size, ClassType t)
        {
            return (TString[])luaM_realloc__TString(L, block, new_size, t);
        }

        public static CallInfo[] luaM_reallocv_CallInfo(lua_State L, CallInfo[] block, int new_size, ClassType t)
        {
            return (CallInfo[])luaM_realloc__CallInfo(L, block, new_size, t);
        }

        public static long[] luaM_reallocv_long(lua_State L, long[] block, int new_size, ClassType t)
        {
            return (long[])luaM_realloc__long(L, block, new_size, t);
        }

        public static int[] luaM_reallocv_int(lua_State L, int[] block, int new_size, ClassType t)
        {
            return (int[])luaM_realloc__int(L, block, new_size, t);
        }

        public static Proto[] luaM_reallocv_Proto(lua_State L, Proto[] block, int new_size, ClassType t)
        {
            return (Proto[])luaM_realloc__Proto(L, block, new_size, t);
        }

        public static LocVar[] luaM_reallocv_LocVar(lua_State L, LocVar[] block, int new_size, ClassType t)
        {
            return (LocVar[])luaM_realloc__LocVar(L, block, new_size, t);
        }

        public static Node[] luaM_reallocv_Node(lua_State L, Node[] block, int new_size, ClassType t)
        {
            return (Node[])luaM_realloc__Node(L, block, new_size, t);
        }

        public static GCObject[] luaM_reallocv_GCObject(lua_State L, GCObject[] block, int new_size, ClassType t)
        {
            return (GCObject[])luaM_realloc__GCObject(L, block, new_size, t);
        }

        //-------------------------------

		//#define luaM_freemem(L, b, s)	luaM_realloc_(L, (b), (s), 0)
		//#define luaM_free(L, b)		luaM_realloc_(L, (b), sizeof(*(b)), 0)
		//public static void luaM_freearray(lua_State L, object b, int n, Type t) { luaM_reallocv(L, b, n, 0, Marshal.SizeOf(b)); }

		// C# has it's own gc, so nothing to do here...in theory...
		public static void luaM_freemem_Udata(lua_State L, Udata b, ClassType t) 
		{ 
			luaM_realloc__Udata(L, new Udata[] {b}, 0, t); 
		}

        public static void luaM_freemem_TString(lua_State L, TString b, ClassType t)
        {
            luaM_realloc__TString(L, new TString[] { b }, 0, t);
        }

        //-------------------------------

		public static void luaM_free_Table(lua_State L, Table b, ClassType t) 
		{ 
			luaM_realloc__Table(L, new Table[] {b}, 0, t); 
		}

        public static void luaM_free_UpVal(lua_State L, UpVal b, ClassType t)
        {
            luaM_realloc__UpVal(L, new UpVal[] { b }, 0, t);
        }

        public static void luaM_free_Proto(lua_State L, Proto b, ClassType t)
        {
            luaM_realloc__Proto(L, new Proto[] { b }, 0, t);
        }

        //-------------------------------

		public static void luaM_freearray_long(lua_State L, long[] b, ClassType t) 
		{ 
			luaM_reallocv_long(L, b, 0, t); 
		}

        public static void luaM_freearray_Proto(lua_State L, Proto[] b, ClassType t)
        {
            luaM_reallocv_Proto(L, b, 0, t);
        }

        public static void luaM_freearray_TValue(lua_State L, TValue[] b, ClassType t)
        {
            luaM_reallocv_TValue(L, b, 0, t);
        }

        public static void luaM_freearray_int(lua_State L, int[] b, ClassType t)
        {
            luaM_reallocv_int(L, b, 0, t);
        }

        public static void luaM_freearray_LocVar(lua_State L, LocVar[] b, ClassType t)
        {
            luaM_reallocv_LocVar(L, b, 0, t);
        }

        public static void luaM_freearray_TString(lua_State L, TString[] b, ClassType t)
        {
            luaM_reallocv_TString(L, b, 0, t);
        }

        public static void luaM_freearray_Node(lua_State L, Node[] b, ClassType t)
        {
            luaM_reallocv_Node(L, b, 0, t);
        }

        public static void luaM_freearray_CallInfo(lua_State L, CallInfo[] b, ClassType t)
        {
            luaM_reallocv_CallInfo(L, b, 0, t);
        }

        public static void luaM_freearray_GCObject(lua_State L, GCObject[] b, ClassType t)
        {
            luaM_reallocv_GCObject(L, b, 0, t);
        }

        //-------------------------------


		//public static T luaM_malloc<T>(lua_State L, ClassType t) 
		//{ 
		//	return (T)luaM_realloc_<T>(L, t); 
		//}

        public static Proto luaM_new_Proto(lua_State L, ClassType t) 
		{ 
			return (Proto)luaM_realloc__Proto(L, t); 
		}

        public static Closure luaM_new_Closure(lua_State L, ClassType t)
        {
            return (Closure)luaM_realloc__Closure(L, t);
        }

        public static UpVal luaM_new_UpVal(lua_State L, ClassType t)
        {
            return (UpVal)luaM_realloc__UpVal(L, t);
        }

        public static lua_State luaM_new_lua_State(lua_State L, ClassType t)
        {
            return (lua_State)luaM_realloc__lua_State(L, t);
        }

        public static Table luaM_new_Table(lua_State L, ClassType t)
        {
            return (Table)luaM_realloc__Table(L, t);
        }


        //-------------------------------

		public static long[] luaM_newvector_long(lua_State L, int n, ClassType t)
		{
			return luaM_reallocv_long(L, null, n, t);
		}

        public static TString[] luaM_newvector_TString(lua_State L, int n, ClassType t)
		{
            return luaM_reallocv_TString(L, null, n, t);
		}

        public static LocVar[] luaM_newvector_LocVar(lua_State L, int n, ClassType t)
        {
            return luaM_reallocv_LocVar(L, null, n, t);
        }

        public static int[] luaM_newvector_int(lua_State L, int n, ClassType t)
        {
            return luaM_reallocv_int(L, null, n, t);
        }

        public static Proto[] luaM_newvector_Proto(lua_State L, int n, ClassType t)
        {
            return luaM_reallocv_Proto(L, null, n, t);
        }

        public static TValue[] luaM_newvector_TValue(lua_State L, int n, ClassType t)
        {
            return luaM_reallocv_TValue(L, null, n, t);
        }

        public static CallInfo[] luaM_newvector_CallInfo(lua_State L, int n, ClassType t)
        {
            return luaM_reallocv_CallInfo(L, null, n, t);
        }

        public static Node[] luaM_newvector_Node(lua_State L, int n, ClassType t)
        {
            return luaM_reallocv_Node(L, null, n, t);
        }

        //-------------------------------







		public static void luaM_growvector_long(lua_State L, /*ref*/ long[][] v, int nelems, /*ref*/ int[] size, int limit, CharPtr e, ClassType t)
		{
			if (nelems + 1 > size[0])
			{
				v[0] = (long[])luaM_growaux__long(L, /*ref*/ v, /*ref*/ size, limit, e, t);
			}
		}

        public static void luaM_growvector_Proto(lua_State L, /*ref*/ Proto[][] v, int nelems, /*ref*/ int[] size, int limit, CharPtr e, ClassType t)
		{
			if (nelems + 1 > size[0])
			{
				v[0] = (Proto[])luaM_growaux__Proto(L, /*ref*/ v, /*ref*/ size, limit, e, t);
			}
		}

        public static void luaM_growvector_TString(lua_State L, /*ref*/ TString[][] v, int nelems, /*ref*/ int[] size, int limit, CharPtr e, ClassType t)
        {
            if (nelems + 1 > size[0])
            {
                v[0] = (TString[])luaM_growaux__TString(L, /*ref*/ v, /*ref*/ size, limit, e, t);
            }
        }

        public static void luaM_growvector_TValue(lua_State L, /*ref*/ TValue[][] v, int nelems, /*ref*/ int[] size, int limit, CharPtr e, ClassType t)
        {
            if (nelems + 1 > size[0])
            {
                v[0] = (TValue[])luaM_growaux__TValue(L, /*ref*/ v, /*ref*/ size, limit, e, t);
            }
        }

        public static void luaM_growvector_LocVar(lua_State L, /*ref*/ LocVar[][] v, int nelems, /*ref*/ int[] size, int limit, CharPtr e, ClassType t)
        {
            if (nelems + 1 > size[0])
            {
                v[0] = (LocVar[])luaM_growaux__LocVar(L, /*ref*/ v, /*ref*/ size, limit, e, t);
            }
        }

        public static void luaM_growvector_int(lua_State L, /*ref*/ int[][] v, int nelems, /*ref*/ int[] size, int limit, CharPtr e, ClassType t)
        {
            if (nelems + 1 > size[0])
            {
                v[0] = (int[])luaM_growaux__int(L, /*ref*/ v, /*ref*/ size, limit, e, t);
            }
        }

        //-------------------------------



		public static char[] luaM_reallocvector_char(lua_State L, /*ref*/ char[][] v, int oldn, int n, ClassType t)
		{
			ClassType.Assert((v[0] == null && oldn == 0) || (v[0].Length == oldn));
			v[0] = luaM_reallocv_char(L, v[0], n, t);
			return v[0];
		}

        public static TValue[] luaM_reallocvector_TValue(lua_State L, /*ref*/ TValue[][] v, int oldn, int n, ClassType t)
        {
            ClassType.Assert((v[0] == null && oldn == 0) || (v[0].Length == oldn));
            v[0] = luaM_reallocv_TValue(L, v[0], n, t);
            return v[0];
        }

        public static TString[] luaM_reallocvector_TString(lua_State L, /*ref*/ TString[][] v, int oldn, int n, ClassType t)
        {
            ClassType.Assert((v[0] == null && oldn == 0) || (v[0].Length == oldn));
            v[0] = luaM_reallocv_TString(L, v[0], n, t);
            return v[0];
        }

        public static CallInfo[] luaM_reallocvector_CallInfo(lua_State L, /*ref*/ CallInfo[][] v, int oldn, int n, ClassType t)
        {
            ClassType.Assert((v[0] == null && oldn == 0) || (v[0].Length == oldn));
            v[0] = luaM_reallocv_CallInfo(L, v[0], n, t);
            return v[0];
        }

        public static long[] luaM_reallocvector_long(lua_State L, /*ref*/ long[][] v, int oldn, int n, ClassType t)
        {
            ClassType.Assert((v[0] == null && oldn == 0) || (v[0].Length == oldn));
            v[0] = luaM_reallocv_long(L, v[0], n, t);
            return v[0];
        }

        public static int[] luaM_reallocvector_int(lua_State L, /*ref*/ int[][] v, int oldn, int n, ClassType t)
        {
            ClassType.Assert((v[0] == null && oldn == 0) || (v[0].Length == oldn));
            v[0] = luaM_reallocv_int(L, v[0], n, t);
            return v[0];
        }

        public static Proto[] luaM_reallocvector_Proto(lua_State L, /*ref*/ Proto[][] v, int oldn, int n, ClassType t)
        {
            ClassType.Assert((v[0] == null && oldn == 0) || (v[0].Length == oldn));
            v[0] = luaM_reallocv_Proto(L, v[0], n, t);
            return v[0];
        }

        public static LocVar[] luaM_reallocvector_LocVar(lua_State L, /*ref*/ LocVar[][] v, int oldn, int n, ClassType t)
        {
            ClassType.Assert((v[0] == null && oldn == 0) || (v[0].Length == oldn));
            v[0] = luaM_reallocv_LocVar(L, v[0], n, t);
            return v[0];
        }

        //-------------------------------


		/*
		 ** About the realloc function:
		 ** void * frealloc (void *ud, void *ptr, uint osize, uint nsize);
		 ** (`osize' is the old size, `nsize' is the new size)
		 **
		 ** Lua ensures that (ptr == null) iff (osize == 0).
		 **
		 ** * frealloc(ud, null, 0, x) creates a new block of size `x'
		 **
		 ** * frealloc(ud, p, x, 0) frees the block `p'
		 ** (in this specific case, frealloc must return null).
		 ** particularly, frealloc(ud, null, 0, 0) does nothing
		 ** (which is equivalent to free(null) in ANSI C)
		 **
		 ** frealloc returns null if it cannot create or reallocate the area
		 ** (any reallocation to an equal or smaller size cannot fail!)
		 */
		public const int MINSIZEARRAY = 4;


		public static long[] luaM_growaux__long(lua_State L, /*ref*/ long[][] block, /*ref*/ int[] size,
			int limit, CharPtr errormsg, ClassType t)
		{
			long[] newblock;
			int newsize;
			if (size[0] >= limit / 2)
			{  
				/* cannot double it? */
				if (size[0] >= limit)  /* cannot grow even a little? */
				{
					LuaDebug.luaG_runerror(L, errormsg);
				}
				newsize = limit;  /* still have at least one free place */
			}
			else
			{
				newsize = size[0] * 2;
				if (newsize < MINSIZEARRAY)
				{
					newsize = MINSIZEARRAY;  /* minimum size */
				}
			}
			newblock = luaM_reallocv_long(L, block[0], newsize, t);
			size[0] = newsize;  /* update only when everything else is OK */
			return newblock;
		}

        public static Proto[] luaM_growaux__Proto(lua_State L, /*ref*/ Proto[][] block, /*ref*/ int[] size,
                    int limit, CharPtr errormsg, ClassType t)
        {
            Proto[] newblock;
            int newsize;
            if (size[0] >= limit / 2)
            {
                /* cannot double it? */
                if (size[0] >= limit)  /* cannot grow even a little? */
                {
                    LuaDebug.luaG_runerror(L, errormsg);
                }
                newsize = limit;  /* still have at least one free place */
            }
            else
            {
                newsize = size[0] * 2;
                if (newsize < MINSIZEARRAY)
                {
                    newsize = MINSIZEARRAY;  /* minimum size */
                }
            }
            newblock = luaM_reallocv_Proto(L, block[0], newsize, t);
            size[0] = newsize;  /* update only when everything else is OK */
            return newblock;
        }

        public static TString[] luaM_growaux__TString(lua_State L, /*ref*/ TString[][] block, /*ref*/ int[] size,
                            int limit, CharPtr errormsg, ClassType t)
        {
            TString[] newblock;
            int newsize;
            if (size[0] >= limit / 2)
            {
                /* cannot double it? */
                if (size[0] >= limit)  /* cannot grow even a little? */
                {
                    LuaDebug.luaG_runerror(L, errormsg);
                }
                newsize = limit;  /* still have at least one free place */
            }
            else
            {
                newsize = size[0] * 2;
                if (newsize < MINSIZEARRAY)
                {
                    newsize = MINSIZEARRAY;  /* minimum size */
                }
            }
            newblock = luaM_reallocv_TString(L, block[0], newsize, t);
            size[0] = newsize;  /* update only when everything else is OK */
            return newblock;
        }

        public static TValue[] luaM_growaux__TValue(lua_State L, /*ref*/ TValue[][] block, /*ref*/ int[] size,
                            int limit, CharPtr errormsg, ClassType t)
        {
            TValue[] newblock;
            int newsize;
            if (size[0] >= limit / 2)
            {
                /* cannot double it? */
                if (size[0] >= limit)  /* cannot grow even a little? */
                {
                    LuaDebug.luaG_runerror(L, errormsg);
                }
                newsize = limit;  /* still have at least one free place */
            }
            else
            {
                newsize = size[0] * 2;
                if (newsize < MINSIZEARRAY)
                {
                    newsize = MINSIZEARRAY;  /* minimum size */
                }
            }
            newblock = luaM_reallocv_TValue(L, block[0], newsize, t);
            size[0] = newsize;  /* update only when everything else is OK */
            return newblock;
        }

        public static LocVar[] luaM_growaux__LocVar(lua_State L, /*ref*/ LocVar[][] block, /*ref*/ int[] size,
                              int limit, CharPtr errormsg, ClassType t)
        {
            LocVar[] newblock;
            int newsize;
            if (size[0] >= limit / 2)
            {
                /* cannot double it? */
                if (size[0] >= limit)  /* cannot grow even a little? */
                {
                    LuaDebug.luaG_runerror(L, errormsg);
                }
                newsize = limit;  /* still have at least one free place */
            }
            else
            {
                newsize = size[0] * 2;
                if (newsize < MINSIZEARRAY)
                {
                    newsize = MINSIZEARRAY;  /* minimum size */
                }
            }
            newblock = luaM_reallocv_LocVar(L, block[0], newsize, t);
            size[0] = newsize;  /* update only when everything else is OK */
            return newblock;
        }

        public static int[] luaM_growaux__int(lua_State L, /*ref*/ int[][] block, /*ref*/ int[] size,
                              int limit, CharPtr errormsg, ClassType t)
        {
            int[] newblock;
            int newsize;
            if (size[0] >= limit / 2)
            {
                /* cannot double it? */
                if (size[0] >= limit)  /* cannot grow even a little? */
                {
                    LuaDebug.luaG_runerror(L, errormsg);
                }
                newsize = limit;  /* still have at least one free place */
            }
            else
            {
                newsize = size[0] * 2;
                if (newsize < MINSIZEARRAY)
                {
                    newsize = MINSIZEARRAY;  /* minimum size */
                }
            }
            newblock = luaM_reallocv_int(L, block[0], newsize, t);
            size[0] = newsize;  /* update only when everything else is OK */
            return newblock;
        }

        //-------------------------------

		public static object luaM_toobig(lua_State L) 
		{
			LuaDebug.luaG_runerror(L, CharPtr.toCharPtr("memory allocation error: block too big"));
			return null;  /* to avoid warnings */
		}

		/*
		 ** generic allocation routine.
		 */
		public static object luaM_realloc_(lua_State L, ClassType t)
		{
			int unmanaged_size = (int)LuaConf.GetUnmanagedSize(t);
			int nsize = unmanaged_size;
			object new_obj = t.Alloc();
			AddTotalBytes(L, nsize);
			return new_obj;
		}

		public static object luaM_realloc__Proto(lua_State L, ClassType t)
		{
			int unmanaged_size = (int)t.GetUnmanagedSize();//LuaConf.GetUnmanagedSize(typeof(T));
			int nsize = unmanaged_size;
            Proto new_obj = (Proto)t.Alloc();//System.Activator.CreateInstance(typeof(T));
			AddTotalBytes(L, nsize);
			return new_obj;
		}

        public static object luaM_realloc__Closure(lua_State L, ClassType t)
        {
            int unmanaged_size = (int)t.GetUnmanagedSize();//LuaConf.GetUnmanagedSize(typeof(T));
            int nsize = unmanaged_size;
            Closure new_obj = (Closure)t.Alloc();//System.Activator.CreateInstance(typeof(T));
            AddTotalBytes(L, nsize);
            return new_obj;
        }

        public static object luaM_realloc__UpVal(lua_State L, ClassType t)
        {
            int unmanaged_size = (int)t.GetUnmanagedSize();//LuaConf.GetUnmanagedSize(typeof(T));
            int nsize = unmanaged_size;
            UpVal new_obj = (UpVal)t.Alloc();//System.Activator.CreateInstance(typeof(T));
            AddTotalBytes(L, nsize);
            return new_obj;
        }

        public static object luaM_realloc__lua_State(lua_State L, ClassType t)
        {
            int unmanaged_size = (int)t.GetUnmanagedSize();//LuaConf.GetUnmanagedSize(typeof(T));
            int nsize = unmanaged_size;
            lua_State new_obj = (lua_State)t.Alloc();//System.Activator.CreateInstance(typeof(T));
            AddTotalBytes(L, nsize);
            return new_obj;
        }

        public static object luaM_realloc__Table(lua_State L, ClassType t)
        {
            int unmanaged_size = (int)t.GetUnmanagedSize();//LuaConf.GetUnmanagedSize(typeof(T));
            int nsize = unmanaged_size;
            Table new_obj = (Table)t.Alloc();//System.Activator.CreateInstance(typeof(T));
            AddTotalBytes(L, nsize);
            return new_obj;
        }







        //---------------------------------


		//public static object luaM_realloc_<T>(lua_State L, T obj, ClassType t)
		//{
		//	int unmanaged_size = (int)t.GetUnmanagedSize();//LuaConf.GetUnmanagedSize(typeof(T))
		//	int old_size = (obj == null) ? 0 : unmanaged_size;
		//	int osize = old_size * unmanaged_size;
		//	int nsize = unmanaged_size;
        //   T new_obj = (T)t.Alloc(); //System.Activator.CreateInstance(typeof(T))
		//	SubtractTotalBytes(L, osize);
		//	AddTotalBytes(L, nsize);
		//	return new_obj;
        //}

		//public static object luaM_realloc_<T>(lua_State L, T[] old_block, int new_size, ClassType t)
		//{
		//	int unmanaged_size = (int)t.GetUnmanagedSize();//LuaConf.GetUnmanagedSize(typeof(T));
		//	int old_size = (old_block == null) ? 0 : old_block.Length;
		//	int osize = old_size * unmanaged_size;
		//	int nsize = new_size * unmanaged_size;
		//	T[] new_block = new T[new_size];
		//	for (int i = 0; i < Math.Min(old_size, new_size); i++)
		//	{
		//		new_block[i] = old_block[i];
		//	}
		//	for (int i = old_size; i < new_size; i++)
		//	{
        //       new_block[i] = (T)t.Alloc();// System.Activator.CreateInstance(typeof(T));
		//	}
		//	if (CanIndex(t))
		//	{
        //		for (int i = 0; i < new_size; i++)
		//		{
		//			ArrayElement elem = new_block[i] as ArrayElement;
		//			ClassType.Assert(elem != null, String.Format("Need to derive type {0} from ArrayElement", t.GetTypeString()));
		//			elem.set_index(i);
		//			elem.set_array(new_block);
		//		}
		//	}
		//	SubtractTotalBytes(L, osize);
		//	AddTotalBytes(L, nsize);
		//	return new_block;
		//}

        public static object luaM_realloc__Table(lua_State L, Table[] old_block, int new_size, ClassType t)
        {
            int unmanaged_size = (int)t.GetUnmanagedSize();//LuaConf.GetUnmanagedSize(typeof(T));
            int old_size = (old_block == null) ? 0 : old_block.Length;
            int osize = old_size * unmanaged_size;
            int nsize = new_size * unmanaged_size;
            Table[] new_block = new Table[new_size];
            for (int i = 0; i < Math.Min(old_size, new_size); i++)
            {
                new_block[i] = old_block[i];
            }
            for (int i = old_size; i < new_size; i++)
            {
                new_block[i] = (Table)t.Alloc();// System.Activator.CreateInstance(typeof(T));
            }
            if (CanIndex(t))
            {
                for (int i = 0; i < new_size; i++)
                {
                    ArrayElement elem = new_block[i] as ArrayElement;
                    ClassType.Assert(elem != null, String.Format("Need to derive type {0} from ArrayElement", t.GetTypeString()));
                    elem.set_index(i);
                    elem.set_array(new_block);
                }
            }
            SubtractTotalBytes(L, osize);
            AddTotalBytes(L, nsize);
            return new_block;
        }

        public static object luaM_realloc__UpVal(lua_State L, UpVal[] old_block, int new_size, ClassType t)
        {
            int unmanaged_size = (int)t.GetUnmanagedSize();//LuaConf.GetUnmanagedSize(typeof(T));
            int old_size = (old_block == null) ? 0 : old_block.Length;
            int osize = old_size * unmanaged_size;
            int nsize = new_size * unmanaged_size;
            UpVal[] new_block = new UpVal[new_size];
            for (int i = 0; i < Math.Min(old_size, new_size); i++)
            {
                new_block[i] = old_block[i];
            }
            for (int i = old_size; i < new_size; i++)
            {
                new_block[i] = (UpVal)t.Alloc();// System.Activator.CreateInstance(typeof(T));
            }
            if (CanIndex(t))
            {
                for (int i = 0; i < new_size; i++)
                {
                    ArrayElement elem = new_block[i] as ArrayElement;
                    ClassType.Assert(elem != null, String.Format("Need to derive type {0} from ArrayElement", t.GetTypeString()));
                    elem.set_index(i);
                    elem.set_array(new_block);
                }
            }
            SubtractTotalBytes(L, osize);
            AddTotalBytes(L, nsize);
            return new_block;
        }

        public static object luaM_realloc__char(lua_State L, char[] old_block, int new_size, ClassType t)
        {
            int unmanaged_size = (int)t.GetUnmanagedSize();//LuaConf.GetUnmanagedSize(typeof(T));
            int old_size = (old_block == null) ? 0 : old_block.Length;
            int osize = old_size * unmanaged_size;
            int nsize = new_size * unmanaged_size;
            char[] new_block = new char[new_size];
            for (int i = 0; i < Math.Min(old_size, new_size); i++)
            {
                new_block[i] = old_block[i];
            }
            for (int i = old_size; i < new_size; i++)
            {
                new_block[i] = (char)t.Alloc();// System.Activator.CreateInstance(typeof(T));
            }
            if (CanIndex(t)) // FIXME:not necessary
            {
                /*
                for (int i = 0; i < new_size; i++)
                {
                    ArrayElement elem = new_block[i] as ArrayElement;
                    ClassType.Assert(elem != null, String.Format("Need to derive type {0} from ArrayElement", t.GetTypeString()));
                    elem.set_index(i);
                    elem.set_array(new_block);
                }
                */
            }
            SubtractTotalBytes(L, osize);
            AddTotalBytes(L, nsize);
            return new_block;
        }

        public static object luaM_realloc__TValue(lua_State L, TValue[] old_block, int new_size, ClassType t)
        {
            int unmanaged_size = (int)t.GetUnmanagedSize();//LuaConf.GetUnmanagedSize(typeof(T));
            int old_size = (old_block == null) ? 0 : old_block.Length;
            int osize = old_size * unmanaged_size;
            int nsize = new_size * unmanaged_size;
            TValue[] new_block = new TValue[new_size];
            for (int i = 0; i < Math.Min(old_size, new_size); i++)
            {
                new_block[i] = old_block[i];
            }
            for (int i = old_size; i < new_size; i++)
            {
                new_block[i] = (TValue)t.Alloc();// System.Activator.CreateInstance(typeof(T));
            }
            if (CanIndex(t))
            {
                for (int i = 0; i < new_size; i++)
                {
                    ArrayElement elem = new_block[i] as ArrayElement;
                    ClassType.Assert(elem != null, String.Format("Need to derive type {0} from ArrayElement", t.GetTypeString()));
                    elem.set_index(i);
                    elem.set_array(new_block);
                }
            }
            SubtractTotalBytes(L, osize);
            AddTotalBytes(L, nsize);
            return new_block;
        }

        public static object luaM_realloc__TString(lua_State L, TString[] old_block, int new_size, ClassType t)
        {
            int unmanaged_size = (int)t.GetUnmanagedSize();//LuaConf.GetUnmanagedSize(typeof(T));
            int old_size = (old_block == null) ? 0 : old_block.Length;
            int osize = old_size * unmanaged_size;
            int nsize = new_size * unmanaged_size;
            TString[] new_block = new TString[new_size];
            for (int i = 0; i < Math.Min(old_size, new_size); i++)
            {
                new_block[i] = old_block[i];
            }
            for (int i = old_size; i < new_size; i++)
            {
                new_block[i] = (TString)t.Alloc();// System.Activator.CreateInstance(typeof(T));
            }
            if (CanIndex(t))
            {
                for (int i = 0; i < new_size; i++)
                {
                    ArrayElement elem = new_block[i] as ArrayElement;
                    ClassType.Assert(elem != null, String.Format("Need to derive type {0} from ArrayElement", t.GetTypeString()));
                    elem.set_index(i);
                    elem.set_array(new_block);
                }
            }
            SubtractTotalBytes(L, osize);
            AddTotalBytes(L, nsize);
            return new_block;
        }

        public static object luaM_realloc__Udata(lua_State L, Udata[] old_block, int new_size, ClassType t)
        {
            int unmanaged_size = (int)t.GetUnmanagedSize();//LuaConf.GetUnmanagedSize(typeof(T));
            int old_size = (old_block == null) ? 0 : old_block.Length;
            int osize = old_size * unmanaged_size;
            int nsize = new_size * unmanaged_size;
            Udata[] new_block = new Udata[new_size];
            for (int i = 0; i < Math.Min(old_size, new_size); i++)
            {
                new_block[i] = old_block[i];
            }
            for (int i = old_size; i < new_size; i++)
            {
                new_block[i] = (Udata)t.Alloc();// System.Activator.CreateInstance(typeof(T));
            }
            if (CanIndex(t))
            {
                for (int i = 0; i < new_size; i++)
                {
                    ArrayElement elem = new_block[i] as ArrayElement;
                    ClassType.Assert(elem != null, String.Format("Need to derive type {0} from ArrayElement", t.GetTypeString()));
                    elem.set_index(i);
                    elem.set_array(new_block);
                }
            }
            SubtractTotalBytes(L, osize);
            AddTotalBytes(L, nsize);
            return new_block;
        }

        public static object luaM_realloc__CallInfo(lua_State L, CallInfo[] old_block, int new_size, ClassType t)
        {
            int unmanaged_size = (int)t.GetUnmanagedSize();//LuaConf.GetUnmanagedSize(typeof(T));
            int old_size = (old_block == null) ? 0 : old_block.Length;
            int osize = old_size * unmanaged_size;
            int nsize = new_size * unmanaged_size;
            CallInfo[] new_block = new CallInfo[new_size];
            for (int i = 0; i < Math.Min(old_size, new_size); i++)
            {
                new_block[i] = old_block[i];
            }
            for (int i = old_size; i < new_size; i++)
            {
                new_block[i] = (CallInfo)t.Alloc();// System.Activator.CreateInstance(typeof(T));
            }
            if (CanIndex(t))
            {
                for (int i = 0; i < new_size; i++)
                {
                    ArrayElement elem = new_block[i] as ArrayElement;
                    ClassType.Assert(elem != null, String.Format("Need to derive type {0} from ArrayElement", t.GetTypeString()));
                    elem.set_index(i);
                    elem.set_array(new_block);
                }
            }
            SubtractTotalBytes(L, osize);
            AddTotalBytes(L, nsize);
            return new_block;
        }

        public static object luaM_realloc__long(lua_State L, long[] old_block, int new_size, ClassType t)
        {
            int unmanaged_size = (int)t.GetUnmanagedSize();//LuaConf.GetUnmanagedSize(typeof(T));
            int old_size = (old_block == null) ? 0 : old_block.Length;
            int osize = old_size * unmanaged_size;
            int nsize = new_size * unmanaged_size;
            long[] new_block = new long[new_size];
            for (int i = 0; i < Math.Min(old_size, new_size); i++)
            {
                new_block[i] = old_block[i];
            }
            for (int i = old_size; i < new_size; i++)
            {
                new_block[i] = (long)t.Alloc();// System.Activator.CreateInstance(typeof(T));
            }
            if (CanIndex(t)) //FIXME: not necessary
            {
                /*
                for (int i = 0; i < new_size; i++)
                {
                    ArrayElement elem = new_block[i] as ArrayElement;
                    ClassType.Assert(elem != null, String.Format("Need to derive type {0} from ArrayElement", t.GetTypeString()));
                    elem.set_index(i);
                    elem.set_array(new_block);
                }
                */
            }
            SubtractTotalBytes(L, osize);
            AddTotalBytes(L, nsize);
            return new_block;
        }

        public static object luaM_realloc__int(lua_State L, int[] old_block, int new_size, ClassType t)
        {
            int unmanaged_size = (int)t.GetUnmanagedSize();//LuaConf.GetUnmanagedSize(typeof(T));
            int old_size = (old_block == null) ? 0 : old_block.Length;
            int osize = old_size * unmanaged_size;
            int nsize = new_size * unmanaged_size;
            int[] new_block = new int[new_size];
            for (int i = 0; i < Math.Min(old_size, new_size); i++)
            {
                new_block[i] = old_block[i];
            }
            for (int i = old_size; i < new_size; i++)
            {
                new_block[i] = (int)t.Alloc();// System.Activator.CreateInstance(typeof(T));
            }
            if (CanIndex(t)) //FIXME: not necessary
            {
                /*
                for (int i = 0; i < new_size; i++)
                {
                    ArrayElement elem = new_block[i] as ArrayElement;
                    ClassType.Assert(elem != null, String.Format("Need to derive type {0} from ArrayElement", t.GetTypeString()));
                    elem.set_index(i);
                    elem.set_array(new_block);
                }
                */
            }
            SubtractTotalBytes(L, osize);
            AddTotalBytes(L, nsize);
            return new_block;
        }

        public static object luaM_realloc__Proto(lua_State L, Proto[] old_block, int new_size, ClassType t)
        {
            int unmanaged_size = (int)t.GetUnmanagedSize();//LuaConf.GetUnmanagedSize(typeof(T));
            int old_size = (old_block == null) ? 0 : old_block.Length;
            int osize = old_size * unmanaged_size;
            int nsize = new_size * unmanaged_size;
            Proto[] new_block = new Proto[new_size];
            for (int i = 0; i < Math.Min(old_size, new_size); i++)
            {
                new_block[i] = old_block[i];
            }
            for (int i = old_size; i < new_size; i++)
            {
                new_block[i] = (Proto)t.Alloc();// System.Activator.CreateInstance(typeof(T));
            }
            if (CanIndex(t))
            {
                for (int i = 0; i < new_size; i++)
                {
                    ArrayElement elem = new_block[i] as ArrayElement;
                    ClassType.Assert(elem != null, String.Format("Need to derive type {0} from ArrayElement", t.GetTypeString()));
                    elem.set_index(i);
                    elem.set_array(new_block);
                }
            }
            SubtractTotalBytes(L, osize);
            AddTotalBytes(L, nsize);
            return new_block;
        }

        public static object luaM_realloc__LocVar(lua_State L, LocVar[] old_block, int new_size, ClassType t)
        {
            int unmanaged_size = (int)t.GetUnmanagedSize();//LuaConf.GetUnmanagedSize(typeof(T));
            int old_size = (old_block == null) ? 0 : old_block.Length;
            int osize = old_size * unmanaged_size;
            int nsize = new_size * unmanaged_size;
            LocVar[] new_block = new LocVar[new_size];
            for (int i = 0; i < Math.Min(old_size, new_size); i++)
            {
                new_block[i] = old_block[i];
            }
            for (int i = old_size; i < new_size; i++)
            {
                new_block[i] = (LocVar)t.Alloc();// System.Activator.CreateInstance(typeof(T));
            }
            if (CanIndex(t))
            {
                for (int i = 0; i < new_size; i++)
                {
                    ArrayElement elem = new_block[i] as ArrayElement;
                    ClassType.Assert(elem != null, String.Format("Need to derive type {0} from ArrayElement", t.GetTypeString()));
                    elem.set_index(i);
                    elem.set_array(new_block);
                }
            }
            SubtractTotalBytes(L, osize);
            AddTotalBytes(L, nsize);
            return new_block;
        }

        public static object luaM_realloc__Node(lua_State L, Node[] old_block, int new_size, ClassType t)
        {
            int unmanaged_size = (int)t.GetUnmanagedSize();//LuaConf.GetUnmanagedSize(typeof(T));
            int old_size = (old_block == null) ? 0 : old_block.Length;
            int osize = old_size * unmanaged_size;
            int nsize = new_size * unmanaged_size;
            Node[] new_block = new Node[new_size];
            for (int i = 0; i < Math.Min(old_size, new_size); i++)
            {
                new_block[i] = old_block[i];
            }
            for (int i = old_size; i < new_size; i++)
            {
                new_block[i] = (Node)t.Alloc();// System.Activator.CreateInstance(typeof(T));
            }
            if (CanIndex(t))
            {
                for (int i = 0; i < new_size; i++)
                {
                    ArrayElement elem = new_block[i] as ArrayElement;
                    ClassType.Assert(elem != null, String.Format("Need to derive type {0} from ArrayElement", t.GetTypeString()));
                    elem.set_index(i);
                    elem.set_array(new_block);
                }
            }
            SubtractTotalBytes(L, osize);
            AddTotalBytes(L, nsize);
            return new_block;
        }

        public static object luaM_realloc__GCObject(lua_State L, GCObject[] old_block, int new_size, ClassType t)
        {
            int unmanaged_size = (int)t.GetUnmanagedSize();//LuaConf.GetUnmanagedSize(typeof(T));
            int old_size = (old_block == null) ? 0 : old_block.Length;
            int osize = old_size * unmanaged_size;
            int nsize = new_size * unmanaged_size;
            GCObject[] new_block = new GCObject[new_size];
            for (int i = 0; i < Math.Min(old_size, new_size); i++)
            {
                new_block[i] = old_block[i];
            }
            for (int i = old_size; i < new_size; i++)
            {
                new_block[i] = (GCObject)t.Alloc();// System.Activator.CreateInstance(typeof(T));
            }
            if (CanIndex(t))
            {
                for (int i = 0; i < new_size; i++)
                {
                    ArrayElement elem = new_block[i] as ArrayElement;
                    ClassType.Assert(elem != null, String.Format("Need to derive type {0} from ArrayElement", t.GetTypeString()));
                    elem.set_index(i);
                    elem.set_array(new_block);
                }
            }
            SubtractTotalBytes(L, osize);
            AddTotalBytes(L, nsize);
            return new_block;
        }


		public static bool CanIndex(ClassType t)
		{
            return t.CanIndex();
		}

		public static void AddTotalBytes(lua_State L, int num_bytes) 
		{
			LuaState.G(L).totalbytes += (int/*uint*/)num_bytes; 
		}
		
		public static void SubtractTotalBytes(lua_State L, int num_bytes) 
		{ 
			LuaState.G(L).totalbytes -= (int/*uint*/)num_bytes; 
		}

		//static void AddTotalBytes(lua_State L, uint num_bytes) {G(L).totalbytes += num_bytes;}
		//static void SubtractTotalBytes(lua_State L, uint num_bytes) {G(L).totalbytes -= num_bytes;}
	}
}
