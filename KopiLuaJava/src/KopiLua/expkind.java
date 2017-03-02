package KopiLua;

//
// ** $Id: lparser.c,v 2.42.1.3 2007/12/28 15:32:23 roberto Exp $
// ** Lua Parser
// ** See Copyright Notice in lua.h
// 
//    
//	 ** Expression descriptor
//	 
public enum expkind {
	VVOID(0), // no value 
	VNIL(1),
	VTRUE(2),
	VFALSE(3),
	VK(4), // info = index of constant in `k' 
	VKNUM(5), // nval = numerical value 
	VLOCAL(6), // info = local register 
	VUPVAL(7), // info = index of upvalue in `upvalues' 
	VGLOBAL(8), // info = index of table; aux = index of global name in `k' 
	VINDEXED(9), // info = table register; aux = index register (or `k') 
	VJMP(10), // info = instruction pc 
	VRELOCABLE(11), // info = instruction pc 
	VNONRELOC(12), // info = result register 
	VCALL(13), // info = instruction pc 
	VVARARG(14); // info = instruction pc 

	private int intValue;
	private static java.util.HashMap<Integer, expkind> mappings;
	private synchronized static java.util.HashMap<Integer, expkind> getMappings() {
		if (mappings == null) {
			mappings = new java.util.HashMap<Integer, expkind>();
		}
		return mappings;
	}

	private expkind(int value) {
		intValue = value;
		expkind.getMappings().put(value, this);
	}

	public int getValue() {
		return intValue;
	}

	public static expkind forValue(int value) {
		return getMappings().get(value);
	}
}