using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {
	public class SppfBranch : SppfNode2 {
		public DecoratedProduction DecoratedProduction { get; }
		private readonly int _cachedHash;

		public SppfBranch(DecoratedProduction dprod, int startPos, int endPos) : base(startPos, endPos) {
			DecoratedProduction = dprod;
			_cachedHash = MyGetHashCode();
		}

		public static bool operator ==(SppfBranch x, SppfBranch y) {
			if (ReferenceEquals(x, null)) {
				return ReferenceEquals(y, null);
			}
			return x.Equals(y);
		}
		public static bool operator !=(SppfBranch x, SppfBranch y) {
			return !(x == y);
		}
		public override bool Equals(object other) {
			var x = this;
			var y = other as SppfBranch;
			if (y == null) {
				return false;
			}

			if (x.StartPosition != y.StartPosition) {
				return false;
			}
			if (x.EndPosition != y.EndPosition) {
				return false;
			}
			if (x.DecoratedProduction != y.DecoratedProduction) {
				return false;
			}

			return true;
		}

		public override int GetHashCode() {
			return _cachedHash;
		}

		// based on http://stackoverflow.com/a/263416/2877032
		public int MyGetHashCode() {
			unchecked {
				int hash = 17;
				hash = hash * 23 + this.StartPosition.GetHashCode();
				hash = hash * 23 + this.EndPosition.GetHashCode();
				hash = hash * 23 + this.DecoratedProduction.GetHashCode();

				return hash;
			}
		}

		public override string ToString() {
			var firstBit = DecoratedProduction.ToString();
			return string.Format("{0} {1} {2}", firstBit, StartPosition, EndPosition);
		}
	}
}
