using System;
using System.Collections.Generic;

namespace CFGLib {
	/// <summary>
	/// This class represents a generic but concrete production
	/// </summary>
	public class Production : BaseProduction {
		private Sentence _rhs;
		
		public override Sentence Rhs {
			get { return _rhs; }
		}

		public Production(Nonterminal lhs, Sentence rhs, int weight = 1) {
			if (lhs == null) {
				throw new ArgumentNullException("Lhs must be non-null");
			}
			if (rhs == null) {
				throw new ArgumentNullException("Rhs must be non-null");
			}
			if (weight < 1) {
				throw new ArgumentOutOfRangeException("Weights must be positive");
			}
			this.Lhs = lhs;
			_rhs = rhs;
			this.Weight = weight;
		}

		internal override BaseProduction DeepClone() {
			return new Production(this.Lhs, new Sentence(_rhs), this.Weight);
		}
	}
}