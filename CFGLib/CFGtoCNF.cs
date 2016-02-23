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

			var productions = GrammarHelpers.CloneGrammar(_grammar);
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
					resultProductions.Add(production);
				} else if (production.Rhs.Count == 1) {
					var rhs = production.Rhs[0];
					if (rhs.IsNonterminal) {
						throw new Exception("Didn't expect unit production");
					}
					resultProductions.Add(production);
				} else if (production.Rhs.Count == 0) {
					producesEmptyWeight += production.Weight;
						// GetGrammarFromProductionList(production, productions);
				}
			}

			resultProductions.Add(Production.New(_startSymbol, new Sentence(), producesEmptyWeight));

			return new CNFGrammar(resultProductions, _startSymbol);
		}

		/// <summary>
		/// Eliminate the start symbol from right-hand sides
		/// </summary>
		/// <param name="productions"></param>
		private void StepStart(ISet<Production> productions) {
			var fresh = GetFresh();
			productions.Add(
				// new Production(fresh, new Sentence { Nonterminal.Of("S") })
				Production.New(fresh, new Sentence { _grammar.Start })
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
					if (word.IsNonterminal) {
						continue;
					}
					Terminal terminal = (Terminal)word;
					Nonterminal fresh;
					if (!lookup.TryGetValue(terminal, out fresh)) {
						fresh = GetFresh();
						lookup[terminal] = fresh;
						newProductions.Add(
							Production.New(fresh, new Sentence { terminal })
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
						Production.New(curr, new Sentence { left, newFresh }, weight)
					);
					curr = newFresh;
				}
				finalProductions.Add(
					Production.New(curr, new Sentence { rhs[rhs.Count - 2], rhs[rhs.Count - 1] })
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
			// TODO: maybe we shouldn't allow self loops?
			RemoveSelfLoops(productions);

			var toRemove = BaseGrammar.RemoveDuplicatesHelper(productions);
			foreach (var production in toRemove) {
				productions.Remove(production);
			}

			var finalProductions = RemoveUnits(productions);
			productions.Clear();
			productions.UnionWith(finalProductions);
		}

		private IEnumerable<Production> RemoveUnits(IEnumerable<Production> productions) {
			var productionTable = new Dictionary<Production, Production>(new ProductionComparer());
			foreach (var production in productions) {
				productionTable[production] = production;
			}

			var oldSumOfProbability = double.MaxValue;

			// we keep looping until the probability of any unit production has been driven to zero
			// as an invariant, we make sure that the sum of the unit probabilities goes down each iteration
			while (oldSumOfProbability > 0) {
				// TODO: don't need to build this table every round
				var productionsByNonterminal = GrammarHelpers.BuildLookupTable(productionTable.Keys);
				var newSumOfProbability = 0.0;

				var toAdd = new List<Production>();
				var toRemove = new List<Production>();

				// find all the unit productions and replace them with equivalent rules
				// X -> Y gets replaced with rules X -> Z for all Y -> Z
				foreach (var production in productionTable.Keys) {
					if (production.IsUnit()) {
						var thisProb = GetProbability(production, productionsByNonterminal);
						if (double.IsNaN(thisProb)) {
							continue;
						}
						newSumOfProbability += thisProb;
						var replacements = UnitReplacementProductions(production, productionsByNonterminal);
						toAdd.AddRange(replacements);
						toRemove.Add(production);
					}
				}
				if (oldSumOfProbability < newSumOfProbability) {
					throw new Exception("Invariant didn't hold, we want probability sums to decrease every iteration");
				}
				oldSumOfProbability = newSumOfProbability;

				foreach (var production in toRemove) {
					production.Weight = 0.0;
				}
				MergeProductions(productionTable, toAdd);
			}

			return productionTable.Keys.Where((p) => p.Weight > 0.0);
		}

		private IEnumerable<Production> UnitReplacementProductions(Production unitProduction, Dictionary<Nonterminal, ICollection<Production>> productionsByNonterminal) {
			var retval = new List<Production>();

			var productions = productionsByNonterminal.LookupEnumerable((Nonterminal)unitProduction.Rhs[0]);

			foreach (var production in productions) {
				var productionProb = GetProbability(production, productionsByNonterminal);
				var newWeight = unitProduction.Weight * productionProb;
				var newProduction = Production.New(unitProduction.Lhs, production.Rhs, newWeight);
				if (newProduction.IsSelfLoop) {
					continue;
				}
				retval.Add(newProduction);
			}

			return retval;
		}

		private double GetProbability(Production production, Dictionary<Nonterminal, ICollection<Production>> productionsByNonterminal) {
			var sum = productionsByNonterminal.LookupEnumerable(production.Lhs).Sum((p) => p.Weight);
			return production.Weight / sum;
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
		
		private static void MergeProductions(Dictionary<Production, Production> results, IEnumerable<Production> toAdd) {
			foreach (var newProduction in toAdd) {
				Production existingProduction;
				if (results.TryGetValue(newProduction, out existingProduction)) {
					existingProduction.Weight += newProduction.Weight;
				} else {
					results[newProduction] = newProduction;
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
