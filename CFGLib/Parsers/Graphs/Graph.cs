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

		internal string ToDot() {
			var retval = "";
			retval += "digraph G {\n";

//			retval += @"
//subgraph cluster_key {
//  rank=min;
//  label=""Key"";
//  rankdir=LR;
//  nodesep=0.02;

//  k1[shape=plaintext, style=solid, label=""Nonterminal Node\r""]
//  k2[shape=plaintext, style=solid, label=""Apply Production\r""]
//  k3[shape=plaintext, style=solid, label=""Children Node\r""]
//  k4[shape=plaintext, style=solid, label=""Terminal Node\r""]

//  kc1[label=""<S> (2, 3)""];
//  kc2pre[label=""""];
//  kc2post[label="""" shape=box];
//  kc3[label=""<S> '+' <S> (0, 3)"" shape=box];
//  kc4[label=""'+' (0, 1)"" style=filled fillcolor=yellow];

//  {
//    rank=same;
//    k1 kc1
//  }
//  {
//    rank=same;
//    k2 kc2pre kc2post
//  }
//  {
//    rank=same;
//    k3 kc3
//  }
//  {
//    rank=same;
//    k4 kc4
//  }

//  k1->k2->k3->k4[style=invis];

//  kc2pre->kc2post[label=""<S> → '+'""]

//  k1->kc1[style=invis, minlen=0.25, len=0.25];
//  k2->kc2pre[style=invis];
//  k3->kc3[style=invis];
//  k4->kc4[style=invis];
//}
//";

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
				retval += string.Format("\"{0}\" [{4} shape={2} style=filled fillcolor={3} label=\"{1}\"];\n", node.Name, node.Label, node.Shape, node.Color, node.Ordering);
			}
			foreach (var edge in Edges) {
				retval += string.Format("\"{0}\" -> \"{1}\" [label=\"{2}\"]\n", edge.Left.Name, edge.Right.Name, edge.Label?.ToStringNoWeight());
			}
			retval += "}\n";
			return retval;
		}

		internal bool AddEdge(INode node1, INode node2, Production label = null) {
			// var node1 = new NodeNode(forestInternal);
			// var node2 = new NodeNode(option);
			var newEdge = new Edge(node1, node2, label);
			Nodes.Add(node1);
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
