/*
 ** $Id: lstrlib.c,v 1.132.1.4 2008/07/11 17:27:21 roberto Exp $
 ** Standard library for string operations and pattern-matching
 ** See Copyright Notice in lua.h
 */
namespace KopiLua
{
	public class MatchState
	{
		public class capture_
		{
			public CharPtr init;
            public int/*Int32*//*ptrdiff_t*/ len;
		}
		
		public CharPtr src_init;  /* init of source string */
		public CharPtr src_end;  /* end (`\0') of source string */
		public lua_State L;
		public int level;  /* total number of captures (finished or unfinished) */

		public capture_[] capture = new capture_[LuaConf.LUA_MAXCAPTURES];
		
		public MatchState()
		{
			for (int i = 0; i < LuaConf.LUA_MAXCAPTURES; i++)
			{
				capture[i] = new capture_();
			}
		}
	}
}
