/*
 ** $Id: lparser.c,v 2.42.1.3 2007/12/28 15:32:23 roberto Exp $
 ** Lua Parser
 ** See Copyright Notice in lua.h
 */
namespace KopiLua
{
	/*
	 ** Expression descriptor
	 */
	public enum expkind
	{
		VVOID,	/* no value */
		VNIL,
		VTRUE,
		VFALSE,
		VK,		/* info = index of constant in `k' */
		VKNUM,	/* nval = numerical value */
		VLOCAL,	/* info = local register */
		VUPVAL,       /* info = index of upvalue in `upvalues' */
		VGLOBAL,	/* info = index of table; aux = index of global name in `k' */
		VINDEXED,	/* info = table register; aux = index register (or `k') */
		VJMP,		/* info = instruction pc */
		VRELOCABLE,	/* info = instruction pc */
		VNONRELOC,	/* info = result register */
		VCALL,	/* info = instruction pc */
		VVARARG	/* info = instruction pc */
	}
}
