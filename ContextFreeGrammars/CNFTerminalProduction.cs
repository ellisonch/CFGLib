namespace ContextFreeGrammars {
	public class CNFTerminalProduction {
		private readonly Variable _lhs;
		private readonly Terminal _rhs;

		public CNFTerminalProduction(Production production) {
			_lhs = production.Lhs;
			_rhs = (Terminal)production.Rhs[0];
		}

		public CNFTerminalProduction(Variable lhs, Terminal rhs) {
			_lhs = lhs;
			_rhs = rhs;
		}

		public Variable Lhs {
			get { return _lhs; }
		}
		public Terminal Rhs {
			get { return _rhs; }
		}

		public override string ToString() {
			var lhss = _lhs.ToString();
			var rhss = _rhs.ToString();

			return lhss + " → " + rhss;
		}
	}
}