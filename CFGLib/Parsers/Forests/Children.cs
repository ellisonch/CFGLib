using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Forests {
	public class Children {
		public Production Production;
		public Sppf Left;
		public Sppf Right;

		public Children(Production production, IEnumerable<Sppf> children) {
			Production = production;
			if (children.Count() > 2) {
				throw new Exception();
			}
			Left = children.ElementAtOrDefault(0);
			Right = children.ElementAtOrDefault(1);
		}

		internal string ToStringHelper(string padding, HashSet<Sppf> seen) {
			var result = "";
			if (Production != null) {
				result += string.Format("{0}{1}\n", padding, Production);
			}
			var lefts = Left?.ToStringHelper(padding, seen);
			var rights = Right?.ToStringHelper(padding, seen);

			result += string.Format("{0}Left:\n", padding);
			result += lefts;
			result += string.Format("{0}Right:\n", padding);
			result += rights;

			return result;
		}
	}
}
