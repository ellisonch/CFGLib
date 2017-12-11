using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {
	internal class SppfNode2 {
		private Word Word { get; }
		private DecoratedProduction DecoratedProduction { get; }
		private int StartPosition { get; }
		private int EndPosition { get; }

		internal HashSet<Family2> Families { get; }

		public SppfNode2(Tuple<Word, DecoratedProduction> tuple, int startPos, int endPos) {
			if (startPos > endPos) {
				throw new Exception();
			}
			Word = tuple.Item1;
			DecoratedProduction = tuple.Item2;
			StartPosition = startPos;
			EndPosition = endPos;
			Families = new HashSet<Family2>();
		}
		public SppfNode2(Word word, int startPos, int endPos) : this(Tuple.Create<Word, DecoratedProduction>(word, null), startPos, endPos) {
		}

		public static bool operator ==(SppfNode2 x, SppfNode2 y) {
			if (ReferenceEquals(x, null)) {
				return ReferenceEquals(y, null);
			}
			return x.Equals(y);
		}
		public static bool operator !=(SppfNode2 x, SppfNode2 y) {
			return !(x == y);
		}
		public override bool Equals(object other) {
			var x = this;
			var y = other as SppfNode2;
			if (y == null) {
				return false;
			}

			if (x.StartPosition != y.StartPosition) {
				return false;
			}
			if (x.EndPosition != y.EndPosition) {
				return false;
			}
			if (x.Word != y.Word) {
				return false;
			}
			if (x.DecoratedProduction != y.DecoratedProduction) {
				return false;
			}

			return true;
		}

		// based on http://stackoverflow.com/a/263416/2877032
		public override int GetHashCode() {
			unchecked {
				int hash = 17;
				hash = hash * 23 + this.StartPosition.GetHashCode();
				hash = hash * 23 + this.EndPosition.GetHashCode();
				hash = hash * 23 + (this.Word == null ? 0 : this.Word.GetHashCode());
				hash = hash * 23 + (this.DecoratedProduction == null ? 0 : this.DecoratedProduction.GetHashCode());

				return hash;
			}
		}
		
		internal void AddFamily() {
			var family = new Family2();
			Families.Add(family);
		}
		internal void AddFamily(SppfNode2 v) {
			var family = new Family2(v);
			Families.Add(family);
		}

		internal void AddFamily(SppfNode2 w, SppfNode2 v) {
			var family = new Family2(w, v);
			Families.Add(family);
		}


		public override string ToString() {
			var firstBit = Word?.ToString() ?? DecoratedProduction.ToString();
			return string.Format("{0} {1} {2}", firstBit, StartPosition, EndPosition);
		}
	}
}
