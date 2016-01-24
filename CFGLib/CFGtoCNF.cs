using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("CFGLibTest")]
namespace CFGLib {
	/// <summary>
	/// A class to help transform a generic grammar into a CNF grammar
	/// </summary>
	internal class CFGtoCNF {
		private Grammar _grammar;
		private int _freshx = 0;
		private Nonterminal _startSymbol;

		internal CFGtoCNF(Grammar grammar) {
			_grammar = grammar;
		}

		/// <summary>
		/// Actually performs the conversion and returns a new CNF grammar based on the old grammar
		/// </summary>
		/// <param name="simplify"></param>
		/// <returns></returns>
		internal CNFGrammar Convert(bool simplify) {
			var productions = CloneGrammar(_grammar);
			StepStart(productions);
			StepTerm(productions);
			StepBin(productions);
			StepDel(productions);
			StepUnit(productions);

			var nonterminalProductions = new List<CNFNonterminalProduction>();
			var terminalProductions = new List<CNFTerminalProduction>();
			var producesEmptyWeight = 0.0;
			
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

			return new CNFGrammar(nonterminalProductions, terminalProductions, producesEmptyWeight, _startSymbol, simplify);
		}

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
		/// <summary>
		/// Eliminate the start symbol from right-hand sides
		/// </summary>
		/// <param name="productions"></param>
		private void StepStart(ISet<BaseProduction> productions) {
			var fresh = GetFresh();
			productions.Add(
				new Production(fresh, new Sentence { Nonterminal.Of("S") })
			);
			_startSymbol = fresh;
		}
		
		/// <summary>
		/// Eliminate rules with nonsolitary terminals
		/// </summary>
		/// <param name="productions"></param>
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
						fresh = GetFresh();
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
		
		/// <summary>
		/// Eliminate right-hand sides with more than 2 nonterminals
		/// </summary>
		/// <param name="productions"></param>
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
					var weight = (curr == production.Lhs) ? production.Weight : 1.0;
					var left = rhs[i];
					var newFresh = GetFresh();
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

		/// <summary>
		/// Eliminate ε-rules
		/// </summary>
		/// <param name="productions"></param>
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

		/// <summary>
		/// Eliminate unit rules (e.g., &lt;X> -> &lt;Y>)
		/// </summary>
		/// <param name="productions"></param>
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

		/// <summary>
		/// Returns the set of all nonterminals that derive ε (in one or many steps)
		/// </summary>
		/// <param name="originalProductions"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Remove &lt;A> -> ε unless A is the start symbol 
		/// </summary>
		/// <param name="productions"></param>
		private void RemoveExtraneousNulls(List<BaseProduction> productions) {
			if (productions.Count == 0) {
				return;
			}
			for (int i = productions.Count - 1; i >= 0; i--) {
				var production = productions[i];
				if (production.Lhs == _startSymbol) {
					continue;
				}
				if (production.IsEmpty) {
					productions.RemoveAt(i);
				}
			}
		}

		/// <summary>
		/// From a production, derive a set of productions for each combination of skipping nullable nonterminals.
		/// E.g., for production S -> AbB and nullable {A, B}, we get productions
		/// S -> AbB | Ab | bB | b
		/// </summary>
		/// <param name="originalProduction"></param>
		/// <param name="nullableSet"></param>
		/// <returns></returns>
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
		private Nonterminal GetFresh() {
			var result = Nonterminal.Of("X_" + _freshx);
			_freshx++;
			return result;
		}
		
		private static Dictionary<Nonterminal, ICollection<BaseProduction>> BuildLookupTable(ISet<BaseProduction> productions) {
			Dictionary<Nonterminal, ICollection<BaseProduction>> table;

			table = Helpers.BuildLookup(
				() => productions,
				(p) => p.Lhs,
				(p) => p,
				() => (ICollection<BaseProduction>)new List<BaseProduction>(),
				(l, p) => l.Add(p)
			);
			return table;
		}
	}
}
