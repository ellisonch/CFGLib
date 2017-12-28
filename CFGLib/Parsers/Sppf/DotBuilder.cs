using CFGLib.Parsers.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Sppf {
	public static class DotBuilder {
		public static string GetRawDot(SppfNode node) {
			var graph = GraphBuilder.GetGraph(node);
			// var graph = ((SymbolNode)_node).GetGraph();
			return graph.ToDot();
		}
	}
}
