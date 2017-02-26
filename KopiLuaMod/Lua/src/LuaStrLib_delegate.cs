namespace KopiLua
{
	public class LuaStrLib_delegate : lua_CFunction
	{
		private string name;
		
		public LuaStrLib_delegate(string name)
		{
			this.name = name;
		}
		
		public int exec(lua_State L)
		{
			if ("str_byte".Equals(name))
			{
				return LuaStrLib.str_byte(L);
			} 
			else if ("str_char".Equals(name)) 
			{
				return LuaStrLib.str_char(L);
			} 
			else if ("str_dump".Equals(name)) 
			{
				return LuaStrLib.str_dump(L);
			} 
			else if ("str_find".Equals(name)) 
			{
			    return LuaStrLib.str_find(L);
			}
			else if ("str_format".Equals(name))
			{
			    return LuaStrLib.str_format(L);
			}
			else if ("gfind_nodef".Equals(name))
			{
				return LuaStrLib.gfind_nodef(L);
			}
			else if ("gmatch".Equals(name))
			{
				return LuaStrLib.gmatch(L);
			}
			else if ("str_gsub".Equals(name))
			{
				return LuaStrLib.str_gsub(L);
			}
			else if ("str_len".Equals(name))
			{
				return LuaStrLib.str_len(L);
			}
			else if ("str_lower".Equals(name))
			{
				return LuaStrLib.str_lower(L);
			}
			else if ("str_match".Equals(name))
			{
				return LuaStrLib.str_match(L);
			}
			else if ("str_rep".Equals(name))
			{
				return LuaStrLib.str_rep(L);
			}
			else if ("str_reverse".Equals(name))
			{
				return LuaStrLib.str_reverse(L);
			}
			else if ("str_sub".Equals(name))
			{
				return LuaStrLib.str_sub(L);
			}
			else if ("str_upper".Equals(name))
			{
				return LuaStrLib.str_upper(L);
			}
			else if ("gmatch_aux".Equals(name))
			{
				return LuaStrLib.gmatch_aux(L);
			}
			else
			{
				return 0;
			}
		}
	}
}
