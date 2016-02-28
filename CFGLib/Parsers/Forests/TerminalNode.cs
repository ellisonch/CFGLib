using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Forests {
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
	}
}
