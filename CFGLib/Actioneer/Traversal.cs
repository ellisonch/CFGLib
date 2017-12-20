using CFGLib;
using CFGLib.Parsers.Forests;
using CFGLib.Parsers.Sppf;
using CFGLib.Parsers.Sppf.Old;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Actioneer {
	public class Traversal {
		private readonly SppfNode2 _root;
		private readonly Sentence _input;
		private readonly GrammarPlus _annotatedGrammar;

		public Traversal(SppfNode2 root, Sentence input, GrammarPlus annotatedGrammar) {
			_root = root;
			_input = input;
			_annotatedGrammar = annotatedGrammar;
		}

		public TraverseResultCollection Traverse() {
			return Traverse(_root, 0);
		}

		private TraverseResultCollection Traverse(SppfNode2 node, int level) {
			//if (node is InteriorNode ni) {
			//	return TraverseInternal(ni, level);
			//}
			//if (node is TerminalNode nt) {
			//	return TraverseTerminal(nt, level);
			//}

			throw new ArgumentException(string.Format("Unhandled case {0}", node.GetType().Name));
		}

		//private TraverseResultCollection TraverseTerminal(TerminalNode nt, int level) {
		//	var resultList = new List<TraverseResult> { new TraverseResult(nt.Terminal.Name, nt, null) };
		//	return new TraverseResultCollection(resultList);
		//}

		//private TraverseResultCollection TraverseInternal(InteriorNode node, int level) {
		//	var start = node.StartPosition;
		//	var length = node.EndPosition - node.StartPosition;
		//	var sub = _input.GetRange(start, length);

		//	var resultList = new List<TraverseResult>();
		//	foreach (var family in node.Families) {
		//		if (family.Production == null) {
		//			throw new Exception();
		//		}
		//		var args = TraverseFamily(node, family, level + 1);
				
		//		foreach (var oneSet in OneOfEach(args)) {
		//			object payload = null;
		//			if (_annotatedGrammar.TryGetValue(family.Production, out ProductionPlus productionPlus)) {
		//				if (!productionPlus.Supports(_annotatedGrammar, oneSet)) {
		//					continue;
		//				}
		//				var action = productionPlus.Action;
		//				payload = action.Act(oneSet);
		//			}

		//			var result = new TraverseResult(payload, node, family.Production);
		//			resultList.Add(result);
		//		}
		//	}
		//	return new TraverseResultCollection(resultList);
		//}
		
		//private IEnumerable<T[]> OneOfEach<T>(IEnumerable<T>[] args) {
		//	var count = args.Length;
		//	var start = new T[count];
		//	var startList = new List<T[]> { start };
		//	for (var position = count - 1; position >= 0; position--) {
		//		startList = OneOfEachAux(args, startList, position);
		//	}
		//	foreach (var option in startList) {
		//		foreach (var place in option) {
		//			if (place == null) {
		//				throw new Exception();
		//			}
		//		}
		//	}
		//	return startList;
		//}

		//private List<T[]> OneOfEachAux<T>(IEnumerable<T>[] args, List<T[]> startList, int position) {
		//	if (position < 0 || position >= args.Length) {
		//		throw new ArgumentException();
		//	}
		//	var newList = new List<T[]>();
		//	foreach (var old in startList) {
		//		foreach (var option in args[position]) {
		//			var newThing = old.ToArray();
		//			newThing[position] = option;
		//			newList.Add(newThing);
		//		}
		//	}

		//	return newList;			
		//}

		//private TraverseResultCollection[] TraverseFamily(InteriorNode node, Family2<SppfNode> family, int level) {
		//	var count = family.Production.Rhs.Count;
		//	if (count == 0) {
		//		if (family.Members.Count != 1) {
		//			throw new Exception();
		//		}
		//		var leaf = (EpsilonNode)family.Members[0];
		//		var resultList = new List<TraverseResult> { TraverseEpsilon(leaf, level) };
		//		return new TraverseResultCollection[] { new TraverseResultCollection(resultList) };
		//	}
		//	return TraverseChildren(node, family, count, level + 1);
		//}

		//private TraverseResultCollection[] TraverseChildren(InteriorNode node, Family2<SppfNode> family, int count, int level) {
		//	var start = new TraverseResultCollection[count];
		//	var startList = new List<TraverseResultCollection[]> { start };
		//	TraverseChildrenHelper(node, family, startList, family.Production.Rhs, count - 1, level);
		//	// TODO: just picking one for now
		//	var actual = startList[0];
		//	return actual;
		//}

		//private void TraverseChildrenHelper(InteriorNode node, Family2<SppfNode> family, List<TraverseResultCollection[]> startList, Sentence rhs, int position, int level) {
		//	if (position + 1 != rhs.Count && family.Production != null) {
		//		throw new Exception();
		//	}
		//	if (family.Members.Count == 1) {
		//		if (position != 0) {
		//			throw new Exception();
		//		}
		//		var onlyNode = family.Members[0];				
		//		var result = Traverse(onlyNode, level);
		//		AddNode(result, startList, rhs, position);
		//	} else if (family.Members.Count == 2) {
		//		var rightNode = family.Members[1];
		//		var result = Traverse(rightNode, level);
		//		AddNode(result, startList, rhs, position);
		//		var intermediateNode = (IntermediateNode)family.Members[0];
				
		//		var firstCopy = startList.ToList();
		//		startList.Clear();
		//		foreach (var subfamily in intermediateNode.Families) {
		//			var listCopy = firstCopy.ToList();
		//			TraverseChildrenHelper(node, subfamily, listCopy, rhs, position - 1, level);
		//			startList.AddRange(listCopy);
		//		}
		//	} else {
		//		throw new Exception();
		//	}
		//}
		
		//private static void AddNode(TraverseResultCollection result, List<TraverseResultCollection[]> startList, Sentence rhs, int position) {
		//	foreach (var children in startList) {
		//		children[position] = result;
		//	}
		//}

		//private TraverseResult TraverseEpsilon(EpsilonNode leaf, int level) {
		//	return new TraverseResult("", leaf, null);
		//}
	}
}
