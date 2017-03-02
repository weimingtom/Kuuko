/*
 ** $Id: ltable.c,v 2.32.1.2 2007/12/28 15:32:23 roberto Exp $
 ** Lua tables (hash)
 ** See Copyright Notice in lua.h
 */
using System;

namespace KopiLua
{
	//using TValue = Lua.TValue;
	//using StkId = TValue;
	//using lua_Number = System.Double;

	public class LuaTable
	{
		/*
		 ** Implementation of tables (aka arrays, objects, or hash tables).
		 ** Tables keep its elements in two parts: an array part and a hash part.
		 ** Non-negative integer keys are all candidates to be kept in the array
		 ** part. The actual size of the array is the largest `n' such that at
		 ** least half the slots between 0 and n are in use.
		 ** Hash uses a mix of chained scatter table with Brent's variation.
		 ** A main invariant of these tables is that, if an element is not
		 ** in its main position (i.e. the `original' position that its hash gives
		 ** to it), then the colliding element is in its own main position.
		 ** Hence even when the load factor reaches 100%, performance remains good.
		 */

		public static Node gnode(Table t, int i)	
		{
			return t.node[i];
		}
		
		public static TKey_nk gkey(Node n)
		{ 
			return n.i_key.nk; 
		}
		
		public static TValue gval(Node n)
		{
			return n.i_val;
		}
		
		public static Node gnext(Node n)
		{
			return n.i_key.nk.next;
		}
		
		public static void gnext_set(Node n, Node v) 
		{ 
			n.i_key.nk.next = v; 
		}

		public static TValue key2tval(Node n) 
		{ 
			return n.i_key.getTvk(); 
		}

		/*
		 ** max size of array part is 2^MAXBITS
		 */
		//#if LUAI_BITSINT > 26
		public const int MAXBITS = 26;	/* in the dotnet port LUAI_BITSINT is 32 */
		//#else
		//public const int MAXBITS		= (LUAI_BITSINT-2);
		//#endif

		public const int MAXASIZE = (1 << MAXBITS);

		//public static Node gnode(Table t, int i)	{return t.node[i];}
		public static Node hashpow2(Table t, Double/*lua_Number*/ n) 
		{ 
			return gnode(t, (int)LuaConf.lmod(n, LuaObject.sizenode(t))); 
		}
		
		public static Node hashstr(Table t, TString str)  
		{
            return hashpow2(t, str.getTsv().hash);
		}
		
		public static Node hashboolean(Table t, int p)
		{
			return hashpow2(t, p);
		}

		/*
		 ** for some types, it is better to avoid modulus by power of 2, as
		 ** they tend to have many 2 factors.
		 */
		public static Node hashmod(Table t, int n) 
		{ 
			return gnode(t, (n % ((LuaObject.sizenode(t) - 1) | 1))); 
		}

		public static Node hashpointer(Table t, object p) 
		{ 
			return hashmod(t, p.GetHashCode()); 
		}

		/*
		 ** number of ints inside a lua_Number
		 */
		public /*const*/static readonly int numints = ClassType.GetNumInts();

		//static const Node dummynode_ = {
		//{{null}, LUA_TNIL},  /* value */
		//{{{null}, LUA_TNIL, null}}  /* key */
		//};
		public static Node dummynode_ = new Node(new TValue(new Value(), Lua.LUA_TNIL), new TKey(new Value(), Lua.LUA_TNIL, null));
		public static Node dummynode = dummynode_;

		/*
		 ** hash for lua_Numbers
		 */
		private static Node hashnum(Table t, Double/*lua_Number*/ n)
		{
			byte[] a = ClassType.GetBytes(n);
			for (int i = 1; i < a.Length; i++) 
			{
				a[0] += a[i];
			}
			return hashmod(t, (int)a[0]);
		}

		/*
		 ** returns the `main' position of an element in a table (that is, the index
		 ** of its hash value)
		 */
		private static Node mainposition(Table t, TValue key) 
		{
			switch (LuaObject.ttype(key))
			{
				case Lua.LUA_TNUMBER:
					{
						return hashnum(t, LuaObject.nvalue(key));
					}
				case Lua.LUA_TSTRING:
					{
						return hashstr(t, LuaObject.rawtsvalue(key));
					}
				case Lua.LUA_TBOOLEAN:
					{
						return hashboolean(t, LuaObject.bvalue(key));
					}
				case Lua.LUA_TLIGHTUSERDATA:
					{
						return hashpointer(t, LuaObject.pvalue(key));
					}
				default:
					{
						return hashpointer(t, LuaObject.gcvalue(key));
					}
			}
		}


		/*
		 ** returns the index for `key' if `key' is an appropriate key to live in
		 ** the array part of the table, -1 otherwise.
		 */
		private static int arrayindex(TValue key) 
		{
			if (LuaObject.ttisnumber(key))
			{
				Double/*lua_Number*/ n = LuaObject.nvalue(key);
				int[] k = new int[1];
				LuaConf.lua_number2int(/*out*/ k, n);
				if (LuaConf.luai_numeq(LuaLimits.cast_num(k[0]), n))
				{
					return k[0];
				}
			}
			return -1;  /* `key' did not match some condition */
		}

		/*
		 ** returns the index of a `key' for table traversals. First goes all
		 ** elements in the array part, then elements in the hash part. The
		 ** beginning of a traversal is signalled by -1.
		 */
		private static int findindex(lua_State L, Table t, TValue/*StkId*/ key)
		{
			int i;
			if (LuaObject.ttisnil(key)) 
			{
				return -1;  /* first iteration */
			}
			i = arrayindex(key);
			if (0 < i && i <= t.sizearray)  /* is `key' inside array part? */
			{
				return i-1;  /* yes; that's the index (corrected to C) */
			}
			else 
			{
				Node n = mainposition(t, key);
				do {  
					/* check whether `key' is somewhere in the chain */
					/* key may be dead already, but it is ok to use it in `next' */
					if ((LuaObject.luaO_rawequalObj(key2tval(n), key) != 0) ||
					    (LuaObject.ttype(gkey(n)) == LuaObject.LUA_TDEADKEY && LuaObject.iscollectable(key) &&
					     LuaObject.gcvalue(gkey(n)) == LuaObject.gcvalue(key)))
					{
						i = LuaLimits.cast_int(Node.minus(n, gnode(t, 0)));  /* key index in hash table */
						/* hash elements are numbered after array ones */
						return i + t.sizearray;
					}
					else 
					{
						n = gnext(n);
					}
                } while (Node.isNotEqual(n, null));
				LuaDebug.luaG_runerror(L, CharPtr.toCharPtr("invalid key to " + LuaConf.LUA_QL("next")));  /* key not found */
				return 0;  /* to avoid warnings */
			}
		}

		public static int luaH_next(lua_State L, Table t, TValue/*StkId*/ key)
		{
			int i = findindex(L, t, key);  /* find original element */
			for (i++; i < t.sizearray; i++) 
			{  
				/* try first array part */
				if (!LuaObject.ttisnil(t.array[i]))
				{  
					/* a non-nil value? */
					LuaObject.setnvalue(key, LuaLimits.cast_num(i + 1));
					LuaObject.setobj2s(L, TValue.plus(key, 1), t.array[i]);
					return 1;
				}
			}
			for (i -= t.sizearray; i < LuaObject.sizenode(t); i++)
			{  
				/* then hash part */
				if (!LuaObject.ttisnil(gval(gnode(t, i))))
				{  
					/* a non-nil value? */
					LuaObject.setobj2s(L, key, key2tval(gnode(t, i)));
					LuaObject.setobj2s(L, TValue.plus(key, 1), gval(gnode(t, i)));
					return 1;
				}
			}
			return 0;  /* no more elements */
		}


		/*
		 ** {=============================================================
		 ** Rehash
		 ** ==============================================================
		 */

		private static int computesizes(int[] nums, /*ref*/ int[] narray)
		{
			int i;
			int twotoi;  /* 2^i */
			int a = 0;  /* number of elements smaller than 2^i */
			int na = 0;  /* number of elements to go to array part */
			int n = 0;  /* optimal size for array part */
			for (i = 0, twotoi = 1; twotoi / 2 < narray[0]; i++, twotoi *= 2)
			{
				if (nums[i] > 0) 
				{
					a += nums[i];
					if (a > twotoi / 2) 
					{  
						/* more than half elements present? */
						n = twotoi;  /* optimal size (till now) */
						na = a;  /* all elements smaller than n will go to array part */
					}
				}
				if (a == narray[0])
				{
					break;  /* all elements already counted */
				}
			}
			narray[0] = n;
			LuaLimits.lua_assert(narray[0] / 2 <= na && na <= narray[0]);
			return na;
		}

		private static int countint(TValue key, int[] nums) 
		{
			int k = arrayindex(key);
			if (0 < k && k <= MAXASIZE) 
			{  
				/* is `key' an appropriate array index? */
				nums[LuaObject.ceillog2(k)]++;  /* count as such */
				return 1;
			}
			else
			{
				return 0;
			}
		}

		private static int numusearray(Table t, int[] nums) 
		{
			int lg;
			int ttlg;  /* 2^lg */
			int ause = 0;  /* summation of `nums' */
			int i = 1;  /* count to traverse all array keys */
			for (lg = 0, ttlg = 1; lg <= MAXBITS; lg++, ttlg *= 2) 
			{  
				/* for each slice */
				int lc = 0;  /* counter */
				int lim = ttlg;
				if (lim > t.sizearray) 
				{
					lim = t.sizearray;  /* adjust upper limit */
					if (i > lim)
					{
						break;  /* no more elements to count */
					}
				}
				/* count elements in range (2^(lg-1), 2^lg] */
				for (; i <= lim; i++) 
				{
					if (!LuaObject.ttisnil(t.array[i - 1]))
					{
						lc++;
					}
				}
				nums[lg] += lc;
				ause += lc;
			}
			return ause;
		}

		private static int numusehash(Table t, int[] nums, /*ref*/ int[] pnasize)
		{
			int totaluse = 0;  /* total number of elements */
			int ause = 0;  /* summation of `nums' */
			int i = LuaObject.sizenode(t);
			while ((i--) != 0) 
			{
				Node n = t.node[i];
				if (!LuaObject.ttisnil(gval(n)))
				{
					ause += countint(key2tval(n), nums);
					totaluse++;
				}
			}
			pnasize[0] += ause;
			return totaluse;
		}

		private static void setarrayvector(lua_State L, Table t, int size) 
		{
			int i;
			TValue[][] array_ref = new TValue[1][];
			array_ref[0] = t.array;
			LuaMem.luaM_reallocvector_TValue(L, /*ref*/ array_ref, t.sizearray, size/*, TValue*/, new ClassType(ClassType.TYPE_TVALUE));
			t.array = array_ref[0];
			for (i = t.sizearray; i < size; i++)
			{
				LuaObject.setnilvalue(t.array[i]);
			}
			t.sizearray = size;
		}

		private static void setnodevector (lua_State L, Table t, int size) 
		{
			int lsize;
			if (size == 0) 
			{  
				/* no elements to hash part? */
				t.node = new Node[] { dummynode };  /* use common `dummynode' */
				lsize = 0;
			}
			else 
			{
				int i;
				lsize = LuaObject.ceillog2(size);
				if (lsize > MAXBITS)
				{
					LuaDebug.luaG_runerror(L, CharPtr.toCharPtr("table overflow"));
				}
				size = LuaObject.twoto(lsize);
				Node[] nodes = LuaMem.luaM_newvector_Node(L, size, new ClassType(ClassType.TYPE_NODE));
				t.node = nodes;
				for (i = 0; i < size; i++) 
				{
					Node n = gnode(t, i);
					gnext_set(n, null);
					LuaObject.setnilvalue(gkey(n));
					LuaObject.setnilvalue(gval(n));
				}
			}
			t.lsizenode = LuaLimits.cast_byte(lsize);
			t.lastfree = size;  /* all positions are free */
		}

		private static void resize(lua_State L, Table t, int nasize, int nhsize) 
		{
			int i;
			int oldasize = t.sizearray;
			int oldhsize = t.lsizenode;
			Node[] nold = t.node;  /* save old hash ... */
			if (nasize > oldasize)  /* array part must grow? */
			{
				setarrayvector(L, t, nasize);
			}
			/* create new hash part with appropriate size */
			setnodevector(L, t, nhsize);
			if (nasize < oldasize) 
			{  
				/* array part must shrink? */
				t.sizearray = nasize;
				/* re-insert elements from vanishing slice */
				for (i = nasize; i < oldasize; i++) 
				{
					if (!LuaObject.ttisnil(t.array[i]))
					{
						LuaObject.setobjt2t(L, luaH_setnum(L, t, i + 1), t.array[i]);
					}
				}
				/* shrink array */
				TValue[][] array_ref = new TValue[1][];
				array_ref[0] = t.array;
				LuaMem.luaM_reallocvector_TValue(L, /*ref*/ array_ref, oldasize, nasize/*, TValue*/, new ClassType(ClassType.TYPE_TVALUE));
				t.array = array_ref[0];
			}
			/* re-insert elements from hash part */
			for (i = LuaObject.twoto(oldhsize) - 1; i >= 0; i--)
			{
				Node old = nold[i];
				if (!LuaObject.ttisnil(gval(old)))
				{
					LuaObject.setobjt2t(L, luaH_set(L, t, key2tval(old)), gval(old));
				}
			}
			if (Node.isNotEqual(nold[0], dummynode))
			{
				LuaMem.luaM_freearray_Node(L, nold, new ClassType(ClassType.TYPE_NODE));  /* free old array */
			}
		}

		public static void luaH_resizearray(lua_State L, Table t, int nasize) 
		{
			int nsize = (Node.isEqual(t.node[0], dummynode)) ? 0 : LuaObject.sizenode(t);
			resize(L, t, nasize, nsize);
		}

		private static void rehash(lua_State L, Table t, TValue ek) 
		{
			int[] nasize = new int[1];
			int na;
			int[] nums = new int[MAXBITS + 1];  /* nums[i] = number of keys between 2^(i-1) and 2^i */
			int i;
			int totaluse;
			for (i = 0; i <= MAXBITS; i++) 
			{
				nums[i] = 0;  /* reset counts */
			}
			nasize[0] = numusearray(t, nums);  /* count keys in array part */
			totaluse = nasize[0];  /* all those keys are integer keys */
			totaluse += numusehash(t, nums, /*ref*/ nasize);  /* count keys in hash part */
			/* count extra key */
			nasize[0] += countint(ek, nums);
			totaluse++;
			/* compute new size for array part */
			na = computesizes(nums, /*ref*/ nasize);
			/* resize the table to new computed sizes */
			resize(L, t, nasize[0], totaluse - na);
		}

		/*
		 ** }=============================================================
		 */
		
		public static Table luaH_new(lua_State L, int narray, int nhash) 
		{
			Table t = LuaMem.luaM_new_Table(L, new ClassType(ClassType.TYPE_TABLE));
			LuaGC.luaC_link(L, LuaState.obj2gco(t), (byte)Lua.LUA_TTABLE);
			t.metatable = null;
			t.flags = LuaLimits.cast_byte(~0);
			/* temporary values (kept only if some malloc fails) */
			t.array = null;
			t.sizearray = 0;
			t.lsizenode = 0;
			t.node = new Node[] { dummynode };
			setarrayvector(L, t, narray);
			setnodevector(L, t, nhash);
			return t;
		}

		public static void luaH_free(lua_State L, Table t) 
		{
			if (Node.isNotEqual(t.node[0], dummynode))
			{
				LuaMem.luaM_freearray_Node(L, t.node, new ClassType(ClassType.TYPE_NODE));
			}
			LuaMem.luaM_freearray_TValue(L, t.array, new ClassType(ClassType.TYPE_TVALUE));
			LuaMem.luaM_free_Table(L, t, new ClassType(ClassType.TYPE_TABLE));
		}

		private static Node getfreepos(Table t) 
		{
			while (t.lastfree-- > 0) 
			{
				if (LuaObject.ttisnil(gkey(t.node[t.lastfree])))
				{
					return t.node[t.lastfree];
				}
			}
			return null;  /* could not find a free place */
		}

		/*
		 ** inserts a new key into a hash table; first, check whether key's main
		 ** position is free. If not, check whether colliding node is in its main
		 ** position or not: if it is not, move colliding node to an empty place and
		 ** put new key in its main position; otherwise (colliding node is in its main
		 ** position), new key goes to an empty position.
		 */
		private static TValue newkey(lua_State L, Table t, TValue key) 
		{
			Node mp = mainposition(t, key);
			if (!LuaObject.ttisnil(gval(mp)) || Node.isEqual(mp, dummynode))
			{
				Node othern;
				Node n = getfreepos(t);  /* get a free place */
				if (Node.isEqual(n, null)) 
				{  
					/* cannot find a free place? */
					rehash(L, t, key);  /* grow table */
					return luaH_set(L, t, key);  /* re-insert key into grown table */
				}
				LuaLimits.lua_assert(Node.isNotEqual(n, dummynode));
				othern = mainposition(t, key2tval(mp));
				if (Node.isNotEqual(othern, mp)) 
				{  
					/* is colliding node out of its main position? */
					/* yes; move colliding node into free position */
					while (Node.isNotEqual(gnext(othern), mp)) 
					{
						othern = gnext(othern);  /* find previous */
					}
					gnext_set(othern, n);  /* redo the chain with `n' in place of `mp' */
					n.i_val = new TValue(mp.i_val);	/* copy colliding node into free pos. (mp.next also goes) */
					n.i_key = new TKey(mp.i_key);
					gnext_set(mp, null);  /* now `mp' is free */
					LuaObject.setnilvalue(gval(mp));
				}
				else 
				{  
					/* colliding node is in its own main position */
					/* new node will go into free position */
					gnext_set(n, gnext(mp));  /* chain new position */
					gnext_set(mp, n);
					mp = n;
				}
			}
			gkey(mp).value.copyFrom(key.value);
			gkey(mp).tt = key.tt;
			LuaGC.luaC_barriert(L, t, key);
			LuaLimits.lua_assert(LuaObject.ttisnil(gval(mp)));
			return gval(mp);
		}

		/*
		 ** search function for integers
		 */
		public static TValue luaH_getnum(Table t, int key)
		{
			/* (1 <= key && key <= t.sizearray) */
			if ((long/*uint*/)((key - 1) & 0xffffffff) < (long/*uint*/)(t.sizearray & 0xffffffff))
			{
				return t.array[key - 1];
			}
			else 
			{
				Double/*lua_Number*/ nk = LuaLimits.cast_num(key);
				Node n = hashnum(t, nk);
				do 
				{  
					/* check whether `key' is somewhere in the chain */
					if (LuaObject.ttisnumber(gkey(n)) && LuaConf.luai_numeq(LuaObject.nvalue(gkey(n)), nk))
					{
						return gval(n);  /* that's it */
					}
					else 
					{
						n = gnext(n);
					}
				} while (Node.isNotEqual(n, null));
				return LuaObject.luaO_nilobject;
			}
		}

		/*
		 ** search function for strings
		 */
		public static TValue luaH_getstr(Table t, TString key) 
		{
			Node n = hashstr(t, key);
			do 
			{  
				/* check whether `key' is somewhere in the chain */
				if (LuaObject.ttisstring(gkey(n)) && LuaObject.rawtsvalue(gkey(n)) == key)
				{
					return gval(n);  /* that's it */
				}
				else 
				{
					n = gnext(n);
				}
			} while (Node.isNotEqual(n, null));
			return LuaObject.luaO_nilobject;
		}

		/*
		 ** main search function
		 */
		public static TValue luaH_get(Table t, TValue key) 
		{
			switch (LuaObject.ttype(key))
			{
				case Lua.LUA_TNIL: 
					{
						return LuaObject.luaO_nilobject;
					}
				case Lua.LUA_TSTRING: 
					{
						return luaH_getstr(t, LuaObject.rawtsvalue(key));
					}
				case Lua.LUA_TNUMBER:
					{
						int[] k = new int[1];
						Double/*lua_Number*/ n = LuaObject.nvalue(key);
						LuaConf.lua_number2int(/*out*/ k, n);
						if (LuaConf.luai_numeq(LuaLimits.cast_num(k[0]), LuaObject.nvalue(key))) /* index is int? */
						{
							return luaH_getnum(t, k[0]);  /* use specialized version */
						}
						/* else go through ... actually on second thoughts don't, because this is C#*/
						Node node = mainposition(t, key);
						do
						{  
							/* check whether `key' is somewhere in the chain */
							if (LuaObject.luaO_rawequalObj(key2tval(node), key) != 0)
							{
								return gval(node);  /* that's it */
							}
							else 
							{
								node = gnext(node);
							}
						} while (Node.isNotEqual(node, null));
						return LuaObject.luaO_nilobject;
					}
				default: 
					{
						Node node = mainposition(t, key);
						do 
						{  
							/* check whether `key' is somewhere in the chain */
							if (LuaObject.luaO_rawequalObj(key2tval(node), key) != 0)
							{
								return gval(node);  /* that's it */
							}
							else 
							{
								node = gnext(node);
							}
						} while (Node.isNotEqual(node, null));
						return LuaObject.luaO_nilobject;
					}
			}
		}

		public static TValue luaH_set(lua_State L, Table t, TValue key) 
		{
			TValue p = luaH_get(t, key);
			t.flags = 0;
			if (p != LuaObject.luaO_nilobject)
			{
				return (TValue)p;
			}
			else 
			{
				if (LuaObject.ttisnil(key)) 
				{
					LuaDebug.luaG_runerror(L, CharPtr.toCharPtr("table index is nil"));
				}
				else if (LuaObject.ttisnumber(key) && LuaConf.luai_numisnan(LuaObject.nvalue(key)))
				{
					LuaDebug.luaG_runerror(L, CharPtr.toCharPtr("table index is NaN"));
				}
				return newkey(L, t, key);
			}
		}
		
		public static TValue luaH_setnum(lua_State L, Table t, int key) 
		{
			TValue p = luaH_getnum(t, key);
			if (p != LuaObject.luaO_nilobject)
			{
				return (TValue)p;
			}
			else 
			{
				TValue k = new TValue();
				LuaObject.setnvalue(k, LuaLimits.cast_num(key));
				return newkey(L, t, k);
			}
		}

		public static TValue luaH_setstr(lua_State L, Table t, TString key) 
		{
			TValue p = luaH_getstr(t, key);
			if (p != LuaObject.luaO_nilobject)
			{
				return (TValue)p;
			}
			else 
			{
				TValue k = new TValue();
				LuaObject.setsvalue(L, k, key);
				return newkey(L, t, k);
			}
		}

		public static int unbound_search(Table t, int/*uint*/ j) 
		{
			int/*uint*/ i = j;  /* i is zero or a present index */
			j++;
			/* find `i' and `j' such that i is present and j is not */
			while (!LuaObject.ttisnil(luaH_getnum(t, (int)j)))
			{
				i = j;
				j *= 2;
				if (j > (int/*uint*/)LuaLimits.MAX_INT)
				{  
					/* overflow? */
					/* table was built with bad purposes: resort to linear search */
					i = 1;
					while (!LuaObject.ttisnil(luaH_getnum(t, (int)i))) 
					{
						i++;
					}
					return (int)(i - 1);
				}
			}
			/* now do a binary search between them */
			while (j - i > 1) 
			{
				int/*uint*/ m = (i + j) / 2;
				if (LuaObject.ttisnil(luaH_getnum(t, (int)m))) 
				{
					j = m;
				}
				else 
				{
					i = m;
				}
			}
			return (int)i;
		}

		/*
		 ** Try to find a boundary in table `t'. A `boundary' is an integer index
		 ** such that t[i] is non-nil and t[i+1] is nil (and 0 if t[1] is nil).
		 */
		public static int luaH_getn(Table t) 
		{
			int/*uint*/ j = (int/*uint*/)t.sizearray;
			if (j > 0 && LuaObject.ttisnil(t.array[j - 1]))
			{
				/* there is a boundary in the array part: (binary) search for it */
				int/*uint*/ i = 0;
				while (j - i > 1) 
				{
					int/*uint*/ m = (i + j) / 2;
					if (LuaObject.ttisnil(t.array[m - 1])) 
					{
						j = m;
					}
					else 
					{
						i = m;
					}
				}
				return (int)i;
			}
			/* else must find a boundary in hash part */
			else if (Node.isEqual(t.node[0], dummynode))  /* hash part is empty? */
			{
				return (int)j;  /* that is easy... */
			}
			else 
			{
				return unbound_search(t, j);
			}
		}

		//#if defined(LUA_DEBUG)

		//Node *luaH_mainposition (const Table *t, const TValue *key) {
		//  return mainposition(t, key);
		//}

		//int luaH_isdummy (Node *n) { return n == dummynode; }

		//#endif
	}
}
