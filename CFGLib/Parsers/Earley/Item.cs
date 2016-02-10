using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {
	internal class Item {
		public readonly Production Production;
		public readonly int CurrentPosition; // where the dot is
		public readonly int StartPosition; // where the parse started

		public Word Next {
			get {
				if (CurrentPosition >= Production.Rhs.Count) {
					return null;
				}
				return Production.Rhs[CurrentPosition];
			}
		}

		public Item(Production production, int currentPosition, int startPosition) {
			Production = production;
			CurrentPosition = currentPosition;
			StartPosition = startPosition;
		}
		public override string ToString() {
			var beforeDot = Production.Rhs.GetRange(0, CurrentPosition);
			var beforeDotString = beforeDot.Count == 0 ? "" : beforeDot.ToString();
			var afterDot = Production.Rhs.GetRange(CurrentPosition, Production.Rhs.Count - CurrentPosition);
			var afterDotString = afterDot.Count == 0 ? "" : afterDot.ToString() + " ";
			if (beforeDotString == "" && afterDotString == "") {
				beforeDotString = "ε";
			}

			return string.Format("{0} → {1} o {2}[{3}] ({4})", Production.Lhs, beforeDotString, afterDotString, Production.Weight, StartPosition);
		}

		internal Item Increment() {
			var copy = new Item(Production, CurrentPosition + 1, StartPosition);
			return copy;
		}

		internal bool IsComplete() {
			return this.Next == null;
		}
	}
}
