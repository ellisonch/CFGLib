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
					var nextWord = item.Next;
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
			}
			// var trees = CollectTrees(S, s, successes);

			return successes.Count() == 0 ? 0.0 : 1.0;
		}

		private void PrintTree(Item item, string padding = "") {
			Console.WriteLine("{0}{1}", padding, item);
			foreach (var child in item.Reductions) {
				PrintTree(child.Item, padding + "  ");
			}
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
				if (item.Next != completedItem.Production.Lhs) {
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
