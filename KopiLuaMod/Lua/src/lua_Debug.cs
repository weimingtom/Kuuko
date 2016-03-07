/*
 ** $Id: lua.h,v 1.218.1.5 2008/08/06 13:30:12 roberto Exp $
 ** Lua - An Extensible Extension Language
 ** Lua.org, PUC-Rio, Brazil (http://www.lua.org)
 ** See Copyright Notice at the end of this file
 */
namespace KopiLua
{
	public class lua_Debug
	{
		public int event_;
		public CharPtr name;	/* (n) */
		public CharPtr namewhat;	/* (n) `global', `local', `field', `method' */
		public CharPtr what;	/* (S) `Lua', `C', `main', `tail' */
		public CharPtr source;	/* (S) */
		public int currentline;	/* (l) */
		public int nups;		/* (u) number of upvalues */
		public int linedefined;	/* (S) */
		public int lastlinedefined;	/* (S) */
		public CharPtr short_src = CharPtr.toCharPtr(new char[LuaConf.LUA_IDSIZE]); /* (S) */
		/* private part */
		public int i_ci;  /* active function */
	}
}
