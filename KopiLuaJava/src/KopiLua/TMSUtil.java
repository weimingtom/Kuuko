package KopiLua;

public class TMSUtil {
	public static int convertTMStoInt(TMS tms) {
		switch (tms) {
			case TM_INDEX:
				return 0;
			case TM_NEWINDEX:
				return 1;
			case TM_GC:
				return 2;
			case TM_MODE:
				return 3;
			case TM_EQ:
				return 4;
			case TM_ADD:
				return 5;
			case TM_SUB:
				return 6;
			case TM_MUL:
				return 7;
			case TM_DIV:
				return 8;
			case TM_MOD:
				return 9;
			case TM_POW:
				return 10;
			case TM_UNM:
				return 11;
			case TM_LEN:
				return 12;
			case TM_LT:
				return 13;
			case TM_LE:
				return 14;
			case TM_CONCAT:
				return 15;
			case TM_CALL:
				return 16;
			case TM_N:
				return 17;
		}
		throw new RuntimeException("convertTMStoInt error");
	}
}