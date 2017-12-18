using CFGLib.Parsers.Forests;
using CFGLib.Parsers.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Sppf {
	internal abstract class SppfNode {
		private static int _nextId = 0;

		public readonly int StartPosition;
		public readonly int EndPosition;
		public readonly int Id = _nextId++;
		private readonly HashSet<Family2<SppfNode>> _families = new HashSet<Family2<SppfNode>>();

		// An ordered version of the hash
		internal IEnumerable<Family2<SppfNode>> Families { get { return _families; } }

		protected SppfNode(int startPosition, int endPosition) {
			StartPosition = startPosition;
			EndPosition = endPosition;
		}
		
		// internal abstract void FinishFamily();

		internal abstract string ToStringSimple();

		internal abstract void GetGraphHelper(Graph g, SppfNodeNode myNode, HashSet<InteriorNode> visited);

		internal void AddFamily(Family2<SppfNode> family) {
			_families.Add(family);
		}
	}
}
