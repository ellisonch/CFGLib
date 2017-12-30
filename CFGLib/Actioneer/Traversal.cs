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
		private readonly BaseGrammar _grammar;
		private readonly Dictionary<SppfNode, TraverseResultCollection> _cache = new Dictionary<SppfNode, TraverseResultCollection>();
		private readonly Dictionary<SppfNode, TraverseResultCollection[]> _branchCache = new Dictionary<SppfNode, TraverseResultCollection[]>();
		private readonly Stack<SppfNode> _stack1 = new Stack<SppfNode>();
		private readonly Stack<SppfNode> _stack2 = new Stack<SppfNode>();

		public Traversal(SppfNode root, BaseGrammar grammar) {
			_root = root;
			_grammar = grammar;
			_stack1.Push(root);
		}

		public TraverseResultCollection Traverse() {
			var curr = _root;

			while (_stack1.Count > 0) {
				var node = _stack1.Pop();
				_stack2.Push(node);
				foreach (var family in node.Families) {
					foreach (var child in family.Members) {
						_stack1.Push(child);
					}
				}
			}
			while (_stack2.Count > 0) {
				var node = _stack2.Pop();
				BuildAndSetResult(node);
			}
			return _cache[_root];
			// return Traverse(_root);
			// throw new NotImplementedException();
		}

		private void BuildAndSetResult(SppfNode node) {
			if (_cache.ContainsKey(node)) {
				return;
			}
			if (node.Families.Count() == 0) {
				if (node is SppfEpsilon) {
					_cache[node] = new TraverseResultCollection(new List<TraverseResult> { new TraverseResult("", node, null) });
				} else if (node is SppfWord sppfWord) {
					if (!(sppfWord.Word is Terminal terminal)) {
						throw new Exception();
					}
					_cache[node] = new TraverseResultCollection(new List<TraverseResult> { new TraverseResult(terminal.Name, node, null) });
				}
				return;
			}

			var resultList = new List<TraverseResult>();
			foreach (var family in node.Families) {
				// var argList = new List<TraverseResultCollection>();
				if (family.Members.Count == 0) {
					throw new NotImplementedException();
				} else if (family.Members.Count == 1) {
					var child = family.Members[0];
					var childValue = _cache[child];

					if (node is SppfBranch) { // this only happens in old (not contracted) sppf
						if (family.Production != null) {
							throw new Exception();
						}
						var newValues = new TraverseResultCollection[] { childValue };
						_branchCache[node] = newValues;
					} else {
						if (family.Production == null) {
							throw new Exception();
						}
						foreach (var tr in childValue) {
							var args = new TraverseResult[1] { tr };
							var payload = GetPayload(args, family.Production);
							var oneResult = new TraverseResult(payload, node, family.Production);
							resultList.Add(oneResult);
						}
					}
				} else if (family.Members.Count == 2) {
					var left = family.Members[0];
					var right = family.Members[1];

					if (family.Production == null) {
						if (left is SppfBranch) { // middle of long production
							var leftValues = _branchCache[left];
							var rightValue = _cache[right];
							var newValues = AppendToArray(leftValues, rightValue);
							_branchCache[node] = newValues;
						} else { // bottom left of a long production
							var leftValue = _cache[left];
							var rightValue = _cache[right];
							var newValues = new TraverseResultCollection[] { leftValue, rightValue };
							_branchCache[node] = newValues;
						}
					} else { // top of production
						var rightValue = _cache[right];
						TraverseResultCollection[] args;
						if (left is SppfBranch) {
							var leftValues = _branchCache[left];
							args = AppendToArray(leftValues, rightValue);
						} else {
							var leftValue = _cache[left];
							args = new TraverseResultCollection[] { leftValue, rightValue };
						}
						var someResults = BuildResultList(args, node, family.Production);
						resultList.AddRange(someResults);
						// throw new NotImplementedException();
					}					
				} else {
					throw new Exception();
				}

				//foreach (var child in family.Members) {
				//	if (!_cache.TryGetValue(child, out var childValue)) {
				//		throw new Exception();
				//	}
				//	argList.Add(childValue);
				//}
				// argSet.Add(argList.ToArray());
			}
			var collection = new TraverseResultCollection(resultList);
			_cache[node] = collection;
			// var value = new TraverseResultCollection
			// var payload = GetPayload(_emptyArgs, )
		}

		private List<TraverseResult> BuildResultList(TraverseResultCollection[] args, SppfNode node, Production production) {
			var resultList = new List<TraverseResult>();
			foreach (var oneSet in OneOfEach(args)) {
				object payload = null;
				var action = production.Annotations.Action;
				if (action == null) {
					if (oneSet.Length > 0) {
						payload = oneSet[0].Payload; // default action
					} else {
						payload = null;
					}
				} else {
					payload = action.Act(oneSet);
				}

				var oneResult = new TraverseResult(payload, node, production);
				resultList.Add(oneResult);
			}
			return resultList;
		}

		private static IEnumerable<T[]> OneOfEach<T>(IEnumerable<T>[] args) {
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

		private static List<T[]> OneOfEachAux<T>(IEnumerable<T>[] args, List<T[]> startList, int position) {
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

		private static T[] AppendToArray<T>(T[] leftValues, T rightValue) {
			var newValues = new T[leftValues.Length + 1];
			leftValues.CopyTo(newValues, 0);
			newValues[leftValues.Length] = rightValue;
			return newValues;
		}

		private object GetPayload(TraverseResult[] args, Production production) {
			object payload = null;
			var action = production.Annotations.Action;
			if (action == null) {
				if (args.Length > 0) {
					payload = args[0].Payload; // default action
				} else {
					payload = null;
				}
			} else {
				payload = action.Act(args);
			}
			return payload;
		}
	}
}
