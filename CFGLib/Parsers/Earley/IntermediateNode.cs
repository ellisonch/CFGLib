using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {
	internal class IntermediateNode : InteriorNode {
		public Item Item;

		public IntermediateNode(Item item, int startPosition, int endPosition) : base(startPosition, endPosition) {
			Item = item;
		}

		//public IntermediateNode(Item item) : this(item, item.StartPosition, item.EndPosition) {
		//}

		public override int GetHashCode() {
			return new { StartPosition, EndPosition, Item }.GetHashCode();
		}

		public override bool Equals(Object other) {
			if (other == null) {
				return false;
			}
			var localOther = other as IntermediateNode;
			if (localOther == null) {
				return false;
			}

			if (StartPosition != localOther.StartPosition) {
				return false;
			}
			if (EndPosition != localOther.EndPosition) {
				return false;
			}
			if (Item != localOther.Item) {
				return false;
			}

			return true;
		}

		public override string ToString() {
			return string.Format("({0}, {1}, {2})", Item.ProductionToString(), StartPosition, EndPosition);
		}
	}
}
