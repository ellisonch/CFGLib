using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {
	internal class Pointer {
		public readonly int Label;
		public readonly Item Item;

		public Pointer(int label, Item item) {
			Label = label;
			Item = item;
		}

		public override int GetHashCode() {
			//return new {
			//	Label,
			//	Item
			//}.GetHashCode();
			// based on http://stackoverflow.com/a/263416/2877032
			unchecked {
				int hash = 17;
				hash = hash * 23 + Item.GetHashCode();
				hash = hash * 23 + Label;
				return hash;
			}
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
