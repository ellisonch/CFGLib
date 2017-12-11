using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {
	internal class Family2 {
		private List<SppfNode2> Children { get; }

		public Family2() {
			Children = new List<SppfNode2> { };
		}
		public Family2(SppfNode2 v) {
			Children = new List<SppfNode2> { v };
		}
		public Family2(SppfNode2 w, SppfNode2 v) {
			Children = new List<SppfNode2> { w, v };
		}

		public override int GetHashCode() {
			var first = Children.ElementAtOrDefault(0);
			var second = Children.ElementAtOrDefault(1);

			return new { first, second }.GetHashCode();
		}

		public override bool Equals(Object other) {
			if (other == null) {
				return false;
			}
			var localOther = other as Family2;
			if (localOther == null) {
				return false;
			}

			return Children.SequenceEqual(localOther.Children);
		}

		public override string ToString() {
			if (this.Children.Count == 0) {
				return string.Format("ε");
			}
			return string.Format(string.Join(" | ", this.Children));
		}
	}
}
