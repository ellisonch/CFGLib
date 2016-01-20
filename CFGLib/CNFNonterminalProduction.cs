using System;

namespace CFGLib {
	public class CNFNonterminalProduction : BaseProduction {
		private readonly Nonterminal[] _rhs = new Nonterminal[2];

		public CNFNonterminalProduction(BaseProduction production) {
			if (production.Rhs.Count != 2) {
				throw new ArgumentException("CNFNonterminalProductions need RHSs with exactly two items");
			}
			this.Lhs = production.Lhs;
			_rhs[0] = (Nonterminal)production.Rhs[0];
			_rhs[1] = (Nonterminal)production.Rhs[1];
			this.Weight = production.Weight;
		}
		public CNFNonterminalProduction(Nonterminal lhs, Nonterminal rhs1, Nonterminal rhs2, int weight = 1) {
			this.Lhs = lhs;
			_rhs[0] = rhs1;
			_rhs[1] = rhs2;
			this.Weight = weight;
		}
		public override Sentence Rhs {
			get { return new Sentence(this.SpecificRhs); }
		}

		public Nonterminal[] SpecificRhs {
			get { return _rhs; }
		}

		internal override BaseProduction DeepClone() {
			return new CNFNonterminalProduction(this.Lhs, this.SpecificRhs[0], this.SpecificRhs[1], this.Weight);
		}
	}
}