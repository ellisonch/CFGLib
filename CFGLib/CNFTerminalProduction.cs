using System;

namespace CFGLib {
	public class CNFTerminalProduction : BaseProduction {
		private readonly Terminal _rhs;

		public CNFTerminalProduction(BaseProduction production) {
			this.Lhs = production.Lhs;
			_rhs = (Terminal)production.Rhs[0];
			this.Weight = production.Weight;
		}

		public CNFTerminalProduction(Nonterminal lhs, Terminal rhs, int weight = 1) {
			if (weight < 1) {
				throw new ArgumentOutOfRangeException("Weights must be positive");
			}
			this.Lhs = lhs;
			_rhs = rhs;
			this.Weight = weight;
		}
		public override Sentence Rhs {
			get { return new Sentence { this.SpecificRhs }; }
		}
		public Terminal SpecificRhs {
			get { return _rhs; }
		}
		
		internal override BaseProduction Clone() {
			return new CNFTerminalProduction(this.Lhs, this.SpecificRhs, this.Weight);
		}
	}
}