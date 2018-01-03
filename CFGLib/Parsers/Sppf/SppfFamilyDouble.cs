using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Sppf {
	internal class SppfFamilyDouble : SppfFamily {
		private readonly SppfNode _firstChild;
		private readonly SppfNode _secondChild;

		public override IList<SppfNode> Members {
			get {
				return new List<SppfNode> { _firstChild, _secondChild };
			}
		}

		public SppfFamilyDouble(Production production, SppfNode w, SppfNode v) : base(production) {
			_firstChild = w;
			_secondChild = v;
		}

		public override int GetHashCode() {
			//return _cachedHash;
			var first = _firstChild.GetHashCode();
			var second = _secondChild.GetHashCode();

			// based on http://stackoverflow.com/a/263416/2877032
			unchecked {
				int hash = 17;
				hash = hash * 23 + first;
				hash = hash * 23 + second;

				return hash;
			}
		}

		public override bool Equals(Object other) {
			if (other == null) {
				return false;
			}
			if (!(other is SppfFamilyDouble localOther)) {
				return false;
			}
			
			return this._firstChild == localOther._firstChild
				&& this._secondChild == localOther._secondChild;
		}
	}
}
