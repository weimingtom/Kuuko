/*
 ** $Id: lobject.c,v 2.22.1.1 2007/12/27 13:02:25 roberto Exp $
 ** Some generic functions over Lua objects
 ** See Copyright Notice in lua.h
 */
namespace KopiLua
{
	/*
	 ** Function Prototypes
	 */
	public class Proto : GCObject
	{
		public Proto[] protos = null;
		public int index = 0;
		
		public TValue[] k;  /* constants used by the function */
		public long[]/*UInt32[]*//*Instruction[]*/ code;
		public /*new*/ Proto[] p;  /* functions defined inside the function */
		public int[] lineinfo;  /* map from opcodes to source lines */
		public LocVar[] locvars;  /* information about local variables */
		public TString[] upvalues;  /* upvalue names */
		public TString source;
		public int sizeupvalues;
		public int sizek;  /* size of `k' */
		public int sizecode;
		public int sizelineinfo;
		public int sizep;  /* size of `p' */
		public int sizelocvars;
		public int linedefined;
		public int lastlinedefined;
		public GCObject gclist;
		public byte nups;  /*Byte*/ /*lu_byte*/ /* number of upvalues */
		public byte numparams;  /*Byte*/ /*lu_byte*/ 
		public byte is_vararg;  /*Byte*/ /*lu_byte*/
		public byte maxstacksize;  /*Byte*/ /*lu_byte*/

        //Proto this[int offset] get
		public Proto get(int offset) 
		{ 
			return this.protos[this.index + offset]; 
		}
	}
}
