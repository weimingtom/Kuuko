/*
 ** $Id: lstate.c,v 2.36.1.2 2008/01/03 15:20:39 roberto Exp $
 ** Global State
 ** See Copyright Notice in lua.h
 */
namespace KopiLua
{
	/*
	 ** `global state', shared by all threads of this state
	 */
	public class global_State
	{
		public stringtable strt = new stringtable(); /* hash table for strings */
		public lua_Alloc frealloc;  /* function to reallocate memory */
		public object ud;         /* auxiliary data to `frealloc' */
		public byte currentwhite;  /*Byte*/ /*lu_byte*/
		public byte gcstate; /*Byte*/ /*lu_byte*/  /* state of garbage collector */
		public int sweepstrgc;  /* position of sweep in `strt' */
		public GCObject rootgc;  /* list of all collectable objects */
		public GCObjectRef sweepgc;  /* position of sweep in `rootgc' */
		public GCObject gray;  /* list of gray objects */
		public GCObject grayagain;  /* list of objects to be traversed atomically */
		public GCObject weak;  /* list of weak tables (to be cleared) */
		public GCObject tmudata;  /* last element of list of userdata to be GC */
		public Mbuffer buff = new Mbuffer();  /* temporary buffer for string concatentation */
		public long/*UInt32*//*lu_mem*/ GCthreshold;
		public long/*UInt32*//*lu_mem*/ totalbytes;  /* number of bytes currently allocated */
		public long/*UInt32*//*lu_mem*/ estimate;  /* an estimate of number of bytes actually in use */
		public long/*UInt32*//*lu_mem*/ gcdept;  /* how much GC is `behind schedule' */
		public int gcpause;  /* size of pause between successive GCs */
		public int gcstepmul;  /* GC `granularity' */
		public lua_CFunction panic;  /* to be called in unprotected errors */
		public TValue l_registry = new TValue();
		public lua_State mainthread;
		public UpVal uvhead = new UpVal();  /* head of double-linked list of all open upvalues */
		public Table[] mt = new Table[LuaObject.NUM_TAGS];  /* metatables for basic types */
		public TString[] tmname = new TString[(int)TMS.TM_N];  /* array with tag-method names */
	}
}
