using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {
	internal class IntermediateNode : InteriorNode {
		public readonly Item Item;

		// these two are used just for figuring out equality
		private readonly Production _production;
		private readonly int _currentPosition;

		public IntermediateNode(Item item, int startPosition, int endPosition) : base(startPosition, endPosition) {
			Item = item;

			// these two are used just for figuring out equality
			_production = item.Production;
			_currentPosition = item.CurrentPosition;
		}

		//public IntermediateNode(Item item) : this(item, item.StartPosition, item.EndPosition) {
		//}

		public override int GetHashCode() {
			return new {
				StartPosition,
				EndPosition,
				_production,
				_currentPosition
			}.GetHashCode();
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
			if (_production != localOther._production) {
				return false;
			}
			if (_currentPosition != localOther._currentPosition) {
				return false;
			}

			return true;
		}

		public override string ToString() {
			return string.Format("({0}, {1}, {2}){3}", Item.ProductionToString(), StartPosition, EndPosition, ProductionsToString());
		}
	}
}
