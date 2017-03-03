/*
 ** $Id: lstrlib.c,v 1.132.1.4 2008/07/11 17:27:21 roberto Exp $
 ** Standard library for string operations and pattern-matching
 ** See Copyright Notice in lua.h
 */
namespace KopiLua
{
	//using ptrdiff_t = System.Int32;
	//using lua_Integer = System.Int32;
	//using LUA_INTFRM_T = System.Int64;
	//using UNSIGNED_LUA_INTFRM_T = System.UInt64;

	public class LuaStrLib
	{
		public static int str_len(lua_State L) 
		{
			int[]/*uint*/ l = new int[1];
			LuaAuxLib.luaL_checklstring(L, 1, /*out*/ l);
			LuaAPI.lua_pushinteger(L, /*(int)*/l[0]);
			return 1;
		}

		private static int/*Int32*//*ptrdiff_t*/ posrelat(int/*Int32*//*ptrdiff_t*/ pos, int/*uint*/ len)
		{
			/* relative string position: negative means back from end */
			if (pos < 0) 
			{
				pos += (int/*Int32*//*ptrdiff_t*/)len + 1;
			}
			return (pos >= 0) ? pos : 0;
		}

		public static int str_sub(lua_State L) 
		{
			int[]/*uint*/ l = new int[1];
			CharPtr s = LuaAuxLib.luaL_checklstring(L, 1, /*out*/ l);
			int/*Int32*//*ptrdiff_t*/ start = posrelat(LuaAuxLib.luaL_checkinteger(L, 2), l[0]);
			int/*Int32*//*ptrdiff_t*/ end = posrelat(LuaAuxLib.luaL_optinteger(L, 3, -1), l[0]);
			if (start < 1) 
			{
				start = 1;
			}
			if (end > (int/*Int32*//*ptrdiff_t*/)l[0]) 
			{
				end = (int/*Int32*//*ptrdiff_t*/)l[0];
			}
			if (start <= end)
			{
				LuaAPI.lua_pushlstring(L, CharPtr.plus(s, start - 1), /*(uint)*/(end - start + 1));
			}
			else 
			{
				Lua.lua_pushliteral(L, CharPtr.toCharPtr(""));
			}
			return 1;
		}

		public static int str_reverse(lua_State L) 
		{
			int[]/*uint*/ l = new int[1];
			luaL_Buffer b = new luaL_Buffer();
			CharPtr s = LuaAuxLib.luaL_checklstring(L, 1, /*out*/ l);
			LuaAuxLib.luaL_buffinit(L, b);
			while ((l[0]--) != 0)
			{
				LuaAuxLib.luaL_addchar(b, s.get(l[0]));
			}
			LuaAuxLib.luaL_pushresult(b);
			return 1;
		}

		public static int str_lower(lua_State L) 
		{
			int[]/*uint*/ l = new int[1];
			int/*uint*/ i;
			luaL_Buffer b = new luaL_Buffer();
			CharPtr s = LuaAuxLib.luaL_checklstring(L, 1, /*out*/ l);
			LuaAuxLib.luaL_buffinit(L, b);
			for (i = 0; i < l[0]; i++)
			{
				LuaAuxLib.luaL_addchar(b, LuaConf.tolower(s.get(i)));
			}
			LuaAuxLib.luaL_pushresult(b);
			return 1;
		}

		public static int str_upper(lua_State L) 
		{
			int[]/*uint*/ l = new int[1];
			int/*uint*/ i;
			luaL_Buffer b = new luaL_Buffer();
			CharPtr s = LuaAuxLib.luaL_checklstring(L, 1, /*out*/ l);
			LuaAuxLib.luaL_buffinit(L, b);
			for (i = 0; i < l[0]; i++)
			{
				LuaAuxLib.luaL_addchar(b, LuaConf.toupper(s.get(i)));
			}
			LuaAuxLib.luaL_pushresult(b);
			return 1;
		}

		public static int str_rep(lua_State L) 
		{
			int[]/*uint*/ l = new int[1];
			luaL_Buffer b = new luaL_Buffer();
			CharPtr s = LuaAuxLib.luaL_checklstring(L, 1, /*out*/ l);
			int n = LuaAuxLib.luaL_checkint(L, 2);
			LuaAuxLib.luaL_buffinit(L, b);
			while (n-- > 0)
			{
				LuaAuxLib.luaL_addlstring(b, s, l[0]);
			}
			LuaAuxLib.luaL_pushresult(b);
			return 1;
		}

		public static int str_byte(lua_State L)
		{
			int[]/*uint*/ l = new int[1];
			CharPtr s = LuaAuxLib.luaL_checklstring(L, 1, /*out*/ l);
			int/*Int32*//*ptrdiff_t*/ posi = posrelat(LuaAuxLib.luaL_optinteger(L, 2, 1), l[0]);
            int/*Int32*//*ptrdiff_t*/ pose = posrelat(LuaAuxLib.luaL_optinteger(L, 3, posi), l[0]);
			int n, i;
			if (posi <= 0) 
			{
				posi = 1;
			}
			if ((int/*uint*/)pose > l[0])
			{
				pose = (int)l[0];
			}
			if (posi > pose) 
			{
				return 0;  /* empty interval; return no values */
			}
			n = (int)(pose -  posi + 1);
			if (posi + n <= pose)  /* overflow? */
			{
				LuaAuxLib.luaL_error(L, CharPtr.toCharPtr("string slice too long"));
			}
			LuaAuxLib.luaL_checkstack(L, n, CharPtr.toCharPtr("string slice too long"));
			for (i = 0; i < n; i++)
			{
				LuaAPI.lua_pushinteger(L, (byte)(s.get(posi + i - 1)));
			}
			return n;
		}

		public static int str_char(lua_State L) 
		{
			int n = LuaAPI.lua_gettop(L);  /* number of arguments */
			int i;
			luaL_Buffer b = new luaL_Buffer();
			LuaAuxLib.luaL_buffinit(L, b);
			for (i = 1; i <= n; i++) 
			{
				int c = LuaAuxLib.luaL_checkint(L, i);
				LuaAuxLib.luaL_argcheck(L, (byte)(c) == c, i, "invalid value");
				LuaAuxLib.luaL_addchar(b, (char)(byte)c);
			}
			LuaAuxLib.luaL_pushresult(b);
			return 1;
		}

		private static int writer(lua_State L, object b, int/*uint*/ size, object B, ClassType t)
		{
            //FIXME:b always is CharPtr
            //if (b.GetType() != typeof(CharPtr))
			if (t.GetTypeID() == ClassType.TYPE_CHARPTR)
            {
                byte[] bytes = t.ObjToBytes2(b);
                char[] chars = new char[bytes.Length];
                for (int i = 0; i < bytes.Length; i++)
                {
                    chars[i] = (char)bytes[i];
                }
                b = new CharPtr(chars);
			}
			LuaAuxLib.luaL_addlstring((luaL_Buffer)B, (CharPtr)b, size);
			return 0;
		}
		
		public class writer_delegate : lua_Writer
		{
			public int exec(lua_State L, CharPtr p, int/*uint*/ sz, object ud)
			{
				return writer(L, p, sz, ud, new ClassType(ClassType.TYPE_CHARPTR));
			}
		}

		public static int str_dump(lua_State L) 
		{
			luaL_Buffer b = new luaL_Buffer();
			LuaAuxLib.luaL_checktype(L, 1, Lua.LUA_TFUNCTION);
			LuaAPI.lua_settop(L, 1);
			LuaAuxLib.luaL_buffinit(L, b);
			if (LuaAPI.lua_dump(L, new writer_delegate(), b) != 0)
			{
				LuaAuxLib.luaL_error(L, CharPtr.toCharPtr("unable to dump given function"));
			}
			LuaAuxLib.luaL_pushresult(b);
			return 1;
		}

		/*
		 ** {======================================================
		 ** PATTERN MATCHING
		 ** =======================================================
		 */
		
		public const int CAP_UNFINISHED	= (-1);
		public const int CAP_POSITION = (-2);

		public const char L_ESC	= '%';
		public const string SPECIALS = "^$*+?.([%-";

		private static int check_capture(MatchState ms, int l) 
		{
			l -= '1';
			if (l < 0 || l >= ms.level || ms.capture[l].len == CAP_UNFINISHED)
			{
				return LuaAuxLib.luaL_error(ms.L, CharPtr.toCharPtr("invalid capture index"));
			}
			return l;
		}

		private static int capture_to_close(MatchState ms) 
		{
			int level = ms.level;
			for (level--; level >= 0; level--)
			{	
				if (ms.capture[level].len == CAP_UNFINISHED) 
				{
					return level;
				}
			}
			return LuaAuxLib.luaL_error(ms.L, CharPtr.toCharPtr("invalid pattern capture"));
		}

		private static CharPtr classend(MatchState ms, CharPtr p) 
		{
			p = new CharPtr(p);
			char c = p.get(0);
			p = p.next();
			switch (c) 
			{
				case L_ESC: 
					{
						if (p.get(0) == '\0')
						{
							LuaAuxLib.luaL_error(ms.L, CharPtr.toCharPtr("malformed pattern (ends with " + LuaConf.LUA_QL("%%") + ")"));
						}
						return CharPtr.plus(p, 1);
					}
				case '[': 
					{
						if (p.get(0) == '^') 
						{
							p = p.next();
						}
						do 
						{  
							/* look for a `]' */
							if (p.get(0) == '\0')
							{
								LuaAuxLib.luaL_error(ms.L, CharPtr.toCharPtr("malformed pattern (missing " + LuaConf.LUA_QL("]") + ")"));
							}
							c = p.get(0);
							p = p.next();
							if (c == L_ESC && p.get(0) != '\0')
							{
								p = p.next();  /* skip escapes (e.g. `%]') */
							}
						} while (p.get(0) != ']');
						return CharPtr.plus(p, 1);
					}
				default: 
					{
						return p;
					}
			}
		}


		private static int match_class(int c, int cl) 
		{
			bool res;
			switch (LuaConf.tolower(cl))
			{
				case 'a': 
					{
						res = LuaConf.isalpha(c); 
						break;
					}
				case 'c': 
					{
						res = LuaConf.iscntrl(c); 
						break;
					}
				case 'd': 
					{
						res = LuaConf.isdigit(c); 
						break;
					}
				case 'l': 
					{
						res = LuaConf.islower(c); 
						break;
					}
				case 'p': 
					{
						res = LuaConf.ispunct(c); 
						break;
					}
				case 's': 
					{
						res = LuaConf.isspace(c); 
						break;
					}
				case 'u': 
					{
						res = LuaConf.isupper(c); 
						break;
					}
				case 'w': 
					{
						res = LuaConf.isalnum(c); 
						break;
					}
				case 'x': 
					{
						res = LuaConf.isxdigit((char)c); 
						break;
					}
				case 'z' : 
					{
						res = (c == 0); 
						break;
					}
				default: 
					{
						return (cl == c) ? 1 : 0;
					}
			}
			return (LuaConf.islower(cl) ? (res ? 1 : 0) : ((!res) ? 1 : 0));
		}

		private static int matchbracketclass(int c, CharPtr p, CharPtr ec) 
		{
			int sig = 1;
			if (p.get(1) == '^') 
			{
				sig = 0;
				p = p.next();  /* skip the `^' */
			}
			while (CharPtr.lessThan((p = p.next()), ec)) 
			{
				if (CharPtr.isEqualChar(p, L_ESC)) 
				{
					p = p.next();
					if (match_class(c, (byte)(p.get(0))) != 0)
					{
						return sig;
					}
				}
                else if ((p.get(1) == '-') && (CharPtr.lessThan(CharPtr.plus(p, 2), ec)))
				{
					p = CharPtr.plus(p, 2);
					if ((byte)((p.get(-2))) <= c && (c <= (byte)p.get(0)))
					{
						return sig;
					}
				}
				else if ((byte)(p.get(0)) == c) 
				{
					return sig;
				}
			}
			return (sig == 0) ? 1 : 0;
		}

		private static int singlematch(int c, CharPtr p, CharPtr ep) 
		{
			switch (p.get(0)) 
			{
				case '.': 
					{
						return 1;  /* matches any char */
					}
				case L_ESC: 
					{
						return match_class(c, (byte)(p.get(1)));
					}
				case '[': 
					{
						return matchbracketclass(c, p, CharPtr.minus(ep, 1));
					}
				default: 
					{
						return ((byte)(p.get(0)) == c) ? 1 : 0;
					}
			}
		}

		private static CharPtr matchbalance(MatchState ms, CharPtr s,
			CharPtr p) 
		{
			if ((p.get(0) == 0) || (p.get(1) == 0))
			{
				LuaAuxLib.luaL_error(ms.L, CharPtr.toCharPtr("unbalanced pattern"));
			}
			if (s.get(0) != p.get(0)) 
			{
				return null;
			}
			else 
			{
				int b = p.get(0);
				int e = p.get(1);
				int cont = 1;
                while (CharPtr.lessThan((s = s.next()), ms.src_end)) 
				{
					if (s.get(0) == e)
					{
						if (--cont == 0) 
						{
							return CharPtr.plus(s, 1);
						}
					}
					else if (s.get(0) == b) 
					{
						cont++;
					}
				}
			}
			return null;  /* string ends out of balance */
		}

		private static CharPtr max_expand(MatchState ms, CharPtr s,
			CharPtr p, CharPtr ep) 
		{
            int/*Int32*//*ptrdiff_t*/ i = 0;  /* counts maximum expand for item */
            while ((CharPtr.lessThan(CharPtr.plus(s, i), ms.src_end)) && (singlematch((byte)(s.get(i)), p, ep) != 0))
			{
				i++;
			}
			/* keeps trying to match with the maximum repetitions */
			while (i >= 0) 
			{
				CharPtr res = match(ms, CharPtr.plus(s, i), CharPtr.plus(ep, 1));
				if (CharPtr.isNotEqual(res, null)) 
				{
					return res;
				}
				i--;  /* else didn't match; reduce 1 repetition to try again */
			}
			return null;
		}

		private static CharPtr min_expand(MatchState ms, CharPtr s,
			CharPtr p, CharPtr ep) 
		{
			for (;;)
			{
				CharPtr res = match(ms, s, CharPtr.plus(ep, 1));
				if (CharPtr.isNotEqual(res, null))
				{
					return res;
				}
                else if ((CharPtr.lessThan(s, ms.src_end)) && (singlematch((byte)(s.get(0)), p, ep) != 0))
				{
					s = s.next();  /* try with one more repetition */
				}
				else 
				{
					return null;
				}
			}
		}

		private static CharPtr start_capture(MatchState ms, CharPtr s,
			CharPtr p, int what) 
		{
			CharPtr res;
			int level = ms.level;
			if (level >= LuaConf.LUA_MAXCAPTURES) 
			{
				LuaAuxLib.luaL_error(ms.L, CharPtr.toCharPtr("too many captures"));
			}
			ms.capture[level].init = s;
			ms.capture[level].len = what;
			ms.level = level+1;
			if (CharPtr.isEqual((res = match(ms, s, p)), null))  /* match failed? */
			{
				ms.level--;  /* undo capture */
			}
			return res;
		}

		private static CharPtr end_capture(MatchState ms, CharPtr s,
			CharPtr p) 
		{
			int l = capture_to_close(ms);
			CharPtr res;
			ms.capture[l].len = CharPtr.minus(s, ms.capture[l].init);  /* close capture */
			if (CharPtr.isEqual((res = match(ms, s, p)), null))  /* match failed? */
			{
				ms.capture[l].len = CAP_UNFINISHED;  /* undo capture */
			}
			return res;
		}

		private static CharPtr match_capture(MatchState ms, CharPtr s, int l)
		{
			int/*uint*/ len;
			l = check_capture(ms, l);
			len = /*(uint)*/ms.capture[l].len;
			if ((int/*uint*/)CharPtr.minus(ms.src_end, s) >= len &&
			    LuaConf.memcmp(ms.capture[l].init, s, len) == 0)
			{
				return CharPtr.plus(s, len);
			}
			else 
			{
				return null;
			}
		}

		private static CharPtr match(MatchState ms, CharPtr s, CharPtr p) 
		{
			s = new CharPtr(s);
			p = new CharPtr(p);
			//init: /* using goto's to optimize tail recursion */
			while (true)
			{
				bool init = false;
				switch (p.get(0))
				{
					case '(': 
						{  
							/* start capture */
							if (p.get(1) == ')')  /* position capture? */
							{
								return start_capture(ms, s, CharPtr.plus(p, 2), CAP_POSITION);
							}
							else
							{
								return start_capture(ms, s, CharPtr.plus(p, 1), CAP_UNFINISHED);
							}
						}
					case ')': 
						{  
							/* end capture */
							return end_capture(ms, s, CharPtr.plus(p, 1));
						}
					case L_ESC: 
						{
							bool init2 = false;
							switch (p.get(1)) 
							{
								case 'b': 
									{  
										/* balanced string? */
										s = matchbalance(ms, s, CharPtr.plus(p, 2));
										if (CharPtr.isEqual(s, null)) 
										{
											return null;
										}
										p = CharPtr.plus(p, 4);
										//goto init;  /* else return match(ms, s, p+4); */
										init2 = true;
										break;
									}
								case 'f': 
									{  
										/* frontier? */
										CharPtr ep; 
										char previous;
										p = CharPtr.plus(p, 2);
										if (p.get(0) != '[')
										{
											LuaAuxLib.luaL_error(ms.L, CharPtr.toCharPtr("missing " + LuaConf.LUA_QL("[") + " after " +
												LuaConf.LUA_QL("%%f") + " in pattern"));
										}
										ep = classend(ms, p);  /* points to what is next */
										previous = (CharPtr.isEqual(s, ms.src_init)) ? '\0' : s.get(-1);
										if ((matchbracketclass((byte)(previous), p, CharPtr.minus(ep, 1)) != 0) ||
										    (matchbracketclass((byte)(s.get(0)), p, CharPtr.minus(ep, 1)) == 0))
										{
											return null;
										}
										p = ep; 
										//goto init;  /* else return match(ms, s, ep); */
										init2 = true;
										break;
									}
								default: 
									{
										if (LuaConf.isdigit((byte)(p.get(1))))
										{
											/* capture results (%0-%9)? */
											s = match_capture(ms, s, (byte)(p.get(1)));
											if (CharPtr.isEqual(s, null)) 
											{
												return null;
											}
											p = CharPtr.plus(p, 2);
											//goto init;  
                                            // else return match(ms, s, p+2)
											init2 = true;
											break;
										}
										//goto dflt;  
                                        // case default
                                        if (true)
										{
											//------------------dflt start--------------		                        
											/* it is a pattern item */
											CharPtr ep = classend(ms, p);  /* points to what is next */
					                        int m = (CharPtr.lessThan(s, ms.src_end)) && (singlematch((byte)(s.get(0)), p, ep) != 0) ? 1 : 0;
											bool init3 = false;
					                        switch (ep.get(0))
											{
												case '?': 
													{  
														/* optional */
														CharPtr res;
														if ((m != 0) && CharPtr.isNotEqual((res = match(ms, CharPtr.plus(s, 1), CharPtr.plus(ep, 1))), null))
														{
															return res;
														}
														p = CharPtr.plus(ep, 1);
														//goto init;  /* else return match(ms, s, ep+1); */
														init3 = true;
														break;
					                        		}
												case '*': 
													{  
														/* 0 or more repetitions */
														return max_expand(ms, s, p, ep);
													}
												case '+': 
													{  
														/* 1 or more repetitions */
														return ((m!=0) ? max_expand(ms, CharPtr.plus(s, 1), p, ep) : null);
													}
												case '-': 
													{  
														/* 0 or more repetitions (minimum) */
														return min_expand(ms, s, p, ep);
													}
												default: 
					                        		{
														if (m == 0) 
														{
															return null;
														}
														s = s.next(); 
														p = ep; 
														//goto init;  /* else return match(ms, s+1, ep); */
														init3 = true;
														break;
					                        		}
											}
					                        if (init3 == true)
					                        {
					                        	init2 = true;
					                        	break;
					                        }
					                        else
					                        {
					                        	break;
					                        }
											//------------------dflt end--------------	
										}
									}
							}
							if (init2 == true)
							{
								init = true;
								break;
							}
							else
							{
								break;
							}
						}
					case '\0': 
						{  
							/* end of pattern */
							return s;  /* match succeeded */
						}
					case '$': 
						{
							if (p.get(1) == '\0')  /* is the `$' the last char in pattern? */
							{
								return (CharPtr.isEqual(s, ms.src_end)) ? s : null;  /* check end of string */
							}
							else 
							{
								//goto dflt;
								//------------------dflt start--------------		                        
								/* it is a pattern item */
								CharPtr ep = classend(ms, p);  /* points to what is next */
		                        int m = (CharPtr.lessThan(s, ms.src_end)) && (singlematch((byte)(s.get(0)), p, ep) != 0) ? 1 : 0;
								bool init2 = false;
		                        switch (ep.get(0))
								{
									case '?': 
										{  
											/* optional */
											CharPtr res;
											if ((m != 0) && CharPtr.isNotEqual((res = match(ms, CharPtr.plus(s, 1), CharPtr.plus(ep, 1))), null))
											{
												return res;
											}
											p = CharPtr.plus(ep, 1);
											//goto init;  /* else return match(ms, s, ep+1); */
											init2 = true;
											break;
		                        		}
									case '*': 
										{  
											/* 0 or more repetitions */
											return max_expand(ms, s, p, ep);
										}
									case '+': 
										{  
											/* 1 or more repetitions */
											return ((m!=0) ? max_expand(ms, CharPtr.plus(s, 1), p, ep) : null);
										}
									case '-': 
										{  
											/* 0 or more repetitions (minimum) */
											return min_expand(ms, s, p, ep);
										}
									default: 
		                        		{
											if (m == 0) 
											{
												return null;
											}
											s = s.next(); 
											p = ep; 
											//goto init;  
                                            // else return match(ms, s+1, ep);
											init2 = true;
											break;
		                        		}
								}
		                        if (init2 == true)
		                        {
		                        	init = true;
		                        	break;
		                        }
		                        else
		                        {
		                        	break;
		                        }
								//------------------dflt end--------------		                        
							}
						}
					default: 
						{
                            //dflt: 
							// it is a pattern item
							CharPtr ep = classend(ms, p);  /* points to what is next */
	                        int m = (CharPtr.lessThan(s, ms.src_end)) && (singlematch((byte)(s.get(0)), p, ep) != 0) ? 1 : 0;
							bool init2 = false;
	                        switch (ep.get(0))
							{
								case '?': 
									{  
										/* optional */
										CharPtr res;
										if ((m != 0) && CharPtr.isNotEqual((res = match(ms, CharPtr.plus(s, 1), CharPtr.plus(ep, 1))), null))
										{
											return res;
										}
										p = CharPtr.plus(ep, 1);
										//goto init;  /* else return match(ms, s, ep+1); */
										init2 = true;
										break;
	                        		}
								case '*': 
									{  
										/* 0 or more repetitions */
										return max_expand(ms, s, p, ep);
									}
								case '+': 
									{  
										/* 1 or more repetitions */
										return ((m!=0) ? max_expand(ms, CharPtr.plus(s, 1), p, ep) : null);
									}
								case '-': 
									{  
										/* 0 or more repetitions (minimum) */
										return min_expand(ms, s, p, ep);
									}
								default: 
	                        		{
										if (m == 0) 
										{
											return null;
										}
										s = s.next(); 
										p = ep; 
										//goto init;  /* else return match(ms, s+1, ep); */
										init2 = true;
										break;
	                        		}
							}
	                        if (init2 == true)
	                        {
	                        	init = true;
	                        	break;
	                        }
	                        else
	                        {
	                        	break;
	                        }
						}
				}
				if (init == true) 
				{
					continue;
				}
				else
				{
					break;
				}
			}
			return null; //FIXME:unreachable
		}

		private static CharPtr lmemfind(CharPtr s1, int/*uint*/ l1,
			CharPtr s2, int/*uint*/ l2) 
		{
			if (l2 == 0) 
			{
				return s1;  /* empty strings are everywhere */
			}
			else if (l2 > l1) 
			{
				return null;  /* avoids a negative `l1' */
			}
			else 
			{
				CharPtr init;  /* to search for a `*s2' inside `s1' */
				l2--;  /* 1st char will be checked by `memchr' */
				l1 = l1 - l2;  /* `s2' cannot be found after that */
				while (l1 > 0 && CharPtr.isNotEqual((init = LuaConf.memchr(s1, s2.get(0), l1)), null))
				{
					init = init.next();   /* 1st char is already checked */
					if (LuaConf.memcmp(init, CharPtr.plus(s2, 1), l2) == 0)
					{
						return CharPtr.minus(init, 1);
					}
					else 
					{  
						/* correct `l1' and `s1' to try again */
						l1 -= (/*uint*/int)CharPtr.minus(init, s1);
						s1 = init;
					}
				}
				return null;  /* not found */
			}
		}

		private static void push_onecapture(MatchState ms, int i, CharPtr s,
			CharPtr e) 
		{
			if (i >= ms.level) 
			{
				if (i == 0)  /* ms.level == 0, too */
				{
					LuaAPI.lua_pushlstring(ms.L, s, /*(uint)*/CharPtr.minus(e, s));  /* add whole match */
				}
				else
				{
					LuaAuxLib.luaL_error(ms.L, CharPtr.toCharPtr("invalid capture index"));
				}
			}
			else 
			{
                int/*Int32*//*ptrdiff_t*/ l = ms.capture[i].len;
				if (l == CAP_UNFINISHED) 
				{
					LuaAuxLib.luaL_error(ms.L, CharPtr.toCharPtr("unfinished capture"));
				}
				if (l == CAP_POSITION)
				{
					LuaAPI.lua_pushinteger(ms.L, CharPtr.minus(ms.capture[i].init, ms.src_init) + 1);
				}
				else
				{
					LuaAPI.lua_pushlstring(ms.L, ms.capture[i].init, /*(uint)*/l);
				}
			}
		}

		private static int push_captures(MatchState ms, CharPtr s, CharPtr e) 
		{
			int i;
			int nlevels = ((ms.level == 0) && (CharPtr.isNotEqual(s, null))) ? 1 : ms.level;
			LuaAuxLib.luaL_checkstack(ms.L, nlevels, CharPtr.toCharPtr("too many captures"));
			for (i = 0; i < nlevels; i++)
			{
				push_onecapture(ms, i, s, e);
			}
			return nlevels;  /* number of strings pushed */
		}

		private static int str_find_aux(lua_State L, int find) 
		{
			int[]/*uint*/ l1 = new int[1];
			int[]/*uint*/ l2 = new int[1];
			CharPtr s = LuaAuxLib.luaL_checklstring(L, 1, /*out*/ l1);
			CharPtr p = LuaAuxLib.luaL_checklstring(L, 2, /*out*/ l2);
            int/*Int32*//*ptrdiff_t*/ init = posrelat(LuaAuxLib.luaL_optinteger(L, 3, 1), l1[0]) - 1;
			if (init < 0) 
			{
				init = 0;
			}
            else if ((int/*uint*/)(init) > l1[0])
			{
                init = (int/*Int32*//*ptrdiff_t*/)l1[0];
			}
			if ((find != 0) && ((LuaAPI.lua_toboolean(L, 4) != 0) ||  /* explicit request? */
				CharPtr.isEqual(LuaConf.strpbrk(p, CharPtr.toCharPtr(SPECIALS)), null)))
			{  
				/* or no special characters? */
				/* do a plain search */
				CharPtr s2 = lmemfind(CharPtr.plus(s, init), (/*uint*/int)(l1[0] - init), p, (/*uint*/int)(l2[0]));
				if (CharPtr.isNotEqual(s2, null)) 
				{
					LuaAPI.lua_pushinteger(L, CharPtr.minus(s2, s) + 1);
					LuaAPI.lua_pushinteger(L, (int)(CharPtr.minus(s2, s) + l2[0]));
					return 2;
				}
			}
			else 
			{
				MatchState ms = new MatchState();
				int anchor = 0;
				if (p.get(0) == '^')
				{
					p = p.next();
					anchor = 1;
				}
				CharPtr s1 = CharPtr.plus(s, init);
				ms.L = L;
				ms.src_init = s;
				ms.src_end = CharPtr.plus(s, l1[0]);
				do 
				{
					CharPtr res;
					ms.level = 0;
					if (CharPtr.isNotEqual((res = match(ms, s1, p)), null)) 
					{
						if (find != 0) 
						{
							LuaAPI.lua_pushinteger(L, CharPtr.minus(s1, s) + 1);  /* start */
							LuaAPI.lua_pushinteger(L, CharPtr.minus(res, s));   /* end */
							return push_captures(ms, null, null) + 2;
						}
						else
						{
							return push_captures(ms, s1, res);
						}
					}
				} while ((CharPtr.lessEqual((s1 = s1.next()), ms.src_end)) && (anchor == 0));
			}
			LuaAPI.lua_pushnil(L);  /* not found */
			return 1;
		}

		public static int str_find(lua_State L) 
		{
			return str_find_aux(L, 1);
		}

		public static int str_match(lua_State L) 
		{
			return str_find_aux(L, 0);
		}

		public static int gmatch_aux(lua_State L) 
		{
			MatchState ms = new MatchState();
			int[]/*uint*/ ls = new int[1];
			CharPtr s = LuaAPI.lua_tolstring(L, Lua.lua_upvalueindex(1), /*out*/ ls);
			CharPtr p = Lua.lua_tostring(L, Lua.lua_upvalueindex(2));
			CharPtr src;
			ms.L = L;
			ms.src_init = s;
			ms.src_end = CharPtr.plus(s, ls[0]);
			for (src = CharPtr.plus(s, /*(uint)*/LuaAPI.lua_tointeger(L, Lua.lua_upvalueindex(3)));
                 CharPtr.lessEqual(src, ms.src_end);
			     src = src.next()) 
			{
				CharPtr e;
				ms.level = 0;
				if (CharPtr.isNotEqual((e = match(ms, src, p)), null)) 
				{
                    int/*Int32*//*lua_Integer*/ newstart = CharPtr.minus(e, s);
					if (CharPtr.isEqual(e, src)) 
					{
						newstart++;  /* empty match? go at least one position */
					}
					LuaAPI.lua_pushinteger(L, newstart);
					LuaAPI.lua_replace(L, Lua.lua_upvalueindex(3));
					return push_captures(ms, src, e);
				}
			}
			return 0;  /* not found */
		}

		public static int gmatch(lua_State L) 
		{
			LuaAuxLib.luaL_checkstring(L, 1);
			LuaAuxLib.luaL_checkstring(L, 2);
			LuaAPI.lua_settop(L, 2);
			LuaAPI.lua_pushinteger(L, 0);
			LuaAPI.lua_pushcclosure(L, new LuaStrLib_delegate("gmatch_aux"), 3);
			return 1;
		}

		public static int gfind_nodef(lua_State L) 
		{
			return LuaAuxLib.luaL_error(L, CharPtr.toCharPtr(LuaConf.LUA_QL("string.gfind") + " was renamed to " +
				LuaConf.LUA_QL("string.gmatch")));
		}

		private static void add_s(MatchState ms, luaL_Buffer b, CharPtr s,
			CharPtr e) 
		{
			int[]/*uint*/ l = new int[1];
			int i;
			CharPtr news = LuaAPI.lua_tolstring(ms.L, 3, /*out*/ l);
			for (i = 0; i < l[0]; i++)
			{
				if (news.get(i) != L_ESC)
				{
					LuaAuxLib.luaL_addchar(b, news.get(i));
				}
				else 
				{
					i++;  /* skip ESC */
					if (!LuaConf.isdigit((byte)(news.get(i))))
					{
						LuaAuxLib.luaL_addchar(b, news.get(i));
					}
					else if (news.get(i) == '0')
					{
						LuaAuxLib.luaL_addlstring(b, s, /*(uint)*/CharPtr.minus(e, s));
					}
					else 
					{
						push_onecapture(ms, news.get(i) - '1', s, e);
						LuaAuxLib.luaL_addvalue(b);  /* add capture to accumulated result */
					}
				}
			}
		}


		private static void add_value(MatchState ms, luaL_Buffer b, CharPtr s,
			CharPtr e) 
		{
			lua_State L = ms.L;
			switch (LuaAPI.lua_type(L, 3))
			{
				case Lua.LUA_TNUMBER:
				case Lua.LUA_TSTRING:
					{
						add_s(ms, b, s, e);
						return;
					}
				case Lua.LUA_TFUNCTION:
					{
						int n;
						LuaAPI.lua_pushvalue(L, 3);
						n = push_captures(ms, s, e);
						LuaAPI.lua_call(L, n, 1);
						break;
					}
				case Lua.LUA_TTABLE:
					{
						push_onecapture(ms, 0, s, e);
						LuaAPI.lua_gettable(L, 3);
						break;
					}
			}
			if (LuaAPI.lua_toboolean(L, -1) == 0)
			{  
				/* nil or false? */
				Lua.lua_pop(L, 1);
				LuaAPI.lua_pushlstring(L, s, /*(uint)*/CharPtr.minus(e, s));  /* keep original text */
			}
			else if (LuaAPI.lua_isstring(L, -1) == 0)
			{
				LuaAuxLib.luaL_error(L, CharPtr.toCharPtr("invalid replacement value (a %s)"), LuaAuxLib.luaL_typename(L, -1));
			}
			LuaAuxLib.luaL_addvalue(b);  /* add result to accumulator */
		}

		public static int str_gsub(lua_State L) 
		{
			int[]/*uint*/ srcl = new int[1];
			CharPtr src = LuaAuxLib.luaL_checklstring(L, 1, /*out*/ srcl);
			CharPtr p = LuaAuxLib.luaL_checkstring(L, 2);
			int tr = LuaAPI.lua_type(L, 3);
			int max_s = LuaAuxLib.luaL_optint(L, 4, (int)(srcl[0] + 1));
			int anchor = 0;
			if (p.get(0) == '^')
			{
				p = p.next();
				anchor = 1;
			}
			int n = 0;
			MatchState ms = new MatchState();
			luaL_Buffer b = new luaL_Buffer();
			LuaAuxLib.luaL_argcheck(L, tr == Lua.LUA_TNUMBER || tr == Lua.LUA_TSTRING ||
				tr == Lua.LUA_TFUNCTION || tr == Lua.LUA_TTABLE, 3,
			    "string/function/table expected");
			LuaAuxLib.luaL_buffinit(L, b);
			ms.L = L;
			ms.src_init = src;
			ms.src_end = CharPtr.plus(src, srcl[0]);
			while (n < max_s) 
			{
				CharPtr e;
				ms.level = 0;
				e = match(ms, src, p);
				if (CharPtr.isNotEqual(e, null)) 
				{
					n++;
					add_value(ms, b, src, e);
				}
				if ((CharPtr.isNotEqual(e, null)) && CharPtr.greaterThan(e, src)) /* non empty match? */
				{
					src = e;  /* skip it */
				}
                else if (CharPtr.lessThan(src, ms.src_end))
				{
					char c = src.get(0);
					src = src.next();
					LuaAuxLib.luaL_addchar(b, c);
				}
				else 
				{
					break;
				}
				if (anchor != 0) break;
			}
			LuaAuxLib.luaL_addlstring(b, src, /*(uint)*/CharPtr.minus(ms.src_end, src));
			LuaAuxLib.luaL_pushresult(b);
			LuaAPI.lua_pushinteger(L, n);  /* number of substitutions */
			return 2;
		}

		/* }====================================================== */

		/* maximum size of each formatted item (> len(format('%99.99f', -1e308))) */
		public const int MAX_ITEM = 512;
		/* valid flags in a format specification */
		public const string FLAGS = "-+ #0";
		/*
		 ** maximum size of each format specification (such as '%-099.99d')
		 ** (+10 accounts for %99.99x plus margin of error)
		 */
		public static readonly int MAX_FORMAT = (FLAGS.Length + 1) + (LuaConf.LUA_INTFRMLEN.Length + 1) + 10;

		private static void addquoted (lua_State L, luaL_Buffer b, int arg) 
		{
			int[]/*uint*/ l = new int[1];
			CharPtr s = LuaAuxLib.luaL_checklstring(L, arg, /*out*/ l);
			LuaAuxLib.luaL_addchar(b, '"');
			while ((l[0]--) != 0)
			{
				switch (s.get(0)) 
				{
					case '"': 
					case '\\': 
					case '\n': 
						{
							LuaAuxLib.luaL_addchar(b, '\\');
							LuaAuxLib.luaL_addchar(b, s.get(0));
							break;
						}
					case '\r': 
						{
							LuaAuxLib.luaL_addlstring(b, CharPtr.toCharPtr("\\r"), 2);
							break;
						}
					case '\0': 
						{
							LuaAuxLib.luaL_addlstring(b, CharPtr.toCharPtr("\\000"), 4);
							break;
						}
					default: 
						{
							LuaAuxLib.luaL_addchar(b, s.get(0));
							break;
						}
				}
				s = s.next();
			}
			LuaAuxLib.luaL_addchar(b, '"');
		}

		private static CharPtr scanformat(lua_State L, CharPtr strfrmt, CharPtr form) 
		{
			CharPtr p = strfrmt;
			while (p.get(0) != '\0' && CharPtr.isNotEqual(LuaConf.strchr(CharPtr.toCharPtr(FLAGS), p.get(0)), null)) 
			{
				p = p.next();  /* skip flags */
			}
			if ((int/*uint*/)(CharPtr.minus(p, strfrmt)) >= (FLAGS.Length + 1))
			{
				LuaAuxLib.luaL_error(L, CharPtr.toCharPtr("invalid format (repeated flags)"));
			}
			if (LuaConf.isdigit((byte)(p.get(0)))) 
			{
				p = p.next();  /* skip width */
			}
			if (LuaConf.isdigit((byte)(p.get(0)))) 
			{
				p = p.next();  /* (2 digits at most) */
			}
			if (p.get(0) == '.') 
			{
				p = p.next();
				if (LuaConf.isdigit((byte)(p.get(0)))) 
				{
					p = p.next();  /* skip precision */
				}
				if (LuaConf.isdigit((byte)(p.get(0)))) 
				{
					p = p.next();  /* (2 digits at most) */
				}
			}
			if (LuaConf.isdigit((byte)(p.get(0))))
			{
				LuaAuxLib.luaL_error(L, CharPtr.toCharPtr("invalid format (width or precision too long)"));
			}
			form.set(0, '%');
			form = form.next();
			LuaConf.strncpy(form, strfrmt, CharPtr.minus(p, strfrmt) + 1);
			form = CharPtr.plus(form, CharPtr.minus(p, strfrmt) + 1);
			form.set(0, '\0');
			return p;
		}

		private static void addintlen(CharPtr form) 
		{
			int/*uint*/ l = /*(uint)*/LuaConf.strlen(form);
			char spec = form.get(l - 1);
			LuaConf.strcpy(CharPtr.plus(form, l - 1), CharPtr.toCharPtr(LuaConf.LUA_INTFRMLEN));
			form.set(l + (LuaConf.LUA_INTFRMLEN.Length + 1) - 2, spec);
			form.set(l + (LuaConf.LUA_INTFRMLEN.Length + 1) - 1, '\0');
		}

		public static int str_format(lua_State L) 
		{
			int arg = 1;
			int[]/*uint*/ sfl = new int[1];
			CharPtr strfrmt = LuaAuxLib.luaL_checklstring(L, arg, /*out*/ sfl);
			CharPtr strfrmt_end = CharPtr.plus(strfrmt, sfl[0]);
			luaL_Buffer b = new luaL_Buffer();
			LuaAuxLib.luaL_buffinit(L, b);
            while (CharPtr.lessThan(strfrmt, strfrmt_end)) 
			{
				if (strfrmt.get(0) != L_ESC)
				{
					LuaAuxLib.luaL_addchar(b, strfrmt.get(0));
					strfrmt = strfrmt.next();
				}
				else if (strfrmt.get(1) == L_ESC)
				{
					LuaAuxLib.luaL_addchar(b, strfrmt.get(0));  /* %% */
					strfrmt = CharPtr.plus(strfrmt, 2);
				}
				else
				{ 
					/* format item */
					strfrmt = strfrmt.next();
					CharPtr form = CharPtr.toCharPtr(new char[MAX_FORMAT]);  /* to store the format (`%...') */
					CharPtr buff = CharPtr.toCharPtr(new char[MAX_ITEM]);  /* to store the formatted item */
					arg++;
					strfrmt = scanformat(L, strfrmt, form);
					char ch = strfrmt.get(0);
					strfrmt = strfrmt.next();
					switch (ch)
					{
						case 'c':
							{
								LuaConf.sprintf(buff, form, (int)LuaAuxLib.luaL_checknumber(L, arg));
								break;
							}
						case 'd':
						case 'i':
							{
								addintlen(form);
								LuaConf.sprintf(buff, form, (long/*Int64*//*LUA_INTFRM_T*/)LuaAuxLib.luaL_checknumber(L, arg));
								break;
							}
						case 'o':
						case 'u':
						case 'x':
						case 'X':
							{
								addintlen(form);
								LuaConf.sprintf(buff, form, (long/*UInt64*//*UNSIGNED_LUA_INTFRM_T*/)LuaAuxLib.luaL_checknumber(L, arg));
								break;
							}
						case 'e':
						case 'E':
						case 'f':
						case 'g':
						case 'G':
							{
								LuaConf.sprintf(buff, form, (double)LuaAuxLib.luaL_checknumber(L, arg));
								break;
							}
						case 'q':
							{
								addquoted(L, b, arg);
								continue;  /* skip the 'addsize' at the end */
							}
						case 's':
							{
								int[]/*uint*/ l = new int[1];
								CharPtr s = LuaAuxLib.luaL_checklstring(L, arg, /*out*/ l);
								if (CharPtr.isEqual(LuaConf.strchr(form, '.'), null) && l[0] >= 100)
								{
									/* no precision and string is too long to be formatted;
									 keep original string */
									LuaAPI.lua_pushvalue(L, arg);
									LuaAuxLib.luaL_addvalue(b);
									continue;  /* skip the `addsize' at the end */
								}
								else
								{
									LuaConf.sprintf(buff, form, s);
									break;
								}
							}
						default:
							{  /* also treat cases `pnLlh' */
								return LuaAuxLib.luaL_error(L, CharPtr.toCharPtr("invalid option " + LuaConf.LUA_QL("%%%c") + " to " +
									LuaConf.LUA_QL("format")), strfrmt.get(-1));
							}
					}
					LuaAuxLib.luaL_addlstring(b, buff, /*(uint)*/LuaConf.strlen(buff));
				}
			}
			LuaAuxLib.luaL_pushresult(b);
			return 1;
		}

		private readonly static luaL_Reg[] strlib = {
			new luaL_Reg(CharPtr.toCharPtr("byte"), new LuaStrLib_delegate("str_byte")),
			new luaL_Reg(CharPtr.toCharPtr("char"), new LuaStrLib_delegate("str_char")),
			new luaL_Reg(CharPtr.toCharPtr("dump"), new LuaStrLib_delegate("str_dump")),
			new luaL_Reg(CharPtr.toCharPtr("find"), new LuaStrLib_delegate("str_find")),
			new luaL_Reg(CharPtr.toCharPtr("format"), new LuaStrLib_delegate("str_format")),
			new luaL_Reg(CharPtr.toCharPtr("gfind"), new LuaStrLib_delegate("gfind_nodef")),
			new luaL_Reg(CharPtr.toCharPtr("gmatch"), new LuaStrLib_delegate("gmatch")),
			new luaL_Reg(CharPtr.toCharPtr("gsub"), new LuaStrLib_delegate("str_gsub")),
			new luaL_Reg(CharPtr.toCharPtr("len"), new LuaStrLib_delegate("str_len")),
			new luaL_Reg(CharPtr.toCharPtr("lower"), new LuaStrLib_delegate("str_lower")),
			new luaL_Reg(CharPtr.toCharPtr("match"), new LuaStrLib_delegate("str_match")),
			new luaL_Reg(CharPtr.toCharPtr("rep"), new LuaStrLib_delegate("str_rep")),
			new luaL_Reg(CharPtr.toCharPtr("reverse"), new LuaStrLib_delegate("str_reverse")),
			new luaL_Reg(CharPtr.toCharPtr("sub"), new LuaStrLib_delegate("str_sub")),
			new luaL_Reg(CharPtr.toCharPtr("upper"), new LuaStrLib_delegate("str_upper")),
			new luaL_Reg(null, null)
		};

		private static void createmetatable(lua_State L) 
		{
			LuaAPI.lua_createtable(L, 0, 1);  /* create metatable for strings */
			Lua.lua_pushliteral(L, CharPtr.toCharPtr(""));  /* dummy string */
			LuaAPI.lua_pushvalue(L, -2);
			LuaAPI.lua_setmetatable(L, -2);  /* set string metatable */
			Lua.lua_pop(L, 1);  /* pop dummy string */
			LuaAPI.lua_pushvalue(L, -2);  /* string library... */
			LuaAPI.lua_setfield(L, -2, CharPtr.toCharPtr("__index"));  /* ...is the __index metamethod */
			Lua.lua_pop(L, 1);  /* pop metatable */
		}

		/*
		 ** Open string library
		 */
		public static int luaopen_string(lua_State L) 
		{
			LuaAuxLib.luaL_register(L, CharPtr.toCharPtr(LuaLib.LUA_STRLIBNAME), strlib);
			//#if LUA_COMPAT_GFIND
			LuaAPI.lua_getfield(L, -1, CharPtr.toCharPtr("gmatch"));
			LuaAPI.lua_setfield(L, -2, CharPtr.toCharPtr("gfind"));
			//#endif
			createmetatable(L);
			return 1;
		}
	}
}
