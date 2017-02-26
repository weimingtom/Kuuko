/*
 ** $Id: lcode.c,v 2.25.1.3 2007/12/28 15:32:23 roberto Exp $
 ** Code generator for Lua
 ** See Copyright Notice in lua.h
 */
namespace KopiLua
{
	public class InstructionPtr
	{
		public long[]/*UInt32[]*//*Instruction[]*/ codes;
		public int pc;
		
		public InstructionPtr()
		{
			this.codes = null;
			this.pc = -1;
		}
		
		public InstructionPtr(long[]/*UInt32[]*//*Instruction[]*/ codes, int pc)
		{
			this.codes = codes;
			this.pc = pc;
		}
		
		public static InstructionPtr Assign(InstructionPtr ptr)
		{
			if (ptr == null)
			{
				return null;
			}
			return new InstructionPtr(ptr.codes, ptr.pc);
		}

        //UInt32/*Instruction*/ this[int index]
        public long/*UInt32*//*Instruction*/ get(int index)
        {
                return this.codes[pc + index];
        }
        public void set(int index, long/*UInt32*//*Instruction*/ val)
        {
            this.codes[pc + index] = val;
        }

        public static InstructionPtr inc(/*ref*/ InstructionPtr[] ptr)
		{
        	InstructionPtr result = new InstructionPtr(ptr[0].codes, ptr[0].pc);
			ptr[0].pc++;
			return result;
		}
		
        public static InstructionPtr dec(/*ref*/ InstructionPtr[] ptr)
		{
        	InstructionPtr result = new InstructionPtr(ptr[0].codes, ptr[0].pc);
			ptr[0].pc--;
			return result;
		}

        //operator <
		public static bool lessThan(InstructionPtr p1, InstructionPtr p2)
		{
            ClassType.Assert(p1.codes == p2.codes);
			return p1.pc < p2.pc;
		}

        //operator >
		public static bool greaterThan(InstructionPtr p1, InstructionPtr p2)
		{
            ClassType.Assert(p1.codes == p2.codes);
			return p1.pc > p2.pc;
		}

        //operator <=
		public static bool lessEqual(InstructionPtr p1, InstructionPtr p2)
		{
            ClassType.Assert(p1.codes == p2.codes);
			return p1.pc < p2.pc;
		}

        //operator >=
		public static bool greaterEqual(InstructionPtr p1, InstructionPtr p2)
		{
            ClassType.Assert(p1.codes == p2.codes);
			return p1.pc > p2.pc;
		}
	}
}
