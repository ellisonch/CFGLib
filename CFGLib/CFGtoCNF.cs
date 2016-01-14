using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib {
	public class CFGtoCNF {
		private Grammar _grammar;
		private int _freshx = 0;
		private Variable _startSymbol;

		public CFGtoCNF(Grammar grammar) {
			_grammar = grammar;
		}

		public CNFGrammar Convert() {
			var productions = CloneGrammar(_grammar);
			StepStart(productions);
			StepTerm(productions);
			StepBin(productions);
			StepDel(productions);
			StepUnit(productions);

			var nonterminalProductions = new List<CNFNonterminalProduction>();
			var terminalProductions = new List<CNFTerminalProduction>();
			double producesEmpty = 0.0;

			foreach (var production in productions) {
				if (production.Rhs.Count > 2) {
					throw new Exception("Didn't expect more than 2");
				} else if (production.Rhs.Count == 2) {
					nonterminalProductions.Add(new CNFNonterminalProduction(production));
				} else if (production.Rhs.Count == 1) {
					var rhs = production.Rhs[0];
					terminalProductions.Add(new CNFTerminalProduction(production));
				} else if (production.Rhs.Count == 0) {
					producesEmpty = GetGrammarFromProductionList(production, productions);
				}
			}

			return new CNFGrammar(nonterminalProductions, terminalProductions, producesEmpty, _startSymbol);

			// BuildLookups();
		}


		private static double GetGrammarFromProductionList(Production target, List<Production> productions) {
			double sumWeight = 0.0;
			foreach (var production in productions) {
				if (production.Lhs == target.Lhs) {
					sumWeight += production.Weight;
				}
			}
			return target.Weight / sumWeight;
		}

		private static List<Production> CloneGrammar(Grammar grammar) {
			return CloneProductions(grammar.Productions);
		}

		private static List<Production> CloneProductions(List<Production> productions) {
			var result = new List<Production>();
			foreach (var production in productions) {
				// var productions = grammar.Productions;
				result.Add(production.Clone());
			}
			return result;
		}

		// TODO needs to be updated when we can have non-S start symbols
		// Eliminate the start symbol from right-hand sides
		private void StepStart(List<Production> productions) {
			var fresh = Getfresh();
			productions.Add(
				new Production(fresh, new Sentence { Variable.Of("S") })
			);
			_startSymbol = fresh;
		}

		// Eliminate rules with nonsolitary terminals
		private void StepTerm(List<Production> productions) {
			var newProductions = new List<Production>();
			var lookup = new Dictionary<Terminal, Variable>();
			foreach (var production in productions) {
				if (production.Rhs.Count < 2) {
					continue;
				}
				for (int i = 0; i < production.Rhs.Count; i++) {
					var word = production.Rhs[i];
					if (word.IsVariable()) {
						continue;
					}
					Terminal terminal = (Terminal)word;
					Variable fresh;
					if (!lookup.TryGetValue(terminal, out fresh)) {
						fresh = Getfresh();
						lookup[terminal] = fresh;
						newProductions.Add(
							new Production(fresh, new Sentence { terminal })
						);
					}
					production.Rhs[i] = fresh;
				}
			}
			productions.AddRange(newProductions);
		}

		// Eliminate right-hand sides with more than 2 nonterminals
		private void StepBin(List<Production> productions) {
			List<Production> finalProductions = new List<Production>();
			foreach (var production in productions) {
				if (production.Rhs.Count < 3) {
					finalProductions.Add(production);
					continue;
				}
				var rhs = production.Rhs;
				var curr = production.Lhs;
				for (int i = 0; i < rhs.Count - 2; i++) {
					int weight = (curr == production.Lhs) ? production.Weight : 1;
					var left = rhs[i];
					var newFresh = Getfresh();
					finalProductions.Add(
						new Production(curr, new Sentence { left, newFresh }, weight)
					);
					curr = newFresh;
				}
				finalProductions.Add(
					new Production(curr, new Sentence { rhs[rhs.Count - 2], rhs[rhs.Count - 1] })
				);
			}
			productions.Clear();
			productions.AddRange(finalProductions);
		}

		// Eliminate ε-rules
		// TODO: definitely does not preserve weights; fix Nullate()
		private void StepDel(List<Production> productions) {
			var nullableSet = GetNullable(productions);
			var newRules = new List<Production>();
			foreach (var production in productions) {
				var toAdd = Nullate(production, nullableSet);
				RemoveExtraneousNulls(toAdd);
				newRules.AddRange(toAdd);
			}
			productions.Clear();
			productions.AddRange(newRules);
		}

		// Eliminate unit rules
		private static void StepUnit(List<Production> productions) {
			bool changed = true;
			while (changed) {
				changed = StepUnitOnce(productions);
			}
		}

		// TODO messes up weights
		private static bool StepUnitOnce(List<Production> productions) {
			var table = BuildLookupTable(productions);
			var result = new List<Production>(productions);
			var changed = false;

			foreach (var production in productions) {
				if (production.Rhs.Count == 1) {
					var rhs = production.Rhs[0];
					if (!rhs.IsVariable()) {
						continue;
					}
					changed = true;
					result.Remove(production);
					var entries = table[(Variable)rhs];
					foreach (var entry in entries) {
						var newProd = new Production(production.Lhs, entry.Rhs);
						if (!newProd.IsSelfLoop) {
							result.Add(newProd);
						}
					}
					break;
				}
			}
			productions.Clear();
			productions.AddRange(result);
			return changed;
		}

		// returns the set of all nonterminals that derive ε
		private static ISet<Variable> GetNullable(List<Production> originalProductions) {
			var productions = CloneProductions(originalProductions);
			var newProductions = new List<Production>();
			var nullableVariables = new HashSet<Variable>();
			var changed = true;
			while (changed) {
				changed = false;
				foreach (var production in productions) {
					if (production.IsEmpty) {
						nullableVariables.Add(production.Lhs);
						changed = true;
						continue;
					}
					if (production.Rhs.OnlyTerminals()) {
						continue;
					}
					for (int i = production.Rhs.Count - 1; i >= 0; i--) {
						var word = production.Rhs[i];
						if (nullableVariables.Contains(word)) {
							production.Rhs.RemoveAt(i);
							changed = true;
						}
					}
					newProductions.Add(production);
				}
				var oldProductions = productions;
				productions = newProductions;
				newProductions = oldProductions;
				newProductions.Clear();
			}
			return nullableVariables;
		}

		// remove A -> X unless A is the start symbol
		private void RemoveExtraneousNulls(List<Production> productions) {
			// var results = new List<Production>();
			if (productions.Count == 0) {
				return;
			}
			for (int i = productions.Count - 1; i >= 0; i--) {
				var production = productions[i];
				if (production.IsEmpty) {
					if (production.Lhs != _startSymbol) {
						productions.RemoveAt(i);
					}
				}
				// results.Add(production);
			}
		}

		private static List<Production> Nullate(Production originalProduction, ISet<Variable> nullableSet) {
			var results = new List<Production>();
			results.Add(originalProduction);
			if (originalProduction.IsEmpty) {
				return results;
			}
			for (int i = originalProduction.Rhs.Count - 1; i >= 0; i--) {
				var newResults = new List<Production>();
				foreach (var production in results) {
					var word = production.Rhs[i];
					if (!nullableSet.Contains(word)) {
						continue;
					}
					// var with = production.Clone();
					var without = production.Clone();
					without.Rhs.RemoveAt(i);
					newResults.Add(without);
				}
				results.AddRange(newResults);
			}
			// NullateAux(production, nullableSet, 0, result);

			return results;
		}

		// todo: horrible
		private Variable Getfresh() {
			var result = Variable.Of("X_" + _freshx);
			_freshx++;
			return result;
		}
		
		private static Dictionary<Variable, List<Production>> BuildLookupTable(List<Production> productions) {
			var table = new Dictionary<Variable, List<Production>>();
			foreach (var production in productions) {
				List<Production> entries;
				if (!table.TryGetValue(production.Lhs, out entries)) {
					entries = new List<Production>();
					table[production.Lhs] = entries;
				}
				entries.Add(production);
			}
			return table;
		}
	}
}
