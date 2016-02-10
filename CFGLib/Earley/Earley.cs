using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.EarleyNS {
	internal class Earley {
		private readonly BaseGrammar _grammar;
		public Earley(BaseGrammar grammar) {
			_grammar = grammar;
		}

		public double GetProbability(Sentence s) {
			var S = new StateSet[s.Count + 1];
			// var nullableDict = GrammarHelpers.GetNullable(new HashSet<Production>(Productions));

			// Initialize S
			for (int i = 0; i < S.Length; i++) {
				S[i] = new StateSet();
			}

			// Initialize S(0)
			foreach (var production in _grammar.ProductionsFrom(_grammar.Start)) {
				var item = new Item(production, 0, 0);
				S[0].Add(item);
			}

			// outer loop
			for (int stateIndex = 0; stateIndex < S.Length; stateIndex++) {
				var state = S[stateIndex];
				StateSet nextState = null;
				if (stateIndex + 1 < S.Length) {
					nextState = S[stateIndex + 1];
				}
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
						Completion(S, state, item);
					} else if (nextWord.IsNonterminal()) {
						Prediction(state, (Nonterminal)nextWord, stateIndex, item);
					} else {
						Scan(state, nextState, item, (Terminal)nextWord, s, inputTerminal);
					}
				}
			}

			var successes = GetSuccesses(S, s);

			// return successes.Count != 0;
			return successes.Count() == 0 ? 0.0 : 1.0;
		}

		private IEnumerable<Item> GetSuccesses(StateSet[] S, Sentence s) {
			//if (s.Count == 0) {
			//	throw new Exception("Not handling nulls yet");
			//}
			var successes = new List<Item>();
			// foreach (StateSet state in S[s.Count - 1]) {
			// for (int stateIndex = 0; stateIndex < s.Count - 1; stateIndex++) {
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
			//}
			return successes;
		}

		private void Completion(StateSet[] S, StateSet state, Item completedItem) {
			var Si = S[completedItem.StartPosition];
			var toAdd = new StateSet();
			foreach (var item in Si) {
				if (item.Next == completedItem.Production.Lhs) {
					toAdd.Add(item.Increment());
				}
			}
			foreach (var item in toAdd) {
				InsertWithoutDuplicating(state, item);
			}
			// state.AddRange(toAdd);
		}
		private void Prediction(StateSet state, Nonterminal nonterminal, int predictionPoint, Item item) {
			var productions = _grammar.ProductionsFrom(nonterminal);

			// insert, but avoid duplicates
			foreach (var production in productions) {
				var newItem = new Item(production, 0, predictionPoint);
				InsertWithoutDuplicating(state, newItem);
			}

			// If the thing we're trying to produce is nullable, go ahead and eagerly derive epsilon.
			// This is due to Aycock and Horspool's "Practical Earley Parsing" (2002)
			if (_grammar.NullableProbabilities[nonterminal] > 0.0) {
				var newItem = item.Increment();
				InsertWithoutDuplicating(state, newItem);
			}
		}

		private void InsertWithoutDuplicating(StateSet state, Item newItem) {
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
			if (!state.Exists(equalityCheck)) {
				state.Add(newItem);
			}
		}

		private void Scan(StateSet state, StateSet nextState, Item item, Terminal terminal, Sentence s, Terminal currentTerminal) {
			if (nextState == null) {
				// throw new Exception("Trying to scan past the bounds of the sentence");
				return;
			}

			// var currentInput = item.StartPosition + item.CurrentPosition;
			// var currentTerminal = s[currentInput]; // TODO: safety
			if (currentTerminal == terminal) {
				var newItem = item.Increment();
				InsertWithoutDuplicating(nextState, newItem);
				// nextState.Add(item.Increment());
			}
		}
	}
}
