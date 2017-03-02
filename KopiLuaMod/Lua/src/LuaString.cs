/*
 ** $Id: lstring.c,v 2.8.1.1 2007/12/27 13:02:25 roberto Exp $
 ** String table (keeps all strings handled by Lua)
 ** See Copyright Notice in lua.h
 */
using System;

namespace KopiLua
{
	//using lu_byte = System.Byte;

	public class LuaString
	{
		public static int sizestring(TString s) 
		{ 
			return ((int)s.len + 1) * LuaConf.GetUnmanagedSize(new ClassType(ClassType.TYPE_CHAR)); //char 
		}

		public static int sizeudata(Udata u) 
		{ 
			return (int)u.len; 
		}

		public static TString luaS_new(lua_State L, CharPtr s) 
		{ 
			return luaS_newlstr(L, s, /*(uint)*/LuaConf.strlen(s)); 
		}
		
		public static TString luaS_newliteral(lua_State L, CharPtr s) 
		{ 
			return luaS_newlstr(L, s, /*(uint)*/LuaConf.strlen(s)); 
		}

		public static void luaS_fix(TString s)
		{
			Byte/*lu_byte*/ marked = s.getTsv().marked;	// can't pass properties in as ref
			Byte[] marked_ref = new Byte[1];
			marked_ref[0] = marked;
			LuaGC.l_setbit(/*ref*/ marked_ref, LuaGC.FIXEDBIT);
			marked = marked_ref[0];
			s.getTsv().marked = marked;
		}

		public static void luaS_resize(lua_State L, int newsize) 
		{
			GCObject[] newhash;
			stringtable tb;
			int i;
			if (LuaState.G(L).gcstate == LuaGC.GCSsweepstring)
			{
				return;  /* cannot resize during GC traverse */
			}
			
			// todo: fix this up
			// I'm treating newhash as a regular C# array, but I need to allocate a dummy array
			// so that the garbage collector behaves identical to the C version.
			//newhash = luaM_newvector<GCObjectRef>(L, newsize);
			newhash = new GCObject[newsize];
            LuaMem.AddTotalBytes(L, newsize * LuaConf.GetUnmanagedSize(new ClassType(ClassType.TYPE_GCOBJECTREF))); //typeof(GCObjectRef)

			tb = LuaState.G(L).strt;
			for (i = 0; i < newsize; i++) 
			{
				newhash[i] = null;
			}

			/* rehash */
			for (i = 0; i < tb.size; i++) 
			{
				GCObject p = tb.hash[i];
				while (p != null) 
				{  
					/* for each node in the list */
                    GCObject next = p.getGch().next;  /* save next */
					long/*int*//*uint*/ h = LuaState.gco2ts(p).hash;
					int h1 = (int)LuaConf.lmod(h, newsize);  /* new position */
					LuaLimits.lua_assert((int)(h % newsize) == LuaConf.lmod(h, newsize));
                    p.getGch().next = newhash[h1];  /* chain it */
					newhash[h1] = p;
					p = next;
				}
			}
			//luaM_freearray(L, tb.hash);
			if (tb.hash != null)
			{
                LuaMem.SubtractTotalBytes(L, tb.hash.Length * LuaConf.GetUnmanagedSize(new ClassType(ClassType.TYPE_GCOBJECTREF))); //typeof(GCObjectRef)
			}
			tb.size = newsize;
			tb.hash = newhash;
		}

		public static TString newlstr(lua_State L, CharPtr str, int/*uint*/ l, long/*int*//*uint*/ h) 
		{
			TString ts;
			stringtable tb;
            if (l + 1 > LuaLimits.MAX_SIZET / LuaConf.GetUnmanagedSize(new ClassType(ClassType.TYPE_CHAR))) //typeof(char)
			{
				LuaMem.luaM_toobig(L);
			}
			ts = new TString(CharPtr.toCharPtr(new char[l + 1]));
            LuaMem.AddTotalBytes(L, (int)(l + 1) * LuaConf.GetUnmanagedSize(new ClassType(ClassType.TYPE_CHAR)) + LuaConf.GetUnmanagedSize(new ClassType(ClassType.TYPE_TSTRING))); //typeof(TString)//typeof(char)
			ts.getTsv().len = l;
            ts.getTsv().hash = h;
            ts.getTsv().marked = LuaGC.luaC_white(LuaState.G(L));
            ts.getTsv().tt = Lua.LUA_TSTRING;
            ts.getTsv().reserved = 0;
			//memcpy(ts+1, str, l*GetUnmanagedSize(typeof(char)));
			LuaConf.memcpy_char(ts.str.chars, str.chars, str.index, (int)l);
			ts.str.set(l, '\0');  /* ending 0 */
			tb = LuaState.G(L).strt;
			h = (int/*uint*/)LuaConf.lmod(h, tb.size);
            ts.getTsv().next = tb.hash[(int)h];  /* chain new entry */
			tb.hash[(int)h] = LuaState.obj2gco(ts);
			tb.nuse++;
			if ((tb.nuse > (int)tb.size) && (tb.size <= LuaLimits.MAX_INT / 2))
			{
				luaS_resize(L, tb.size*2);  /* too crowded */
			}
			return ts;
		}

		public static TString luaS_newlstr(lua_State L, CharPtr str, int/*uint*/ l) 
		{
			GCObject o;
			/*FIXME:*/
			long/*int*//*uint*/ h = /*(uint)*/l & 0xffffffff;  /* seed */
			int/*uint*/ step = (l >> 5) + 1;  /* if string is too long, don't hash all its chars */
			int/*uint*/ l1;
			for (l1 = l; l1 >= step; l1 -= step) 
			{
				/*FIXME:*/
				/* compute hash */
				h = (0xffffffff) & (h ^ ((h << 5)+(h >> 2) + (byte)str.get(l1 - 1)));
			}
			for (o = LuaState.G(L).strt.hash[(int)LuaConf.lmod(h, LuaState.G(L).strt.size)];
			     o != null;
                 o = o.getGch().next) 
			{
				TString ts = LuaState.rawgco2ts(o);
                if (ts.getTsv().len == l && (LuaConf.memcmp(str, LuaObject.getstr(ts), l) == 0))
				{
					/* string may be dead */
					if (LuaGC.isdead(LuaState.G(L), o)) 
					{
						LuaGC.changewhite(o);
					}
					return ts;
				}
			}
			//return newlstr(L, str, l, h);  /* not found */
			TString res = newlstr(L, str, l, h);
			return res;
		}

		public static Udata luaS_newudata(lua_State L, int/*uint*/ s, Table e)
		{
			Udata u = new Udata();
			u.uv.marked = LuaGC.luaC_white(LuaState.G(L));  /* is not finalized */
			u.uv.tt = Lua.LUA_TUSERDATA;
			u.uv.len = s;
			u.uv.metatable = null;
			u.uv.env = e;
			u.user_data = new byte[s];
			/* chain it on udata list (after main thread) */
			u.uv.next = LuaState.G(L).mainthread.next;
			LuaState.G(L).mainthread.next = LuaState.obj2gco(u);
			return u;
		}

        public static Udata luaS_newudata(lua_State L, ClassType t, Table e)
		{
			Udata u = new Udata();
			u.uv.marked = LuaGC.luaC_white(LuaState.G(L));  /* is not finalized */
			u.uv.tt = Lua.LUA_TUSERDATA;
			u.uv.len = 0;
			u.uv.metatable = null;
			u.uv.env = e;
			u.user_data = LuaMem.luaM_realloc_(L, t);
            LuaMem.AddTotalBytes(L, LuaConf.GetUnmanagedSize(new ClassType(ClassType.TYPE_UDATA)));//typeof(Udata)
			/* chain it on udata list (after main thread) */
			u.uv.next = LuaState.G(L).mainthread.next;
			LuaState.G(L).mainthread.next = LuaState.obj2gco(u);
			return u;
		}
	}
}
