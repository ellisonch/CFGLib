using CFGLib.Parsers.Sppf;
using CFGLib.ProductionAnnotations.Actioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Actioneer {
	public class Traversal2 {
		private readonly SppfNode _root;
		private readonly BaseGrammar _grammar;
		private readonly Dictionary<SppfNode, TraverseResultCollection> _cache = new Dictionary<SppfNode, TraverseResultCollection>();
		private readonly Dictionary<SppfNode, Tuple<TraverseResultCollection, TraverseResultCollection>> _branchCache = new Dictionary<SppfNode, Tuple<TraverseResultCollection, TraverseResultCollection>>();
		private readonly Stack<SppfNode> _stack1 = new Stack<SppfNode>();
		private readonly Stack<SppfNode> _stack2 = new Stack<SppfNode>();

		public Traversal2(SppfNode root, BaseGrammar grammar) {
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
			// return Traverse(_root);
			throw new NotImplementedException();
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
				if (family.Production == null) {
					throw new Exception();
				}

				// var argList = new List<TraverseResultCollection>();
				if (family.Members.Count == 0) {
					throw new NotImplementedException();
				} else if (family.Members.Count == 1) {
					var child = family.Members[0];
					var childValue = _cache[child];
					foreach (var tr in childValue) {
						var args = new TraverseResult[1] { tr };
						var payload = GetPayload(args, family.Production);
						var oneResult = new TraverseResult(payload, node, family.Production);
						resultList.Add(oneResult);
					}
				} else if (family.Members.Count == 2) {
					throw new NotImplementedException();
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
