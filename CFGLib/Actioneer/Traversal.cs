using CFGLib;
using CFGLib.Parsers.Sppf;
using CFGLib.ProductionAnnotations.Actioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Actioneer {
	public class Traversal {
		private readonly SppfNode _root;
		//private readonly Sentence _input;
		private readonly BaseGrammar _annotatedGrammar;
		// private Dictionary<SppfNode, TraverseResultCollection> _seen = new Dictionary<SppfNode, TraverseResultCollection>();

		public Traversal(SppfNode root, BaseGrammar annotatedGrammar) {
			_root = root;
			//_input = input;
			_annotatedGrammar = annotatedGrammar;
		}

		public TraverseResultCollection Traverse() {
			return Traverse(_root);
		}

		private TraverseResultCollection Traverse(SppfNode node) {
			//if (_seen.TryGetValue(node, out var cached)) {
			//	return cached;
			//}

			var start = node.StartPosition;
			var length = node.EndPosition - node.StartPosition;

			if (node.Families.Count() == 0) {
				return TraverseLeaf(node);
			}

			var resultList = new List<TraverseResult>();
			foreach (var family in node.Families) {
				if (family.Production == null) {
					throw new Exception();
				}
				var args = TraverseFamily(node, family);

				foreach (var oneSet in OneOfEach(args)) {
					object payload = null;
					var productionPlus = family.Production;
					var action = productionPlus.Annotations.Action;
					if (action == null) {
						if (oneSet.Length > 0) {
							payload = oneSet[0].Payload; // default action
						} else {
							payload = null;
						}
					} else {
						payload = action.Act(oneSet);
					}

					var oneResult = new TraverseResult(payload, node, family.Production);
					resultList.Add(oneResult);
				}
			}
			var result = new TraverseResultCollection(resultList);
			// _seen[node] = result;
			return result;
		}

		private TraverseResultCollection TraverseLeaf(SppfNode node) {
			if (node is SppfEpsilon) {
				return new TraverseResultCollection(new List<TraverseResult> { new TraverseResult("", node, null) });
			} else if (node is SppfWord sppfWord) {
				if (!(sppfWord.Word is Terminal terminal)) {
					throw new Exception();
				}
				return new TraverseResultCollection(new List<TraverseResult> { new TraverseResult(terminal.Name, node, null) });
			}
			throw new Exception();
		}
		
		private IEnumerable<T[]> OneOfEach<T>(IEnumerable<T>[] args) {
			var count = args.Length;
			var start = new T[count];
			var startList = new List<T[]> { start };
			for (var position = count - 1; position >= 0; position--) {
				startList = OneOfEachAux(args, startList, position);
			}
			foreach (var option in startList) {
				foreach (var place in option) {
					if (place == null) {
						throw new Exception();
					}
				}
			}
			return startList;
		}

		private List<T[]> OneOfEachAux<T>(IEnumerable<T>[] args, List<T[]> startList, int position) {
			if (position < 0 || position >= args.Length) {
				throw new ArgumentException();
			}
			var newList = new List<T[]>();
			foreach (var old in startList) {
				foreach (var option in args[position]) {
					var newThing = old.ToArray();
					newThing[position] = option;
					newList.Add(newThing);
				}
			}

			return newList;
		}

		private TraverseResultCollection[] TraverseFamily(SppfNode node, SppfFamily family) {
			var count = family.Production.Rhs.Count;
			return TraverseChildren(node, family, count);
		}

		private TraverseResultCollection[] TraverseChildren(SppfNode node, SppfFamily family, int count) {
			if (count == 0) {
				return new TraverseResultCollection[0];
			}
			var start = new TraverseResultCollection[count];
			var startList = new List<TraverseResultCollection[]> { start };
			TraverseChildrenHelper(node, family, startList, family.Production.Rhs, count - 1);
			// TODO: just picking one for now
			// this triggers when you've got a long RHS
			if (startList.Count > 1) {
				throw new Exception();
			}
			var actual = startList[0];
			return actual;
		}

		private void TraverseChildrenHelper(SppfNode node, SppfFamily family, List<TraverseResultCollection[]> startList, Sentence rhs, int position) {
			if (position < 0) {
				throw new ArgumentOutOfRangeException();
			}
			HandleFamily(node, family, startList, rhs, position);
		}

		private void HandleFamily(SppfNode node, SppfFamily family, List<TraverseResultCollection[]> startList, Sentence rhs, int position) {
			if (family.Members.Count == 1) {
				if (position != 0) {
					throw new Exception();
				}
				var onlyNode = family.Members[0];
				var result = Traverse(onlyNode);
				AddNode(result, startList, position);
			} else if (family.Members.Count == 2) {
				if (position < 1) {
					throw new Exception();
				}
				if (position == 1) {
					var leftNode = family.Members[0];
					if (leftNode is SppfBranch sppfBranch) {
						if (!sppfBranch.DecoratedProduction.AtEnd) {
							HandleBranch(node, family, startList, rhs, position);
							return;
						}
					}
					HandleContraction(family, startList, position, leftNode);
				} else {
					HandleBranch(node, family, startList, rhs, position);
				}
			} else {
				throw new Exception();
			}
		}

		private void HandleBranch(SppfNode node, SppfFamily family, List<TraverseResultCollection[]> startList, Sentence rhs, int position) {
			var rightNode = family.Members[1];
			var rightResult = Traverse(rightNode);
			AddNode(rightResult, startList, position);
			var intermediateNode = family.Members[0]; // (IntermediateNode)

			var firstCopy = startList.ToList();
			startList.Clear();
			foreach (var subfamily in intermediateNode.Families) {
				var listCopy = firstCopy.ToList();
				TraverseChildrenHelper(node, subfamily, listCopy, rhs, position - 1);
				startList.AddRange(listCopy);
			}
		}

		private void HandleContraction(SppfFamily family, List<TraverseResultCollection[]> startList, int position, SppfNode leftNode) {
			var rightNode = family.Members[1];
			var leftResult = Traverse(leftNode);
			var rightResult = Traverse(rightNode);
			AddNode(rightResult, startList, position);
			AddNode(leftResult, startList, position - 1);
		}

		private static void AddNode(TraverseResultCollection result, List<TraverseResultCollection[]> startList, int position) {
			foreach (var children in startList) {
				children[position] = result;
			}
		}

		//private TraverseResult TraverseEpsilon(SppfEpsilon leaf, int level) {
		//	return new TraverseResult("", leaf, null);
		//}
	}
}
