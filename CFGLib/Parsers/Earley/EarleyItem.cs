using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {
	internal class EarleyItem {
		public DecoratedProduction DecoratedProduction { get; }
		public int StartPosition { get; }
		public SppfNode2 SppfNode { get; internal set; }

		public EarleyItem(DecoratedProduction decoratedProduction, int startPosition, SppfNode2 sppfNode) {
			if (decoratedProduction == null) {
				throw new ArgumentNullException();
			}
			DecoratedProduction = decoratedProduction;
			StartPosition = startPosition;
			SppfNode = sppfNode;
		}

		public Word NextWord {
			get {
				return DecoratedProduction.NextWord;
			}
		}

		public Sentence Tail {
			get {
				return DecoratedProduction.Tail;
			}
		}

		public static bool operator ==(EarleyItem x, EarleyItem y) {
			if (ReferenceEquals(x, null)) {
				return ReferenceEquals(y, null);
			}
			return x.Equals(y);
		}
		public static bool operator !=(EarleyItem x, EarleyItem y) {
			return !(x == y);
		}
		public override bool Equals(object other) {
			var x = this;
			var y = other as EarleyItem;
			if (ReferenceEquals(y, null)) {
				return false;
			}

			if (x.StartPosition != y.StartPosition) {
				return false;
			}

			if (x.DecoratedProduction != y.DecoratedProduction) {
				return false;
			}
			if (x.SppfNode != y.SppfNode) {
				return false;
			}

			return true;
		}

		// based on http://stackoverflow.com/a/263416/2877032
		public override int GetHashCode() {
			unchecked {
				int hash = 17;
				hash = hash * 23 + this.DecoratedProduction.GetHashCode();
				hash = hash * 23 + this.StartPosition.GetHashCode();

				// TODO: because SppfNode has to change, we can't use it in the hash easily
				hash = hash * 23 + (this.SppfNode == null ? 0 : this.SppfNode.GetHashCode());
				
				return hash;
			}
		}

		public override string ToString() {
			return string.Format("{0} ({1})", DecoratedProduction, StartPosition);
		}
	}
}
