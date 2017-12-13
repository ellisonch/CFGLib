using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {
	internal struct Family2 {
		// TODO: remove children interface entirely
		// private List<SppfNode2> _children;
		public List<SppfNode2> Children {
			get {
				var _children = new List<SppfNode2>();
				if (_firstChild != null) {
					_children.Add(_firstChild);
					if (_secondChild != null) {
						_children.Add(_secondChild);
					}
				}
				return _children;
			}
		}
		private readonly int _cachedHash;
		private readonly SppfNode2 _firstChild;
		private readonly SppfNode2 _secondChild;
		
		//public Family2() {
		//	//Children = new List<SppfNode2> { };
		//	_cachedHash = 0;
		//}
		public Family2(SppfNode2 v) {
			// Children = new List<SppfNode2> { v };
			_cachedHash = v.GetHashCode();
			_firstChild = v;
			_secondChild = null;
		}
		public Family2(SppfNode2 w, SppfNode2 v) {
			//Children = new List<SppfNode2> { w, v };
			_cachedHash = unchecked((17 * 23 + w.GetHashCode()) * 23 + v.GetHashCode());
			_firstChild = w;
			_secondChild = v;
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
			if (!(other is Family2 localOther)) {
				return false;
			}
			// var localOther = other as Family2;
			//if (localOther == null) {
			//	return false;
			//}

			// return Children.SequenceEqual(localOther.Children);
			return this._firstChild == localOther._firstChild
				&& this._secondChild == localOther._secondChild;
		}

		public override string ToString() {
			if (this.Children.Count == 0) {
				return string.Format("ε");
			}
			return string.Format(string.Join(" | ", this.Children));
		}
	}
}
