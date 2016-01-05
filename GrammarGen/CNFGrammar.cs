using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrammarGen {
	class CNFGrammar {
		List<CNFNonterminalProduction> _nonterminalProductions = new List<CNFNonterminalProduction>();
		List<CNFTerminalProduction> _terminalProductions = new List<CNFTerminalProduction>();
		bool _producesEmpty = false;
		Variable _start = Variable.Of("S");

		public CNFGrammar(Grammar grammar) {
			// var productions = CloneGrammar(grammar);
			var productions = grammar.Productions;
			StepStart(productions);
			Console.WriteLine(grammar);
			StepTerm(productions);
			Console.WriteLine(grammar);
			StepBin(productions);
			Console.WriteLine(grammar);
			StepDel(productions);
			Console.WriteLine(grammar);
			StepUnit(productions);
			Console.WriteLine(grammar);
			//foreach (var production in productions) {
			//	if (production.Rhs.Count > 2) {
			//		throw new Exception("Didn't expect more than 2");
			//	} else if (production.Rhs.Count == 2) {
			//		_nonterminalProductions.Add(new CNFNonterminalProduction(production));
			//	} else if (production.Rhs.Count == 1) {
			//		var rhs = production.Rhs[0];
			//		_terminalProductions.Add(new CNFTerminalProduction(production));
			//	} else if (production.Rhs.Count == 0) {
			//		_producesEmpty = true;
			//	}
			//}
		}	

		private void StepStart(List<Production> productions) {
			var fresh = Getfresh();
			productions.Add(
				new Production(fresh, new Sentence { Variable.Of("S") })
			);
			_start = fresh;
		}

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

		private void StepBin(List<Production> productions) {
			// List<Production> newProductions = new List<Production>();
			List<Production> finalProductions = new List<Production>();
			foreach (var production in productions) {
				if (production.Rhs.Count < 3) {
					finalProductions.Add(production);
					continue;
				}
				var rhs = production.Rhs;
				var curr = production.Lhs;
				for (int i = 0; i < rhs.Count-2; i++) {
					var left = rhs[i];
					var newFresh = Getfresh();
					finalProductions.Add(
						new Production(curr, new Sentence { left, newFresh })
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

		private void StepUnit(List<Production> productions) {
			bool changed = true;
			while (changed) {
				changed = StepUnitOnce(productions);
			}
		}

		private bool StepUnitOnce(List<Production> productions) {
			var table = BuildLookupTable(productions);
			var result = new List<Production>(productions);
			var changed = false;
			
			//if (productions.Count > 100) {
			//	throw new Exception("Something's gone wrong");
			//}

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

		private Dictionary<Variable, List<Production>> BuildLookupTable(List<Production> productions) {
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

		private ISet<Variable> GetNullable(List<Production> originalProductions) {
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
					for (int i = production.Rhs.Count-1; i >= 0; i--) {
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
					if (production.Lhs != _start) {
						productions.RemoveAt(i);
					}
				}
				// results.Add(production);
			}
		}

		private List<Production> Nullate(Production originalProduction, ISet<Variable> nullableSet) {
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
		private int _freshx = 0;
		private Variable Getfresh() {
			var result = Variable.Of("X_" + _freshx);
			_freshx++;
			return result;
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


		public override string ToString() {
			var retval = "CNFGrammar{\n";

			foreach (var production in _nonterminalProductions) {
				retval += "  " + production.ToString() + "\n";
			}
			foreach (var production in _terminalProductions) {
				retval += "  " + production.ToString() + "\n";
			}
			if (_producesEmpty) {
				retval += "  " + _start + " → ε";
			}
			retval += "}\n";
			return retval;
		}
	}
}
