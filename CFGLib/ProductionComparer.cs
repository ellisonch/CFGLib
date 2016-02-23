using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib {
	internal class ProductionComparer : IEqualityComparer<Production> {
		public bool Equals(Production x, Production y) {
			if (x == y) {
				return true;
			}
			if (x == null) {
				return false;
			}
			if (y == null) {
				return false;
			}

			if (x.Lhs != y.Lhs) {
				return false;
			}
			if (!x.Rhs.SequenceEqual(y.Rhs)) {
				return false;
			}

			return true;
		}

		// based on http://stackoverflow.com/a/263416/2877032
		public int GetHashCode(Production production) {
			if (production == null) {
				return 0;
			}
			unchecked {
				int hash = 17;
				hash = hash * 23 + production.Lhs.GetHashCode();
				foreach (var word in production.Rhs) {
					hash = hash * 23 + word.GetHashCode();
				}
				return hash;
			}

			// return new { production.Lhs, production.Rhs }.GetHashCode();
		}
	}
}
