/*
** $Id: lstate.c,v 2.36.1.2 2008/01/03 15:20:39 roberto Exp $
** Global State
** See Copyright Notice in lua.h
*/
namespace KopiLua
{
    /*
    ** informations about a call
    */
    public class CallInfo : ArrayElement
    {
        private CallInfo[] values = null;
        private int index = -1;
    
        public TValue base_;  /*StkId*/ /* base for this function */
        public TValue func;  /*StkId*/ /* function index in the stack */
        public TValue top;  /*StkId*/ /* top for this function */
        public InstructionPtr savedpc;
        public int nresults;  /* expected number of results from this function */
        public int tailcalls;  /* number of tail calls lost under this entry */
        
        public void set_index(int index)
        {
            this.index = index;
        }
    
        public void set_array(object array)
        {
            this.values = (CallInfo[])array;
            ClassType.Assert(this.values != null);
        }
        
        public CallInfo get(int offset)
        {
        	return values[index + offset]; 
        }
    
        public static CallInfo plus(CallInfo value, int offset)
        {
            return value.values[value.index + offset];
        }
    
        public static CallInfo minus(CallInfo value, int offset)
        {
            return value.values[value.index - offset];
        }
    
        public static int minus(CallInfo ci, CallInfo[] values)
        {
            ClassType.Assert(ci.values == values);
            return ci.index;
        }
    
        public static int minus(CallInfo ci1, CallInfo ci2)
        {
            ClassType.Assert(ci1.values == ci2.values);
            return ci1.index - ci2.index;
        }
    
        public static bool lessThan(CallInfo ci1, CallInfo ci2)
        {
            ClassType.Assert(ci1.values == ci2.values);
            return ci1.index < ci2.index;
        }
    
        public static bool lessEqual(CallInfo ci1, CallInfo ci2)
        {
            ClassType.Assert(ci1.values == ci2.values);
            return ci1.index <= ci2.index;
        }
    
        public static bool greaterThan(CallInfo ci1, CallInfo ci2)
        {
            ClassType.Assert(ci1.values == ci2.values);
            return ci1.index > ci2.index;
        }
    
        public static bool greaterEqual(CallInfo ci1, CallInfo ci2)
        {
            ClassType.Assert(ci1.values == ci2.values);
            return ci1.index >= ci2.index;
        }
    
        public static CallInfo inc(/*ref*/ CallInfo[] value)
        {
        	value[0] = value[0].get(1);
        	return value[0].get(-1);
        }
    
        public static CallInfo dec(/*ref*/ CallInfo[] value)
        {
        	value[0] = value[0].get(-1);
        	return value[0].get(1);
        }
    }
}
