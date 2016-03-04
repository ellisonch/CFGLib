using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CFGLib.Parsers.Forests;
using CFGLib.Parsers.Graphs;

namespace CFGLib.Parsers.Forests {
	internal abstract class LeafNode : SppfNode {
		protected LeafNode(int start, int end) : base(start, end) {
		}

		public abstract Sentence GetSentence();
		
		private static IList<Family> _families = new List<Family>().AsReadOnly();
		internal override IList<Family> Families {
			get {
				return _families;
			}
		}
		internal override void FinishFamily() {
		}

		public override string ToString() {
			return string.Format("Leaf({0}, {1}, {2})", GetSentence().ToString(), StartPosition, EndPosition);
		}
		internal override string ToStringSimple() {
			return string.Format("{0} ({1}, {2})", GetSentence().ToString(), StartPosition, EndPosition);
		}

		internal override void GetGraphHelper(Graph g, SppfNodeNode myNode, HashSet<InteriorNode> visited) {
			return;
		}
	}
}
