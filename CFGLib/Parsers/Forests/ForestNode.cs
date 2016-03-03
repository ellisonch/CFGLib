using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Forests {
	public abstract class ForestNode {
		public readonly int StartPosition;
		public readonly int EndPosition;

		public abstract int Id {
			get;
		}

		public ForestNode(int startPosition, int endPosition) {
			StartPosition = startPosition;
			EndPosition = endPosition;
		}

		internal abstract string ToStringHelper(int level);
		internal abstract void GetGraphHelper(Graph g, NodeNode myNode, HashSet<ForestNode> visited, Dictionary<InteriorNode, int> ids, ref int id, bool share = false);

		internal abstract string ToStringSelf();
	}
}
