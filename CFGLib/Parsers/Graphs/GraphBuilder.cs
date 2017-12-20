using CFGLib.Parsers.Sppf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Graphs {
	internal static class GraphBuilder {
		public static Graph GetGraph(SppfNode2 node) {
			var graphNode = new SppfNodeNode(node, 0);
			var g = new Graph(graphNode);
			GetGraphHelper(g, node, graphNode, new HashSet<SppfNode2>());
			return g;
		}

		// internal void GetGraphHelper(Graph g, SppfNodeNode myNode, HashSet<InteriorNode> visited);

		private static void GetGraphHelper(Graph g, SppfNode2 node, SppfNodeNode myNode, HashSet<SppfNode2> visited) {
			//if (node is InteriorNode interiorNode) {
			//	GetGraphInterior(g, interiorNode, myNode, visited);
			//} else if (node is LeafNode) {
			//	// do nothing
			//} else {
			//	throw new Exception();
			//}
			GetGraphInterior(g, node, myNode, visited);
		}

		internal static void GetGraphInterior(Graph g, SppfNode2 node, SppfNodeNode myNode, HashSet<SppfNode2> visited) {
			if (visited.Contains(node)) {
				return;
			}
			visited.Add(node);

			var i = 0;
			foreach (var family in node.Families) {
				// for (int i = 0; i < Families.Count; i++) {
				// var family = Families[i];
				//Production singletonProduction = null;
				INode prevNode;
				if (node.Families.Count() == 1) {
					prevNode = myNode;
					//singletonProduction = Families[0].Production;
				} else {
					prevNode = new FamilyNode(family, myNode.Node.Id + "-" + i, myNode.Rank + 1);
					// g.AddEdge(myNode, prevNode, family.Production);
					g.AddEdge(myNode, prevNode);
				}
				prevNode.TheFamily = family;
				foreach (var child in family.Members) {
					var childNode = new SppfNodeNode(child, prevNode.Rank + 1);
					// var childNode = g.GetNode(child, prevNode.Rank + 1);
					// g.AddEdge(prevNode, childNode, singletonProduction);
					g.AddEdge(prevNode, childNode);
					GetGraphHelper(g, child, childNode, visited);
				}
				i++;
			}
		}
	}
}
