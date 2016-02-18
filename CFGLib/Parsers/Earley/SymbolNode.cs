using CFGLib.Parsers.Forests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {
	internal class SymbolNode : InteriorNode {
		public readonly Word Symbol;

		public SymbolNode(Word symbol, int start, int end) : base(start, end) {
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

		internal override Sppf ToSppf(Sentence s, Dictionary<Node, Sppf> dict = null) {
			if (Symbol.IsTerminal()) {
				return null;
			}
			if (dict == null) {
				dict = new Dictionary<Node, Sppf>();
			}

			Sppf previouslySeenSppf;
			if (dict.TryGetValue(this, out previouslySeenSppf)) {
				return previouslySeenSppf;
			}

			var nonterminal = (Nonterminal)Symbol;
			var sppf = new Sppf(nonterminal, s.GetRange(StartPosition, EndPosition - StartPosition));
			dict[this] = sppf;

			// List<Children> families = new List<Children>();
			// foreach (var family in this.Families) {
			for (int i = 0; i < FamiliesList.Count; i++) {
				var family = FamiliesList[i];
				var sppfList = family.Members.Select((l) => l.ToSppf(s, dict));
				var sppfChildren = new Children(ChildProductions[i], sppfList);
				sppf.Families.Add(sppfChildren);
			}

			return sppf;
		}

		public override string ToString() {
			return string.Format("({0}, {1}, {2}){3}", Symbol, StartPosition, EndPosition, ProductionsToString());
		}
	}
}
