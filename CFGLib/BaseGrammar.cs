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

		private double GetProbability(BaseProduction target) {
			int weightTotal = 0;

			var productions = ProductionsFrom(target.Lhs);
			foreach (var production in this.Productions) {
				weightTotal += production.Weight;
			}

			return (double)target.Weight / weightTotal;
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

		public override string ToString() {
			var retval = "Grammar{\n";

			foreach (var production in this.Productions) {
				retval += "  " + production.ToString() + "\n";
			}
			retval += "}\n";
			return retval;
		}
	}
}
