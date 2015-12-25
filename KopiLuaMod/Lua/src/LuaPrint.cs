/*
 ** $Id: print.c,v 1.55a 2006/05/31 13:30:05 lhf Exp $
 ** print bytecodes
 ** See Copyright Notice in lua.h
 */
using System;

namespace KopiLua
{
	//using TValue = Lua.TValue;
	//using Instruction = System.UInt32;

	public class LuaPrint
	{
		public static void luaU_print(Proto f, int full) 
		{
			PrintFunction(f, full);
		}

		//#define Sizeof(x)	((int)sizeof(x))
		//#define VOID(p)		((const void*)(p))

		public static void PrintString(TString ts)
		{
			CharPtr s = LuaObject.getstr(ts);
			int/*uint*/ i, n = ts.tsv.len;
			LuaConf.putchar('"');
			for (i = 0; i < n; i++)
			{
				int c = s[i];
				switch (c)
				{
					case '"': 
						{
							LuaConf.printf("\\\""); 
							break;
						}
					case '\\': 
						{
							LuaConf.printf("\\\\"); 
							break;
						}
					case '\a': 
						{
							LuaConf.printf("\\a"); 
							break;
						}
					case '\b': 
						{
							LuaConf.printf("\\b"); 
							break;
						}
					case '\f': 
						{
							LuaConf.printf("\\f"); 
							break;
						}
					case '\n': 
						{
							LuaConf.printf("\\n"); 
							break;
						}
					case '\r': 
						{
							LuaConf.printf("\\r"); 
							break;
						}
					case '\t': 
						{
							LuaConf.printf("\\t"); 
							break;
						}
					case '\v': 
						{
							LuaConf.printf("\\v"); 
							break;
						}
					default: 
						{
							if (LuaConf.isprint((byte)c))
							{
								LuaConf.putchar(c);
							}
							else
							{
								LuaConf.printf("\\%03u", (byte)c);
							}
							break;
						}
				}
			}
			LuaConf.putchar('"');
		}

		private static void PrintConstant(Proto f, int i)
		{
			/*const*/ TValue o = f.k[i];
			switch (LuaObject.ttype(o))
			{
			case Lua.LUA_TNIL:
				{
					LuaConf.printf("nil");
					break;
				}
			case Lua.LUA_TBOOLEAN:
				{
					LuaConf.printf(LuaObject.bvalue(o) != 0 ? "true" : "false");
					break;
				}
			case Lua.LUA_TNUMBER:
				{
					LuaConf.printf(LuaConf.LUA_NUMBER_FMT, LuaObject.nvalue(o));
					break;
				}
			case Lua.LUA_TSTRING:
				{
					PrintString(LuaObject.rawtsvalue(o));
					break;
				}
			default:				
				{
					/* cannot happen */
					LuaConf.printf("? type=%d", LuaObject.ttype(o));
					break;
				}
			}
		}

		private static void PrintCode(Proto f)
		{
			UInt32[]/*Instruction[]*/ code = f.code;
			int pc, n = f.sizecode;
			for (pc = 0; pc < n; pc++)
			{
				UInt32/*Instruction*/ i = f.code[pc];
				OpCode o = LuaOpCodes.GET_OPCODE(i);
				int a = LuaOpCodes.GETARG_A(i);
				int b = LuaOpCodes.GETARG_B(i);
				int c = LuaOpCodes.GETARG_C(i);
				int bx = LuaOpCodes.GETARG_Bx(i);
				int sbx = LuaOpCodes.GETARG_sBx(i);
				int line = LuaDebug.getline(f,pc);
				LuaConf.printf("\t%d\t", pc + 1);
				if (line > 0) 
				{
					LuaConf.printf("[%d]\t", line);
				}
				else 
				{
					LuaConf.printf("[-]\t");
				}
				LuaConf.printf("%-9s\t", LuaOpCodes.luaP_opnames[(int)o]);
				switch (LuaOpCodes.getOpMode(o))
				{
					case OpMode.iABC:
						{
							LuaConf.printf("%d", a);
							if (LuaOpCodes.getBMode(o) != OpArgMask.OpArgN) 
							{
								LuaConf.printf(" %d", (LuaOpCodes.ISK(b) != 0) ? (-1 - LuaOpCodes.INDEXK(b)) : b);
							}
							if (LuaOpCodes.getCMode(o) != OpArgMask.OpArgN) 
							{
								LuaConf.printf(" %d", (LuaOpCodes.ISK(c) != 0) ? (-1 - LuaOpCodes.INDEXK(c)) : c);
							}
							break;
						}
					case OpMode.iABx:
						{
							if (LuaOpCodes.getBMode(o) == OpArgMask.OpArgK) 
							{
								LuaConf.printf("%d %d", a, -1 - bx); 
							}
							else 
							{
								LuaConf.printf("%d %d", a, bx);
							}
							break;
						}
					case OpMode.iAsBx:
						if (o == OpCode.OP_JMP) 
						{
							LuaConf.printf("%d", sbx);
						}
						else 
						{
							LuaConf.printf("%d %d", a, sbx);
						}
						break;
				}
				switch (o)
				{
					case OpCode.OP_LOADK:
						{
							LuaConf.printf("\t; ");
							PrintConstant(f,bx);
							break;
						}
					case OpCode.OP_GETUPVAL:
					case OpCode.OP_SETUPVAL:
						{
							LuaConf.printf("\t; %s", (f.sizeupvalues > 0) ? LuaObject.getstr(f.upvalues[b]) : "-");
							break;
						}
					case OpCode.OP_GETGLOBAL:
					case OpCode.OP_SETGLOBAL:
						{
							LuaConf.printf("\t; %s", LuaObject.svalue(f.k[bx]));
							break;
						}
					case OpCode.OP_GETTABLE:
					case OpCode.OP_SELF:
						{
							if (LuaOpCodes.ISK(c) != 0)
							{ 
								LuaConf.printf("\t; "); 
								PrintConstant(f, LuaOpCodes.INDEXK(c)); 
							}
							break;
						}
					case OpCode.OP_SETTABLE:
					case OpCode.OP_ADD:
					case OpCode.OP_SUB:
					case OpCode.OP_MUL:
					case OpCode.OP_DIV:
					case OpCode.OP_POW:
					case OpCode.OP_EQ:
					case OpCode.OP_LT:
					case OpCode.OP_LE:
						{
							if (LuaOpCodes.ISK(b) != 0 || LuaOpCodes.ISK(c) != 0)
							{
								LuaConf.printf("\t; ");
								if (LuaOpCodes.ISK(b) != 0) 
								{
									PrintConstant(f, LuaOpCodes.INDEXK(b));
								}
								else 
								{
									LuaConf.printf("-");
								}
								LuaConf.printf(" ");
								if (LuaOpCodes.ISK(c) != 0) 
								{
									PrintConstant(f, LuaOpCodes.INDEXK(c));
								}
								else 
								{
									LuaConf.printf("-");
								}
							}
							break;
						}
					case OpCode.OP_JMP:
					case OpCode.OP_FORLOOP:
					case OpCode.OP_FORPREP:
						{
							LuaConf.printf("\t; to %d", sbx + pc + 2);
							break;
						}
					case OpCode.OP_CLOSURE:
						{
							LuaConf.printf("\t; %p", LuaConf.VOID(f.p[bx]));
							break;
						}
					case OpCode.OP_SETLIST:
						{
							if (c == 0) 
							{
								LuaConf.printf("\t; %d", (int)code[++pc]);
							}
							else 
							{
								LuaConf.printf("\t; %d", c);
							}
							break;
						}
					default:
						{
							break;
						}
				}
				LuaConf.printf("\n");
			}
		}

		public static string SS(int x) 
		{ 
			return (x == 1) ? "" : "s"; 
		}
		
		//#define S(x)	x,SS(x)

		private static void PrintHeader(Proto f)
		{
			CharPtr s = LuaObject.getstr(f.source);
			if (s[0] == '@' || s[0] == '=')
			{
				s = s.next();
			}
			else if (s[0] == Lua.LUA_SIGNATURE[0])
			{
				s = "(bstring)";
			}
			else
			{
				s = "(string)";
			}
			LuaConf.printf("\n%s <%s:%d,%d> (%d Instruction%s, %d bytes at %p)\n",
               	(f.linedefined == 0) ? "main" : "function", s,
               	f.linedefined, f.lastlinedefined,
               	f.sizecode, SS(f.sizecode), f.sizecode * LuaConf.GetUnmanagedSize(typeof(UInt32/*Instruction*/)), LuaConf.VOID(f));
			LuaConf.printf("%d%s param%s, %d slot%s, %d upvalue%s, ",
               	f.numparams, (f.is_vararg != 0) ? "+" : "", SS(f.numparams),
            	f.maxstacksize, SS(f.maxstacksize), f.nups, SS(f.nups));
			LuaConf.printf("%d local%s, %d constant%s, %d function%s\n",
				f.sizelocvars, SS(f.sizelocvars), f.sizek, SS(f.sizek), f.sizep, SS(f.sizep));
		}

		private static void PrintConstants(Proto f)
		{
			int i, n = f.sizek;
			LuaConf.printf("constants (%d) for %p:\n", n, LuaConf.VOID(f));
			for (i = 0; i < n; i++)
			{
				LuaConf.printf("\t%d\t", i + 1);
				PrintConstant(f, i);
				LuaConf.printf("\n");
			}
		}

		private static void PrintLocals(Proto f)
		{
			int i, n = f.sizelocvars;
			LuaConf.printf("locals (%d) for %p:\n", n, LuaConf.VOID(f));
			for (i = 0; i < n; i++)
			{
				LuaConf.printf("\t%d\t%s\t%d\t%d\n",
					i, LuaObject.getstr(f.locvars[i].varname), f.locvars[i].startpc + 1, f.locvars[i].endpc + 1);
			}
		}

		private static void PrintUpvalues(Proto f)
		{
			int i, n = f.sizeupvalues;
			LuaConf.printf("upvalues (%d) for %p:\n", n, LuaConf.VOID(f));
			if (f.upvalues == null) 
			{
				return;
			}
			for (i = 0; i < n; i++)
			{
				LuaConf.printf("\t%d\t%s\n", i, LuaObject.getstr(f.upvalues[i]));
			}
		}

		public static void PrintFunction(Proto f, int full)
		{
			int i, n = f.sizep;
			PrintHeader(f);
			PrintCode(f);
			if (full != 0)
			{
				PrintConstants(f);
				PrintLocals(f);
				PrintUpvalues(f);
			}
			for (i = 0; i < n; i++) 
			{
				PrintFunction(f.p[i], full);
			}
		}
	}
}
