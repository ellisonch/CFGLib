using CFGLib.Parsers.Forests;
using CFGLib.Parsers.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Sppf {
	internal class SymbolNode : InteriorNode {
		public readonly Nonterminal Symbol;

		public SymbolNode(Nonterminal symbol, int start, int end) : base(start, end) {
			Symbol = symbol;
		}

		//public SymbolNode(Item item) : this(item.Production.Lhs, item.StartPosition, item.EndPosition) {
		//}

		public override int GetHashCode() {
			return new { StartPosition, EndPosition, Symbol }.GetHashCode();
		}

		public override bool Equals(Object other) {
			if (other == null) {
				return false;
			}
			var localOther = other as SymbolNode;
			if (localOther == null) {
				return false;
			}

			if (StartPosition != localOther.StartPosition) {
				return false;
			}
			if (EndPosition != localOther.EndPosition) {
				return false;
			}
			if (Symbol != localOther.Symbol) {
				return false;
			}

			return true;
		}

		public override string ToString() {
			return string.Format("({0}, {1}, {2}){3}", Symbol, StartPosition, EndPosition, ProductionsToString());
		}
		internal override string ToStringSimple() {
			return string.Format("{0} ({1}, {2})", Symbol, StartPosition, EndPosition);
		}

		public Graph GetGraph() {
			var node = new SppfNodeNode(this, 0);
			var g = new Graph(node);
			GetGraphHelper(g, node, new HashSet<InteriorNode>());
			return g;
		}
	}
}
