using System;
using System.Collections.Generic;

namespace CFGLib {
	public class Production : BaseProduction {
		private Sentence _rhs;

		public override Sentence Rhs {
			get { return _rhs; }
		}

		public Production(Nonterminal lhs, Sentence rhs, int weight = 1) {
			if (weight < 1) {
				throw new ArgumentOutOfRangeException("Weights must be positive");
			}
			this.Lhs = lhs;
			_rhs = rhs;
			this.Weight = weight;
		}

		internal override BaseProduction Clone() {
			return new Production(this.Lhs, new Sentence(_rhs), this.Weight);
		}
	}
}