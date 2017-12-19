using CFGLib.Parsers.Forests;
using CFGLib.Parsers.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Sppf.Old {
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
			return string.Format("({0}, {1}, {2})", Symbol, StartPosition, EndPosition);
		}
		internal override string ToStringSimple() {
			return string.Format("{0} ({1}, {2})", Symbol, StartPosition, EndPosition);
		}
	}
}
