using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {
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
			foreach (var success in successes) {
				PrintTree(success);
				Console.WriteLine("");
				var allItems = GetItems(success);
				var node = new SymbolNode(success);
				var nodes = new Dictionary<InteriorNode, InteriorNode>();
				var processed = new HashSet<Item>();
				nodes[node] = node;
				BuildTree(nodes, processed, node, success);
				PrintForest(node);
			}
			// var trees = CollectTrees(S, s, successes);

			return successes.Count() == 0 ? 0.0 : 1.0;
		}

		private void PrintForest(InteriorNode node, string padding = "") {
			Console.WriteLine("{0}{1}", padding, node);
			//if (node.Families.Count == 2) {
			//	Console.WriteLine("{0}There are two alternatives:");
			//}
			var l = node.Families.ToList();
			if (l.Count == 2) {
				Console.WriteLine("{0}Alternative 1", padding);
				foreach (var member in l[0].Members) {
					PrintForest(member, padding + "  ");
				}
				Console.WriteLine("{0}Alternative 2", padding);
				foreach (var member in l[1].Members) {
					PrintForest(member, padding + "  ");
				}
			} else if (l.Count == 1) {
				foreach (var member in l[0].Members) {
					PrintForest(member, padding + "  ");
				}
			}
		}

		private void BuildTree(Dictionary<InteriorNode, InteriorNode> nodes, HashSet<Item> processed, InteriorNode node, Item item) {
			// item.Processed = true;
			processed.Add(item);

			if (item.Production.Rhs.Count == 0) {
				throw new Exception("");
				//if there is no SPPF node v labelled (A, i, i)
				//create one with child node ϵ
				//if u does not have a family(v) then add the family (v)to u
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
				throw new Exception("");
				//if there is no SPPF node v labelled (a, i − 1, i) create one
				//if there is no SPPF node w labelled (A::= α ′ · aβ, j, i − 1) create one
				//for each target p ′ of a predecessor pointer labelled i − 1 from p {
				//					if p ′ is not marked as processed Buildtree(w, p ′ ) }
				//				if u does not have a family(w, v) add the family(w, v) to u
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

		private T NewOrExistingNode<T>(Dictionary<InteriorNode, InteriorNode> nodes, T node) where T : InteriorNode {
			InteriorNode existingNode;
			if (!nodes.TryGetValue(node, out existingNode)) {
				existingNode = node;
				nodes[node] = node;
			}
			node = (T)existingNode;
			return node;
		}

		private void PrintTree(Item item, string padding = "") {
			Console.WriteLine("{0}{1}", padding, item);
			foreach (var child in item.Reductions) {
				PrintTree(child.Item, padding + "  ");
			}
		}

		private HashSet<Item> GetItems(Item item, HashSet<Item> items = null) {
			if (items == null) {
				items = new HashSet<Item>();
			}
			items.Add(item);
			foreach (var pred in item.Predecessors) {
				GetItems(pred.Item, items);
			}
			foreach (var red in item.Reductions) {
				GetItems(red.Item, items);
			}
			return items;
		}

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

		private IEnumerable<Item> GetSuccesses(StateSet[] S, Sentence s) {
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

			// If the thing we're trying to produce is nullable, go ahead and eagerly derive epsilon.
			// This is due to Aycock and Horspool's "Practical Earley Parsing" (2002)
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
					newItem.AddPredecessor(stateIndex - 1, newItem);
				}
				InsertWithoutDuplicating(nextState, stateIndex + 1, newItem);		
			}
		}
	}
}
