using CFGLib.Parsers.Earley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Sppf {
	public class SppfBranch : SppfNode {
		public DecoratedProduction DecoratedProduction { get; }

		public SppfBranch(DecoratedProduction dprod, int startPos, int endPos) : base(startPos, endPos) {
			DecoratedProduction = dprod;
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
			if (ReferenceEquals(y, null)) {
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

		// based on http://stackoverflow.com/a/263416/2877032
		public override int GetHashCode() {
			unchecked {
				int hash = 17;
				hash = hash * 23 + this.StartPosition.GetHashCode();
				hash = hash * 23 + this.EndPosition.GetHashCode();
				hash = hash * 23 + this.DecoratedProduction.GetHashCode();

				return hash;
			}
		}
		
		protected override string PayloadToString() {
			return DecoratedProduction.ToString();
		}
	}
}
