package KopiLua;

//
// ** $Id: lobject.c,v 2.22.1.1 2007/12/27 13:02:25 roberto Exp $
// ** Some generic functions over Lua objects
// ** See Copyright Notice in lua.h
// 
public class Node implements ArrayElement {
	private Node[] values = null;
	private int index = -1;

	public static int ids = 0;
	public int id = ids++;

	public TValue i_val;
	public TKey i_key;

	public final void set_index(int index) {
		this.index = index;
	}

	public final void set_array(Object array) {
		this.values = (Node[])array;
		ClassType.Assert(this.values != null);
	}

	public Node() {
		this.i_val = new TValue();
		this.i_key = new TKey();
	}

	public Node(Node copy) {
		this.values = copy.values;
		this.index = copy.index;
		this.i_val = new TValue(copy.i_val);
		this.i_key = new TKey(copy.i_key);
	}

	public Node(TValue i_val, TKey i_key) {
		this.values = new Node[] { this };
		this.index = 0;
		this.i_val = i_val;
		this.i_key = i_key;
	}

	//Node this[int offset]
	public final Node get(int offset) {
		return this.values[this.index + offset];
	}

	//Node this[uint offset]
	//public Node get(uint offset)
	//{
	//    return this.values[this.index + (int)offset];
	//}

	//operator -
	public static int minus(Node n1, Node n2) {
		ClassType.Assert(n1.values == n2.values);
		return n1.index - n2.index;
	}

	public static Node inc(Node[] node) { //ref
		node[0] = node[0].get(1);
		return node[0].get(-1);
	}

	public static Node dec(Node[] node) { //ref
		node[0] = node[0].get(-1);
		return node[0].get(1);
	}

	//operator >
	public static boolean greaterThan(Node n1, Node n2) {
		ClassType.Assert(n1.values == n2.values);
		return n1.index > n2.index;
	}

	//operator >=
	public static boolean greaterEqual(Node n1, Node n2) {
		ClassType.Assert(n1.values == n2.values);
		return n1.index >= n2.index;
	}

	//operator <
	public static boolean lessThan(Node n1, Node n2) {
		ClassType.Assert(n1.values == n2.values);
		return n1.index < n2.index;
	}

	//operator <=
	public static boolean lessEqual(Node n1, Node n2) {
		ClassType.Assert(n1.values == n2.values);
		return n1.index <= n2.index;
	}

	//operator ==
	public static boolean isEqual(Node n1, Node n2) {
		Object o1 = (Node)((n1 instanceof Node) ? n1 : null);
		Object o2 = (Node)((n2 instanceof Node) ? n2 : null);
		if ((o1 == null) && (o2 == null)) {
			return true;
		}
		if (o1 == null) {
			return false;
		}
		if (o2 == null) {
			return false;
		}
		if (n1.values != n2.values) {
			return false;
		}
		return n1.index == n2.index;
	}

	//operator !=
	public static boolean isNotEqual(Node n1, Node n2) {
		//return !(n1 == n2); 
		return !isEqual(n1, n2);
	}

	@Override
	public boolean equals(Object o) {
		//return this == (Node)o; 
		return Node.isEqual(this, (Node)o);
	}

	@Override
	public int hashCode() {
		return 0;
	}
}