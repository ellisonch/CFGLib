using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {

	/*
	Inspired by
	 * Elizabeth Scott's 2008 paper "SPPF-Style Parsing From Earley Recognisers" (http://dx.doi.org/10.1016/j.entcs.2008.03.044) [ES2008]
	 * John Aycock and Nigel Horspool's 2002 paper "Practical Earley Parsing" (http://dx.doi.org/10.1093/comjnl/45.6.620) [AH2002]
	 * Loup Vaillant's tutorial (http://loup-vaillant.fr/tutorials/earley-parsing/)
	*/

	internal class EarleyParser : Parser {
		private readonly BaseGrammar _grammar;
		public EarleyParser(BaseGrammar grammar) {
			_grammar = grammar;
		}

		public override double GetProbability(Sentence s) {
			StateSet[] S = FreshS(s.Count + 1);

			// Initialize S(0)
			foreach (var production in _grammar.ProductionsFrom(_grammar.Start)) {
				var item = new Item(production, 0, 0, 0);
				S[0].Add(item);
			}

			// outer loop
			for (int stateIndex = 0; stateIndex < S.Length; stateIndex++) {
				var state = S[stateIndex];
				Terminal inputTerminal = null;
				if (stateIndex < s.Count) {
					inputTerminal = (Terminal)s[stateIndex];
				}

				// If there are no items in the current state, we're stuck
				if (state.Count == 0) {
					return 0.0;
				}

				// inner loop
				for (int itemIndex = 0; itemIndex < state.Count; itemIndex++) {
					var item = state[itemIndex];
					var nextWord = item.NextWord;
					if (nextWord == null) {
						Completion(S, stateIndex, item);
					} else if (nextWord.IsNonterminal()) {
						Prediction(S, stateIndex, (Nonterminal)nextWord, item);
					} else {
						Scan(S, stateIndex, item, (Terminal)nextWord, s, inputTerminal);
					}
				}
			}

			var successes = GetSuccesses(S, s);

			var sppf = ConstructSPPF(successes, s);
			PrintForest(sppf);
			Console.WriteLine("---------------------------------");
			PrintDerivations(sppf);
			// var trees = CollectTrees(S, s, successes);

			return successes.Count() == 0 ? 0.0 : 1.0;
		}
		
		// TODO use visitor
		private void PrintDerivations(Node node, string padding = "", HashSet<Node> seen = null) {
			if (seen == null) {
				seen = new HashSet<Node>();
			}
			if (node is IntermediateNode) {
				var intermediateNode = (IntermediateNode)node;
				if (intermediateNode.Item.CurrentPosition == intermediateNode.Item.Production.Rhs.Count - 1) {
					Console.WriteLine("{0}APPLY {1}", padding, intermediateNode.Item.Production);
				}
			}
			Console.WriteLine("{0}{1}", padding, node);
			if (node.Families.Count > 0 && seen.Contains(node)) {
				Console.WriteLine("{0}Already seen this node!", padding);
				return;
			}
			seen.Add(node);

			var l = node.Families.ToList();
			for (int i = 0; i < l.Count; i++) {
				var alternative = l[i];
				if (l.Count > 1) {
					Console.WriteLine("{0}Alternative {1}", padding, i);
				}

				var members = l[i].Members;
				if (members.Count == 0) {
					PrintDerivationsChildren(node, padding, seen);
				} else if (members.Count == 1) {
					var child = members[0];

					PrintDerivationsChildren(node, child, padding, seen);
				} else if (members.Count == 2) {
					var left = members[0];
					var right = members[1];

					PrintDerivationsChildren(node, left, right, padding, seen);
				} else {
					throw new Exception("Should only be 0--2 children");
				}
			}
		}

		private void PrintDerivationsChildren(Node parent, string padding, HashSet<Node> seen) {
			Console.WriteLine("{0}Don't handle 0 case", padding);
		}

		private void PrintDerivationsChildren(Node parent, Node child, string padding, HashSet<Node> seen) {
			Word parentSymbol = null;
			if (parent is SymbolNode) {
				var symbolParent = (SymbolNode)parent;
				parentSymbol = symbolParent.Symbol;
			} else {
				var intermediateParent = (IntermediateNode)parent;
				if (intermediateParent.Item.CurrentPosition != 1) {
					throw new Exception("Expected to be at beginning of item");
				}
				parentSymbol = intermediateParent.Item.Production.Rhs[0];
			}
			Console.WriteLine("{0}  Parent symbol = {1}", padding, parentSymbol);

			if (child is SymbolNode) {
				var symbolChild = (SymbolNode)child;
				if (symbolChild.Symbol.IsNonterminal()) {
					Console.WriteLine("{0}  Nonterminal Symbol Child", padding);
					if (parent is SymbolNode) {
						Console.WriteLine("{0}  APPLY {1} -> {2}", padding, parentSymbol, symbolChild.Symbol);
					} else {
						//if (parentSymbol != symbolChild.Symbol) {
						//	throw new Exception("Symbols don't match");
						//}
					}
					PrintDerivations(symbolChild, padding + "  ", new HashSet<Node>(seen));					
				} else {
					if (parentSymbol.IsNonterminal()) {
						Console.WriteLine("{0}  APPLY {1} -> {2}", padding, parentSymbol, symbolChild.Symbol);
					} else {
						Console.WriteLine("{0}  Terminal Symbol Child", padding);
					}
				}
			} else if (child is IntermediateNode) {
				throw new Exception("Don't handle intermediate");
			} else if (child is EpsilonNode) {
				Console.WriteLine("{0}  APPLY {1} -> Epsilon", padding, parentSymbol);
				// Console.WriteLine("{0}  Epsilon", padding);
				// Console.WriteLine("{0}APPLY {1} -> epsilon", padding, node.);
			}
		}

		private void PrintDerivationsChildren(Node parent, Node left, Node right, string padding, HashSet<Node> seen) {
			if (!(left is IntermediateNode)) {
				Console.WriteLine("{0}Left isn't intermediate", padding);
				throw new Exception();
			}
			if (!(right is SymbolNode)) {
				Console.WriteLine("{0}Right isn't symbol", padding);
				throw new Exception();
			}

			PrintDerivations(left, padding + "  ", new HashSet<Node>(seen));
			PrintDerivations(right, padding + "  ", new HashSet<Node>(seen));
		}

		private SymbolNode ConstructSPPF(IList<Item> successes, Sentence s) {
			var root = new SymbolNode(_grammar.Start, 0, s.Count);
			var processed = new HashSet<Item>();
			var nodes = new Dictionary<Node, Node>();
			nodes[root] = root;

			foreach (var success in successes) {
				BuildTree(nodes, processed, root, success);
			}

			return root;
		}

		private void PrintForest(Node node, string padding = "", HashSet<Node> seen = null) {
			if (seen == null) {
				seen = new HashSet<Node>();
			}
			Console.WriteLine("{0}{1}", padding, node);

			if (node.Families.Count > 0 && seen.Contains(node)) {
				Console.WriteLine("{0}Already seen this node!", padding);
				return;
			}
			seen.Add(node);
			
			var l = node.Families.ToList();
			for (int i = 0; i < l.Count; i++) {
				var alternative = l[i];
				if (l.Count > 1) {
					Console.WriteLine("{0}Alternative {1}", padding, i);
				}
				foreach (var member in l[i].Members) {
					PrintForest(member, padding + "  ", new HashSet<Node>(seen));
				}
			}
		}

		// [Sec 4, ES2008]
		private void BuildTree(Dictionary<Node, Node> nodes, HashSet<Item> processed, InteriorNode node, Item item) {
			// item.Processed = true;
			processed.Add(item);

			if (item.Production.Rhs.Count == 0) {
				var i = node.EndPosition;
				var v = NewOrExistingNode(nodes, new SymbolNode(item.Production.Lhs, i, i));
				//if there is no SPPF node v labelled (A, i, i)
				//create one with child node ϵ
				v.AddFamily(new Family(EpsilonNode.Node));
				// basically, SymbolNodes with no children have empty children

				// node.AddFamily(new Family(v));
			} else if (item.CurrentPosition == 1) {
				var prevWord = item.PrevWord;
				if (prevWord.IsTerminal()) {
					var a = (Terminal)prevWord;
					var i = node.EndPosition;
					var v = NewOrExistingNode(nodes, new SymbolNode(a, i - 1, i));
					node.AddFamily(new Family(v));
				} else {
					var C = (Nonterminal)prevWord;
					var j = node.StartPosition;
					var i = node.EndPosition;
					var v = NewOrExistingNode(nodes, new SymbolNode(C, j, i));
					node.AddFamily(new Family(v));
					foreach (var reduction in item.Reductions) {
						if (reduction.Label != j) {
							continue;
						}
						var q = reduction.Item;
						if (!processed.Contains(q)) {
							BuildTree(nodes, processed, v, q);
						}
					}
				}
			} else if (item.PrevWord.IsTerminal()) {
				var a = (Terminal)item.PrevWord;
				var j = node.StartPosition;
				var i = node.EndPosition;
				var v = NewOrExistingNode(nodes, new SymbolNode(a, i - 1, i));
				var w = NewOrExistingNode(nodes, new IntermediateNode(item.Decrement(), j, i - 1));
				foreach (var predecessor in item.Predecessors) {
					if (predecessor.Label != i - 1) {
						continue;
					}
					var pPrime = predecessor.Item;
					if (!processed.Contains(pPrime)) {
						BuildTree(nodes, processed, w, pPrime);
					}
				}

				node.AddFamily(new Family(w, v));
			} else {
				var C = (Nonterminal)item.PrevWord;
				foreach (var reduction in item.Reductions) {
					var l = reduction.Label;
					var q = reduction.Item;
					var j = node.StartPosition;
					var i = node.EndPosition;
					var v = NewOrExistingNode(nodes, new SymbolNode(C, l, i));
					if (!processed.Contains(q)) {
						BuildTree(nodes, processed, v, q);
					}
					var w = NewOrExistingNode(nodes, new IntermediateNode(item.Decrement(), j, l));
					foreach (var predecessor in item.Predecessors) {
						if (predecessor.Label != l) {
							continue;
						}
						var pPrime = predecessor.Item;
						if (!processed.Contains(pPrime)) {
							BuildTree(nodes, processed, w, pPrime);
						}
					}
					node.AddFamily(new Family(w, v));
				}
			}
		}

		private T NewOrExistingNode<T>(Dictionary<Node, Node> nodes, T node) where T : Node {
			Node existingNode;
			if (!nodes.TryGetValue(node, out existingNode)) {
				existingNode = node;
				nodes[node] = node;
			}
			node = (T)existingNode;
			return node;
		}

		//private void PrintTree(Item item, string padding = "") {
		//	Console.WriteLine("{0}{1}", padding, item);
		//	foreach (var child in item.Reductions) {
		//		PrintTree(child.Item, padding + "  ");
		//	}
		//}

		private static StateSet[] FreshS(int length) {
			var S = new StateSet[length];

			// Initialize S
			for (int i = 0; i < S.Length; i++) {
				S[i] = new StateSet();
			}

			return S;
		}

		private object CollectTrees(StateSet[] S, Sentence s, IEnumerable<Item> successes) {
			var reversedS = FreshS(S.Length);
			// make stateIndex correspond to item.StartPosition instead of item.EndPosition
			// also, throw away incomplete items
			for (int stateIndex = 0; stateIndex < S.Length; stateIndex++) {
				var state = S[stateIndex];
				foreach (var item in state) {
					if (!item.IsComplete()) {
						continue;
					}
					reversedS[item.StartPosition].Add(item);
				}
			}

			// var pg = new ParseGraph(reversedS, s);


			//foreach (var success in successes) {
			//	// pg.DFS(success);
			//	scottSec4(reversedS, success)
			//}

			return null;
		}

		private IList<Item> GetSuccesses(StateSet[] S, Sentence s) {
			var successes = new List<Item>();
			var lastState = S[s.Count];
			foreach (Item item in lastState) {
				if (!item.IsComplete()) {
					continue;
				}
				if (item.StartPosition != 0) {
					continue;
				}
				if (item.Production.Lhs != _grammar.Start) {
					continue;
				}
				successes.Add(item);
			}
			return successes;
		}

		private void Completion(StateSet[] S, int stateIndex, Item completedItem) {
			var state = S[stateIndex];
			var Si = S[completedItem.StartPosition];
			var toAdd = new StateSet();
			foreach (var item in Si) {
				// make sure it's the same nonterminal
				if (item.NextWord != completedItem.Production.Lhs) {
					continue;
				}
				var newItem = item.Increment();
				newItem.AddReduction(completedItem.StartPosition, completedItem);
				if (item.CurrentPosition != 0) {
					newItem.AddPredecessor(completedItem.StartPosition, item);
				}
				toAdd.Add(newItem);
			}
			foreach (var item in toAdd) {
				InsertWithoutDuplicating(state, stateIndex, item);
			}
		}
		private void Prediction(StateSet[] S, int stateIndex, Nonterminal nonterminal, Item item) {
			var state = S[stateIndex];
			var productions = _grammar.ProductionsFrom(nonterminal);

			// insert, but avoid duplicates
			foreach (var production in productions) {
				var newItem = new Item(production, 0, stateIndex, stateIndex);
				InsertWithoutDuplicating(state, stateIndex, newItem);
			}

			// If the thing we're trying to produce is nullable, go ahead and eagerly derive epsilon. [AH2002]
			if (_grammar.NullableProbabilities[nonterminal] > 0.0) {
				var newItem = item.Increment();
				InsertWithoutDuplicating(state, stateIndex, newItem);
			}
		}

		private void InsertWithoutDuplicating(StateSet state, int stateIndex, Item newItem) {
			// the endPosition should always equal the stateIndex of the state it resides in
			newItem.EndPosition = stateIndex; 
			// TODO: opportunity for StateSet feature?
			Predicate<Item> equalityCheck = (item) => {
				if (!item.Production.ValueEquals(newItem.Production)) {
					return false;
				}
				if (item.CurrentPosition != newItem.CurrentPosition) {
					return false;
				}
				if (item.StartPosition != newItem.StartPosition) {
					return false;
				}
				return true;
			};

			var existingItem = state.Find(equalityCheck);
			if (existingItem == null) {
				state.Add(newItem);
			} else {
				existingItem.Predecessors.AddRange(newItem.Predecessors);
				existingItem.Reductions.AddRange(newItem.Reductions);
			}
		}
		
		private void Scan(StateSet[] S, int stateIndex, Item item, Terminal terminal, Sentence s, Terminal currentTerminal) {
			var state = S[stateIndex];

			StateSet nextState = null;
			if (stateIndex + 1 < S.Length) {
				nextState = S[stateIndex + 1];
			} else {
				return;
			}

			if (currentTerminal == terminal) {
				var newItem = item.Increment();
				if (item.CurrentPosition != 0) {
					newItem.AddPredecessor(stateIndex, item);
				}
				InsertWithoutDuplicating(nextState, stateIndex + 1, newItem);		
			}
		}
	}
}
