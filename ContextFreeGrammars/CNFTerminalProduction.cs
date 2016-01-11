namespace ContextFreeGrammars {
	public class CNFTerminalProduction : CNFProduction {
		private readonly Variable _lhs;
		private readonly Terminal _rhs;
		private readonly int _weight = 1;

		public CNFTerminalProduction(Production production) {
			_lhs = production.Lhs;
			_rhs = (Terminal)production.Rhs[0];
			_weight = production.Weight;
		}

		public CNFTerminalProduction(Variable lhs, Terminal rhs, int weight = 1) {
			_lhs = lhs;
			_rhs = rhs;
			_weight = weight;
		}

		public Variable Lhs {
			get { return _lhs; }
		}
		public Terminal Rhs {
			get { return _rhs; }
		}
		public int Weight {
			get { return _weight; }
		}

		public override string ToString() {
			var lhss = _lhs.ToString();
			var rhss = _rhs.ToString();

			return lhss + " → " + rhss;
		}
	}
}