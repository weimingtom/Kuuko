package KopiLua;

public final class Tools {
	public static String sprintf(String Format, Object... Parameters) {
		return String.format(Format, Parameters);
	}
	public static void printf(String Format, Object... Parameters) {
		System.out.print(Tools.sprintf(Format, Parameters));
	}
}
