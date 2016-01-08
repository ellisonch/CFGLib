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

		public List<Sentence> ProduceToDepth(int depth) {
			var start = new Sentence { Variable.Of("S") };
			List<Sentence>[] intermediate = new List<Sentence>[depth + 1];
			intermediate[0] = new List<Sentence> { start };

			for (int i = 0; i < depth; i++) {
				var prev = intermediate[i];
				var next = new List<Sentence>();
				intermediate[i + 1] = next;
				foreach (var sentence in prev) {
					if (!sentence.OnlyTerminals()) {
						var steps = GoOneStep(sentence);
						next.AddRange(steps);
					}
				}
			}
			var results = new List<Sentence>();
			foreach (var step in intermediate) {
				foreach (var sentence in step) {
					if (sentence.OnlyTerminals()) {
						results.Add(sentence);
					}
				}
			}
			return results;
		}

		private List<Sentence> GoOneStep(Sentence sentence) {
			var results = new List<Sentence> { new Sentence() };
			foreach (var word in sentence) {
				if (word.IsVariable()) {
					results = StepVariable(results, word);
				} else {
					results = StepTerminal(results, word);
				}
			}
			return results;
		}

		private List<Sentence> StepVariable(List<Sentence> results, Word word) {
			var newResults = new List<Sentence>();

			var variable = (Variable)word;
			foreach (var production in _table[variable]) {
				var copies = DeepCopy(results);
				foreach (var copy in copies) {
					copy.AddRange(production.Rhs);
				}
				newResults.AddRange(copies);
			}
			return newResults;
		}

		private List<Sentence> StepTerminal(List<Sentence> results, Word word) {
			foreach (var result in results) {
				result.Add(word);
			}
			return results;
		}

		private List<Sentence> DeepCopy(List<Sentence> sentences) {
			var results = new List<Sentence>();
			foreach (var sentence in sentences) {
				results.Add(new Sentence(sentence));
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
