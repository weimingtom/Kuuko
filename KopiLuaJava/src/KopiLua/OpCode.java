package KopiLua;

//
// ** $Id: lopcodes.c,v 1.37.1.1 2007/12/27 13:02:25 roberto Exp $
// ** See Copyright Notice in lua.h
// 
public enum OpCode {
//        ----------------------------------------------------------------------
//		name		args	description
//		------------------------------------------------------------------------
	OP_MOVE(0), // A B R(A) := R(B) 
	OP_LOADK(1), // A Bx R(A) := Kst(Bx) 
	OP_LOADBOOL(2), // A B C R(A) := (Bool)B; if (C) pc++ 
	OP_LOADNIL(3), // A B R(A) :=... := R(B) := nil 
	OP_GETUPVAL(4), // A B R(A) := UpValue[B] 

	OP_GETGLOBAL(5), // A Bx R(A) := Gbl[Kst(Bx)] 
	OP_GETTABLE(6), // A B C R(A) := R(B)[RK(C)] 

	OP_SETGLOBAL(7), // A Bx Gbl[Kst(Bx)] := R(A) 
	OP_SETUPVAL(8), // A B UpValue[B] := R(A) 
	OP_SETTABLE(9), // A B C R(A)[RK(B)] := RK(C) 

	OP_NEWTABLE(10), // A B C R(A) := {} (size = B,C) 

	OP_SELF(11), // A B C R(A+1) := R(B); R(A) := R(B)[RK(C)] 

	OP_ADD(12), // A B C R(A) := RK(B) + RK(C) 
	OP_SUB(13), // A B C R(A) := RK(B) - RK(C) 
	OP_MUL(14), // A B C R(A) := RK(B) * RK(C) 
	OP_DIV(15), // A B C R(A) := RK(B) / RK(C) 
	OP_MOD(16), // A B C R(A) := RK(B) % RK(C) 
	OP_POW(17), // A B C R(A) := RK(B) ^ RK(C) 
	OP_UNM(18), // A B R(A) := -R(B) 
	OP_NOT(19), // A B R(A) := not R(B) 
	OP_LEN(20), // A B R(A) := length of R(B) 

	OP_CONCAT(21), // A B C R(A) := R(B).......R(C) 

	OP_JMP(22), // sBx pc+=sBx 

	OP_EQ(23), // A B C if ((RK(B) == RK(C)) ~= A) then pc++ 
	OP_LT(24), // A B C if ((RK(B) < RK(C)) ~= A) then pc++ 
	OP_LE(25), // A B C if ((RK(B) <= RK(C)) ~= A) then pc++ 

	OP_TEST(26), // A C if not (R(A) <=> C) then pc++ 
	OP_TESTSET(27), // A B C if (R(B) <=> C) then R(A) := R(B) else pc++ 

	OP_CALL(28), // A B C R(A),... ,R(A+C-2) := R(A)(R(A+1),... ,R(A+B-1)) 
	OP_TAILCALL(29), // A B C return R(A)(R(A+1),... ,R(A+B-1)) 
	OP_RETURN(30), // A B return R(A),... ,R(A+B-2) (see note) 

	OP_FORLOOP(31), // A sBx R(A)+=R(A+2);
//					if R(A) <?= R(A+1) then { pc+=sBx; R(A+3)=R(A) }
	OP_FORPREP(32), // A sBx R(A)-=R(A+2); pc+=sBx 

	OP_TFORLOOP(33), // A C R(A+3),... ,R(A+2+C) := R(A)(R(A+1), R(A+2));
//								if R(A+3) ~= nil then R(A+2)=R(A+3) else pc++	
	OP_SETLIST(34), // A B C R(A)[(C-1)*FPF+i] := R(A+i), 1 <= i <= B 

	OP_CLOSE(35), // A close all variables in the stack up to (>=) R(A)
	OP_CLOSURE(36), // A Bx R(A) := closure(KPROTO[Bx], R(A),... ,R(A+n)) 

	OP_VARARG(37); // A B R(A), R(A+1),..., R(A+B-1) = vararg 

	private int intValue;
	private static java.util.HashMap<Integer, OpCode> mappings;
	private synchronized static java.util.HashMap<Integer, OpCode> getMappings() {
		if (mappings == null) {
			mappings = new java.util.HashMap<Integer, OpCode>();
		}
		return mappings;
	}

	private OpCode(int value) {
		intValue = value;
		OpCode.getMappings().put(value, this);
	}

	public int getValue() {
		return intValue;
	}

	public static OpCode forValue(int value) {
		return getMappings().get(value);
	}
}