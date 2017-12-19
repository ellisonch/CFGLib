using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CFGLib.Parsers.Forests;
using CFGLib.Parsers.Graphs;

namespace CFGLib.Parsers.Sppf {
	internal abstract class LeafNode : SppfNode {
		protected LeafNode(int start, int end) : base(start, end) {
		}
		public abstract Sentence GetSentence();

		public override string ToString() {
			return string.Format("Leaf({0}, {1}, {2})", GetSentence().ToString(), StartPosition, EndPosition);
		}
		internal override string ToStringSimple() {
			return string.Format("{0} ({1}, {2})", GetSentence().ToString(), StartPosition, EndPosition);
		}
	}
}
