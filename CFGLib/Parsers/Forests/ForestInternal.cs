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

		public string ToDot() {
			var graph = GetGraph();
			return graph.ToDot();
		}

		private Graph GetGraph() {
			var g = new Graph();
			int id = 0;
			var myNode = new NodeNode(this, id++);
			GetGraphHelper(g, myNode, new HashSet<ForestNode>(), ref id);
			return g;
		}
		internal override void GetGraphHelper(Graph g, NodeNode myNode, HashSet<ForestNode> seen, ref int id) {
			if (seen.Contains(this)) {
				return;
			}
			// var myNode = new NodeNode(this, id++);
			seen.Add(this);

			//g.Add(this);
			//bool changes = false;
			// foreach (var option in _options) {
			// var myNode = new NodeNode(this, seen.Count);
			for (int i = 0; i < _options.Count; i++) {
				var option = _options[i];
				var optionNode = new ChildNode(option.Production.Rhs, this.StartPosition, this.EndPosition, id++);

				g.AddEdge(myNode, optionNode, option.Production);
				foreach (var children in option.Children()) {
					foreach (var child in children) {
						var childNode = new NodeNode(child, id++);
						g.AddEdge(optionNode, childNode);
						child.GetGraphHelper(g, childNode, seen, ref id);
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
