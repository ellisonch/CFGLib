using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Forests {
	internal struct Edge {
		public readonly INode Left;
		public readonly INode Right;

		public Production Label;

		public Edge(INode left, INode right, Production label) {
			Left = left;
			Right = right;
			Label = label;
		}

	}
	internal interface INode {
		string Label { get; }
		string Name { get; }
	}
	internal struct NodeNode : INode {
		public readonly ForestNode Node;
		public readonly int Id;

		public string Name {
			get {
				// return string.Format("\"{0} {1}\"", Node.ToStringSelf(), Id);
				return string.Format("{0}", Id);
				// return string.Format("{0}", &Node);
			}
		}
		public string Label {
			get {
				// return string.Format("{0} {1}", Node.ToStringSelf(), Id);
				return string.Format("{0}", Node.ToStringSelf());
			}
		}
		public NodeNode(ForestNode node, int id) {
			Node = node;
			Id = id;
		}
	}
	internal struct ChildNode : INode {
		public readonly Sentence Sentence;
		public readonly int StartPosition;
		public readonly int EndPosition;
		public readonly int Id;

		public string Name {
			get {
				// return string.Format("\"{0} ({1}, {2}) {3}\"", Sentence, StartPosition, EndPosition, Id);
				return string.Format("{0}", Id);
			}
		}
		public string Label {
			get {
				// return string.Format("{0} ({1}, {2}) {3}", Sentence, StartPosition, EndPosition, Id);
				return string.Format("{0} ({1}, {2})", Sentence, StartPosition, EndPosition);
			}
		}
		public ChildNode(Sentence rhs, int startPosition, int endPosition, int id) : this() {
			Sentence = rhs;
			StartPosition = startPosition;
			EndPosition = endPosition;
			Id = id;
		}
	}
	internal class Graph {
		private Dictionary<INode, int> dict = new Dictionary<INode, int>();
		public HashSet<INode> Nodes = new HashSet<INode>();
		public HashSet<Edge> Edges = new HashSet<Edge>();

		internal string ToDot() {
			var retval = "";
			retval += "digraph G {\n";
			foreach (var node in Nodes) {
				retval += string.Format("{0} [label=\"{1}\"];\n", node.Name, node.Label);
			}
			foreach (var edge in Edges) {
				retval += string.Format("{0} -> {1} [label=\"{2}\"]\n", edge.Left.Name, edge.Right.Name, edge.Label?.ToStringNoWeight());
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
