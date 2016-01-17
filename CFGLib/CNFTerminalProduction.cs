namespace CFGLib {
	public class CNFTerminalProduction : CNFProduction {
		private readonly Nonterminal _lhs;
		private readonly Terminal _rhs;
		// TODO: figure out how to keep this readonly?
		private int _weight = 1;

		public CNFTerminalProduction(Production production) {
			_lhs = production.Lhs;
			_rhs = (Terminal)production.Rhs[0];
			_weight = production.Weight;
		}

		public CNFTerminalProduction(Nonterminal lhs, Terminal rhs, int weight = 1) {
			_lhs = lhs;
			_rhs = rhs;
			_weight = weight;
		}

		public Nonterminal Lhs {
			get { return _lhs; }
		}
		public Terminal Rhs {
			get { return _rhs; }
		}
		public int Weight {
			get { return _weight; }
			set { _weight = value; }
		}

		public override string ToString() {
			var lhss = _lhs.ToString();
			var rhss = _rhs.ToString();

			return lhss + " → " + rhss;
		}
	}
}