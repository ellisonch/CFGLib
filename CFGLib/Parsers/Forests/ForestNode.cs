using CFGLib.Parsers.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Forests {
	public abstract class ForestNode {
		public readonly int StartPosition;
		public readonly int EndPosition;
		
		public ForestNode(int startPosition, int endPosition) {
			StartPosition = startPosition;
			EndPosition = endPosition;
		}

		internal abstract string ToStringHelper(int level, HashSet<InteriorNode> visited);
		internal abstract void GetGraphHelper(Graph g, ForestNodeNode myNode, HashSet<InteriorNode> visited, Dictionary<InteriorNode, int> ids, ref int id, bool share = false);

		internal abstract string ToStringSelf();
	}
}
