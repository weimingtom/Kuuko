package KopiLua;

//
// ** $Id: lstate.c,v 2.36.1.2 2008/01/03 15:20:39 roberto Exp $
// ** Global State
// ** See Copyright Notice in lua.h
// 
//    
//	 ** `global state', shared by all threads of this state
//	 
public class global_State {
	public stringtable strt = new stringtable(); // hash table for strings 
	public lua_Alloc frealloc; // function to reallocate memory 
	public Object ud; // auxiliary data to `frealloc' 
	public byte currentwhite; //Byte
 //lu_byte
	public byte gcstate; //Byte
 //lu_byte
  // state of garbage collector 
	public int sweepstrgc; // position of sweep in `strt' 
	public GCObject rootgc; // list of all collectable objects 
	public GCObjectRef sweepgc; // position of sweep in `rootgc' 
	public GCObject gray; // list of gray objects 
	public GCObject grayagain; // list of objects to be traversed atomically 
	public GCObject weak; // list of weak tables (to be cleared) 
	public GCObject tmudata; // last element of list of userdata to be GC 
	public Mbuffer buff = new Mbuffer(); // temporary buffer for string concatentation 
	public long GCthreshold; //lu_mem - UInt32
	public long totalbytes; // number of bytes currently allocated  - lu_mem - UInt32
	public long estimate; // an estimate of number of bytes actually in use  - lu_mem - UInt32
	public long gcdept; // how much GC is `behind schedule'  - lu_mem - UInt32
	public int gcpause; // size of pause between successive GCs 
	public int gcstepmul; // GC `granularity' 
	public lua_CFunction panic; // to be called in unprotected errors 
	public TValue l_registry = new TValue();
	public lua_State mainthread;
	public UpVal uvhead = new UpVal(); // head of double-linked list of all open upvalues 
	public Table[] mt = new Table[LuaObject.NUM_TAGS]; // metatables for basic types 
	public TString[] tmname = new TString[TMS.TM_N.getValue()]; // array with tag-method names 
}