using CFGLib.Parsers.Sppf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Actioneer {
	public class TraversalLoopException : Exception {
		public readonly SppfNode Node;
		public TraversalLoopException(SppfNode node) {
			Node = node;
		}
	}
}
