package KopiLua;

//
// ** $Id: lcode.c,v 2.25.1.3 2007/12/28 15:32:23 roberto Exp $
// ** Code generator for Lua
// ** See Copyright Notice in lua.h
// 
public class InstructionPtr {
	public long[] codes; //Instruction[] - UInt32[]
	public int pc;

	public InstructionPtr() {
		this.codes = null;
		this.pc = -1;
	}

	public InstructionPtr(long[] codes, int pc) { //Instruction[] - UInt32[]
		this.codes = codes;
		this.pc = pc;
	}

	public static InstructionPtr Assign(InstructionPtr ptr) {
		if (ptr == null) {
			return null;
		}
		return new InstructionPtr(ptr.codes, ptr.pc);
	}

	//UInt32/*Instruction*/ this[int index]
	public final long get(int index) { //Instruction - UInt32
			return this.codes[pc + index];
	}
	public final void set(int index, long val) { //Instruction - UInt32
		this.codes[pc + index] = val;
	}

	public static InstructionPtr inc(InstructionPtr[] ptr) { //ref
		InstructionPtr result = new InstructionPtr(ptr[0].codes, ptr[0].pc);
		ptr[0].pc++;
		return result;
	}

	public static InstructionPtr dec(InstructionPtr[] ptr) { //ref
		InstructionPtr result = new InstructionPtr(ptr[0].codes, ptr[0].pc);
		ptr[0].pc--;
		return result;
	}

	//operator <
	public static boolean lessThan(InstructionPtr p1, InstructionPtr p2) {
		ClassType.Assert(p1.codes == p2.codes);
		return p1.pc < p2.pc;
	}

	//operator >
	public static boolean greaterThan(InstructionPtr p1, InstructionPtr p2) {
		ClassType.Assert(p1.codes == p2.codes);
		return p1.pc > p2.pc;
	}

	//operator <=
	public static boolean lessEqual(InstructionPtr p1, InstructionPtr p2) {
		ClassType.Assert(p1.codes == p2.codes);
		return p1.pc < p2.pc;
	}

	//operator >=
	public static boolean greaterEqual(InstructionPtr p1, InstructionPtr p2) {
		ClassType.Assert(p1.codes == p2.codes);
		return p1.pc > p2.pc;
	}
}