using CFGLib.Parsers.Sppf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.ProductionAnnotations.Actioning {
	public class TraverseResult {
		public object Payload { get; }
		internal SppfNode Node { get; }
		public Production Production { get; }

		public int Start { get { return Node.StartPosition; } }
		public int End { get { return Node.EndPosition; } }

		internal TraverseResult(object payload, SppfNode node, Production production) {
			Payload = payload;
			Node = node;
			Production = production;
		}
	}
}
