/*
 ** $Id: lopcodes.c,v 1.37.1.1 2007/12/27 13:02:25 roberto Exp $
 ** See Copyright Notice in lua.h
 */
using System;

namespace KopiLua
{
	//using lu_byte = System.Byte;
	//using Instruction = System.UInt32;

	public class LuaOpCodes
	{
		/*===========================================================================
		  We assume that instructions are unsigned numbers.
		  All instructions have an opcode in the first 6 bits.
		  Instructions can have the following fields:
			`A' : 8 bits
			`B' : 9 bits
			`C' : 9 bits
			`Bx' : 18 bits (`B' and `C' together)
			`sBx' : signed Bx

		  A signed argument is represented in excess K; that is, the number
		  value is the unsigned value minus K. K is exactly the maximum value
		  for that argument (so that -max is represented by 0, and +max is
		  represented by 2*max), which is half the maximum for the corresponding
		  unsigned argument.
		===========================================================================*/



		/*
		 ** size and position of opcode arguments.
		 */
		public const int SIZE_C	= 9;
		public const int SIZE_B	= 9;
		public const int SIZE_Bx = (SIZE_C + SIZE_B);
		public const int SIZE_A = 8;

		public const int SIZE_OP = 6;

		public const int POS_OP	= 0;
		public const int POS_A = (POS_OP + SIZE_OP);
		public const int POS_C = (POS_A + SIZE_A);
		public const int POS_B = (POS_C + SIZE_C);
		public const int POS_Bx	= POS_C;

		/*
		 ** limits for opcode arguments.
		 ** we use (signed) int to manipulate most arguments,
		 ** so they must fit in LUAI_BITSINT-1 bits (-1 for sign)
		 */
		//#if SIZE_Bx < LUAI_BITSINT-1
		public const int MAXARG_Bx = ((1<<SIZE_Bx)-1);
		public const int MAXARG_sBx = (MAXARG_Bx>>1);         /* `sBx' is signed */
		//#else
		//public const int MAXARG_Bx			= System.Int32.MaxValue;
		//public const int MAXARG_sBx			= System.Int32.MaxValue;
		//#endif

		//public const uint MAXARG_A = (uint)((1 << (int)SIZE_A) -1);
		//public const uint MAXARG_B = (uint)((1 << (int)SIZE_B) -1);
		//public const uint MAXARG_C = (uint)((1 << (int)SIZE_C) -1);
		public const long MAXARG_A = (long)(((1 << (int)SIZE_A) -1) & 0xffffffff);
		public const long MAXARG_B = (long)(((1 << (int)SIZE_B) -1) & 0xffffffff);
		public const long MAXARG_C = (long)(((1 << (int)SIZE_C) -1) & 0xffffffff);

		/* creates a mask with `n' 1 bits at position `p' */
		//public static int MASK1(int n, int p) { return ((~((~(Instruction)0) << n)) << p); }
		public static long/*uint*/ MASK1(int n, int p) 
		{ 
			//return (uint)((~((~0) << n)) << p); 
			return (long)(((~((~0) << n)) << p) & 0xffffffff);
		}

		/* creates a mask with `n' 0 bits at position `p' */
		public static long/*uint*/ MASK0(int n, int p) 
		{ 
			//return (uint)(~MASK1(n, p)); 
			return (long)((~MASK1(n, p)) & 0xffffffff);
		}

		/*
		 ** the following macros help to manipulate instructions
		 */
		public static OpCode GET_OPCODE(long/*UInt32*//*Instruction*/ i)
		{
            return (OpCode)OpCodeUtil.longToOpCode((i >> POS_OP) & MASK1(SIZE_OP, 0));
		}
		
		public static OpCode GET_OPCODE(InstructionPtr i) 
		{ 
			return GET_OPCODE(i.get(0)); 
		}

		public static void SET_OPCODE(/*ref*/ long[]/*UInt32*//*Instruction*/ i, long/*UInt32*//*Instruction*/ o)
		{
		    i[0] = (long/*UInt32*//*Instruction*/)(i[0] & MASK0(SIZE_OP, POS_OP)) | ((o << POS_OP) & MASK1(SIZE_OP, POS_OP));
		}
		
		public static void SET_OPCODE(/*ref*/ long[]/*UInt32*//*Instruction*/ i, OpCode opcode)
		{
		    i[0] = (long/*UInt32*//*Instruction*/)(i[0] & MASK0(SIZE_OP, POS_OP)) | (((long/*uint*/)opcode << POS_OP) & MASK1(SIZE_OP, POS_OP));
		}
		
		public static void SET_OPCODE(InstructionPtr i, OpCode opcode) 
		{ 
			long[] c_ref = new long[1];
			c_ref[0] = i.codes[i.pc];
			SET_OPCODE(/*ref*/ c_ref, opcode);
			i.codes[i.pc] = c_ref[0];
		}

		public static int GETARG_A(long/*UInt32*//*Instruction*/ i)
		{
			return (int)((i >> POS_A) & MASK1(SIZE_A, 0));
		}
		
		public static int GETARG_A(InstructionPtr i) 
		{ 
			return GETARG_A(i.get(0)); 
		}

		public static void SETARG_A(InstructionPtr i, int u)
		{
			i.set(0, (long/*UInt32*//*Instruction*/)((i.get(0) & MASK0(SIZE_A, POS_A)) | ((u << POS_A) & MASK1(SIZE_A, POS_A))));
		}

		public static int GETARG_B(long/*UInt32*//*Instruction*/ i)
		{
			return (int)((i >> POS_B) & MASK1(SIZE_B, 0));
		}
		
		public static int GETARG_B(InstructionPtr i)
		{ 
			return GETARG_B(i.get(0)); 
		}

		public static void SETARG_B(InstructionPtr i, int b)
		{
			i.set(0, (long/*UInt32*//*Instruction*/)((i.get(0) & MASK0(SIZE_B, POS_B)) | ((b << POS_B) & MASK1(SIZE_B, POS_B))));
		}

		public static int GETARG_C(long/*UInt32*//*Instruction*/ i)
		{
			return (int)((i>>POS_C) & MASK1(SIZE_C,0));
		}
		
		public static int GETARG_C(InstructionPtr i)
		{ 
			return GETARG_C(i.get(0)); 
		}

		public static void SETARG_C(InstructionPtr i, int b)
		{
			i.set(0, (long/*UInt32*//*Instruction*/)((i.get(0) & MASK0(SIZE_C, POS_C)) | ((b << POS_C) & MASK1(SIZE_C, POS_C))));
		}

		public static int GETARG_Bx(long/*UInt32*//*Instruction*/ i)
		{
			return (int)((i>>POS_Bx) & MASK1(SIZE_Bx,0));
		}
		
		public static int GETARG_Bx(InstructionPtr i)
		{ 
			return GETARG_Bx(i.get(0)); 
		}

		public static void SETARG_Bx(InstructionPtr i, int b)
		{
			i.set(0, (long/*UInt32*//*Instruction*/)((i.get(0) & MASK0(SIZE_Bx, POS_Bx)) | ((b << POS_Bx) & MASK1(SIZE_Bx, POS_Bx))));
		}

		public static int GETARG_sBx(long/*UInt32*//*Instruction*/ i)
		{
			return (GETARG_Bx(i) - MAXARG_sBx);
		}
		
		public static int GETARG_sBx(InstructionPtr i)
		{ 
			return GETARG_sBx(i.get(0)); 
		}

		public static void SETARG_sBx(InstructionPtr i, int b)
		{
			SETARG_Bx(i, b + MAXARG_sBx);
		}

		//FIXME:long
		public static int CREATE_ABC(OpCode o, int a, int b, int c)
		{
			return (int)(((int)o << POS_OP) | (a << POS_A) | (b << POS_B) | (c << POS_C));
		}

		//FIXME:long
		public static int CREATE_ABx(OpCode o, int a, int bc)
		{
			int result = (int)(((int)o << POS_OP) | (a << POS_A) | (bc << POS_Bx));
			return (int)(((int)o << POS_OP) | (a << POS_A) | (bc << POS_Bx));
		}

		/*
		 ** Macros to operate RK indices
		 */
		
		/* this bit 1 means constant (0 means register) */
		public readonly static int BITRK = (1 << (SIZE_B - 1));

		/* test whether value is a constant */
		public static int ISK(int x)		
		{
			return x & BITRK;
		}

		/* gets the index of the constant */
		public static int INDEXK(int r)	
		{
			return r & (~BITRK);
		}

		public static readonly int MAXINDEXRK = BITRK - 1;

		/* code a constant index as a RK value */
		public static int RKASK(int x)	
		{
			return x | BITRK;
		}

		/*
		 ** invalid register that fits in 8 bits
		 */
		public static readonly int NO_REG = (int)MAXARG_A;

		/*
		 ** R(x) - register
		 ** Kst(x) - constant (in constant table)
		 ** RK(x) == if ISK(x) then Kst(INDEXK(x)) else R(x)
		 */


		/*
		 ** grep "ORDER OP" if you change these enums
		 */
		public static OpMode getOpMode(OpCode m)	
		{
            switch (luaP_opmodes[(int)m] & 3)
            {
                default:
                case 0:
                    return OpMode.iABC;
                case 1:
                    return OpMode.iABx;
                case 2:
                    return OpMode.iAsBx;
            }
		}
		
		public static OpArgMask getBMode(OpCode m) 
		{
            switch ((luaP_opmodes[(int)m] >> 4) & 3)
            {
                default:
                case 0:
                    return OpArgMask.OpArgN;
                case 1:
                    return OpArgMask.OpArgU;
                case 2:
                    return OpArgMask.OpArgR;
                case 3:
                    return OpArgMask.OpArgK;
            }
		}
		
		public static OpArgMask getCMode(OpCode m)
		{
            switch ((luaP_opmodes[(int)m] >> 2) & 3)
            {
                default:
                case 0:
                    return OpArgMask.OpArgN;
                case 1:
                    return OpArgMask.OpArgU;
                case 2:
                    return OpArgMask.OpArgR;
                case 3:
                    return OpArgMask.OpArgK;
            }
		}
		
		public static int testAMode(OpCode m) 
		{ 
			return luaP_opmodes[(int)m] & (1 << 6); 
		}
		
		public static int testTMode(OpCode m)
		{ 
			return luaP_opmodes[(int)m] & (1 << 7); 
		}

		/* number of list items to accumulate before a SETLIST instruction */
		public const int LFIELDS_PER_FLUSH = 50;

		/* ORDER OP */
		public/*private*/ readonly static CharPtr[] luaP_opnames = {
			CharPtr.toCharPtr("MOVE"),
			CharPtr.toCharPtr("LOADK"),
			CharPtr.toCharPtr("LOADBOOL"),
			CharPtr.toCharPtr("LOADNIL"),
			CharPtr.toCharPtr("GETUPVAL"),
			CharPtr.toCharPtr("GETGLOBAL"),
			CharPtr.toCharPtr("GETTABLE"),
			CharPtr.toCharPtr("SETGLOBAL"),
			CharPtr.toCharPtr("SETUPVAL"),
			CharPtr.toCharPtr("SETTABLE"),
			CharPtr.toCharPtr("NEWTABLE"),
			CharPtr.toCharPtr("SELF"),
			CharPtr.toCharPtr("ADD"),
			CharPtr.toCharPtr("SUB"),
			CharPtr.toCharPtr("MUL"),
			CharPtr.toCharPtr("DIV"),
			CharPtr.toCharPtr("MOD"),
			CharPtr.toCharPtr("POW"),
			CharPtr.toCharPtr("UNM"),
			CharPtr.toCharPtr("NOT"),
			CharPtr.toCharPtr("LEN"),
			CharPtr.toCharPtr("CONCAT"),
			CharPtr.toCharPtr("JMP"),
			CharPtr.toCharPtr("EQ"),
			CharPtr.toCharPtr("LT"),
			CharPtr.toCharPtr("LE"),
			CharPtr.toCharPtr("TEST"),
			CharPtr.toCharPtr("TESTSET"),
			CharPtr.toCharPtr("CALL"),
			CharPtr.toCharPtr("TAILCALL"),
			CharPtr.toCharPtr("RETURN"),
			CharPtr.toCharPtr("FORLOOP"),
			CharPtr.toCharPtr("FORPREP"),
			CharPtr.toCharPtr("TFORLOOP"),
			CharPtr.toCharPtr("SETLIST"),
			CharPtr.toCharPtr("CLOSE"),
			CharPtr.toCharPtr("CLOSURE"),
			CharPtr.toCharPtr("VARARG"),
		};


		private static Byte/*lu_byte*/ opmode(Byte/*lu_byte*/ t, Byte/*lu_byte*/ a, OpArgMask b, OpArgMask c, OpMode m)
		{
            int bValue = 0;
            int cValue = 0;
            int mValue = 0;
            switch (b)
            {
                case OpArgMask.OpArgN:
                    bValue = 0;
                    break;
                case OpArgMask.OpArgU:
                    bValue = 1;
                    break;
                case OpArgMask.OpArgR:
                    bValue = 2;
                    break;
                case OpArgMask.OpArgK:
                    bValue = 3;
                    break;
            }
            switch (c)
            {
                case OpArgMask.OpArgN:
                    cValue = 0;
                    break;
                case OpArgMask.OpArgU:
                    cValue = 1;
                    break;
                case OpArgMask.OpArgR:
                    cValue = 2;
                    break;
                case OpArgMask.OpArgK:
                    cValue = 3;
                    break;
            }
            switch (m)
            {
                case OpMode.iABC:
                    mValue = 0;
                    break;
                case OpMode.iABx:
                    mValue = 1;
                    break;
                case OpMode.iAsBx:
                    mValue = 2;
                    break;
            }
            return (Byte/*lu_byte*/)(((t) << 7) | ((a) << 6) | (((Byte/*lu_byte*/)bValue) << 4) | (((Byte/*lu_byte*/)cValue) << 2) | ((Byte/*lu_byte*/)mValue));
		}
		
		//       T  A    B       C     mode		   opcode		
		//lu_byte[]
		private readonly static Byte[] luaP_opmodes = {
			opmode((byte)0, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iABC),
			opmode((byte)0, (byte)1, OpArgMask.OpArgK, OpArgMask.OpArgN, OpMode.iABx),
			opmode((byte)0, (byte)1, OpArgMask.OpArgU, OpArgMask.OpArgU, OpMode.iABC),
			opmode((byte)0, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iABC),
			opmode((byte)0, (byte)1, OpArgMask.OpArgU, OpArgMask.OpArgN, OpMode.iABC),
			opmode((byte)0, (byte)1, OpArgMask.OpArgK, OpArgMask.OpArgN, OpMode.iABx),
			opmode((byte)0, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgK, OpMode.iABC),
			opmode((byte)0, (byte)0, OpArgMask.OpArgK, OpArgMask.OpArgN, OpMode.iABx),
			opmode((byte)0, (byte)0, OpArgMask.OpArgU, OpArgMask.OpArgN, OpMode.iABC),
			opmode((byte)0, (byte)0, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC),
			opmode((byte)0, (byte)1, OpArgMask.OpArgU, OpArgMask.OpArgU, OpMode.iABC),
			opmode((byte)0, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgK, OpMode.iABC),
			opmode((byte)0, (byte)1, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC),
			opmode((byte)0, (byte)1, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC),
			opmode((byte)0, (byte)1, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC),
			opmode((byte)0, (byte)1, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC),
			opmode((byte)0, (byte)1, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC),
			opmode((byte)0, (byte)1, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC),
			opmode((byte)0, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iABC),
			opmode((byte)0, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iABC),
			opmode((byte)0, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iABC),
			opmode((byte)0, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgR, OpMode.iABC),
			opmode((byte)0, (byte)0, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iAsBx),
			opmode((byte)1, (byte)0, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC),
			opmode((byte)1, (byte)0, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC),
			opmode((byte)1, (byte)0, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC),
			opmode((byte)1, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgU, OpMode.iABC),
			opmode((byte)1, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgU, OpMode.iABC),
			opmode((byte)0, (byte)1, OpArgMask.OpArgU, OpArgMask.OpArgU, OpMode.iABC),
			opmode((byte)0, (byte)1, OpArgMask.OpArgU, OpArgMask.OpArgU, OpMode.iABC),
			opmode((byte)0, (byte)0, OpArgMask.OpArgU, OpArgMask.OpArgN, OpMode.iABC),
			opmode((byte)0, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iAsBx),
			opmode((byte)0, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iAsBx),
			opmode((byte)1, (byte)0, OpArgMask.OpArgN, OpArgMask.OpArgU, OpMode.iABC),
			opmode((byte)0, (byte)0, OpArgMask.OpArgU, OpArgMask.OpArgU, OpMode.iABC),
			opmode((byte)0, (byte)0, OpArgMask.OpArgN, OpArgMask.OpArgN, OpMode.iABC),
			opmode((byte)0, (byte)1, OpArgMask.OpArgU, OpArgMask.OpArgN, OpMode.iABx),
			opmode((byte)0, (byte)1, OpArgMask.OpArgU, OpArgMask.OpArgN, OpMode.iABC)
		};

		public const int NUM_OPCODES = (int)OpCode.OP_VARARG;
	}
}
