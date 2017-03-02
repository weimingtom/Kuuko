/*
 ** $Id: ltm.c,v 2.8.1.1 2007/12/27 13:02:25 roberto Exp $
 ** Tag methods
 ** See Copyright Notice in lua.h
 */
namespace KopiLua
{
	//using TValue = Lua.TValue;

	public class LuaTM
	{
		public static TValue gfasttm(global_State g, Table et, TMS e)
		{
			return (et == null) ? null :
				((et.flags & (1 << (int)e)) != 0) ? null :
				luaT_gettm(et, e, g.tmname[(int)e]);
		}

		public static TValue fasttm(lua_State l, Table et, TMS e) 
		{ 
			return gfasttm(LuaState.G(l), et, e); 
		}

		public readonly static CharPtr[] luaT_typenames = {
			CharPtr.toCharPtr("nil"), CharPtr.toCharPtr("boolean"), CharPtr.toCharPtr("userdata"), CharPtr.toCharPtr("number"),
			CharPtr.toCharPtr("string"), CharPtr.toCharPtr("table"), CharPtr.toCharPtr("function"), CharPtr.toCharPtr("userdata"), CharPtr.toCharPtr("thread"),
			CharPtr.toCharPtr("proto"), CharPtr.toCharPtr("upval")
		};

		private readonly static CharPtr[] luaT_eventname = {  /* ORDER TM */
			CharPtr.toCharPtr("__index"), CharPtr.toCharPtr("__newindex"),
			CharPtr.toCharPtr("__gc"), CharPtr.toCharPtr("__mode"), CharPtr.toCharPtr("__eq"),
			CharPtr.toCharPtr("__add"), CharPtr.toCharPtr("__sub"), CharPtr.toCharPtr("__mul"), CharPtr.toCharPtr("__div"), CharPtr.toCharPtr("__mod"),
			CharPtr.toCharPtr("__pow"), CharPtr.toCharPtr("__unm"), CharPtr.toCharPtr("__len"), CharPtr.toCharPtr("__lt"), CharPtr.toCharPtr("__le"),
			CharPtr.toCharPtr("__concat"), CharPtr.toCharPtr("__call")
		};

		public static void luaT_init(lua_State L) 
		{
			int i;
			for (i = 0; i < (int)TMS.TM_N; i++) 
			{
				LuaState.G(L).tmname[i] = LuaString.luaS_new(L, luaT_eventname[i]);
				LuaString.luaS_fix(LuaState.G(L).tmname[i]);  /* never collect these names */
			}
		}

		/*
		 ** function to be used with macro "fasttm": optimized for absence of
		 ** tag methods
		 */
		public static TValue luaT_gettm(Table events, TMS event_, TString ename) 
		{
			//const
			TValue tm = LuaTable.luaH_getstr(events, ename);
			LuaLimits.lua_assert(TMSUtil.convertTMStoInt(event_) <= TMSUtil.convertTMStoInt(TMS.TM_EQ));
			if (LuaObject.ttisnil(tm))
			{  
				/* no tag method? */
				events.flags |= (byte)(1 << (int)event_);  /* cache this fact */
				return null;
			}
			else 
			{
				return tm;
			}
		}

		public static TValue luaT_gettmbyobj(lua_State L, TValue o, TMS event_) 
		{
			Table mt;
			switch (LuaObject.ttype(o))
			{
				case Lua.LUA_TTABLE:
					{
						mt = LuaObject.hvalue(o).metatable;
						break;
					}
				case Lua.LUA_TUSERDATA:
					{
						mt = LuaObject.uvalue(o).metatable;
						break;
					}
				default:
					{
						mt = LuaState.G(L).mt[LuaObject.ttype(o)];
						break;
					}
			}
			return ((mt != null) ? LuaTable.luaH_getstr(mt, LuaState.G(L).tmname[(int)event_]) : LuaObject.luaO_nilobject);
		}
	}
}
