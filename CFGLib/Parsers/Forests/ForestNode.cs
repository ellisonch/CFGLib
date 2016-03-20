using CFGLib.Parsers.Forests.ForestVisitors;
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

		internal abstract string ToStringSelf();
		
		internal abstract bool Accept(IForestVisitor visitor);
	}
}
