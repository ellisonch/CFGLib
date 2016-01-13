using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextFreeGrammars {
	public class Grammar {
		// Variables 
		// Terminals
		// productions
		// Start
		private List<Production> _productions;
		private Random _rand = new Random(0);

		private Dictionary<Variable, List<Production>> _table = new Dictionary<Variable, List<Production>>();

		public List<Production> Productions {
			get { return _productions; }
		}

		public Grammar(List<Production> productions) {
			_productions = productions;
			
			foreach (var production in productions) {
				var lhs = production.Lhs;
				List<Production> results;
				if (!_table.TryGetValue(lhs, out results)) {
					results = new List<Production>();
					_table[lhs] = results;
				}
				results.Add(production);
			}

			if (!_table.ContainsKey(Variable.Of("S"))) {
				throw new Exception("There is no start production in the grammar");
			}
		}

		public CNFGrammar ToCNF() {
			return new CNFGrammar(this);
		}

		private double GetProbability(Production target) {
			int weightTotal = 0;
			
			var productions = _table.LookupEnumerable(target.Lhs);
			foreach (var production in productions) {
				weightTotal += production.Weight;
			}

			return (double)target.Weight / weightTotal;
		}

		internal Sentence ProduceVariable(Variable v) {
			Sentence result = null;

			var productions = _table[v];

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

		public List<SentenceWithProbability> ProduceToDepth(int depth) {
			var start = new Sentence { Variable.Of("S") };
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
				if (word.IsVariable()) {
					results = StepVariable(results, word);
				} else {
					results = StepTerminal(results, word);
				}
			}
			return results;
		}

		private List<SentenceWithProbability> StepVariable(List<SentenceWithProbability> results, Word word) {
			var newResults = new List<SentenceWithProbability>();

			var variable = (Variable)word;
			foreach (var production in _table[variable]) {
				var prob = GetProbability(production);
				var copies = DeepCopy(results);
				foreach (var copy in copies) {  // sigh
				// for (int i = 0; i < copies.Count; i++) {
					// var copy = copies[i];
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

		public List<Sentence> Produce() {
			var history = new List<Sentence>();
			var sentence = new Sentence { Variable.Of("S") };
			
			while (ContainsVariables(sentence)) {
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

		private bool ContainsVariables(Sentence sentence) {
			foreach(var c in sentence) {
				if (c.IsVariable()) {
					return true;
				}
			}
			return false;
		}

		public override string ToString() {
			var retval = "Grammar{\n";

			foreach (var production in _productions) {
				retval += "  " + production.ToString() + "\n";
			}
			retval += "}\n";
			return retval;
		}
	}
}
