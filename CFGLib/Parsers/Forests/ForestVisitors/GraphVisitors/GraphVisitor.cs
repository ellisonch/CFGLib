using CFGLib.Parsers.Graphs;
using CFGLib.Parsers.Sppf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Forests.ForestVisitors.GraphVisitors {
	public class GraphVisitor : DefaultVisitor {
		private Graph _graph;
		private IGraphNode _currentNode;
		private int _id = 0;
		private HashSet<SppfNode> _visited = new HashSet<SppfNode>();
		private Dictionary<SppfNode, int> _ids = new Dictionary<SppfNode, int>();

		public GraphVisitor(ForestInternal root) : base(root) {
		}
		internal Graph Graph() {
			if (_graph == null) {
				//int id = 0;
				_currentNode = new ForestNodeNode(Root, "" + _id++, 0);
				_ids[Root.InternalNode] = 0;
				_graph = new Graph(_currentNode);
				//GetGraphHelper(g, myNode, new HashSet<InteriorNode>(), new Dictionary<InteriorNode, int> { { _node, 0 } }, ref id);
				Root.Accept(this);
			}

			return _graph;
		}

		public override bool Visit(ForestInternal node) {
			if (_visited.Contains(node.InternalNode)) {
				return true;
			}
			_visited.Add(node.InternalNode);

			var currentNode = _currentNode;

			for (int i = 0; i < node.Options.Count; i++) {
				var option = node.Options[i];
				string optionId = _ids[node.InternalNode] + "-" + i;
				var optionNode = new ChildNode(option.Production.Rhs, node.StartPosition, node.EndPosition, optionId, currentNode.Rank + 1);

				_graph.AddEdge(currentNode, optionNode, option.Production);
				foreach (var children in option.Children()) {
					foreach (var child in children) {
						int childSeenId;
						if (child is ForestLeaf) {
							childSeenId = _id++;
						} else {
							var internalChild = (ForestInternal)child;
							if (!_ids.TryGetValue(internalChild.InternalNode, out childSeenId)) {
								childSeenId = _id++;
								_ids[internalChild.InternalNode] = childSeenId;
							}
						}

						string childId = "" + childSeenId;
						var childNode = new ForestNodeNode(child, childId, optionNode.Rank + 1);
						_graph.AddEdge(optionNode, childNode);
						_currentNode = childNode;
						child.Accept(this);
						// child.GetGraphHelper(g, childNode, visited, ids, ref id);
					}
				}
			}
			return true;
		}
		

		/*
		
		internal override void GetGraphHelper(Graph g, ForestNodeNode myNode, HashSet<InteriorNode> visited, Dictionary<InteriorNode, int> ids, ref int id) {
			
		}
	*/

	}
}
