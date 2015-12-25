/*
 ** $Id: lcode.c,v 2.25.1.3 2007/12/28 15:32:23 roberto Exp $
 ** Code generator for Lua
 ** See Copyright Notice in lua.h
 */
using System;
using System.Diagnostics;

namespace KopiLua
{
	public class InstructionPtr
	{
		public UInt32[]/*Instruction[]*/ codes;
		public int pc;
		
		public InstructionPtr()
		{
			this.codes = null;
			this.pc = -1;
		}
		
		public InstructionPtr(UInt32[]/*Instruction[]*/ codes, int pc)
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
		
		public UInt32/*Instruction*/ this[int index]
		{
			get
			{
				return this.codes[pc + index];
			}
			set
			{
				this.codes[pc + index] = value;
			}
		}
		
		public static InstructionPtr inc(ref InstructionPtr ptr)
		{
			InstructionPtr result = new InstructionPtr(ptr.codes, ptr.pc);
			ptr.pc++;
			return result;
		}
		
		public static InstructionPtr dec(ref InstructionPtr ptr)
		{
			InstructionPtr result = new InstructionPtr(ptr.codes, ptr.pc);
			ptr.pc--;
			return result;
		}
		
		public static bool operator <(InstructionPtr p1, InstructionPtr p2)
		{
			Debug.Assert(p1.codes == p2.codes);
			return p1.pc < p2.pc;
		}
		
		public static bool operator >(InstructionPtr p1, InstructionPtr p2)
		{
			Debug.Assert(p1.codes == p2.codes);
			return p1.pc > p2.pc;
		}
		
		public static bool operator <=(InstructionPtr p1, InstructionPtr p2)
		{
			Debug.Assert(p1.codes == p2.codes);
			return p1.pc < p2.pc;
		}
		
		public static bool operator >=(InstructionPtr p1, InstructionPtr p2)
		{
			Debug.Assert(p1.codes == p2.codes);
			return p1.pc > p2.pc;
		}
	}
}
