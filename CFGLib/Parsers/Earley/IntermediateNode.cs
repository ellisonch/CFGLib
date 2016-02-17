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

		internal override Sppf ToSppf(Sentence s, Dictionary<Node, Sppf> dict = null) {
			if (Item.CurrentPosition == 0) {
				throw new Exception();
			}
			if (dict == null) {
				dict = new Dictionary<Node, Sppf>();
			}

			List<Children> families = new List<Children>();
			// foreach (var family in this.Families) {
			var familiesList = FamiliesList;
			for (int i = 0; i < familiesList.Count; i++) {
				var family = familiesList[i];
				var sppfList = family.Members.Select((l) => l.ToSppf(s));
				var sppfChildren = new Children(this.ChildProductions[i], sppfList);
				families.Add(sppfChildren);
			}

			return new Sppf(null, s.GetRange(StartPosition, EndPosition - StartPosition), families);
		}

		public override string ToString() {
			return string.Format("({0}, {1}, {2}){3}", Item.ProductionToString(), StartPosition, EndPosition, ProductionsToString());
		}
	}
}
