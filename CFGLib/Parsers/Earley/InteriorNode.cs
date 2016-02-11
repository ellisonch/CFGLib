using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {
	internal abstract class InteriorNode {
		public int StartPosition;
		public int EndPosition;
		public HashSet<Family> Families;

		protected InteriorNode(int startPosition, int endPosition) {
			StartPosition = startPosition;
			EndPosition = endPosition;
			// Family = new HashSet<InteriorNode>(new NodeComparer());
			Families = new HashSet<Family>();
		}

		internal void AddFamily(Family family) {
			Families.Add(family);
		}

		//internal abstract bool ValueEquals(InteriorNode other);
		//internal abstract int GetValueHashCode();
	}

	//internal class NodeComparer : IEqualityComparer<InteriorNode> {
	//	public bool Equals(InteriorNode left, InteriorNode right) {
	//		return left.ValueEquals(right);
	//	}

	//	public int GetHashCode(InteriorNode item) {
	//		return item.GetValueHashCode();
	//		// return new { item.Item, item.ItemOffset, item.SentenceOffset }.GetHashCode();
	//	}
	//}
}
