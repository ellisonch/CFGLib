using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Sppf {
	public class SppfEpsilon : SppfNode {
		private readonly int _cachedHash;

		public SppfEpsilon(int startPos, int endPos) : base(startPos, endPos) {
			_cachedHash = MyGetHashCode();
		}

		public static bool operator ==(SppfEpsilon x, SppfEpsilon y) {
			if (ReferenceEquals(x, null)) {
				return ReferenceEquals(y, null);
			}
			return x.Equals(y);
		}
		public static bool operator !=(SppfEpsilon x, SppfEpsilon y) {
			return !(x == y);
		}
		public override bool Equals(object other) {
			var x = this;
			var y = other as SppfEpsilon;
			if (ReferenceEquals(y, null)) {
				return false;
			}

			if (x.StartPosition != y.StartPosition) {
				return false;
			}
			if (x.EndPosition != y.EndPosition) {
				return false;
			}

			return true;
		}

		public override int GetHashCode() {
			return _cachedHash;
		}

		// based on http://stackoverflow.com/a/263416/2877032
		private int MyGetHashCode() {
			unchecked {
				int hash = 17;
				hash = hash * 23 + this.StartPosition.GetHashCode();
				hash = hash * 23 + this.EndPosition.GetHashCode();

				return hash;
			}
		}

		//public override string ToString() {
		//	return string.Format("{0} {1} {2}", "ε", StartPosition, EndPosition);
		//}
		protected override string PayloadToString() {
			return "ε";
		}
	}
}
