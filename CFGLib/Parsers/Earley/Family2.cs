using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {
	internal class Family2 {
		public List<SppfNode2> Children { get; }
		private readonly int _cachedHash;

		public Family2() {
			Children = new List<SppfNode2> { };
			_cachedHash = 0;
		}
		public Family2(SppfNode2 v) {
			Children = new List<SppfNode2> { v };
			_cachedHash = v.GetHashCode();
		}
		public Family2(SppfNode2 w, SppfNode2 v) {
			Children = new List<SppfNode2> { w, v };
			_cachedHash = unchecked((17 * 23 + w.GetHashCode()) * 23 + v.GetHashCode());
		}

		public override int GetHashCode() {
			return _cachedHash;
			//var first = Children.ElementAtOrDefault(0);
			//var second = Children.ElementAtOrDefault(1);

			////return new { first, second }.GetHashCode();

			//// based on http://stackoverflow.com/a/263416/2877032
			//unchecked {
			//	int hash = 17;
			//	hash = hash * 23 + (first == null ? 0 : first.GetHashCode());
			//	hash = hash * 23 + (second == null ? 0 : second.GetHashCode());

			//	return hash;
			//}
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
