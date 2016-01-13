using System.Collections.Generic;

namespace GrammarGen {
	internal class Production {
		private Variable _lhs;
		private Sentence _rhs;
		private int _weight = 1;

		public Variable Lhs {
			get { return _lhs; }
		}

		public int Weight {
			get { return _weight; }
		}

		public Sentence Rhs {
			get { return _rhs; }
		}

		public Production(Variable lhs, Sentence rhs) {
			_lhs = lhs;
			_rhs = rhs;
		}

		public override string ToString() {
			var lhss = _lhs.ToString();
			var rhss = _rhs.ToString();
			
			return lhss + " → " + rhss;
		}
	}
}