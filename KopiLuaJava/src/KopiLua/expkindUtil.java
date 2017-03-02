package KopiLua;

public class expkindUtil {
	public static int expkindToInt(expkind exp) {
		switch (exp) {
			case VVOID:
				return 0;
			case VNIL:
				return 1;
			case VTRUE:
				return 2;
			case VFALSE:
				return 3;
			case VK:
				return 4;
			case VKNUM:
				return 5;
			case VLOCAL:
				return 6;
			case VUPVAL:
				return 7;
			case VGLOBAL:
				return 8;
			case VINDEXED:
				return 9;
			case VJMP:
				return 10;
			case VRELOCABLE:
				return 11;
			case VNONRELOC:
				return 12;
			case VCALL:
				return 13;
			case VVARARG:
				return 14;
		}
		throw new RuntimeException("expkindToInt error");
	}
}