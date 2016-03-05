using CFGLib.Parsers.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Forests {
	public class ForestInternal : ForestNode {
		private readonly InteriorNode _node;
		private readonly Nonterminal _nonterminal;
		//private readonly Dictionary<InteriorNode, ForestInternal> _nodeLookup;
		private readonly List<ForestOption> _options = new List<ForestOption>();
		
		public List<ForestOption> Options {
			get {
				return _options;
			}
		}

		public Nonterminal Nonterminal{
			get {
				return _nonterminal;
			}
		}

		internal ForestInternal(InteriorNode node, Nonterminal nonterminal) : base(node.StartPosition, node.EndPosition) {
			_node = node;
			_nonterminal = nonterminal;
			//_nodeLookup = new Dictionary<InteriorNode, ForestInternal>();
			//_nodeLookup[node] = this;

			_options = ForestOption.BuildOptions(node.Families, node.StartPosition, node.EndPosition);
		}

		public override string ToString() {
			return ToStringHelper(0, new HashSet<InteriorNode>());
		}
		internal override string ToStringSelf() {
			return string.Format("{0} ({1}, {2})", Nonterminal, StartPosition, EndPosition);
		}
		internal override string ToStringHelper(int level, HashSet<InteriorNode> visited) {
			var retval = "";

			retval += string.Format("{0}\n", ToStringSelf()).Indent(2 * level);

			if (visited.Contains(_node)) {
				retval += "Already Visited".Indent(2 * level);
				return retval;
			}
			visited.Add(_node);
			// foreach (var option in Options) {
			for (var i = 0; i < Options.Count; i++) {
				var option = Options[i];
				if (Options.Count > 1) {
					retval += string.Format("Alternative {0}:\n", i).Indent(2 * level);
				}
				retval += option.ToStringHelper(level + 1, visited);
			}
			
			//int leafIndex = 0;
			//int symbolIndex = 0;
			//for (int i = 0; i < _localSentence.Count; i++) {
			//	var word = _localSentence[i];
			//	if (word.IsTerminal) {
			//		var leaf = LeafChildren[leafIndex];
			//		leafIndex++;
			//		retval += leaf.ToStringHelper(level);
			//	} else {
			//		var symbol = SymbolChildren[symbolIndex];
			//		symbolIndex++;
			//		retval += symbol.ToStringHelper(level);
			//	}
			//}

			return retval;
		}

		public string GetRawDot() {
			var graph = ((SymbolNode)_node).GetGraph();
			return graph.ToDot();
		}

		public string ToDot() {
			var graph = GetGraph();
			return graph.ToDot();
		}

		private Graph GetGraph() {
			int id = 0;
			var myNode = new ForestNodeNode(this, "" + id++, 0);
			var g = new Graph(myNode);
			GetGraphHelper(g, myNode, new HashSet<InteriorNode>(), new Dictionary<InteriorNode, int> { { _node, 0 } }, ref id);
			return g;
		}
		internal override void GetGraphHelper(Graph g, ForestNodeNode myNode, HashSet<InteriorNode> visited, Dictionary<InteriorNode, int> ids, ref int id) {
			if (visited.Contains(_node)) {
				return;
			}
			visited.Add(_node);
			//if (!ids.ContainsKey(_node)) {
			//	ids[_node] = myNode.Id;
			//}

			for (int i = 0; i < _options.Count; i++) {
				var option = _options[i];
				string optionId = ids[_node] + "-" + i;
				var optionNode = new ChildNode(option.Production.Rhs, this.StartPosition, this.EndPosition, optionId, myNode.Rank + 1);

				g.AddEdge(myNode, optionNode, option.Production);
				foreach (var children in option.Children()) {
					foreach (var child in children) {
						int childSeenId;
						if (child is ForestLeaf) {
							childSeenId = id++;
						} else {
							var internalChild = (ForestInternal)child;
							if (!ids.TryGetValue(internalChild._node, out childSeenId)) {
								childSeenId = id++;
								ids[internalChild._node] = childSeenId;
							}
						}
						
						string childId = "" + childSeenId;
						var childNode = new ForestNodeNode(child, childId, optionNode.Rank + 1);
						g.AddEdge(optionNode, childNode);
						child.GetGraphHelper(g, childNode, visited, ids, ref id);	
					}
				}
			}
		}

	}
}
