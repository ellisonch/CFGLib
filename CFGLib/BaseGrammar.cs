using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib {
	public abstract class BaseGrammar {
		public abstract ISet<Nonterminal> Nonterminals {
			get;
		}
		public abstract ISet<Terminal> Terminals {
			get;
		}
		public abstract IEnumerable<BaseProduction> Productions {
			get;
		}
		public abstract Nonterminal Start {
			get;
		}
		
		internal abstract IEnumerable<BaseProduction> ProductionsFrom(Nonterminal lhs);
		
		private Random _rand = new Random(0);

		private Dictionary<Nonterminal, long> _weightTotalsByNonterminal;

		protected void BuildHelpers() {
			_weightTotalsByNonterminal = Helpers.ConstructCacheValue(
				this.Productions,
				(p) => p.Lhs,
				(p) => p.Weight,
				() => 0L,
				(x, y) => x += y
			);
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
			long weightTotal = _weightTotalsByNonterminal[lhs];
			
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
			var results = new HashSet<Nonterminal>();
			results.Add(this.Start);

			foreach (var production in this.Productions) {
				results.Add(production.Lhs);
				foreach (var word in production.Rhs) {
					var nonterminal = word as Nonterminal;
					if (nonterminal != null) {
						results.Add(nonterminal);
					}
				}
			}

			return results;
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

		internal abstract void RemoveProductions(IEnumerable<BaseProduction> toRemove);

		//public override string ToString() {
		//	var retval = "Grammar{\n";

		//	foreach (var production in this.Productions) {
		//		retval += "  " + production.ToString() + "\n";
		//	}
		//	retval += "}\n";
		//	return retval;
		//}

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
