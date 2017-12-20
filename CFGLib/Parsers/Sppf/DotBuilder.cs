using CFGLib.Parsers.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Sppf {
	public class DotBuilder {
		public static string GetRawDot(SppfNode2 node) {
			var graph = GraphBuilder.GetGraph(node);
			// var graph = ((SymbolNode)_node).GetGraph();
			return graph.ToDot();
		}
	}
}
