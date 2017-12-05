using CFGLib;
using CFGLib.Parsers.Forests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Actioneer {
	public class Traversal {
		private readonly ForestInternal _forestRoot;
		private readonly InteriorNode _root;
		private readonly Sentence _input;
		private readonly Dictionary<Production, IParserAction> _actions;

		public Traversal(ForestInternal root, Sentence input, Dictionary<Production, IParserAction> actions) {
			_forestRoot = root;
			_root = root.InternalNode;
			_input = input;
			_actions = actions;
		}

		public TraverseResult Traverse() {
			return Traverse(_root, 0);
		}

		private TraverseResult Traverse(SppfNode node, int level) {
			var ni = node as InteriorNode;
			if (ni != null) {
				return TraverseInternal(ni, level);				
			}
			var nt = node as TerminalNode;
			if (nt != null) {
				return TraverseTerminal(nt, level);
			}
			//var nl = node as ForestLeaf;
			//if (nl != null) {
			//	TraverseInternal(nl, level);
			//	return;
			//}

			throw new ArgumentException(string.Format("Unhandled case {0}", node.GetType().Name));
		}

		private TraverseResult TraverseTerminal(TerminalNode nt, int level) {
			// Console.WriteLine(string.Format("Terminal({2}) ({0}, {1})", nt.StartPosition, nt.EndPosition, nt.Terminal).Indent(level));
			return new TraverseResult(nt.Terminal.Name, nt);
		}

		private TraverseResult TraverseInternal(InteriorNode node, int level) {
			var start = node.StartPosition;
			var length = node.EndPosition - node.StartPosition;
			var sub = _input.GetRange(start, length);
			
			// var str = string.Format("Internal(({1}, {2})) {3}", 0, node.StartPosition, node.EndPosition, sub).Indent(level);
			// Console.WriteLine(str);

			// TODO: just picking one
			var family = node.Families[0];
			
			//foreach (var family in node.Families) {
			var args = TraverseFamily(node, family, level + 1);

			// var payload = "(" + string.Join("::", args.Select(arg => arg.Payload)) + ")";
			if (family.Production == null) {
				throw new Exception();
			}
			object payload = null;
			IParserAction action;
			if (_actions.TryGetValue(family.Production, out action)) {
				payload = action.Act(args);
			}

			var result = new TraverseResult(payload, node);
			return result;
				//foreach (var member in family.Members) {

				//}
			//}

			//foreach (var option in node.Options) {
			//	TraverseOption(option, level + 1);
			//}
		}

		private TraverseResult[] TraverseFamily(InteriorNode node, Family family, int level) {
			var count = family.Production.Rhs.Count;
			if (count == 0) {
				if (family.Members.Count != 1) {
					throw new Exception();
				}
				var leaf = (EpsilonNode)family.Members[0];
				return new TraverseResult[] { TraverseEpsilon(leaf, level) };
				// return new List<ForestNode[]> { new ForestNode[1] { leaf } };
			}
			return TraverseChildren(node, family, count, level + 1);
			//var start = new ForestNode[count];
			//var startList = new List<ForestNode[]> { start };
			//BuildChildrenHelper(_family, startList, _family.Production.Rhs, count - 1);
			//return startList;
			// throw new Exception();
		}

		private TraverseResult[] TraverseChildren(InteriorNode node, Family family, int count, int level) {
			// Console.WriteLine("Helping:");
			var start = new TraverseResult[count];
			var startList = new List<TraverseResult[]> { start };
			TraverseChildrenHelper(node, family, startList, family.Production.Rhs, count - 1, level);
			// TODO: just picking one for now
			var actual = startList[0];
			// PrintNodeArray(actual);
			return actual;
		}

		private void TraverseChildrenHelper(InteriorNode node, Family family, List<TraverseResult[]> startList, Sentence rhs, int position, int level) {
			// PrintList(startList);
			if (position + 1 != rhs.Count && family.Production != null) {
				throw new Exception();
			}
			if (family.Members.Count == 1) {
				if (position != 0) {
					throw new Exception();
				}
				var onlyNode = family.Members[0];				
				var result = Traverse(onlyNode, level);
				AddNode(result, startList, rhs, position);
			} else if (family.Members.Count == 2) {
				var rightNode = family.Members[1];
				var result = Traverse(rightNode, level);
				AddNode(result, startList, rhs, position);
				var intermediateNode = (IntermediateNode)family.Members[0];

				// TraverseInternal(intermediateNode, level);
				var firstCopy = startList.ToList();
				startList.Clear();
				foreach (var subfamily in intermediateNode.Families) {
					var listCopy = firstCopy.ToList();
					TraverseChildrenHelper(node, subfamily, listCopy, rhs, position - 1, level);
					startList.AddRange(listCopy);
				}
			} else {
				throw new Exception();
			}
		}

		//private static void PrintList(List<ForestNode[]> startList) {
		//	foreach (var list in startList) {
		//		PrintNodeArray(list);
		//	}
		//}

		//private static void PrintNodeArray(ForestNode[] list) {
		//	var strLists = list.Select(x => x == null ? "---" : String.Format("X({0}, {1})", x.StartPosition, x.EndPosition));
		//	// Console.WriteLine("Partial: {0}", string.Join(", ", strLists));
		//}

		private static void AddNode(TraverseResult result, List<TraverseResult[]> startList, Sentence rhs, int position) {
			//ForestNode nodeToAdd;
			//if (node is TerminalNode) {
			//	nodeToAdd = new ForestLeaf((TerminalNode)node);
			//} else {
			//	nodeToAdd = new ForestInternal((SymbolNode)node, (Nonterminal)rhs[position]);
			//}
			foreach (var children in startList) {
				children[position] = result;
			}
		}

		private TraverseResult TraverseEpsilon(EpsilonNode leaf, int level) {
			// Console.WriteLine("Epsilon".Indent(level));
			return new TraverseResult("", leaf);
		}
		//private void TraverseInternal(ForestLeaf leaf, int level) {
		//	var start = leaf.StartPosition;
		//	var length = leaf.EndPosition - leaf.StartPosition;
		//	var sub = _input.GetRange(start, length);

		//	var str = string.Format("Leaf({0}, {1}) {2}", leaf.StartPosition, leaf.EndPosition, sub).Indent(level);
		//	Console.WriteLine(str);
		//}

		//private void TraverseOption(ForestOption option, int level) {
		//	ParserAction action;
		//	if (!_actions.TryGetValue(option.Production, out action)) {
		//		action = new ParserAction((strList) => "");
		//	}
		//	var str = string.Format("Option({0})", option.Production).Indent(level);
		//	Console.WriteLine(str);
		//	foreach (var childSet in option.Children()) {
		//		if (childSet.Length == 1) {
		//			TraverseChildSingle(childSet[0], level + 1);
		//		} else if (childSet.Length == 2) {
		//			TraverseChildDouble((ForestInternal)childSet[0], childSet[1], level + 1);
		//		} else {
		//			throw new Exception();
		//		}
		//		//foreach (var child in childSet) {
		//		// Traverse(child, level + 1);
		//		//}
		//	}
		//}

		//private void TraverseChildDouble(ForestInternal node1, ForestNode node2, int level) {
		//	Traverse(node1, level + 1);
		//	Traverse(node2, level + 1);
		//}

		//private void TraverseChildSingle(ForestNode node, int level) {
		//	Traverse(node, level + 1);
		//}
	}
}
