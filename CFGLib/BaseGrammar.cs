using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib {
	/// <summary>
	/// Represents an abstract (probabilistic) context free grammar
	/// </summary>
	public abstract class BaseGrammar {
		private Random _rand = new Random(0);
		private Nonterminal _start;
		private List<IDirtyable> _caches = new List<IDirtyable>();
		private Cache<Dictionary<Nonterminal, Boxed<double>>> _weightTotalsByNonterminal;
		private Cache<ISet<Nonterminal>> _nonterminals;
		private Cache<ISet<Terminal>> _terminals;

		public abstract IEnumerable<BaseProduction> Productions {
			get;
		}
		
		/// <summary>
		/// The start symbol
		/// </summary>
		public Nonterminal Start {
			get { return _start; }
			protected set { _start = value; }
		}

		protected List<IDirtyable> Caches {
			get {
				return _caches;
			}
		}

		protected void BuildHelpers() {
			_weightTotalsByNonterminal = Cache.Create(() => Helpers.BuildLookup(
				() => this.Productions,
				(p) => p.Lhs,
				(p) => p.Weight,
				() => new Boxed<double>(0.0),
				(x, y) => x.Value += y
			));
			this.Caches.Add(_weightTotalsByNonterminal);

			_nonterminals = Cache.Create(() => {
				var hs = new HashSet<Nonterminal>();
				foreach (var production in this.Productions) {
					hs.Add(production.Lhs);
					foreach (var word in production.Rhs) {
						var nonterminal = word as Nonterminal;
						if (nonterminal != null) {
							hs.Add(nonterminal);
						}
					}
				}
				return (ISet<Nonterminal>)hs;
			});
			this.Caches.Add(_nonterminals);

			_terminals = Cache.Create(() => {
				var hs = new HashSet<Terminal>();
				foreach (var production in this.Productions) {
					foreach (var word in production.Rhs) {
						var terminal = word as Terminal;
						if (terminal != null) {
							hs.Add(terminal);
						}
					}
				}
				return (ISet<Terminal>)hs;
			});
			this.Caches.Add(_terminals);
		}

		internal abstract IEnumerable<BaseProduction> ProductionsFrom(Nonterminal lhs);

		/// <summary>
		/// Call this after making any changes to the grammar so that cached lookup tables can be rebuilt
		/// </summary>
		public void InvalidateCaches() {
			foreach (var cache in this.Caches) {
				cache.SetDirty();
			}
		}

		/// <summary>
		/// Returns all the sentences (with their probabilities) that can be generated up to a certain depth
		/// </summary>
		/// <param name="depth"></param>
		/// <returns></returns>
		public List<Probable<Sentence>> ProduceToDepth(int depth, int limit = int.MaxValue) {
			var start = new Sentence { this.Start };
			var intermediate = new List<Probable<Sentence>>[depth + 1];
			var startSWP = new Probable<Sentence>(1.0, start);
			intermediate[0] = new List<Probable<Sentence>> { startSWP };

			int count = 0;
			for (int i = 0; i < depth; i++) {
				if (count >= limit) {
					break;
				}
				var prev = intermediate[i];
				var next = new List<Probable<Sentence>>();
				intermediate[i + 1] = next;
				foreach (var swp in prev) {
					if (!swp.Value.OnlyTerminals()) {
						var steps = GoOneStep(swp);
						next.AddRange(steps);
						count += steps.Count;
					}
					if (count >= limit) {
						break;
					}
				}
			}
			var results = new List<Probable<Sentence>>();
			// TODO: terrible :(
			var resultDict = new Dictionary<string, Probable<Sentence>>();
			foreach (var step in intermediate) {
				if (step == null) { continue; }
				foreach (var swp in step) {
					if (!swp.Value.OnlyTerminals()) {
						continue;
					}
					Probable<Sentence> psentence;
					var stringrep = swp.Value.ToString();
					if (!resultDict.TryGetValue(stringrep, out psentence)) {
						psentence = new Probable<Sentence>(0.0, swp.Value);
						resultDict[stringrep] = psentence;
					}
					psentence.Probability += swp.Probability;
					// results.Add(swp);
				}
			}

			return resultDict.Values.ToList();
		}

		private List<Probable<Sentence>> GoOneStep(Probable<Sentence> swp) {
			var start = new Probable<Sentence>(swp.Probability, new Sentence());
			var results = new List<Probable<Sentence>> { start };
			foreach (var word in swp.Value) {
				if (word.IsNonterminal()) {
					results = StepNonterminal(results, word);
				} else {
					results = StepTerminal(results, word);
				}
			}
			return results;
		}

		private List<Probable<Sentence>> StepNonterminal(List<Probable<Sentence>> results, Word word) {
			var newResults = new List<Probable<Sentence>>();

			var nonterminal = (Nonterminal)word;
			foreach (var production in ProductionsFrom(nonterminal)) {
				var prob = GetProbability(production);
				var copies = DeepCopy(results);
				foreach (var copy in copies) {
					copy.Value.AddRange(production.Rhs);
					copy.Probability *= prob;
				}
				newResults.AddRange(copies);
			}
			return newResults;
		}

		private List<Probable<Sentence>> StepTerminal(List<Probable<Sentence>> results, Word word) {
			foreach (var result in results) {
				result.Value.Add(word);
			}
			return results;
		}

		private List<Probable<Sentence>> DeepCopy(List<Probable<Sentence>> sentences) {
			var results = new List<Probable<Sentence>>();
			foreach (var sentence in sentences) {
				results.Add(new Probable<Sentence>(sentence.Probability, new Sentence(sentence.Value)));
			}
			return results;
		}
		
		protected double GetProbability(BaseProduction target) {
			var lhs = target.Lhs;
			var weight = target.Weight;
			// 9.23s calculating all
			// 7.06 accessing target
			// 6.4 doing none
			var lookupTable = _weightTotalsByNonterminal.Value;
			var boxedSum = lookupTable[lhs];
			double weightTotal = boxedSum.Value;
			return weight / weightTotal;
		}
		
		// TODO: need to check that this is unbiased
		internal Sentence ProduceNonterminal(Nonterminal v) {
			Sentence result = null;

			var productions = ProductionsFrom(v);

			var totalWeight = productions.Sum(w => w.Weight);
			var targetValue = totalWeight * _rand.NextDouble();

			var currentWeight = 0.0;
			foreach (var production in productions) {
				currentWeight += production.Weight;
				if (currentWeight < targetValue) {
					continue;
				}
				result = new Sentence(production.Rhs);
				break;
			}

			Debug.Assert(result != null);
			return result;
		}
		
		/// <summary>
		/// Returns a list of all the nonterminals used anywhere in the productions
		/// </summary>
		/// <returns></returns>
		public ISet<Nonterminal> GetNonterminals() {
			return _nonterminals.Value;
		}
		/// <summary>
		/// Returns a list of all the terminals used anywhere in the productions
		/// </summary>
		/// <returns></returns>
		public ISet<Terminal> GetTerminals() {
			return _terminals.Value;
		}
		
		// TODO: MIGHT NOT TERMINATE!
		/// <summary>
		/// Returns a random sentence produced by this grammar.
		/// </summary>
		/// <returns></returns>
		public Sentence ProduceRandom() {
			var sentence = new Sentence { this.Start };
			while (sentence.ContainsNonterminal()) {
				// for (int i = 0; i < sentence.Count; i++) {
				for (int i = sentence.Count-1; i >= 0; i--) {
					var word = sentence[i];
					if (!word.IsNonterminal()) {
						continue;
					}
					var newStuff = ProduceNonterminal((Nonterminal)word);
					sentence.RemoveAt(i);
					sentence.InsertRange(i, newStuff);
				}
			}
			return sentence;
		}

		/// <summary>
		/// Tries to estimate the probability of the sentences this grammar can generate by generating a bunch randomly and counting.
		/// </summary>
		/// <param name="iterations"></param>
		public void EstimateProbabilities(long iterations) {
			var dict = new Dictionary<string, long>();
			for (int i = 0; i < iterations; i++) {
				var key = this.ProduceRandom().AsTerminals("");
				if (!dict.ContainsKey(key)) {
					dict[key] = 0;
				}
				dict[key]++;
			}
			List<KeyValuePair<string, long>> listPairs = dict.ToList();
			listPairs.Sort((firstPair, nextPair) => {
				return nextPair.Value.CompareTo(firstPair.Value);
			}
			);
			foreach (var entry in listPairs) {
				var key = entry.Key;
				var value = entry.Value;
				Console.WriteLine("{0}: {1}", key, (double)value / iterations);
			}
		}

		/// <summary>
		/// Simplifies the grammar by removing duplicate productions as well as productions involving unreachable and unproductive nonterminals
		/// </summary>
		public void Simplify() {
			SimplifyWithoutInvalidate();
			InvalidateCaches();
		}
		
		protected void SimplifyWithoutInvalidate() {
			int oldCount;
			do {
				oldCount = this.Productions.Count();
				RemoveUnreachable();
				RemoveUnproductive();
			} while (oldCount != this.Productions.Count());
		}

		protected void RemoveDuplicates() {
			var productionList = new List<BaseProduction>(this.Productions);
			var toRemove = new List<BaseProduction>();

			for (int i = 0; i < productionList.Count; i++) {
				var production = productionList[i];
				for (int j = i + 1; j < productionList.Count; j++) {
					var candidate = productionList[j];
					if (production.Lhs != candidate.Lhs) {
						continue;
					}
					if (candidate.Rhs.SequenceEqual(production.Rhs)) {
						production.Weight += candidate.Weight;
						toRemove.Add(candidate);
					}
				}
			}
			this.RemoveProductions(toRemove);
		}
		
		protected void RemoveUnreachable() {
			var reachableSymbols = new HashSet<Nonterminal>();
			
			var currReachableSymbols = new HashSet<Nonterminal> { this.Start };
			while (currReachableSymbols.Count > 0) {
				reachableSymbols.UnionWith(currReachableSymbols);
				var newReachableSymbols = new HashSet<Nonterminal>();
				foreach (var production in this.Productions) {
					if (!currReachableSymbols.Contains(production.Lhs)) {
						continue;
					}
					foreach (var word in production.Rhs) {
						if (word is Terminal) {
							continue;
						}
						var nt = (Nonterminal)word;
						if (reachableSymbols.Contains(nt)) {
							continue;
						}
						newReachableSymbols.Add(nt);
					}
				}
				currReachableSymbols = newReachableSymbols;
			}

			RemoveProductionsContainingOtherThan(reachableSymbols);
		}

		// TODO: very inefficient
		protected void RemoveUnproductive() {
			var productiveSymbols = new HashSet<Nonterminal>();

			bool changed = true;
			while (changed) {
				changed = false;
				foreach (var production in this.Productions) {
					// if we already know a symbol is productive, we can move on
					if (productiveSymbols.Contains(production.Lhs)) {
						continue;
					}
					if (!IsProductive(production, productiveSymbols)) {
						continue;
					}
					productiveSymbols.Add(production.Lhs);
					changed = true;
				}
			}

			RemoveProductionsContainingOtherThan(productiveSymbols);
		}

		private void RemoveProductionsContainingOtherThan(ISet<Nonterminal> productiveSymbols) {
			var toRemove = new HashSet<BaseProduction>();
			foreach (var production in this.Productions) {
				// remove rule if LHS is nonproductive
				if (!productiveSymbols.Contains(production.Lhs)) {
					toRemove.Add(production);
					continue;
				}
				// remove rule if RHS contains nonproductive
				foreach (var word in production.Rhs) {
					if (word is Terminal) {
						continue;
					}
					var nt = (Nonterminal)word;
					if (!productiveSymbols.Contains(word)) {
						toRemove.Add(production);
					}
				}
			}
			RemoveProductions(toRemove);
		}

		private bool IsProductive(BaseProduction production, HashSet<Nonterminal> productiveSymbols) {
			foreach (var word in production.Rhs) {
				if (word is Terminal) {
					continue;
				}
				var nt = (Nonterminal)word;
				if (!productiveSymbols.Contains(nt)) {
					return false;
				}
			}
			return true;
		}

		internal abstract void RemoveProductions(IEnumerable<BaseProduction> toRemove);
		
		public override string ToString() {
			var retval = "Grammar(" + this.Start + "){\n";

			foreach (var production in this.Productions) {
				var prob = GetProbability(production);
				retval += string.Format("  {1:0.00e+000}: {0}\n", production.ToString(), prob);
			}
			retval += "}\n";
			return retval;
		}
	}
}
