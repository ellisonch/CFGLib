namespace CFGLib {
	public class CNFNonterminalProduction : CNFProduction {
		private readonly Variable _lhs;
		private readonly Variable[] _rhs = new Variable[2];
		private int _weight = 1;

		public CNFNonterminalProduction(Production production) {
			_lhs = production.Lhs;
			_rhs[0] = (Variable)production.Rhs[0];
			_rhs[1] = (Variable)production.Rhs[1];
			_weight = production.Weight;
		}
		public CNFNonterminalProduction(Variable lhs, Variable rhs1, Variable rhs2, int weight = 1) {
			_lhs = lhs;
			_rhs[0] = rhs1;
			_rhs[1] = rhs2;
			_weight = weight;
		}

		public Variable Lhs {
			get { return _lhs; }
		}
		public Variable[] Rhs {
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