using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Sppf {
	internal abstract class SppfFamily {
		public Production Production { get; }

		// TODO: remove children interface entirely
		public abstract IList<SppfNode> Members {
			get;
		}
		public SppfFamily(Production production) {
			Production = production;
		}

		public override string ToString() {
			if (this.Members.Count == 0) {
				return string.Format("ε");
			}
			return string.Format(string.Join(" | ", this.Members));
		}
	}
}
