using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Forests {
	public class ForestLeaf : ForestNode {
		private readonly TerminalNode _terminalNode;
		private readonly Terminal _terminal;

		public override int Id {
			get {
				return _terminalNode.Id;
			}
		}

		internal ForestLeaf(TerminalNode terminalChild) : base(terminalChild.StartPosition, terminalChild.EndPosition) {
			_terminalNode = terminalChild;
			_terminal = terminalChild.Terminal;
		}

		internal override string ToStringSelf() {
			return string.Format("{0} ({1}, {2})", _terminal, StartPosition, EndPosition);
		}

		internal override string ToStringHelper(int level) {
			var retval = "";
			retval += string.Format("{0}\n", ToStringSelf()).Indent(2 * level);
			return retval;
		}

		internal override void GetGraphHelper(Graph g, NodeNode myNode, HashSet<ForestNode> visited, Dictionary<InteriorNode, int> seen, ref int id, bool share = false) {
			return;
		}
	}
}
