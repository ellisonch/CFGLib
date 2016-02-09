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
		private bool _used = false;
		private const double _removeUnitProbabilityThreshold = 0;

		private ISet<ValueUnitProduction> _unitPreviouslyDeleted = new HashSet<ValueUnitProduction>();

		internal CFGtoCNF(Grammar grammar) {
			_grammar = grammar;

		}

		/// <summary>
		/// Actually performs the conversion and returns a new CNF grammar based on the old grammar
		/// </summary>
		internal CNFGrammar Convert() {
			if (_used) {
				throw new Exception("You can only use this object once");
			}
			_used = true;

			var productions = CloneGrammar(_grammar);
			StepStart(productions);
			StepTerm(productions);
			StepBin(productions);
			StepDel(productions);
			StepUnit(productions);

			var resultProductions = new List<Production>();
			//var nonterminalProductions = new List<Production>();
			//var terminalProductions = new List<Production>();
			var producesEmptyWeight = 0.0;
			
			foreach (var production in productions) {
				if (production.Rhs.Count > 2) {
					throw new Exception("Didn't expect more than 2");
				} else if (production.Rhs.Count == 2) {
					resultProductions.Add(new CNFNonterminalProduction(production));
				} else if (production.Rhs.Count == 1) {
					var rhs = production.Rhs[0];
					resultProductions.Add(new CNFTerminalProduction(production));
				} else if (production.Rhs.Count == 0) {
					producesEmptyWeight += production.Weight;
						// GetGrammarFromProductionList(production, productions);
				}
			}

			resultProductions.Add(Production.New(_startSymbol, new Sentence(), producesEmptyWeight));

			return new CNFGrammar(resultProductions, _startSymbol);
		}

		private static ISet<Production> CloneGrammar(Grammar grammar) {
			return CloneProductions(grammar.Productions);
		}

		private static ISet<Production> CloneProductions(IEnumerable<Production> productions) {
			var result = new HashSet<Production>();
			foreach (var production in productions) {
				// var productions = grammar.Productions;
				result.Add(production.DeepClone());
			}
			return result;
		}

		/// <summary>
		/// Eliminate the start symbol from right-hand sides
		/// </summary>
		/// <param name="productions"></param>
		private void StepStart(ISet<Production> productions) {
			var fresh = GetFresh();
			productions.Add(
				// new Production(fresh, new Sentence { Nonterminal.Of("S") })
				new DefaultProduction(fresh, new Sentence { _grammar.Start })
			);
			_startSymbol = fresh;
		}
		
		/// <summary>
		/// Eliminate rules with nonsolitary terminals
		/// </summary>
		/// <param name="productions"></param>
		private void StepTerm(ISet<Production> productions) {
			var newProductions = new List<Production>();
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
							new DefaultProduction(fresh, new Sentence { terminal })
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
		private void StepBin(ISet<Production> productions) {
			List<Production> finalProductions = new List<Production>();
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
						new DefaultProduction(curr, new Sentence { left, newFresh }, weight)
					);
					curr = newFresh;
				}
				finalProductions.Add(
					new DefaultProduction(curr, new Sentence { rhs[rhs.Count - 2], rhs[rhs.Count - 1] })
				);
			}
			productions.Clear();
			productions.UnionWith(finalProductions);
		}

		/// <summary>
		/// Eliminate ε-rules
		/// </summary>
		/// <param name="productions"></param>
		// TODO: Does not preserve weights
		private void StepDel(ISet<Production> productions) {
			var nullableProbabilities = GrammarHelpers.GetNullable(productions);
			var newRules = new List<Production>();
			foreach (var production in productions) {
				var toAdd = Nullate(production, nullableProbabilities);
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
		private void StepUnit(ISet<Production> productions) {
			// var previouslyDeleted = new HashSet<ValueUnitProduction>();
			// TODO: maybe we shouldn't allow self loops?
			RemoveSelfLoops(productions);

			var toRemove = BaseGrammar.RemoveDuplicatesHelper(productions);
			foreach (var production in toRemove) {
				productions.Remove(production);
			}

			bool changed = true;
			while (changed) {
				changed = StepUnitOnce(productions);
			}
		}

		private static void RemoveSelfLoops(ISet<Production> productions) {
			var toDelete = new List<Production>();
			foreach (var production in productions) {
				if (production.IsSelfLoop) {
					toDelete.Add(production);
				}
			}
			foreach (var item in toDelete) {
				productions.Remove(item);
			}			
		}

		private bool StepUnitOnce(ISet<Production> productions) {
			foreach (var production in productions) {
				var lhs = production.Lhs;
				if (production.Rhs.Count != 1) {
					continue;
				}
				var rhs = production.Rhs[0];
				if (rhs.IsTerminal()) {
					continue;
				}
				// at this point we have a unit production lhs -> rhs
				HandleIndividualUnit(productions, production);
				return true;
			}
			return false;
		}

		private void HandleIndividualUnit(ISet<Production> productions, Production production) {
			var table = GrammarHelpers.BuildLookupTable(productions);
			var results = new HashSet<Production>(productions);
			var newProductions = new HashSet<Production>();
			results.Remove(production);
			var unitLhs = production.Lhs;
			var unitRhs = (Nonterminal)production.Rhs[0];
			var vup = new ValueUnitProduction(unitLhs, unitRhs);
			_unitPreviouslyDeleted.Add(vup);

			var entries = table.LookupEnumerable(unitRhs);

			foreach (var entry in entries) {
				var toAdd = HandleUnitBackwards(production, entry, entries, table);
				newProductions.UnionWith(toAdd);
			}

			MergeProductions(results, newProductions);
			productions.Clear();
			productions.UnionWith(results);
		}

		private ISet<Production> HandleUnitBackwards(Production production, Production entry, IEnumerable<Production> entries, Dictionary<Nonterminal, ICollection<Production>> table) {
			var sum = entries.Sum((p) => p.Weight);
			var newProductions = new HashSet<Production>();
			var probThisEntry = entry.Weight / sum;
			// TODO: I've seen this create a 0 weight
			var newProd = new DefaultProduction(production.Lhs, entry.Rhs, production.Weight * probThisEntry);

			// Console.WriteLine("considering {0} and {1}", production, entry);

			if (newProd.IsSelfLoop) {
				return newProductions;
			}
			// TODO: this case messes up probability
			if (UnitPreviouslyDeleted(newProd)) {
				var secondaryProductions = table.LookupEnumerable((Nonterminal)entry.Rhs[0]);
				var secondarySum = secondaryProductions.Sum((p) => p.Weight);
				foreach (var secondaryProduction in secondaryProductions) {
					var secondaryProbThisEntry = secondaryProduction.Weight / secondarySum;
					var newWeight = newProd.Weight * secondaryProbThisEntry;
					if (newWeight <= _removeUnitProbabilityThreshold) {
						continue;
						// Console.WriteLine("foo");
					}
					var newnewProd = new DefaultProduction(production.Lhs, secondaryProduction.Rhs, newWeight);
					if (UnitPreviouslyDeleted(newnewProd)) {
						// Console.WriteLine("foo");
					}
					newProductions.Add(newnewProd);
				}
				
				return newProductions;
			}

			newProductions.Add(newProd);
			return newProductions;
		}

		private bool UnitPreviouslyDeleted(Production newProd) {
			if (newProd.Rhs.Count == 1) {
				if (newProd.Rhs[0].IsNonterminal()) {
					var vup = new ValueUnitProduction(newProd.Lhs, (Nonterminal)newProd.Rhs[0]);
					if (_unitPreviouslyDeleted.Contains(vup)) {
						//// we're trying to add vup.Lhs -> vup.Rhs
						//// but we don't want to add a unit we've already deleted
						//// we need to divide up the newProd.weight among all the 
						//// rules vup.Lhs -> X where vup.Rhs -> X
						// var destinations = table.LookupEnumerable(vup.Rhs);
						//var newSum = destinations.Sum((p) => p.Weight);
						//foreach (var relayed in destinations) {
						//	var newProb = relayed.Weight / newSum;
						//	var relayedProd = new DefaultProduction(lhs, entry.Rhs, production.Weight * probThisEntry);
						//	results.Add()
						//}
						return true;
					}
				}
			}
			return false;
		}

		// TODO need a structure for this to make it fast
		private static void MergeProductions(ISet<Production> results, ISet<Production> b) {
			foreach (var newProd in b) {
				bool found = false;
				foreach (var result in results) {
					if (result.Lhs != newProd.Lhs) {
						continue;
					}
					if (result.Rhs.SequenceEqual(newProd.Rhs)) {
						found = true;
						result.Weight += newProd.Weight;
						break;
					}
				}
				if (!found) {
					results.Add(newProd);
				}
			}
		}

		/// <summary>
		/// Remove &lt;A> -> ε unless A is the start symbol 
		/// </summary>
		/// <param name="productions"></param>
		private void RemoveExtraneousNulls(List<Production> productions) {
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
		/// <returns></returns>
		private static List<Production> Nullate(Production originalProduction, Dictionary<Nonterminal, double> nullableProbabilities) {
			var results = new List<Production>();
			results.Add(originalProduction);
			if (originalProduction.IsEmpty) {
				return results;
			}
			for (int i = originalProduction.Rhs.Count - 1; i >= 0; i--) {
				var newResults = new List<Production>();
				var toRemove = new List<Production>();
				foreach (var production in results) {
					var word = production.Rhs[i];
					var nt = word as Nonterminal;
					if (nt == null) { continue; }
					if (!nullableProbabilities.ContainsKey(nt)) {
						continue;
					}
					// var with = production.Clone();
					var without = production.DeepClone();
					without.Rhs.RemoveAt(i);
					
					var chanceNull = nullableProbabilities[nt];
					var newWithoutWeight = without.Weight * chanceNull;
					var newWithWeight = production.Weight * (1.0 - chanceNull);
					
					if (newWithoutWeight > 0.0) {
						without.Weight = newWithoutWeight;
						newResults.Add(without);
					}
					if (newWithWeight <= 0.0) {
						toRemove.Add(production);
					} else {
						production.Weight = newWithWeight;
					}
				}
				results.AddRange(newResults);
				// TODO: we should just make it so that if a weight is set to 0, the production gets removed from the grammar automatically, and that operation should be fast
				results.RemoveMany(toRemove);
			}
			// NullateAux(production, nullableSet, 0, result);

			if (results.Count == 0) {
				return results;
			}
			// Get rid of productions with zero weight
			//for (int i = results.Count - 1; i >= 0;  i--) {
			//	var result = results[i];
			//	if (result.Weight == 0.0) {
			//		results.RemoveAt(i);
			//	}
			//}
			return results;
		}

		// todo: horrible
		private Nonterminal GetFresh() {
			var originalNonterminals = _grammar.GetNonterminals();
			Nonterminal result;
			do {
				result = Nonterminal.Of("X_" + _freshx);
				_freshx++;
			} while (originalNonterminals.Contains(result));
			return result;
		}
	}
}
