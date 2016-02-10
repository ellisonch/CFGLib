using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {
	internal class PredecessorPointer {
		public int Label;
		public Item Item;

		public PredecessorPointer(int label, Item item) {
			Label = label;
			Item = item;
		}
	}
}
