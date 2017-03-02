using System;
using System.Collections.Generic;
using System.Text;

namespace KopiLua
{
    public class TMSUtil
    {
        public static int convertTMStoInt(TMS tms)
        {
            switch (tms)
            {
                case TMS.TM_INDEX:
                    return 0;
                case TMS.TM_NEWINDEX:
                    return 1;
                case TMS.TM_GC:
                    return 2;
                case TMS.TM_MODE:
                    return 3;
                case TMS.TM_EQ:
                    return 4;
                case TMS.TM_ADD:
                    return 5;
                case TMS.TM_SUB:
                    return 6;
                case TMS.TM_MUL:
                    return 7;
                case TMS.TM_DIV:
                    return 8;
                case TMS.TM_MOD:
                    return 9;
                case TMS.TM_POW:
                    return 10;
                case TMS.TM_UNM:
                    return 11;
                case TMS.TM_LEN:
                    return 12;
                case TMS.TM_LT:
                    return 13;
                case TMS.TM_LE:
                    return 14;
                case TMS.TM_CONCAT:
                    return 15;
                case TMS.TM_CALL:
                    return 16;
                case TMS.TM_N:
                    return 17;
            }
            throw new Exception("convertTMStoInt error");
        }
    }
}
