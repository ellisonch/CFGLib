using CFGLib.Parsers.Forests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {
	internal abstract class Node {
		public readonly HashSet<Family> Families = new HashSet<Family>();
		
		// An ordered version of the hash
		public List<Family> FamiliesList { get; internal set; }

		public Production[] ChildProductions;

		public string ProductionsToString() {
			string retval = "";

			if (ChildProductions != null && ChildProductions.Length > 0) {
				var childrenStrings = new List<string>();
				var children = "";
				for (int i = 0; i < ChildProductions.Length; i++) {
					var child = ChildProductions[i];
					if (child != null) {
						childrenStrings.Add(string.Format("[{0}]={1}", i, child.ToStringNoWeight()));
					}
				}
				if (childrenStrings.Count > 0) {
					children = string.Join(", ", childrenStrings);
					retval += "  |  ";
					retval += children;
				}
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

		internal abstract Sppf ToSppf(Sentence s, Dictionary<Node, Sppf> dict = null);
	}
}
