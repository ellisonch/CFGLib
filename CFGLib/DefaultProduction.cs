using System;
using System.Collections.Generic;

namespace CFGLib {
	/// <summary>
	/// This class represents a generic but concrete production
	/// </summary>
	internal class DefaultProduction : Production {
		private Sentence _rhs;
		
		public override Sentence Rhs {
			get { return _rhs; }
		}

		public DefaultProduction(Nonterminal lhs, Sentence rhs, double weight = 1.0) {
			if (lhs == null) {
				throw new ArgumentNullException("Lhs must be non-null");
			}
			if (rhs == null) {
				throw new ArgumentNullException("Rhs must be non-null");
			}
			if (weight < 0.0) {
				throw new ArgumentOutOfRangeException("Weights must be positive");
			}
			this.Lhs = lhs;
			_rhs = rhs;
			this.Weight = weight;
		}

		internal override Production DeepClone() {
			return new DefaultProduction(this.Lhs, new Sentence(_rhs), this.Weight);
		}
	}
}