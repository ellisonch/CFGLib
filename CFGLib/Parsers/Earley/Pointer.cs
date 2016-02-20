using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {
	internal class Pointer {
		public int Label;
		public Item Item;

		public Pointer(int label, Item item) {
			Label = label;
			Item = item;
		}

		public override int GetHashCode() {
			return new {
				Label,
				Item
			}.GetHashCode();
		}

		public override bool Equals(Object other) {
			if (other == null) {
				return false;
			}
			var localOther = other as Pointer;
			if (localOther == null) {
				return false;
			}

			if (Label != localOther.Label) {
				return false;
			}
			if (Item != localOther.Item) {
				return false;
			}

			return true;
		}
	}
}
