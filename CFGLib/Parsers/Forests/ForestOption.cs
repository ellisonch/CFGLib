using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Forests {
	public class ForestOption {
		// private readonly Production _production;
		private readonly Family _family;
		private List<ForestNode[]> _children;

		//private readonly List<ForestLeaf> _leafChildren = new List<ForestLeaf>();
		//private readonly List<ForestInternal> _internalChildren = new List<ForestInternal>();

		// private readonly List<ForestNode> _options;
		// private readonly Family _family;

		internal static List<ForestOption> BuildOptions(IList<Family> families) {
			var retval = new List<ForestOption>();

			foreach (var family in families) {
				// BuildOptionsStep(retval, family);
				retval.Add(new ForestOption(family));
			}

			return retval;
		}

		internal ForestOption(Family family) {
			_family = family;
			//_production = family.Production;

			//// int count = 0;

			//// var productionStack = new Stack<Word>(_production.Rhs);
			//var nodeStack = new Stack<SppfNode>(family.Members);

			//while (nodeStack.Count > 0) {
			//	// var word = productionStack.Pop();
			//	var child = nodeStack.Pop();

			//	if (child is EpsilonNode) {
			//		// do nothing
			//	} else if (child is TerminalNode) {
			//		var terminalChild = (TerminalNode)child;
			//		_leafChildren.Add(new ForestLeaf(terminalChild));
			//	} else if (child is SymbolNode) {
			//		var symbolChild = (SymbolNode)child;
			//		_internalChildren.Add(new ForestInternal(symbolChild, symbolChild.Symbol));
			//	} else if (child is IntermediateNode) {
			//		var intermediateChild = (IntermediateNode)child;
			//		// TODO: need to duplicate parent options here
			//		throw new Exception();
			//		// intermediateChild.Families
			//	}
			//}
		}

		private List<ForestNode[]> BuildChildren() {
			var count = _family.Production.Rhs.Count;
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

		private List<ForestNode[]> Children() {
			if (_children == null) {
				_children = BuildChildren();
			}
			return _children;
		}

		internal string ToStringHelper(int level) {
			var retval = "";

			var children = "";
			foreach (var childrenSet in Children()) {
				foreach (var child in childrenSet) {
					children += child.ToStringHelper(level + 1);
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