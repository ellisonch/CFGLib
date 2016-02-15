using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {
	internal abstract class Node {
		public readonly HashSet<Family> Families = new HashSet<Family>();
		private List<Production> _production = new List<Production>();
		public List<Production> Productions {
			get {
				return _production;
			}
		}

		public List<Production>[] ChildProductions;

		public string ProductionsToString() {
			string retval = "";
			if (Productions.Count > 0) {
				retval += "  |  ";

				var prods = "{";
				prods += string.Join(", ", Productions);
				prods += "}";

				retval += prods;
			}

			if (ChildProductions != null && ChildProductions.Length > 0) {
				retval += "  |  ";

				var children = "{";
				for (int i = 0; i < ChildProductions.Length; i++) {
					children += string.Format("[{0}]={{{1}}}", i, string.Join(", ", ChildProductions[i]));
				}
				children += "}";

				retval += children;
			}

			return retval;
		}
	}
}
