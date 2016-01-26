using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib {
	internal struct ValueUnitProduction {
		public Nonterminal Lhs;
		public Word Rhs;
		public ValueUnitProduction(Nonterminal lhs, Word rhs) {
			this.Lhs = lhs;
			this.Rhs = rhs;
		}
		public override string ToString() {
			return string.Format("{0} → {1}", this.Lhs, this.Rhs);
		}
	}
}
