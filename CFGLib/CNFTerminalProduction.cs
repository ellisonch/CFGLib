using System;

namespace CFGLib {
	/// <summary>
	/// This class represents a Terminal CNF production (e.g., X -> a)
	/// </summary>
	public class CNFTerminalProduction : BaseProduction {
		private readonly Terminal _rhs;

		public CNFTerminalProduction(BaseProduction production) {
			this.Lhs = production.Lhs;
			_rhs = (Terminal)production.Rhs[0];
			this.Weight = production.Weight;
		}

		public CNFTerminalProduction(Nonterminal lhs, Terminal rhs, double weight = 1.0) {
			if (weight < 0.0) {
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
		
		internal override BaseProduction DeepClone() {
			return new CNFTerminalProduction(this.Lhs, this.SpecificRhs, this.Weight);
		}
	}
}