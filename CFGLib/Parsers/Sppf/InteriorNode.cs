using CFGLib.Parsers.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Sppf {
	internal abstract class InteriorNode : SppfNode {
		protected InteriorNode(int startPosition, int endPosition) : base(startPosition, endPosition) {
		}

		internal override void GetGraphHelper(Graph g, SppfNodeNode myNode, HashSet<InteriorNode> visited) {
			if (visited.Contains(this)) {
				return;
			}
			visited.Add(this);

			var i = 0;
			foreach (var family in Families) {
			// for (int i = 0; i < Families.Count; i++) {
				// var family = Families[i];
				//Production singletonProduction = null;
				INode prevNode;
				if (Families.Count() == 1) {
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
					child.GetGraphHelper(g, childNode, visited);
				}
				i++;
			}
		}
	}
}
