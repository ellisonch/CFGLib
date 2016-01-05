namespace GrammarGen {
	internal class CNFNonterminalProduction {
		private readonly Variable _lhs;
		private readonly Variable[] _rhs = new Variable[2];

		public CNFNonterminalProduction(Production production) {
			_lhs = production.Lhs;
			_rhs[0] = (Variable)production.Rhs[0];
			_rhs[1] = (Variable)production.Rhs[1];
		}
	}
}