using CFGLib.Parsers.Forests.ForestVisitors;
using CFGLib.Parsers.Graphs;
using CFGLib.Parsers.Sppf;
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

		internal InteriorNode InternalNode {
			get {
				return _node;
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

		internal override bool Accept(IForestVisitor visitor) {
			return visitor.Visit(this);
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
			var graph = GraphBuilder.GetGraph(_node);
			// var graph = ((SymbolNode)_node).GetGraph();
			return graph.ToDot();
		}

		public string ToDot() {
			// var graph = GetGraph();
			var gv = new ForestVisitors.GraphVisitors.GraphVisitor(this);
			var graph = gv.Graph();
			return graph.ToDot();
		}


	}
}
