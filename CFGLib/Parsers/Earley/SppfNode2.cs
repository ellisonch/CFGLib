using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {
	internal class SppfNode2 {


		public static bool operator ==(SppfNode2 x, SppfNode2 y) {
			if (ReferenceEquals(x, null)) {
				return ReferenceEquals(y, null);
			}
			return x.Equals(y);
		}
		public static bool operator !=(SppfNode2 x, SppfNode2 y) {
			return !(x == y);
		}
		public override bool Equals(object other) {
			var x = this;
			var y = other as SppfNode2;
			if (y == null) {
				return false;
			}

			//if (x.CurrentPosition != y.CurrentPosition) {
			//	return false;
			//}

			//if (x.Production != y.Production) {
			//	return false;
			//}

			return true;
		}

		// based on http://stackoverflow.com/a/263416/2877032
		public override int GetHashCode() {
			unchecked {
				int hash = 17;
				//hash = hash * 23 + this.Production.GetHashCode();
				//hash = hash * 23 + this.CurrentPosition.GetHashCode();

				return hash;
			}
		}
	}
}
