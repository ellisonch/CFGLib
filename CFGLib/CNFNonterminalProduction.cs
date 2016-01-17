namespace CFGLib {
	public class CNFNonterminalProduction : CNFProduction {
		private readonly Nonterminal _lhs;
		private readonly Nonterminal[] _rhs = new Nonterminal[2];
		private int _weight = 1;

		public CNFNonterminalProduction(Production production) {
			_lhs = production.Lhs;
			_rhs[0] = (Nonterminal)production.Rhs[0];
			_rhs[1] = (Nonterminal)production.Rhs[1];
			_weight = production.Weight;
		}
		public CNFNonterminalProduction(Nonterminal lhs, Nonterminal rhs1, Nonterminal rhs2, int weight = 1) {
			_lhs = lhs;
			_rhs[0] = rhs1;
			_rhs[1] = rhs2;
			_weight = weight;
		}

		public Nonterminal Lhs {
			get { return _lhs; }
		}
		public Nonterminal[] Rhs {
			get { return _rhs; }
		}
		public int Weight {
			get { return _weight; }
			set { _weight = value; }
		}

		public override string ToString() {
			var lhss = _lhs.ToString();
			var rhss = _rhs[0].ToString() + " " + _rhs[1].ToString();

			return lhss + " → " + rhss;
		}
	}
}