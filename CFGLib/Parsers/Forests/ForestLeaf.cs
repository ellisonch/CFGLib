using CFGLib.Parsers.Forests.ForestVisitors;
using CFGLib.Parsers.Graphs;
using CFGLib.Parsers.Sppf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Forests {
	public class ForestLeaf : ForestNode {
		private readonly SppfNode2 _leafNode;
		// private readonly Terminal _terminal;

		internal ForestLeaf(SppfNode2 node) : base(node.StartPosition, node.EndPosition) {
			_leafNode = node;
		}

		internal override bool Accept(IForestVisitor visitor) {
			return visitor.Visit(this);
		}

		internal override string ToStringSelf() {
			return _leafNode.ToString();
		}

		internal override string ToStringHelper(int level, HashSet<SppfNode2> visited) {
			var retval = "";
			retval += string.Format("{0}\n", ToStringSelf()).Indent(2 * level);
			return retval;
		}
	}
}
