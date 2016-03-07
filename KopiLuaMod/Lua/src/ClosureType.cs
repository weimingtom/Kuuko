/*
 ** $Id: lobject.c,v 2.22.1.1 2007/12/27 13:02:25 roberto Exp $
 ** Some generic functions over Lua objects
 ** See Copyright Notice in lua.h
 */
namespace KopiLua
{
	public class ClosureType
	{
		private ClosureHeader header;

        //implicit operator ClosureHeader
		public static ClosureHeader toClosureHeader(ClosureType ctype) 
		{
            return ctype.header; 
		}
		
		public ClosureType(ClosureHeader header) 
		{ 
			this.header = header; 
		}

        /*Byte*/ /*lu_byte*/
        public byte getIsC()
        {
            return header.isC;
        }

        /*Byte*/ /*lu_byte*/
        public void setIsC(byte val) 
        {
            header.isC = val;
        }

        /*Byte*/
        /*lu_byte*/
        public byte getNupvalues()
        {
            return header.nupvalues;
        }

        /*Byte*/
        /*lu_byte*/
        public void setNupvalues(byte val)
        {
            header.nupvalues = val;
        }

        public GCObject getGclist()
        {
            return header.gclist;
        }

        public void setGclist(GCObject val)
        {
            header.gclist = val;
        }

        public Table getEnv()
        {
            return header.env;
        }

        public void setEnv(Table val)
        {
            header.env = val;
        }
	}
}
