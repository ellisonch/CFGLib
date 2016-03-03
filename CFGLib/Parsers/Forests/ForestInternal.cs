using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Forests {
	public class ForestInternal : ForestNode {
		private readonly InteriorNode _node;
		private readonly Nonterminal _nonterminal;

		public override int Id {
			get {
				return _node.Id;
			}
		}

		//private readonly List<ForestLeaf> _leafChildren;
		private readonly List<ForestOption> _options = new List<ForestOption>();

		//public List<ForestLeaf> LeafChildren {
		//	get {
		//		return _leafChildren;
		//	}
		//}
		//public List<ForestOptions> SymbolChildren {
		//	get {
		//		return _symbolChildren;
		//	}
		//}
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

			_options = ForestOption.BuildOptions(node.Families, node.StartPosition, node.EndPosition);
		}

		public override string ToString() {
			return ToStringHelper(0);
		}
		internal override string ToStringSelf() {
			return string.Format("{0} ({1}, {2})", Nonterminal, StartPosition, EndPosition);
		}
		internal override string ToStringHelper(int level) {

			var retval = "";

			retval += string.Format("{0}\n", ToStringSelf()).Indent(2 * level);
			// foreach (var option in Options) {
			for (var i = 0; i < Options.Count; i++) {
				var option = Options[i];
				if (Options.Count > 1) {
					retval += string.Format("Alternative {0}:\n", i).Indent(2 * level);
				}
				retval += option.ToStringHelper(level + 1);
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

		public string ToDot(bool share = false) {
			var graph = GetGraph(share);
			return graph.ToDot();
		}

		private Graph GetGraph(bool share = false) {
			var g = new Graph();
			int id = 0;
			var myNode = new NodeNode(this, "" + id++);
			GetGraphHelper(g, myNode, new HashSet<ForestNode>(), new Dictionary<InteriorNode, int>(), ref id, share);
			return g;
		}
		internal override void GetGraphHelper(Graph g, NodeNode myNode, HashSet<ForestNode> visited, Dictionary<InteriorNode, int> ids, ref int id, bool share = false) {
			if (visited.Contains(this)) {
				return;
			}
			if (!ids.ContainsKey(_node)) {
				ids[_node] = id++;
			}
			// var myNode = new NodeNode(this, id++);
			// seen.Add(this);

			//g.Add(this);
			//bool changes = false;
			// foreach (var option in _options) {
			// var myNode = new NodeNode(this, seen.Count);
			for (int i = 0; i < _options.Count; i++) {
				var option = _options[i];
				string optionId;
				if (share) {
					optionId = ids[_node] + "-" + i;
				} else {
					optionId = "" + id++;
				}
				var optionNode = new ChildNode(option.Production.Rhs, this.StartPosition, this.EndPosition, optionId);

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
						
						string childId;
						if (share) {
							childId = "" + childSeenId;
						} else {
							childId = "" + id++;
						}
						var childNode = new NodeNode(child, childId);
						g.AddEdge(optionNode, childNode);
						child.GetGraphHelper(g, childNode, visited, ids, ref id, share);
					}
				}
			}
			//if (changes) {
			//	for (int i = 0; i < _options.Count; i++) {
			//		var option = _options[i];
			//		foreach (var children in option.Children()) {
			//			foreach (var child in children) {
			//				child.GetGraphHelper(g, seen);
			//			}
			//		}
			//	}
			//}
		}

	}
}
