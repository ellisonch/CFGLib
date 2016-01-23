using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib {
	public abstract class BaseGrammar {
		public abstract IEnumerable<BaseProduction> Productions {
			get;
		}
		private Nonterminal _start;
		public Nonterminal Start {
			get { return _start; }
			protected set { _start = value; }
		}

		private List<IDirtyable> _caches = new List<IDirtyable>();
		protected List<IDirtyable> Caches {
			get {
				return _caches;
			}
		}

		private Random _rand = new Random(0);
		
		private Cache<Dictionary<Nonterminal, Boxed<long>>> _weightTotalsByNonterminal;
		private Cache<ISet<Nonterminal>> _nonterminals;
		private Cache<ISet<Terminal>> _terminals;

		protected void BuildHelpers() {
			// Dictionary<Nonterminal, long>
			_weightTotalsByNonterminal = Cache.Create(() => Helpers.BuildLookup(
				() => this.Productions,
				(p) => p.Lhs,
				(p) => p.Weight,
				() => new Boxed<long>(0L),
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


		public void InvalidateCaches() {
			foreach (var cache in this.Caches) {
				cache.SetDirty();
			}
		}

		public List<SentenceWithProbability> ProduceToDepth(int depth) {
			var start = new Sentence { this.Start };
			var intermediate = new List<SentenceWithProbability>[depth + 1];
			var startSWP = new SentenceWithProbability(1.0, start);
			intermediate[0] = new List<SentenceWithProbability> { startSWP };

			for (int i = 0; i < depth; i++) {
				var prev = intermediate[i];
				var next = new List<SentenceWithProbability>();
				intermediate[i + 1] = next;
				foreach (var swp in prev) {
					if (!swp.Sentence.OnlyTerminals()) {
						var steps = GoOneStep(swp);
						next.AddRange(steps);
					}
				}
			}
			var results = new List<SentenceWithProbability>();
			foreach (var step in intermediate) {
				foreach (var swp in step) {
					if (swp.Sentence.OnlyTerminals()) {
						results.Add(swp);
					}
				}
			}
			return results;
		}

		private List<SentenceWithProbability> GoOneStep(SentenceWithProbability swp) {
			var start = new SentenceWithProbability(swp.Probability, new Sentence());
			var results = new List<SentenceWithProbability> { start };
			foreach (var word in swp.Sentence) {
				if (word.IsNonterminal()) {
					results = StepNonterminal(results, word);
				} else {
					results = StepTerminal(results, word);
				}
			}
			return results;
		}

		private List<SentenceWithProbability> StepNonterminal(List<SentenceWithProbability> results, Word word) {
			var newResults = new List<SentenceWithProbability>();

			var nonterminal = (Nonterminal)word;
			foreach (var production in ProductionsFrom(nonterminal)) {
				var prob = GetProbability(production);
				var copies = DeepCopy(results);
				foreach (var copy in copies) {
					copy.Sentence.AddRange(production.Rhs);
					copy.Probability *= prob;
				}
				newResults.AddRange(copies);
			}
			return newResults;
		}

		private List<SentenceWithProbability> StepTerminal(List<SentenceWithProbability> results, Word word) {
			foreach (var result in results) {
				result.Sentence.Add(word);
			}
			return results;
		}

		private List<SentenceWithProbability> DeepCopy(List<SentenceWithProbability> sentences) {
			var results = new List<SentenceWithProbability>();
			foreach (var sentence in sentences) {
				results.Add(new SentenceWithProbability(sentence.Probability, new Sentence(sentence.Sentence)));
			}
			return results;
		}

		protected double GetProbability(BaseProduction target) {
			return this.GetProbability(target.Lhs, target.Weight);
		}
		// TODO: use checked arithmetic
		protected double GetProbability(Nonterminal lhs, int weight) {
			long weightTotal = _weightTotalsByNonterminal.Value[lhs].Value;
			
			return (double)weight / weightTotal;
		}
		
		internal Sentence ProduceNonterminal(Nonterminal v) {
			Sentence result = null;

			var productions = ProductionsFrom(v);

			var totalWeight = productions.Sum(w => w.Weight);
			var targetValue = _rand.Next(totalWeight) + 1;

			var currentWeight = 0;
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
		
		public ISet<Nonterminal> GetNonterminals() {
			return _nonterminals.Value;
		}
		public ISet<Terminal> GetTerminals() {
			return _terminals.Value;
		}

		// TODO what's this for?
		public List<Sentence> Produce() {
			var history = new List<Sentence>();
			var sentence = new Sentence { this.Start };

			while (sentence.ContainsNonterminal()) {
				history.Add(sentence);
				Sentence newSentence = new Sentence();
				foreach (var word in sentence) {
					var newWords = word.ProduceBy(this);
					newSentence.AddRange(newWords);
				}
				sentence = newSentence;
			}
			history.Add(sentence);
			return history;
		}

		// TODO: MIGHT NOT TERMINATE!
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

		public void PrintProbabilities(long iterations, double correction) {
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
				Console.WriteLine("{0}: {1}", key, (double)value / iterations * correction);
			}
		}

		public void Simplify() {
			SimplifyWithoutInvalidate();
			InvalidateCaches();
		}
		
		protected void SimplifyWithoutInvalidate() {
			RemoveDuplicates();
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
