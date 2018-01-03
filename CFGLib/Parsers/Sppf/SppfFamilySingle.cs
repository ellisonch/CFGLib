using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Sppf {
	internal class SppfFamilySingle : SppfFamily {
		private readonly SppfNode _firstChild;
		public override IList<SppfNode> Members {
			get {
				return new List<SppfNode> { _firstChild };
			}
		}
		public SppfFamilySingle(Production production, SppfNode v) : base(production) {
			_firstChild = v;
		}

		public override int GetHashCode() {
			return _firstChild.GetHashCode();
		}

		public override bool Equals(Object other) {
			if (other == null) {
				return false;
			}
			if (!(other is SppfFamilySingle localOther)) {
				return false;
			}
			
			return this._firstChild == localOther._firstChild;
		}
	}
}
