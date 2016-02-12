using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {
	internal abstract class InteriorNode : Node {
		public int StartPosition;
		public int EndPosition;

		protected InteriorNode(int startPosition, int endPosition) {
			StartPosition = startPosition;
			EndPosition = endPosition;
		}

		internal void AddFamily(Family family) {
			//foreach (var member in family.Members) {
			//	if (member == this) {
			//		throw new Exception("");
			//	}
			//}
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
