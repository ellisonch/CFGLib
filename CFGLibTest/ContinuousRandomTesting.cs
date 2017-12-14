using CFGLib;
using CFGLib.Parsers.CYK;
using CFGLib.Parsers.Earley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLibTest {
	public class ContinuousRandomTesting {
		private readonly Random _r;
		private readonly int _maxNonterminals;
		private readonly int _maxTerminals;
		private readonly int _maxProductions;
		private readonly int _maxProductionLength;
		private readonly int _maxInputLength;
		private readonly GrammarGenerator _gramGen;

		private readonly StreamWriter _parseErrorFile;

		public ContinuousRandomTesting(
			int maxNonterminals,
			int maxTerminals,
			int maxProductions,
			int maxProductionLength,
			int maxInputLength,
			int seed
		) {
			_maxNonterminals = maxNonterminals;
			_maxTerminals = maxTerminals;
			_maxProductions = maxProductions;
			_maxProductionLength = maxProductionLength;
			_maxInputLength = maxInputLength;
			_r = new Random(seed);
			_gramGen = new GrammarGenerator(_r);

			_parseErrorFile = new System.IO.StreamWriter(@"parseErrors.txt");
		}

		public void Run() {
			while (true) {
				ProcessOneGrammar();
				Console.Write(".");
			}
		}

		private void ProcessOneGrammar() {
			var (g, terminals) = NextGrammar();
			
			var preparedSentences = new List<Sentence>();
			for (int length = 0; length <= _maxInputLength; length++) {
				var combinations = CFGLibTest.Helpers.CombinationsWithRepetition(terminals, length);
				foreach (var target in combinations) {
					var sentence = new Sentence(target);
					preparedSentences.Add(sentence);
				}
			}

			// Console.WriteLine("Parsing sentences...");
			EarleyParser earley1;
			EarleyParser2 earley2;
			try {
				earley1 = new EarleyParser(g);
				earley2 = new EarleyParser2(g);
			} catch {
				Report(g);
				return;
			}
			
			foreach (var sentence in preparedSentences) {
				try {
					var p1 = earley1.ParseGetProbability(sentence);
					var p2 = earley2.ParseGetProbability(sentence);

					if (!Helpers.IsNear(p2, p1)) {
						throw new Exception();
					}
				} catch {
					Report(g, sentence);
					return;
					// throw new RandomTestException(e, g, sentence);
				}
			}
		}

		private void Report(Grammar g, Sentence sentence = null) {
			//Console.WriteLine("Offending grammar:");
			//Console.WriteLine(g.ToCodeString());
			//Console.WriteLine("Offending sentence:");
			//Console.WriteLine(sentence);
			_parseErrorFile.WriteLine(g.ToCodeString());
			if (sentence != null) {
				_parseErrorFile.WriteLine(sentence);
			}
			_parseErrorFile.WriteLine("-------------------------------------------");
			_parseErrorFile.Flush();
		}

		private (Grammar, List<Terminal>) NextGrammar() {
			var numNonterminals = _maxNonterminals;
			var numProductions = _maxProductions;
			var maxProductionLength = _maxProductionLength;
			var numTerminals = _maxTerminals;

			var range = Enumerable.Range(0, numTerminals);
			var terminals = new List<Terminal>(range.Select((x) => Terminal.Of("x" + x)));

			var g = _gramGen.NextCFG(numNonterminals, numProductions, maxProductionLength, terminals, true);
			return (g, terminals);
		}
	}
}
