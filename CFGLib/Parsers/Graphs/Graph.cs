using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Graphs {
	internal class Graph {
		private Dictionary<INode, int> dict = new Dictionary<INode, int>();
		public HashSet<INode> Nodes = new HashSet<INode>();
		public HashSet<Edge> Edges = new HashSet<Edge>();
		private readonly INode _root;
		private readonly Dictionary<string, INode> _nodeLookup = new Dictionary<string, INode>();

		public Graph(INode root) {
			_root = root;
		}

		internal string ToDot() {
			var retval = "";

			var highestRank = 0;
			var rankDict = new Dictionary<int, List<INode>>();

			foreach (var node in Nodes) {
				if (node.Rank > highestRank) {
					highestRank = node.Rank;
				}
				List<INode> list;
				if (!rankDict.TryGetValue(node.Rank, out list)) {
					list = new List<INode>();
					rankDict[node.Rank] = list;
				}
				list.Add(node);
			}

			retval += "digraph G {\n";

			//retval += "splines = true;\n";
			//retval += "sep = \"+25,25\";\n";
			//retval += "overlap=scalexy\n";
			retval += "nodesep=0.6;\n";
			retval += "ranksep = \"0.6 equally\"\n";

			//retval += "rankdir=\"LR\"\n";
			retval += string.Join("->", Enumerable.Range(0, highestRank + 1).Select((i) => string.Format("x{0}", i))) + "[style=invis]\n";
			retval += string.Join("\n", Enumerable.Range(0, highestRank + 1).Select((i) => string.Format("x{0} [style=invis]", i))) + "\n";
			retval += "x00 [style=invis]\n";

			// retval += string.Format("{{rank = source; \"{0}\" x0}}\n", _root.Name);
			retval += string.Format("{{rank = source; x00}}\n");
			retval += string.Format("x00 -> \"{0}\"\n", _root.Name);

			foreach (var pair in rankDict) {
				var rank = pair.Key;
				var list = pair.Value;
				var nodeNames = string.Join(" ", list.Select((n) => "\"" + n.Name + "\""));
				retval += string.Format("{{rank = same; {0} x{1}}}\n", nodeNames, rank);
			}

			// retval += Key();

			// retval += "graph [ordering=out];\n";
			var nodes = Nodes.ToList();
			nodes.Sort((a, b) => {
				if (a.StartPosition < b.StartPosition) {
					return -1;
				} else if (a.StartPosition > b.StartPosition) {
					return 1;
				} else if (a.EndPosition < b.EndPosition) {
					return -1;
				} else if (a.EndPosition > b.EndPosition) {
					return 1;
				} else {
					return 0;
				}
			});
			foreach (var node in nodes) {
				retval += string.Format("\"{0}\" [ordering=out {4} shape={2} style=filled fillcolor={3} label=\"{1}\" {5}];\n", node.Name, node.Label, node.Shape, node.Color, node.Ordering, node.Other);
			}
			foreach (var edge in Edges) {
				retval += string.Format("\"{0}\" -> \"{1}\" [label=\"{2}\"]\n", edge.Left.Name, edge.Right.Name, edge.Label?.ToStringNoWeight());
			}
			retval += "}\n";
			return retval;
		}

		private string Key() {
			var retval = "";
			retval += @"
			subgraph cluster_key {
			rank=min;
			label=""Key"";
			rankdir=LR;
			//nodesep=0.02;

			k1[shape=plaintext, style=solid, label=""Nonterminal Node\rN=Nonterminal\r(x, y) = range of input node describes""]
			k2[shape=plaintext, style=solid, label=""Apply Production\r""]
			k3[shape=plaintext, style=solid, label=""Children Node\r""]
			k4[shape=plaintext, style=solid, label=""Terminal Node\r""]

			kc1[label=""<N> (x, y)""];
			kc2pre[label=""""];
			kc2post[label="""" shape=box];
			kc3[label=""<S> '+' <S> (0, 3)"" shape=box];
			kc4[label=""'+' (0, 1)"" style=filled fillcolor=yellow];

			{
			rank=same;
			k1 kc1
			}
			{
			rank=same;
			k2 kc2pre kc2post
			}
			{
			rank=same;
			k3 kc3
			}
			{
			rank=same;
			k4 kc4
			}

			k1->k2->k3->k4[style=invis];

			kc2pre->kc2post[label=""<S> → '+'""]

			k1->kc1[style=invis, minlen=0.25, len=0.25];
			k2->kc2pre[style=invis];
			k3->kc3[style=invis];
			k4->kc4[style=invis];
			}
			";
			return retval;
		}

		internal bool AddEdge(INode newNode1, INode newNode2, Production label = null) {
			// var node1 = new NodeNode(forestInternal);
			// var node2 = new NodeNode(option);
			var newEdge = new Edge(newNode1, newNode2, label);
			INode node1;
			if (!_nodeLookup.TryGetValue(newNode1.Name, out node1)) {
				node1 = newNode1;
				_nodeLookup[newNode1.Name] = newNode1;
			}
			Nodes.Add(node1);
			INode node2;
			if (!_nodeLookup.TryGetValue(newNode2.Name, out node2)) {
				node2 = newNode2;
				_nodeLookup[newNode2.Name] = newNode2;
			}
			Nodes.Add(node2);
			return Edges.Add(newEdge);
		}
		//internal bool AddEdge(ForestOption thing1, ChildNode node2) {
		//	var node1 = new NodeNode(thing1);
		//	var newEdge = new Edge(node1, node2, null);
		//	Nodes.Add(node1);
		//	Nodes.Add(node2);
		//	return Edges.Add(newEdge);
		//}
		//internal bool AddEdge(ChildNode node1, ForestNode thing2) {
		//	var node2 = new NodeNode(thing2);
		//	var newEdge = new Edge(node1, node2, null);
		//	Nodes.Add(node1);
		//	Nodes.Add(node2);
		//	return Edges.Add(newEdge);
		//}
	}
}
