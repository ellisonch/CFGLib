using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {
	internal abstract class Node {
		public readonly HashSet<Family> Families = new HashSet<Family>();
		public Production[] ChildProductions;

		public string ProductionsToString() {
			string retval = "";

			if (ChildProductions != null && ChildProductions.Length > 0) {
				retval += "  |  ";

				var childrenStrings = new List<string>();
				var children = "";
				for (int i = 0; i < ChildProductions.Length; i++) {
					childrenStrings.Add(string.Format("[{0}]={1}", i, ChildProductions[i]));
				}
				children = string.Join(", ", childrenStrings);

				retval += children;
			}

			return retval;
		}

		internal void AddChild(int i, Production production) {
			if (ChildProductions[i] != null) {
				if (production != ChildProductions[i]) {
					throw new Exception();
				}
			}
			ChildProductions[i] = production;
		}
	}
}
