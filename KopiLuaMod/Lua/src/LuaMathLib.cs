/*
 ** $Id: lmathlib.c,v 1.67.1.1 2007/12/27 13:02:25 roberto Exp $
 ** Standard mathematical library
 ** See Copyright Notice in lua.h
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace KopiLua
{
	//using lua_Number = System.Double;

	public class LuaMathLib
	{
		public const double PI = 3.14159265358979323846;
		public const double RADIANS_PER_DEGREE = PI / 180.0;

		private static int math_abs(lua_State L) 
		{
			LuaAPI.lua_pushnumber(L, Math.Abs(LuaAuxLib.luaL_checknumber(L, 1)));
			return 1;
		}

		private static int math_sin(lua_State L) 
		{
			LuaAPI.lua_pushnumber(L, Math.Sin(LuaAuxLib.luaL_checknumber(L, 1)));
			return 1;
		}

		private static int math_sinh(lua_State L) 
		{
			LuaAPI.lua_pushnumber(L, Math.Sinh(LuaAuxLib.luaL_checknumber(L, 1)));
			return 1;
		}

		private static int math_cos(lua_State L)
		{
			LuaAPI.lua_pushnumber(L, Math.Cos(LuaAuxLib.luaL_checknumber(L, 1)));
			return 1;
		}

		private static int math_cosh(lua_State L) 
		{
			LuaAPI.lua_pushnumber(L, Math.Cosh(LuaAuxLib.luaL_checknumber(L, 1)));
			return 1;
		}

		private static int math_tan(lua_State L) 
		{
			LuaAPI.lua_pushnumber(L, Math.Tan(LuaAuxLib.luaL_checknumber(L, 1)));
			return 1;
		}

		private static int math_tanh(lua_State L) 
		{
			LuaAPI.lua_pushnumber(L, Math.Tanh(LuaAuxLib.luaL_checknumber(L, 1)));
			return 1;
		}

		private static int math_asin(lua_State L) 
		{
			LuaAPI.lua_pushnumber(L, Math.Asin(LuaAuxLib.luaL_checknumber(L, 1)));
			return 1;
		}

		private static int math_acos(lua_State L) 
		{
			LuaAPI.lua_pushnumber(L, Math.Acos(LuaAuxLib.luaL_checknumber(L, 1)));
			return 1;
		}

		private static int math_atan(lua_State L) 
		{
			LuaAPI.lua_pushnumber(L, Math.Atan(LuaAuxLib.luaL_checknumber(L, 1)));
			return 1;
		}

		private static int math_atan2(lua_State L) 
		{
			LuaAPI.lua_pushnumber(L, Math.Atan2(LuaAuxLib.luaL_checknumber(L, 1), LuaAuxLib.luaL_checknumber(L, 2)));
			return 1;
		}

		private static int math_ceil(lua_State L)
		{
			LuaAPI.lua_pushnumber(L, Math.Ceiling(LuaAuxLib.luaL_checknumber(L, 1)));
			return 1;
		}

		private static int math_floor(lua_State L)
		{
			LuaAPI.lua_pushnumber(L, Math.Floor(LuaAuxLib.luaL_checknumber(L, 1)));
			return 1;
		}

		private static int math_fmod(lua_State L)
		{
			LuaAPI.lua_pushnumber(L, LuaConf.fmod(LuaAuxLib.luaL_checknumber(L, 1), LuaAuxLib.luaL_checknumber(L, 2)));
			return 1;
		}

		private static int math_modf(lua_State L) 
		{
			double ip;
			double fp = LuaConf.modf(LuaAuxLib.luaL_checknumber(L, 1), out ip);
			LuaAPI.lua_pushnumber(L, ip);
			LuaAPI.lua_pushnumber(L, fp);
			return 2;
		}

		private static int math_sqrt(lua_State L)
		{
			LuaAPI.lua_pushnumber(L, Math.Sqrt(LuaAuxLib.luaL_checknumber(L, 1)));
			return 1;
		}

		private static int math_pow(lua_State L)
		{
			LuaAPI.lua_pushnumber(L, Math.Pow(LuaAuxLib.luaL_checknumber(L, 1), LuaAuxLib.luaL_checknumber(L, 2)));
			return 1;
		}

		private static int math_log(lua_State L)
		{
			LuaAPI.lua_pushnumber(L, Math.Log(LuaAuxLib.luaL_checknumber(L, 1)));
			return 1;
		}

		private static int math_log10(lua_State L)
		{
			LuaAPI.lua_pushnumber(L, Math.Log10(LuaAuxLib.luaL_checknumber(L, 1)));
			return 1;
		}

		private static int math_exp(lua_State L)
		{
			LuaAPI.lua_pushnumber(L, Math.Exp(LuaAuxLib.luaL_checknumber(L, 1)));
			return 1;
		}

		private static int math_deg(lua_State L)
		{
			LuaAPI.lua_pushnumber(L, LuaAuxLib.luaL_checknumber(L, 1) / RADIANS_PER_DEGREE);
			return 1;
		}

		private static int math_rad(lua_State L)
		{
			LuaAPI.lua_pushnumber(L, LuaAuxLib.luaL_checknumber(L, 1) * RADIANS_PER_DEGREE);
			return 1;
		}

		private static int math_frexp(lua_State L)
		{
			int e;
			LuaAPI.lua_pushnumber(L, LuaConf.frexp(LuaAuxLib.luaL_checknumber(L, 1), out e));
			LuaAPI.lua_pushinteger(L, e);
			return 2;
		}

		private static int math_ldexp(lua_State L)
		{
			LuaAPI.lua_pushnumber(L, LuaConf.ldexp(LuaAuxLib.luaL_checknumber(L, 1), LuaAuxLib.luaL_checkint(L, 2)));
			return 1;
		}

		private static int math_min(lua_State L)
		{
			int n = LuaAPI.lua_gettop(L);  /* number of arguments */
			Double/*lua_Number*/ dmin = LuaAuxLib.luaL_checknumber(L, 1);
			int i;
			for (i = 2; i <= n; i++) 
			{
				Double/*lua_Number*/ d = LuaAuxLib.luaL_checknumber(L, i);
				if (d < dmin)
				{
					dmin = d;
				}
			}
			LuaAPI.lua_pushnumber(L, dmin);
			return 1;
		}


		private static int math_max(lua_State L) 
		{
			int n = LuaAPI.lua_gettop(L);  /* number of arguments */
			Double/*lua_Number*/ dmax = LuaAuxLib.luaL_checknumber(L, 1);
			int i;
			for (i = 2; i <= n; i++) 
			{
				Double/*lua_Number*/ d = LuaAuxLib.luaL_checknumber(L, i);
				if (d > dmax)
				{
					dmax = d;
				}
			}
			LuaAPI.lua_pushnumber(L, dmax);
			return 1;
		}

		private static Random rng = new Random();

		private static int math_random(lua_State L) 
		{
			/* the `%' avoids the (rare) case of r==1, and is needed also because on
			 some systems (SunOS!) `rand()' may return a value larger than RAND_MAX */
			//lua_Number r = (lua_Number)(rng.Next()%RAND_MAX) / (lua_Number)RAND_MAX;
			Double/*lua_Number*/ r = (Double/*lua_Number*/)rng.NextDouble();
			switch (LuaAPI.lua_gettop(L))
			{  
				/* check number of arguments */
				case 0: 
					{  
						/* no arguments */
						LuaAPI.lua_pushnumber(L, r);  /* Number between 0 and 1 */
						break;
					}
				case 1: 
					{  
						/* only upper limit */
						int u = LuaAuxLib.luaL_checkint(L, 1);
						LuaAuxLib.luaL_argcheck(L, 1 <= u, 1, "interval is empty");
						LuaAPI.lua_pushnumber(L, Math.Floor(r * u) + 1);  /* int between 1 and `u' */
						break;
					}
				case 2: 
					{  
						/* lower and upper limits */
						int l = LuaAuxLib.luaL_checkint(L, 1);
						int u = LuaAuxLib.luaL_checkint(L, 2);
						LuaAuxLib.luaL_argcheck(L, l <= u, 2, "interval is empty");
						LuaAPI.lua_pushnumber(L, Math.Floor(r * (u - l + 1)) + l);  /* int between `l' and `u' */
						break;
					}
				default: 
					{
						return LuaAuxLib.luaL_error(L, "wrong number of arguments");
					}
			}
			return 1;
		}


		private static int math_randomseed(lua_State L) 
		{
			//srand(luaL_checkint(L, 1));
			rng = new Random(LuaAuxLib.luaL_checkint(L, 1));
			return 0;
		}

		private readonly static luaL_Reg[] mathlib = {
			new luaL_Reg("abs", math_abs),
			new luaL_Reg("acos", math_acos),
			new luaL_Reg("asin", math_asin),
			new luaL_Reg("atan2", math_atan2),
			new luaL_Reg("atan", math_atan),
			new luaL_Reg("ceil", math_ceil),
			new luaL_Reg("cosh", math_cosh),
			new luaL_Reg("cos", math_cos),
			new luaL_Reg("deg", math_deg),
			new luaL_Reg("exp", math_exp),
			new luaL_Reg("floor", math_floor),
			new luaL_Reg("fmod", math_fmod),
			new luaL_Reg("frexp", math_frexp),
			new luaL_Reg("ldexp", math_ldexp),
			new luaL_Reg("log10", math_log10),
			new luaL_Reg("log", math_log),
			new luaL_Reg("max", math_max),
			new luaL_Reg("min", math_min),
			new luaL_Reg("modf", math_modf),
			new luaL_Reg("pow", math_pow),
			new luaL_Reg("rad", math_rad),
			new luaL_Reg("random", math_random),
			new luaL_Reg("randomseed", math_randomseed),
			new luaL_Reg("sinh", math_sinh),
			new luaL_Reg("sin", math_sin),
			new luaL_Reg("sqrt", math_sqrt),
			new luaL_Reg("tanh", math_tanh),
			new luaL_Reg("tan", math_tan),
			new luaL_Reg(null, null)
		};

		/*
		 ** Open math library
		 */
		public static int luaopen_math (lua_State L)
		{
			LuaAuxLib.luaL_register(L, LuaLib.LUA_MATHLIBNAME, mathlib);
			LuaAPI.lua_pushnumber(L, PI);
			LuaAPI.lua_setfield(L, -2, "pi");
			LuaAPI.lua_pushnumber(L, LuaConf.HUGE_VAL);
			LuaAPI.lua_setfield(L, -2, "huge");
			#if LUA_COMPAT_MOD
			LuaAPI.lua_getfield(L, -1, "fmod");
			LuaAPI.lua_setfield(L, -2, "mod");
			#endif
			return 1;
		}
	}
}
