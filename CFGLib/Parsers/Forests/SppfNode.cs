using CFGLib.Parsers.Forests;
using CFGLib.Parsers.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Forests {
	internal abstract class SppfNode {
		public readonly int StartPosition;
		public readonly int EndPosition;
		public int Id; // added later

		// An ordered version of the hash
		internal abstract IList<Family> Families { get; }
		
		protected SppfNode(int startPosition, int endPosition) {
			StartPosition = startPosition;
			EndPosition = endPosition;
		}
		
		internal abstract void FinishFamily();
		
		public string ProductionsToString() {
			string retval = "";

			var childrenStrings = new List<string>();
			var children = "";
			for (int i = 0; i < Families.Count; i++) {
				var family = Families[i];
				var production = family.Production;
				if (production != null) {
					childrenStrings.Add(string.Format("[{0}]={1}", i, production.ToStringNoWeight()));
				}
			}
			if (childrenStrings.Count > 0) {
				children = string.Join(", ", childrenStrings);
				retval += "  |  ";
				retval += children;
			}

			return retval;
		}

		internal abstract string ToStringSimple();

		internal abstract void GetGraphHelper(Graph g, SppfNodeNode myNode, HashSet<InteriorNode> visited);
	}
}
