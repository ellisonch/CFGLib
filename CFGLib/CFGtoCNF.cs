using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib {
	public class CFGtoCNF {
		private Grammar _grammar;
		private int _freshx = 0;
		private Nonterminal _startSymbol;

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
			int producesEmptyWeight = 0;
			
			foreach (var production in productions) {
				if (production.Rhs.Count > 2) {
					throw new Exception("Didn't expect more than 2");
				} else if (production.Rhs.Count == 2) {
					nonterminalProductions.Add(new CNFNonterminalProduction(production));
				} else if (production.Rhs.Count == 1) {
					var rhs = production.Rhs[0];
					terminalProductions.Add(new CNFTerminalProduction(production));
				} else if (production.Rhs.Count == 0) {
					producesEmptyWeight += production.Weight;
						// GetGrammarFromProductionList(production, productions);
				}
			}

			return new CNFGrammar(nonterminalProductions, terminalProductions, producesEmptyWeight, _startSymbol);

			// BuildLookups();
		}


		//private static double GetGrammarFromProductionList(Production target, List<Production> productions) {
		//	double sumWeight = 0.0;
		//	foreach (var production in productions) {
		//		if (production.Lhs == target.Lhs) {
		//			sumWeight += production.Weight;
		//		}
		//	}
		//	return target.Weight / sumWeight;
		//}

		private static ISet<BaseProduction> CloneGrammar(Grammar grammar) {
			return CloneProductions(grammar.Productions);
		}

		private static ISet<BaseProduction> CloneProductions(IEnumerable<BaseProduction> productions) {
			var result = new HashSet<BaseProduction>();
			foreach (var production in productions) {
				// var productions = grammar.Productions;
				result.Add(production.DeepClone());
			}
			return result;
		}

		// TODO needs to be updated when we can have non-S start symbols
		// Eliminate the start symbol from right-hand sides
		private void StepStart(ISet<BaseProduction> productions) {
			var fresh = Getfresh();
			productions.Add(
				new Production(fresh, new Sentence { Nonterminal.Of("S") })
			);
			_startSymbol = fresh;
		}

		// Eliminate rules with nonsolitary terminals
		private void StepTerm(ISet<BaseProduction> productions) {
			var newProductions = new List<BaseProduction>();
			var lookup = new Dictionary<Terminal, Nonterminal>();
			foreach (var production in productions) {
				if (production.Rhs.Count < 2) {
					continue;
				}
				for (int i = 0; i < production.Rhs.Count; i++) {
					var word = production.Rhs[i];
					if (word.IsNonterminal()) {
						continue;
					}
					Terminal terminal = (Terminal)word;
					Nonterminal fresh;
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
			productions.UnionWith(newProductions);
		}

		// Eliminate right-hand sides with more than 2 nonterminals
		private void StepBin(ISet<BaseProduction> productions) {
			List<BaseProduction> finalProductions = new List<BaseProduction>();
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
			productions.UnionWith(finalProductions);
		}

		// Eliminate ε-rules
		// TODO: definitely does not preserve weights; fix Nullate()
		private void StepDel(ISet<BaseProduction> productions) {
			var nullableSet = GetNullable(productions);
			var newRules = new List<BaseProduction>();
			foreach (var production in productions) {
				var toAdd = Nullate(production, nullableSet);
				RemoveExtraneousNulls(toAdd);
				newRules.AddRange(toAdd);
			}
			productions.Clear();
			productions.UnionWith(newRules);
		}

		// Eliminate unit rules
		private static void StepUnit(ISet<BaseProduction> productions) {
			bool changed = true;
			while (changed) {
				changed = StepUnitOnce(productions);
			}
		}

		// TODO messes up weights
		private static bool StepUnitOnce(ISet<BaseProduction> productions) {
			var table = BuildLookupTable(productions);
			var result = new HashSet<BaseProduction>(productions);
			var changed = false;

			foreach (var production in productions) {
				if (production.Rhs.Count == 1) {
					var rhs = production.Rhs[0];
					if (!rhs.IsNonterminal()) {
						continue;
					}
					changed = true;
					result.Remove(production);
					var entries = table.LookupEnumerable((Nonterminal)rhs);
					// var entries = table[(Nonterminal)rhs];
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
			productions.UnionWith(result);
			return changed;
		}

		// returns the set of all nonterminals that derive ε
		private static ISet<Nonterminal> GetNullable(ISet<BaseProduction> originalProductions) {
			var productions = CloneProductions(originalProductions);
			ISet<BaseProduction> newProductions = new HashSet<BaseProduction>();
			var nullableNonterminals = new HashSet<Nonterminal>();
			var changed = true;
			while (changed) {
				changed = false;
				foreach (var production in productions) {
					if (production.IsEmpty) {
						nullableNonterminals.Add(production.Lhs);
						changed = true;
						continue;
					}
					if (production.Rhs.OnlyTerminals()) {
						continue;
					}
					for (int i = production.Rhs.Count - 1; i >= 0; i--) {
						var word = production.Rhs[i];
						if (nullableNonterminals.Contains(word)) {
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
			return nullableNonterminals;
		}

		// remove A -> X unless A is the start symbol
		private void RemoveExtraneousNulls(List<BaseProduction> productions) {
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

		private static List<BaseProduction> Nullate(BaseProduction originalProduction, ISet<Nonterminal> nullableSet) {
			var results = new List<BaseProduction>();
			results.Add(originalProduction);
			if (originalProduction.IsEmpty) {
				return results;
			}
			for (int i = originalProduction.Rhs.Count - 1; i >= 0; i--) {
				var newResults = new List<BaseProduction>();
				foreach (var production in results) {
					var word = production.Rhs[i];
					if (!nullableSet.Contains(word)) {
						continue;
					}
					// var with = production.Clone();
					var without = production.DeepClone();
					without.Rhs.RemoveAt(i);
					newResults.Add(without);
				}
				results.AddRange(newResults);
			}
			// NullateAux(production, nullableSet, 0, result);

			return results;
		}

		// todo: horrible
		private Nonterminal Getfresh() {
			var result = Nonterminal.Of("X_" + _freshx);
			_freshx++;
			return result;
		}
		
		private static Dictionary<Nonterminal, ICollection<BaseProduction>> BuildLookupTable(ISet<BaseProduction> productions) {
			Dictionary<Nonterminal, ICollection<BaseProduction>> table;

			table = Helpers.ConstructCache(
				productions,
				(p) => p.Lhs,
				(p) => p,
				() => (ICollection<BaseProduction>)new List<BaseProduction>(),
				(l, p) => l.Add(p)
			);

			//foreach (var production in productions) {
			//	List<BaseProduction> entries;
			//	if (!table.TryGetValue(production.Lhs, out entries)) {
			//		entries = new List<BaseProduction>();
			//		table[production.Lhs] = entries;
			//	}
			//	entries.Add(production);
			//}
			return table;
		}
	}
}
