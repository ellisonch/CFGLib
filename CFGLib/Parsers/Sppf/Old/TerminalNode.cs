using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Sppf.Old {
	internal class TerminalNode : LeafNode {
		private readonly Terminal _terminal;
		//public TerminalNode(Terminal terminal) {
		//	_terminal = terminal;
		//}
		public Terminal Terminal {
			get { return _terminal; }
		}

		public TerminalNode(Terminal terminal, int start, int end) : base(start, end) {
			_terminal = terminal;
		}

		public override Sentence GetSentence() {
			return new Sentence { _terminal };
		}

		public override bool Equals(object other) {
			var x = this;
			var y = other as TerminalNode;
			if (y == null) {
				return false;
			}

			if (x.StartPosition != y.StartPosition) {
				return false;
			}
			if (x.Terminal != y.Terminal) {
				return false;
			}

			return true;
		}

		// based on http://stackoverflow.com/a/263416/2877032
		public override int GetHashCode() {
			unchecked {
				int hash = 17;
				hash = hash * 23 + this.StartPosition.GetHashCode();
				hash = hash * 23 + this.Terminal.GetHashCode();

				return hash;
			}
		}
	}
}
