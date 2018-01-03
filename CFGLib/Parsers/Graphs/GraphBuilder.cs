using CFGLib.Parsers.Sppf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Graphs {
	internal class GraphBuilder {
		private readonly SppfNode _root;
		private readonly Dictionary<SppfNode, int> _nodeIdDict = new Dictionary<SppfNode, int>();
		private int _nextId = 0;
		private readonly Graph _g;
		private readonly HashSet<SppfNode> _visited = new HashSet<SppfNode>();

		public GraphBuilder(SppfNode root) {
			_root = root;
			var graphNode = new SppfNodeNode(root, 0, GetOrSetId(root));
			_g = new Graph(graphNode);
			GetGraphHelper(root, graphNode);
		}
		public Graph GetGraph() {
			return _g;
		}
		
		internal void GetGraphHelper(SppfNode node, SppfNodeNode myNode) {
			if (_visited.Contains(node)) {
				return;
			}
			_visited.Add(node);

			var i = 0;
			foreach (var family in node.Families) {
				// for (int i = 0; i < Families.Count; i++) {
				// var family = Families[i];
				Production lowerProduction = null;
				IGraphNode prevNode;
				if (node.Families.Count() == 1) {
					prevNode = myNode;
					lowerProduction = node.Families.Single().Production;
				} else {
					var id = GetOrSetId(node);
					prevNode = new FamilyNode(family, id + "-" + i, myNode.Rank + 1);
					_g.AddEdge(myNode, prevNode, family.Production);
					//g.AddEdge(myNode, prevNode);
				}
				prevNode.TheFamily = family;
				foreach (var child in family.Members) {
					var id = GetOrSetId(child);
					var childNode = new SppfNodeNode(child, prevNode.Rank + 1, id);
					// var childNode = g.GetNode(child, prevNode.Rank + 1);
					// g.AddEdge(prevNode, childNode, singletonProduction);
					// g.AddEdge(prevNode, childNode, singletonProduction);
					_g.AddEdge(prevNode, childNode, lowerProduction);
					GetGraphHelper(child, childNode);
				}
				i++;
			}
		}

		private int GetOrSetId(SppfNode node) {
			if (!_nodeIdDict.TryGetValue(node, out var id)) {
				id = _nextId;
				_nextId++;
				_nodeIdDict[node] = id;
			}
			return id;
		}
	}
}
