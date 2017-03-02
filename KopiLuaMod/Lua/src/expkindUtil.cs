using System;
using System.Collections.Generic;
using System.Text;

namespace KopiLua
{
    public class expkindUtil
    {
        public static int expkindToInt(expkind exp)
        {
            switch (exp)
            {
                case expkind.VVOID:
                    return 0;
		        case expkind.VNIL:
                    return 1;
		        case expkind.VTRUE:
                    return 2;
		        case expkind.VFALSE:
                    return 3;
		        case expkind.VK:
                    return 4;		
		        case expkind.VKNUM:
                    return 5;	
		        case expkind.VLOCAL:
                    return 6;	
		        case expkind.VUPVAL:
                    return 7;       
		        case expkind.VGLOBAL:
                    return 8;	
		        case expkind.VINDEXED:
                    return 9;	
		        case expkind.VJMP:
                    return 10;		
		        case expkind.VRELOCABLE:
                    return 11;	
		        case expkind.VNONRELOC:
                    return 12;	
		        case expkind.VCALL:
                    return 13;
                case expkind.VVARARG:
                    return 14;	
            }
            throw new Exception("expkindToInt error");
        }
    }
}
