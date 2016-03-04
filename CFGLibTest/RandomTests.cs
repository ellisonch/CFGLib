using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using CFGLib;
using System.Linq;
using System.Diagnostics;
using CFGLib.Parsers.Earley;
using CFGLib.Parsers.CYK;

namespace CFGLibTest {
	[TestClass]
	public class RandomTests {
		public void RandomParsingTest(
			int _numGrammars = 10000,
			int _numNonterminals = 10,
			int _numTerminals = 5,
			int _numProductions = 30,
			int _maxProductionLength = 8,
			int _maxInputLength = 6,
			int seed = 0
		) {
			var printStatus = true;

			var range = Enumerable.Range(0, _numTerminals);
			var terminals = new List<Terminal>(range.Select((x) => Terminal.Of("x" + x)));
			Console.WriteLine("Preparing sentences");
			var preparedSentences = new List<Sentence>();
			for (int length = 0; length <= _maxInputLength; length++) {
				var combinations = CFGLibTest.Helpers.CombinationsWithRepetition(terminals, length);
				foreach (var target in combinations) {
					var sentence = new Sentence(target);
					preparedSentences.Add(sentence);
				}
			}

			var randg = new GrammarGenerator(seed);
			var preparedGrammars = new List<Grammar>(_numGrammars);
			var preparedGrammarsCNF = new List<CNFGrammar>(_numGrammars);
			Console.WriteLine("Preparing grammars");
			for (int i = 0; i < _numGrammars; i++) {
				Grammar g = null;
				while (g == null) {
					// g = randg.NextCNF(_numNonterminals, _numProductions, terminals);
					g = randg.NextCFG(_numNonterminals, _numProductions, _maxProductionLength, terminals, true);
					if (g.Productions.Count() == 0) {
						g = null;
					}
				}
				// Console.WriteLine("---------------{0}/{1}---------------", i.ToString("D5"), _numGrammars.ToString("D5"));
				// Console.WriteLine(g.ToCodeString());
				var h = g.ToCNF();
				//return;
				// Console.WriteLine(g);
				// g.PrintProbabilities(2, 3);
				preparedGrammars.Add(g);
				preparedGrammarsCNF.Add(h);
			}
			Console.WriteLine("starting");
			var sw = Stopwatch.StartNew();
			int count = 0;
			for (int grammarIndex = 0; grammarIndex < _numGrammars; grammarIndex++) {
				if (printStatus) {
					Console.WriteLine("---------------{0}/{1}---------------", grammarIndex.ToString("D5"), _numGrammars.ToString("D5"));
				}

				var g = preparedGrammars[grammarIndex];
				var h = preparedGrammarsCNF[grammarIndex];

				var earley = new EarleyParser(g);
				var cyk = new CykParser(h);

				// Console.WriteLine(g.ToCodeString());
				// Console.Write("{0}, ", count);
				count++;
				var accepts = 0;
				foreach (var sentence in preparedSentences) {
					try {
						var p1 = earley.ParseGetProbability(sentence);
						var p2 = cyk.ParseGetProbability(sentence);
						if (!Helpers.IsNear(p2, p1)) {
							throw new Exception();
						}
						var accepts1 = p1 > 0;
						var accepts2 = p2 > 0;

						if (accepts2) {
							accepts++;
						}
					} catch (Exception) {
						Report(g, sentence);
						throw;
					}
				}
				if (printStatus) {
					Console.WriteLine("Accepted {0} / {1}", accepts, preparedSentences.Count);
				}
			}
			sw.Stop();
			Console.WriteLine();
			Console.WriteLine("inner Elapsed: {0}s", sw.Elapsed.TotalMilliseconds / 1000.0);
			// Console.WriteLine("Per CYK: {0}ms", sw.Elapsed.TotalMilliseconds / (_numGrammars * preparedSentences.Count));
		}

		private static void Report(Grammar g, Sentence sentence) {
			Console.WriteLine("Offending grammar:");
			Console.WriteLine(g.ToCodeString());
			Console.WriteLine("Offending sentence:");
			Console.WriteLine(sentence);
		}
	}
}
