/*
 ** $Id: lobject.c,v 2.22.1.1 2007/12/27 13:02:25 roberto Exp $
 ** Some generic functions over Lua objects
 ** See Copyright Notice in lua.h
 */
namespace KopiLua
{
	public class Node : ArrayElement
	{
		private Node[] values = null;
		private int index = -1;
		
		public static int ids = 0;
		public int id = ids++;
		
		public TValue i_val;
		public TKey i_key;
		
		public void set_index(int index)
		{
			this.index = index;
		}
		
		public void set_array(object array)
		{
			this.values = (Node[])array;
			ClassType.Assert(this.values != null);
		}
		
		public Node()
		{
			this.i_val = new TValue();
			this.i_key = new TKey();
		}
		
		public Node(Node copy)
		{
			this.values = copy.values;
			this.index = copy.index;
			this.i_val = new TValue(copy.i_val);
			this.i_key = new TKey(copy.i_key);
		}
		
		public Node(TValue i_val, TKey i_key)
		{
			this.values = new Node[] { this };
			this.index = 0;
			this.i_val = i_val;
			this.i_key = i_key;
		}

        //Node this[int offset]
        public Node get(int offset)
        {
            return this.values[this.index + offset];
        }

        //Node this[uint offset]
        //public Node get(uint offset)
        //{
        //    return this.values[this.index + (int)offset];
        //}

        //operator -
		public static int minus(Node n1, Node n2)
		{
			ClassType.Assert(n1.values == n2.values);
			return n1.index - n2.index;
		}
		
		public static Node inc(/*ref*/ Node[] node)
		{
			node[0] = node[0].get(1);
			return node[0].get(-1);
		}
		
		public static Node dec(/*ref*/ Node[] node)
		{
			node[0] = node[0].get(-1);
			return node[0].get(1);
		}
		
        //operator >
		public static bool greaterThan(Node n1, Node n2) 
		{
			ClassType.Assert(n1.values == n2.values); 
			return n1.index > n2.index;
		}
		
        //operator >=
		public static bool greaterEqual(Node n1, Node n2) 
		{ 
			ClassType.Assert(n1.values == n2.values); 
			return n1.index >= n2.index;
		}

        //operator <
		public static bool lessThan(Node n1, Node n2) 
		{ 
			ClassType.Assert(n1.values == n2.values); 
			return n1.index < n2.index;
		}
		
        //operator <=
		public static bool lessEqual(Node n1, Node n2) 
		{ 
			ClassType.Assert(n1.values == n2.values); 
			return n1.index <= n2.index; 
		}

        //operator ==
        public static bool isEqual(Node n1, Node n2)
        {
            object o1 = n1 as Node;
            object o2 = n2 as Node;
            if ((o1 == null) && (o2 == null))
            {
                return true;
            }
            if (o1 == null)
            {
                return false;
            }
            if (o2 == null)
            {
                return false;
            }
            if (n1.values != n2.values)
            {
                return false;
            }
            return n1.index == n2.index;
        }

        //operator !=
        public static bool isNotEqual(Node n1, Node n2)
        {
            //return !(n1 == n2); 
            return !isEqual(n1, n2);
        }

		public override bool Equals(object o) 
		{ 
			//return this == (Node)o; 
            return Node.isEqual(this, (Node)o);
		}
		
		public override int GetHashCode() 
		{ 
			return 0; 
		}
	}
}
