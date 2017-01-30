/*
 ** $Id: lstate.c,v 2.36.1.2 2008/01/03 15:20:39 roberto Exp $
 ** Global State
 ** See Copyright Notice in lua.h
 */
namespace KopiLua
{
    public class ArrayRef : GCObjectRef, ArrayElement
    {
        // ArrayRef is used to reference GCObject objects in an array, the next two members
        // point to that array and the index of the GCObject element we are referencing
        private GCObject[] array_elements;
        private int array_index;
    
        // ArrayRef is itself stored in an array and derived from ArrayElement, the next
        // two members refer to itself i.e. the array and index of it's own instance.
        private ArrayRef[] vals;
        private int index;
    	
        public ArrayRef()
        {
            this.array_elements = null;
            this.array_index = 0;
            this.vals = null;
            this.index = 0;
        }
        
        public ArrayRef(GCObject[] array_elements, int array_index)
        {
            this.array_elements = array_elements;
            this.array_index = array_index;
            this.vals = null;
            this.index = 0;
        }
        
        public void set(GCObject value) 
        { 
        	array_elements[array_index] = value; 
        }
        
        public GCObject get() 
        {
        	return array_elements[array_index]; 
        }
    
        public void set_index(int index)
        {
            this.index = index;
        }
        
        public void set_array(object vals)
        {
            // don't actually need this
            this.vals = (ArrayRef[])vals;
            ClassType.Assert(this.vals != null);
        }
    }
}
