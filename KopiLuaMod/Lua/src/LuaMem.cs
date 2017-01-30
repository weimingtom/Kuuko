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

		public static T[] luaM_reallocv<T>(lua_State L, T[] block, int new_size, ClassType t)
		{
			return (T[])luaM_realloc_(L, block, new_size, t);
		}
		
		//#define luaM_freemem(L, b, s)	luaM_realloc_(L, (b), (s), 0)
		//#define luaM_free(L, b)		luaM_realloc_(L, (b), sizeof(*(b)), 0)
		//public static void luaM_freearray(lua_State L, object b, int n, Type t) { luaM_reallocv(L, b, n, 0, Marshal.SizeOf(b)); }

		// C# has it's own gc, so nothing to do here...in theory...
		public static void luaM_freemem<T>(lua_State L, T b, ClassType t) 
		{ 
			luaM_realloc_<T>(L, new T[] {b}, 0, t); 
		}
		
		public static void luaM_free<T>(lua_State L, T b, ClassType t) 
		{ 
			luaM_realloc_<T>(L, new T[] {b}, 0, t); 
		}
		
		public static void luaM_freearray<T>(lua_State L, T[] b, ClassType t) 
		{ 
			luaM_reallocv(L, b, 0, t); 
		}

		public static T luaM_malloc<T>(lua_State L, ClassType t) 
		{ 
			return (T)luaM_realloc_<T>(L, t); 
		}

        public static T luaM_new<T>(lua_State L, ClassType t) 
		{ 
			return (T)luaM_realloc_<T>(L, t); 
		}
		
		public static T[] luaM_newvector<T>(lua_State L, int n, ClassType t)
		{
			return luaM_reallocv<T>(L, null, n, t);
		}

		public static void luaM_growvector<T>(lua_State L, /*ref*/ T[][] v, int nelems, /*ref*/ int[] size, int limit, CharPtr e, ClassType t)
		{
			if (nelems + 1 > size[0])
			{
				v[0] = (T[])luaM_growaux_(L, /*ref*/ v, /*ref*/ size, limit, e, t);
			}
		}

		public static T[] luaM_reallocvector<T>(lua_State L, /*ref*/ T[][] v, int oldn, int n, ClassType t)
		{
			ClassType.Assert((v[0] == null && oldn == 0) || (v[0].Length == oldn));
			v[0] = luaM_reallocv<T>(L, v[0], n, t);
			return v[0];
		}


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


		public static T[] luaM_growaux_<T>(lua_State L, /*ref*/ T[][] block, /*ref*/ int[] size,
			int limit, CharPtr errormsg, ClassType t)
		{
			T[] newblock;
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
			newblock = luaM_reallocv<T>(L, block[0], newsize, t);
			size[0] = newsize;  /* update only when everything else is OK */
			return newblock;
		}

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

		public static object luaM_realloc_<T>(lua_State L, ClassType t)
		{
			int unmanaged_size = (int)t.GetUnmanagedSize();//LuaConf.GetUnmanagedSize(typeof(T));
			int nsize = unmanaged_size;
            T new_obj = (T)t.Alloc();//System.Activator.CreateInstance(typeof(T));
			AddTotalBytes(L, nsize);
			return new_obj;
		}

		public static object luaM_realloc_<T>(lua_State L, T obj, ClassType t)
		{
			int unmanaged_size = (int)t.GetUnmanagedSize();//LuaConf.GetUnmanagedSize(typeof(T))
			int old_size = (obj == null) ? 0 : unmanaged_size;
			int osize = old_size * unmanaged_size;
			int nsize = unmanaged_size;
            T new_obj = (T)t.Alloc(); //System.Activator.CreateInstance(typeof(T))
			SubtractTotalBytes(L, osize);
			AddTotalBytes(L, nsize);
			return new_obj;
		}

		public static object luaM_realloc_<T>(lua_State L, T[] old_block, int new_size, ClassType t)
		{
			int unmanaged_size = (int)t.GetUnmanagedSize();//LuaConf.GetUnmanagedSize(typeof(T));
			int old_size = (old_block == null) ? 0 : old_block.Length;
			int osize = old_size * unmanaged_size;
			int nsize = new_size * unmanaged_size;
			T[] new_block = new T[new_size];
			for (int i = 0; i < Math.Min(old_size, new_size); i++)
			{
				new_block[i] = old_block[i];
			}
			for (int i = old_size; i < new_size; i++)
			{
                new_block[i] = (T)t.Alloc();// System.Activator.CreateInstance(typeof(T));
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
