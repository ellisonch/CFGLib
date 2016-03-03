using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Forests {
	public class ForestOption {		
		private readonly Family _family;
		private List<ForestNode[]> _children;

		public Production Production {
			get {
				return _family.Production;
			}
		}

		public List<ForestNode[]> Children() {
			if (_children == null) {
				_children = BuildChildren();
			}
			return _children;
		}
		//private readonly List<ForestLeaf> _leafChildren = new List<ForestLeaf>();
		//private readonly List<ForestInternal> _internalChildren = new List<ForestInternal>();

		// private readonly List<ForestNode> _options;
		// private readonly Family _family;

		internal static List<ForestOption> BuildOptions(IList<Family> families, int startPosition, int endPosition) {
			var retval = new List<ForestOption>();

			foreach (var family in families) {
				// BuildOptionsStep(retval, family);
				retval.Add(new ForestOption(family));
			}

			return retval;
		}

		internal ForestOption(Family family) {
			_family = family;
		}

		private List<ForestNode[]> BuildChildren() {
			var count = _family.Production.Rhs.Count;
			if (count == 0) {
				if (_family.Members.Count != 1) {
					throw new Exception();
				}
				var leaf = new ForestLeaf((EpsilonNode)_family.Members[0]);
				return new List<ForestNode[]> { new ForestNode[1] { leaf } };
			}
			var start = new ForestNode[count];
			var startList = new List<ForestNode[]> { start };
			BuildChildrenHelper(_family, startList, _family.Production.Rhs, count - 1);
			return startList;
		}

		private static void BuildChildrenHelper(Family family, List<ForestNode[]> startList, Sentence rhs, int position) {
			if (position + 1 != rhs.Count && family.Production != null) {
				throw new Exception();
			}
			if (family.Members.Count == 1) {
				if (position != 0) {
					throw new Exception();
				}
				var onlyNode = family.Members[0];
				AddNode(onlyNode, startList, rhs, position);
			} else if (family.Members.Count == 2) {
				var rightNode = family.Members[1];
				AddNode(rightNode, startList, rhs, position);
				var intermediateNode = (IntermediateNode)family.Members[0];
				var firstCopy = startList.ToList();
				startList.Clear();
				foreach (var subfamily in intermediateNode.Families) {
					var listCopy = firstCopy.ToList();
					BuildChildrenHelper(subfamily, listCopy, rhs, position - 1);
					startList.AddRange(listCopy);
				}
			} else {
				throw new Exception();
			}
		}

		//internal void GetGraphHelper(Graph g) {
		//	//g.Add(this);
		//	bool changes = false;
		//	var myNode = new NodeNode(this, 0);
		//	foreach (var children in _children) {
		//		var childNode = new ChildNode(_family.Production.Rhs, StartPosition, EndPosition);
		//		changes |= g.AddEdge(myNode, childNode);
		//		foreach (var child in children) {
		//			var mychildNode = new NodeNode(child, 0);
		//			changes |= g.AddEdge(childNode, mychildNode);
		//		}
		//	}
		//	if (changes) {
		//		foreach (var children in _children) {
		//			foreach (var child in children) {
		//				child.GetGraphHelper(g);
		//			}
		//		}
		//	}
		//}

		private static void AddNode(SppfNode node, List<ForestNode[]> startList, Sentence rhs, int position) {
			ForestNode nodeToAdd;
			if (node is TerminalNode) {
				nodeToAdd = new ForestLeaf((TerminalNode)node);
			} else {
				nodeToAdd = new ForestInternal((SymbolNode)node, (Nonterminal)rhs[position]);
			}
			foreach (var children in startList) {
				children[position] = nodeToAdd;
			}
		}

		//internal override string ToStringSelf() {
		//	if (_family.Production == null) {
		//		return "INTERNAL OPTION";
		//	}
		//	return string.Format("{0} ({1}, {2})", _family.Production.Rhs, StartPosition, EndPosition);
		//}
		internal string ToStringHelper(int level, HashSet<InteriorNode> visited) {
			var retval = "";

			var children = "";
			foreach (var childrenSet in Children()) {
				foreach (var child in childrenSet) {
					children += child.ToStringHelper(level + 1, visited);
				}
			}

			// retval += _family.ToString().Indent(2 * level);
			retval += string.Format("Apply {0}\n", _family.Production.ToStringNoWeight()).Indent(2 * level);
			retval += children;

			return retval;
		}

		// public ForestOptions()
	}
}