using CFGLib.Parsers.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Forests {
	public class ForestLeaf : ForestNode {
		private readonly LeafNode _leafNode;
		// private readonly Terminal _terminal;

		internal ForestLeaf(EpsilonNode epsilonChild) : base(epsilonChild.StartPosition, epsilonChild.EndPosition) {
			_leafNode = epsilonChild;
		}
		internal ForestLeaf(TerminalNode terminalChild) : base(terminalChild.StartPosition, terminalChild.EndPosition) {
			_leafNode = terminalChild;
			// _terminal = terminalChild.Terminal;
		}

		internal override string ToStringSelf() {
			return string.Format("{0} ({1}, {2})", _leafNode.GetSentence(), StartPosition, EndPosition);
		}

		internal override string ToStringHelper(int level, HashSet<InteriorNode> visited) {
			var retval = "";
			retval += string.Format("{0}\n", ToStringSelf()).Indent(2 * level);
			return retval;
		}

		internal override void GetGraphHelper(Graph g, ForestNodeNode myNode, HashSet<InteriorNode> visited, Dictionary<InteriorNode, int> seen, ref int id, bool share = false) {
			return;
		}
	}
}
