using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib {
	internal class GrammarHelpers {
		private const double _magicStartProbability = 2.0;

		public static ISet<Production> CloneGrammar(Grammar grammar) {
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

		// TODO: should probably use this for BaseGrammar.GetNonterminals()
		private static ISet<Nonterminal> GetNonterminals(ISet<Production> productions) {
			var hs = new HashSet<Nonterminal>();
			foreach (var production in productions) {
				hs.Add(production.Lhs);
				foreach (var word in production.Rhs) {
					var nonterminal = word as Nonterminal;
					if (nonterminal != null) {
						hs.Add(nonterminal);
					}
				}
			}
			return hs;
		}

		/// <summary>
		/// Returns a dictionary containing the probability that any particular nonterminal yields ε
		/// </summary>
		internal static Dictionary<Nonterminal, double> GetNullable(ISet<Production> originalProductions) {
			var results = new Dictionary<Nonterminal, double>();

			var productionByNt = BuildLookupTable(originalProductions);
			var nonterminals = GetNonterminals(originalProductions);
			foreach (var nt in nonterminals) {
				ICollection<Production> productions;
				if (!productionByNt.TryGetValue(nt, out productions)) {
					productionByNt[nt] = new List<Production>();
				}
			}

			var indexToNonterminal = nonterminals.ToArray();
			var nonterminalToIndex = new Dictionary<Nonterminal, int>();
			for (int i = 0; i < nonterminals.Count; i++) {
				nonterminalToIndex[indexToNonterminal[i]] = i;
			}

			// seeding the estimates with a magic value
			// this keeps all iterations behaving the same
			var previousEstimates = Enumerable.Repeat(_magicStartProbability, indexToNonterminal.Length).ToArray();
			var currentEstimates = new double[indexToNonterminal.Length];

			// figure out which nonterminals are definitely not nullable, and go ahead and set them that way
			var nullableNonterminals = GetNullableNonterminals(originalProductions);
			foreach (var nt in nonterminals) {
				if (!nullableNonterminals.Contains(nt)) {
					previousEstimates[nonterminalToIndex[nt]] = 0.0;
				}
			}

			bool changed = true;
			while (changed == true) {
				changed = false;
				Array.Clear(currentEstimates, 0, currentEstimates.Length);

				// consider each nonterminal.  calculate a new nullable estimate based on the previous
				for (int i = 0; i < currentEstimates.Length; i++) {
					var nt = indexToNonterminal[i];

					var productions = productionByNt[nt];
					double weightSum = 0.0; // productions.Sum((p) => p.Weight);
					foreach (var production in productions) {
						var productionWeight = production.Weight;
						weightSum += productionWeight;
						var innerProb = GetProductionProbability(production, nonterminalToIndex, previousEstimates);
						currentEstimates[i] += productionWeight * innerProb;
					}
					if (weightSum == 0.0) {
						currentEstimates[i] = 0.0;
					} else {
						currentEstimates[i] /= weightSum;
					}
					
					if (currentEstimates[i] > previousEstimates[i]) {
						throw new Exception("Didn't expect estimates to increase");
					} else if (currentEstimates[i] < previousEstimates[i]) {
						changed = true;
					}
				}
				Helpers.Swap(ref previousEstimates, ref currentEstimates);
			}

			for (int i = 0; i < nonterminals.Count; i++) {
				var nt = indexToNonterminal[i];
				results[nt] = previousEstimates[i];
			}

			return results;
		}

		private static ISet<Nonterminal> GetNullableNonterminals(ISet<Production> originalProductions) {
			var productions = CloneProductions(originalProductions);
			ISet<Production> newProductions = new HashSet<Production>();
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

		// TODO: slow
		//private static ISet<Nonterminal> GetReachesTerminal(ISet<Production> originalProductions) {

		//	var reachesTerminal = new HashSet<Nonterminal>();
		//	var oldCount = -1;
		//	while (oldCount != reachesTerminal.Count) {
		//		oldCount = reachesTerminal.Count;
		//		foreach (var production in originalProductions) {
		//			if (!production.Rhs.OnlyNonterminals()) {
		//				reachesTerminal.Add(production.Lhs);
		//			}
		//			foreach (var nt in production.Rhs) {
		//				if (reachesTerminal.Contains(nt)) {
		//					reachesTerminal.Add(production.Lhs);
		//				}
		//			}
		//		}
		//	}
		//	return reachesTerminal;
		//}

		private static double GetProductionProbability(Production production, Dictionary<Nonterminal, int> nonterminalToIndex, double[] previousEstimates) {
			if (production.Rhs.Count == 0) {
				return 1.0;
			}
			// if it contains a terminal, then it always is non-empty
			if (!production.Rhs.OnlyNonterminals()) {
				return 0.0;
			}

			var product = 1.0;
			foreach (var word in production.Rhs) {
				var nt = (Nonterminal)word;
				var rhsIndex = nonterminalToIndex[nt];
				var previous = previousEstimates[rhsIndex];
				// if this is the first iteration, we assume that the previous values were 100% chance of yielding null
				if (previous == _magicStartProbability) {
					previous = 1.0;
				}
				product *= previous;
			}
			if (double.IsNaN(product)) {
				throw new Exception("Didn't expect to get NaN probability");
			}
			return product;
		}

		internal static Dictionary<Nonterminal, ICollection<Production>> BuildLookupTable(ICollection<Production> productions) {
			Dictionary<Nonterminal, ICollection<Production>> table;

			table = Helpers.BuildLookup(
				() => productions,
				(p) => p.Lhs,
				(p) => p,
				() => (ICollection<Production>)new List<Production>(),
				(l, p) => l.Add(p)
			);
			return table;
		}
	}
}
