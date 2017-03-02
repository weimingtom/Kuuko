package KopiLua;

//
// ** $Id: lopcodes.c,v 1.37.1.1 2007/12/27 13:02:25 roberto Exp $
// ** See Copyright Notice in lua.h
// 

//using lu_byte = System.Byte;
//using Instruction = System.UInt32;

public class LuaOpCodes {
//        ===========================================================================
//		  We assume that instructions are unsigned numbers.
//		  All instructions have an opcode in the first 6 bits.
//		  Instructions can have the following fields:
//			`A' : 8 bits
//			`B' : 9 bits
//			`C' : 9 bits
//			`Bx' : 18 bits (`B' and `C' together)
//			`sBx' : signed Bx
//
//		  A signed argument is represented in excess K; that is, the number
//		  value is the unsigned value minus K. K is exactly the maximum value
//		  for that argument (so that -max is represented by 0, and +max is
//		  represented by 2*max), which is half the maximum for the corresponding
//		  unsigned argument.
//		===========================================================================



//        
//		 ** size and position of opcode arguments.
//		 
	public static final int SIZE_C = 9;
	public static final int SIZE_B = 9;
	public static final int SIZE_Bx = (SIZE_C + SIZE_B);
	public static final int SIZE_A = 8;

	public static final int SIZE_OP = 6;

	public static final int POS_OP = 0;
	public static final int POS_A = (POS_OP + SIZE_OP);
	public static final int POS_C = (POS_A + SIZE_A);
	public static final int POS_B = (POS_C + SIZE_C);
	public static final int POS_Bx = POS_C;

//        
//		 ** limits for opcode arguments.
//		 ** we use (signed) int to manipulate most arguments,
//		 ** so they must fit in LUAI_BITSINT-1 bits (-1 for sign)
//		 
	///#if SIZE_Bx < LUAI_BITSINT-1
	public static final int MAXARG_Bx = ((1<<SIZE_Bx)-1);
	public static final int MAXARG_sBx = (MAXARG_Bx>>1); // `sBx' is signed 
	///#else
	//public const int MAXARG_Bx			= System.Int32.MaxValue;
	//public const int MAXARG_sBx			= System.Int32.MaxValue;
	///#endif

	//public const uint MAXARG_A = (uint)((1 << (int)SIZE_A) -1);
	//public const uint MAXARG_B = (uint)((1 << (int)SIZE_B) -1);
	//public const uint MAXARG_C = (uint)((1 << (int)SIZE_C) -1);
	public static final long MAXARG_A = (long)(((1 << (int)SIZE_A) -1) & 0xffffffff);
	public static final long MAXARG_B = (long)(((1 << (int)SIZE_B) -1) & 0xffffffff);
	public static final long MAXARG_C = (long)(((1 << (int)SIZE_C) -1) & 0xffffffff);

	// creates a mask with `n' 1 bits at position `p' 
	//public static int MASK1(int n, int p) { return ((~((~(Instruction)0) << n)) << p); }
	public static long MASK1(int n, int p) { //uint
		//return (uint)((~((~0) << n)) << p); 
		return (long)(((~((~0) << n)) << p) & 0xffffffff);
	}

	// creates a mask with `n' 0 bits at position `p' 
	public static long MASK0(int n, int p) { //uint
		//return (uint)(~MASK1(n, p)); 
		return (long)((~MASK1(n, p)) & 0xffffffff);
	}

//        
//		 ** the following macros help to manipulate instructions
//		 
	public static OpCode GET_OPCODE(long i) { //Instruction - UInt32
		return (OpCode)OpCodeUtil.longToOpCode((i >> POS_OP) & MASK1(SIZE_OP, 0));
	}

	public static OpCode GET_OPCODE(InstructionPtr i) {
		return GET_OPCODE(i.get(0));
	}

	public static void SET_OPCODE(long[] i, long o) { //Instruction - UInt32 - Instruction - UInt32 - ref
		i[0] = (long)(i[0] & MASK0(SIZE_OP, POS_OP)) | ((o << POS_OP) & MASK1(SIZE_OP, POS_OP)); //Instruction - UInt32
	}

	public static void SET_OPCODE(long[] i, OpCode opcode) { //Instruction - UInt32 - ref
		i[0] = (long)(i[0] & MASK0(SIZE_OP, POS_OP)) | (((long)opcode.getValue() << POS_OP) & MASK1(SIZE_OP, POS_OP)); //uint - Instruction - UInt32
	}

	public static void SET_OPCODE(InstructionPtr i, OpCode opcode) {
		long[] c_ref = new long[1];
		c_ref[0] = i.codes[i.pc];
		SET_OPCODE(c_ref, opcode); //ref
		i.codes[i.pc] = c_ref[0];
	}

	public static int GETARG_A(long i) { //Instruction - UInt32
		return (int)((i >> POS_A) & MASK1(SIZE_A, 0));
	}

	public static int GETARG_A(InstructionPtr i) {
		return GETARG_A(i.get(0));
	}

	public static void SETARG_A(InstructionPtr i, int u) {
		i.set(0, (long)((i.get(0) & MASK0(SIZE_A, POS_A)) | ((u << POS_A) & MASK1(SIZE_A, POS_A)))); //Instruction - UInt32
	}

	public static int GETARG_B(long i) { //Instruction - UInt32
		return (int)((i >> POS_B) & MASK1(SIZE_B, 0));
	}

	public static int GETARG_B(InstructionPtr i) {
		return GETARG_B(i.get(0));
	}

	public static void SETARG_B(InstructionPtr i, int b) {
		i.set(0, (long)((i.get(0) & MASK0(SIZE_B, POS_B)) | ((b << POS_B) & MASK1(SIZE_B, POS_B)))); //Instruction - UInt32
	}

	public static int GETARG_C(long i) { //Instruction - UInt32
		return (int)((i>>POS_C) & MASK1(SIZE_C, 0));
	}

	public static int GETARG_C(InstructionPtr i) {
		return GETARG_C(i.get(0));
	}

	public static void SETARG_C(InstructionPtr i, int b) {
		i.set(0, (long)((i.get(0) & MASK0(SIZE_C, POS_C)) | ((b << POS_C) & MASK1(SIZE_C, POS_C)))); //Instruction - UInt32
	}

	public static int GETARG_Bx(long i) { //Instruction - UInt32
		return (int)((i>>POS_Bx) & MASK1(SIZE_Bx, 0));
	}

	public static int GETARG_Bx(InstructionPtr i) {
		return GETARG_Bx(i.get(0));
	}

	public static void SETARG_Bx(InstructionPtr i, int b) {
		i.set(0, (long)((i.get(0) & MASK0(SIZE_Bx, POS_Bx)) | ((b << POS_Bx) & MASK1(SIZE_Bx, POS_Bx)))); //Instruction - UInt32
	}

	public static int GETARG_sBx(long i) { //Instruction - UInt32
		return (GETARG_Bx(i) - MAXARG_sBx);
	}

	public static int GETARG_sBx(InstructionPtr i) {
		return GETARG_sBx(i.get(0));
	}

	public static void SETARG_sBx(InstructionPtr i, int b) {
		SETARG_Bx(i, b + MAXARG_sBx);
	}

	//FIXME:long
	public static int CREATE_ABC(OpCode o, int a, int b, int c) {
		return (int)((o.getValue() << POS_OP) | (a << POS_A) | (b << POS_B) | (c << POS_C));
	}

	//FIXME:long
	public static int CREATE_ABx(OpCode o, int a, int bc) {
		int result = (int)((o.getValue() << POS_OP) | (a << POS_A) | (bc << POS_Bx));
		return (int)((o.getValue() << POS_OP) | (a << POS_A) | (bc << POS_Bx));
	}

//        
//		 ** Macros to operate RK indices
//		 

	// this bit 1 means constant (0 means register) 
	public final static int BITRK = (1 << (SIZE_B - 1));

	// test whether value is a constant 
	public static int ISK(int x) {
		return x & BITRK;
	}

	// gets the index of the constant 
	public static int INDEXK(int r) {
		return r & (~BITRK);
	}

	public static final int MAXINDEXRK = BITRK - 1;

	// code a constant index as a RK value 
	public static int RKASK(int x) {
		return x | BITRK;
	}

//        
//		 ** invalid register that fits in 8 bits
//		 
	public static final int NO_REG = (int)MAXARG_A;

//        
//		 ** R(x) - register
//		 ** Kst(x) - constant (in constant table)
//		 ** RK(x) == if ISK(x) then Kst(INDEXK(x)) else R(x)
//		 


//        
//		 ** grep "ORDER OP" if you change these enums
//		 
	public static OpMode getOpMode(OpCode m) {
		switch (luaP_opmodes[m.getValue()] & 3) {
			default:
			case 0:
				return OpMode.iABC;
			case 1:
				return OpMode.iABx;
			case 2:
				return OpMode.iAsBx;
		}
	}

	public static OpArgMask getBMode(OpCode m) {
		switch ((luaP_opmodes[m.getValue()] >> 4) & 3) {
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

	public static OpArgMask getCMode(OpCode m) {
		switch ((luaP_opmodes[m.getValue()] >> 2) & 3) {
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

	public static int testAMode(OpCode m) {
		return luaP_opmodes[m.getValue()] & (1 << 6);
	}

	public static int testTMode(OpCode m) {
		return luaP_opmodes[m.getValue()] & (1 << 7);
	}

	// number of list items to accumulate before a SETLIST instruction 
	public static final int LFIELDS_PER_FLUSH = 50;

	// ORDER OP 
	public final static CharPtr[] luaP_opnames = { CharPtr.toCharPtr("MOVE"), CharPtr.toCharPtr("LOADK"), CharPtr.toCharPtr("LOADBOOL"), CharPtr.toCharPtr("LOADNIL"), CharPtr.toCharPtr("GETUPVAL"), CharPtr.toCharPtr("GETGLOBAL"), CharPtr.toCharPtr("GETTABLE"), CharPtr.toCharPtr("SETGLOBAL"), CharPtr.toCharPtr("SETUPVAL"), CharPtr.toCharPtr("SETTABLE"), CharPtr.toCharPtr("NEWTABLE"), CharPtr.toCharPtr("SELF"), CharPtr.toCharPtr("ADD"), CharPtr.toCharPtr("SUB"), CharPtr.toCharPtr("MUL"), CharPtr.toCharPtr("DIV"), CharPtr.toCharPtr("MOD"), CharPtr.toCharPtr("POW"), CharPtr.toCharPtr("UNM"), CharPtr.toCharPtr("NOT"), CharPtr.toCharPtr("LEN"), CharPtr.toCharPtr("CONCAT"), CharPtr.toCharPtr("JMP"), CharPtr.toCharPtr("EQ"), CharPtr.toCharPtr("LT"), CharPtr.toCharPtr("LE"), CharPtr.toCharPtr("TEST"), CharPtr.toCharPtr("TESTSET"), CharPtr.toCharPtr("CALL"), CharPtr.toCharPtr("TAILCALL"), CharPtr.toCharPtr("RETURN"), CharPtr.toCharPtr("FORLOOP"), CharPtr.toCharPtr("FORPREP"), CharPtr.toCharPtr("TFORLOOP"), CharPtr.toCharPtr("SETLIST"), CharPtr.toCharPtr("CLOSE"), CharPtr.toCharPtr("CLOSURE"), CharPtr.toCharPtr("VARARG") };


	private static byte opmode(byte t, byte a, OpArgMask b, OpArgMask c, OpMode m) { //lu_byte - lu_byte - lu_byte
		int bValue = 0;
		int cValue = 0;
		int mValue = 0;
		switch (b) {
			case OpArgN:
				bValue = 0;
				break;
			case OpArgU:
				bValue = 1;
				break;
			case OpArgR:
				bValue = 2;
				break;
			case OpArgK:
				bValue = 3;
				break;
		}
		switch (c) {
			case OpArgN:
				cValue = 0;
				break;
			case OpArgU:
				cValue = 1;
				break;
			case OpArgR:
				cValue = 2;
				break;
			case OpArgK:
				cValue = 3;
				break;
		}
		switch (m) {
			case iABC:
				mValue = 0;
				break;
			case iABx:
				mValue = 1;
				break;
			case iAsBx:
				mValue = 2;
				break;
		}
		return (byte)(((t) << 7) | ((a) << 6) | (((byte)bValue) << 4) | (((byte)cValue) << 2) | ((byte)mValue)); //lu_byte - lu_byte - lu_byte - lu_byte
	}

	//       T  A    B       C     mode		   opcode		
	//lu_byte[]
	private final static byte[] luaP_opmodes = { opmode((byte)0, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iABC), opmode((byte)0, (byte)1, OpArgMask.OpArgK, OpArgMask.OpArgN, OpMode.iABx), opmode((byte)0, (byte)1, OpArgMask.OpArgU, OpArgMask.OpArgU, OpMode.iABC), opmode((byte)0, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iABC), opmode((byte)0, (byte)1, OpArgMask.OpArgU, OpArgMask.OpArgN, OpMode.iABC), opmode((byte)0, (byte)1, OpArgMask.OpArgK, OpArgMask.OpArgN, OpMode.iABx), opmode((byte)0, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgK, OpMode.iABC), opmode((byte)0, (byte)0, OpArgMask.OpArgK, OpArgMask.OpArgN, OpMode.iABx), opmode((byte)0, (byte)0, OpArgMask.OpArgU, OpArgMask.OpArgN, OpMode.iABC), opmode((byte)0, (byte)0, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC), opmode((byte)0, (byte)1, OpArgMask.OpArgU, OpArgMask.OpArgU, OpMode.iABC), opmode((byte)0, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgK, OpMode.iABC), opmode((byte)0, (byte)1, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC), opmode((byte)0, (byte)1, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC), opmode((byte)0, (byte)1, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC), opmode((byte)0, (byte)1, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC), opmode((byte)0, (byte)1, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC), opmode((byte)0, (byte)1, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC), opmode((byte)0, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iABC), opmode((byte)0, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iABC), opmode((byte)0, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iABC), opmode((byte)0, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgR, OpMode.iABC), opmode((byte)0, (byte)0, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iAsBx), opmode((byte)1, (byte)0, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC), opmode((byte)1, (byte)0, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC), opmode((byte)1, (byte)0, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC), opmode((byte)1, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgU, OpMode.iABC), opmode((byte)1, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgU, OpMode.iABC), opmode((byte)0, (byte)1, OpArgMask.OpArgU, OpArgMask.OpArgU, OpMode.iABC), opmode((byte)0, (byte)1, OpArgMask.OpArgU, OpArgMask.OpArgU, OpMode.iABC), opmode((byte)0, (byte)0, OpArgMask.OpArgU, OpArgMask.OpArgN, OpMode.iABC), opmode((byte)0, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iAsBx), opmode((byte)0, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iAsBx), opmode((byte)1, (byte)0, OpArgMask.OpArgN, OpArgMask.OpArgU, OpMode.iABC), opmode((byte)0, (byte)0, OpArgMask.OpArgU, OpArgMask.OpArgU, OpMode.iABC), opmode((byte)0, (byte)0, OpArgMask.OpArgN, OpArgMask.OpArgN, OpMode.iABC), opmode((byte)0, (byte)1, OpArgMask.OpArgU, OpArgMask.OpArgN, OpMode.iABx), opmode((byte)0, (byte)1, OpArgMask.OpArgU, OpArgMask.OpArgN, OpMode.iABC) };

	public static final int NUM_OPCODES = OpCode.OP_VARARG.getValue();
}