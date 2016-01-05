namespace GrammarGen {
	internal class CNFTerminalProduction {
		private readonly Variable _lhs;
		private readonly Terminal _rhs;

		public CNFTerminalProduction(Production production) {
			_lhs = production.Lhs;
			_rhs = (Terminal)production.Rhs[0];
		}
	}
}