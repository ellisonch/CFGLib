using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {
	public class SppfWord : SppfNode2 {
		private readonly int _cachedHash;

		public Word Word { get; }

		public SppfWord(Word word, int startPos, int endPos) : base(startPos, endPos) {
			Word = word;
			_cachedHash = MyGetHashCode();
		}

		public static bool operator ==(SppfWord x, SppfWord y) {
			if (ReferenceEquals(x, null)) {
				return ReferenceEquals(y, null);
			}
			return x.Equals(y);
		}
		public static bool operator !=(SppfWord x, SppfWord y) {
			return !(x == y);
		}
		public override bool Equals(object other) {
			var x = this;
			var y = other as SppfWord;
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

			return true;
		}

		public override int GetHashCode() {
			return _cachedHash;
		}

		// based on http://stackoverflow.com/a/263416/2877032
		private int MyGetHashCode() {
			unchecked {
				int hash = 17;
				hash = hash * 23 + this.StartPosition.GetHashCode();
				hash = hash * 23 + this.EndPosition.GetHashCode();
				hash = hash * 23 + this.Word.GetHashCode();

				return hash;
			}
		}

		public override string ToString() {
			var firstBit = Word.ToString();
			return string.Format("{0} {1} {2}", firstBit, StartPosition, EndPosition);
		}
	}
}
