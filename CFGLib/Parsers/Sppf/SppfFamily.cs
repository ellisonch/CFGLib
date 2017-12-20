using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Sppf {
	internal struct SppfFamily {
		private readonly int _cachedHash;
		private readonly SppfNode _firstChild;
		private readonly SppfNode _secondChild;
		public Production Production { get; }

		// TODO: remove children interface entirely
		// private List<SppfNode2> _children;
		public IList<SppfNode> Members {
			get {
				var _children = new List<SppfNode>();
				if (_firstChild != null) {
					_children.Add(_firstChild);
					if (_secondChild != null) {
						_children.Add(_secondChild);
					}
				}
				return _children;
			}
		}

		//public Family2(Production production) {
		//	_cachedHash = 0;
		//	// _firstChild = ;
		//	_secondChild = null;
		//	Production = production;
		//}
		public SppfFamily(Production production, SppfNode v) {
			// Children = new List<SppfNode2> { v };
			_cachedHash = v.GetHashCode();
			_firstChild = v;
			_secondChild = null;
			Production = production;
		}
		public SppfFamily(Production production, SppfNode w, SppfNode v) {
			//Children = new List<SppfNode2> { w, v };
			_cachedHash = unchecked((17 * 23 + w.GetHashCode()) * 23 + v.GetHashCode());
			_firstChild = w;
			_secondChild = v;
			Production = production;
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
			if (!(other is SppfFamily localOther)) {
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
			if (this.Members.Count == 0) {
				return string.Format("ε");
			}
			return string.Format(string.Join(" | ", this.Members));
		}
	}
}
