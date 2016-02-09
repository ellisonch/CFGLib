using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using CFGLib;
using System.Linq;
using System.Diagnostics;

namespace CFGLibTest {
	[TestClass]
	public class RandomTests {
		[TestMethod]
		[Ignore]
		public void RandomAcceptanceTest() {
			int _maxDepth = 10;
			var _numGrammars = 50000;
			var _maxTestSentences = 20000;
			var _maxNonterminals = 10;
			var _maxProductions = 15;
			var _maxProductionLength = 5;
			var _numTerminals = 5;

			var randg = new GrammarGenerator();
			var range = Enumerable.Range(0, _numTerminals);
			var terminals = new List<Terminal>(range.Select((x) => Terminal.Of("x" + x)));

			var rand = new Random(0);

			for (int i = 0; i < _numGrammars; i++) {
				Console.WriteLine("---------------{0}/{1}---------------", i.ToString("D5"), _numGrammars.ToString("D5"));

				var numProductions = rand.Next(_maxProductions) + 1;
				var numNonterminals = rand.Next(_maxNonterminals) + 1;
				var g = randg.NextCFG(numNonterminals, numProductions, _maxProductionLength, terminals);
				Console.WriteLine(g);
				var h = g.ToCNF();				
				Console.WriteLine(h);

				var swps = g.ProduceToDepth(_maxDepth, _maxTestSentences);
				foreach (var swp in swps) {
					Assert.IsTrue(h.Accepts(swp.Value));
				}
			}
		}

		[TestMethod]
		[Ignore]
		public void RandomCFGToCNFTest() {
			int _maxDepth = 8;
			var _numGrammars = 200000;
			var _maxGenerateThing = 200000;
			var _maxTestSentences = 1000;
			var _numNonterminals = 7;
			var _numProductions = 15;
			var _maxProductionLength = 6;
			var _numTerminals = 5;
			var _useNulls = true;

			var randg = new GrammarGenerator();
			var range = Enumerable.Range(0, _numTerminals);
			var terminals = new List<Terminal>(range.Select((x) => Terminal.Of("x" + x)));

			//var preparedSentences = new List<Sentence>();
			//foreach (var target in CFGLibTest.Helpers.CombinationsWithRepetition(terminals, _maxInputLength)) {
			//	var sentence = new Sentence(target);
			//	preparedSentences.Add(sentence);
			//}

			for (int i = 0; i < _numGrammars; i++) {
				Console.WriteLine("---------------{0}/{1}---------------", i.ToString("D5"), _numGrammars.ToString("D5"));
				var g = randg.NextCFG(_numNonterminals, _numProductions, _maxProductionLength, terminals, _useNulls);
				Console.WriteLine(g.ToCodeString());
				var h = g.ToCNF();
				Console.WriteLine(h.ToCodeString());

				var swps = g.ProduceToDepth(_maxDepth, _maxGenerateThing);
				var count = 0;
				foreach (var swp in swps) {
					if (count > _maxTestSentences) {
						break;
					}
					// Console.WriteLine(swp);
					var p1 = swp.Probability;
					var p2 = h.Cyk(swp.Value);
					Assert.IsTrue(p1 <= p2 || Helpers.IsNear(p1, p2));
					count++;
				}
				// Console.WriteLine("-------------------------------");
				//foreach (var sentence in preparedSentences) {
				//	var pg = g.C
				//}
			}
		}

		[TestMethod]
		[Ignore]
		public void RandomParsingTest() {
			// CFGParser.Production(@"<X> -> <X0> <X1> 'asdf as_-""fw' <Z23X>");
			int _numNonterminals = 10;
			int _numProductions = 20;
			int _numTerminals = 5;
			int _maxLength = 6;
			int _numGrammars = 5000;
			int _maxProductionLength = 8;

			var range = Enumerable.Range(0, _numTerminals);
			var terminals = new List<Terminal>(range.Select((x) => Terminal.Of("x" + x)));
			Console.WriteLine("Preparing sentences");
			var preparedSentences = new List<Sentence>();
			for (int length = 1; length <= _maxLength; length++) {
				foreach (var target in CFGLibTest.Helpers.CombinationsWithRepetition(terminals, length)) {
					var sentence = new Sentence(target);
					preparedSentences.Add(sentence);
				}
			}

			var randg = new GrammarGenerator();
			var preparedGrammars = new List<Grammar>();
			var preparedGrammarsCNF = new List<CNFGrammar>();
			Console.WriteLine("Preparing grammars");
			for (int i = 0; i < _numGrammars; i++) {
				// var g = randg.NextCNF(_numNonterminals, _numProductions, terminals);
				var g = randg.NextCFG(_numNonterminals, _numProductions, _maxProductionLength, terminals, true);
				var h = g.ToCNF();
				// Console.WriteLine(g);
				// g.PrintProbabilities(2, 3);
				preparedGrammars.Add(g);
				preparedGrammarsCNF.Add(h);
			}

			var sw = Stopwatch.StartNew();
			int count = 0;
			for (int grammarIndex = 0; grammarIndex < preparedGrammars.Count; grammarIndex++) {
				var g = preparedGrammars[grammarIndex];
				var h = preparedGrammarsCNF[grammarIndex];
				Console.WriteLine("---------------{0}/{1}---------------", grammarIndex.ToString("D5"), _numGrammars.ToString("D5"));
				Console.WriteLine(g.ToCodeString());
				// Console.Write("{0}, ", count);
				count++;
				foreach (var sentence in preparedSentences) {
					//var p1 = g.Cyk(sentence);
					//var p1 = h.Cyk(sentence);
					var p1 = g.Earley(sentence);
					var p2 = h.Cyk(sentence);
					var accepts1 = p1 > 0;
					var accepts2 = p2 > 0;
					if (accepts1 != accepts2) {
						throw new Exception("Didn't match");
					}
					// Console.WriteLine("{0}: {1}", sentence, chance);
				}
			}
			sw.Stop();
			Console.WriteLine();
			Console.WriteLine("Elapsed: {0}s", sw.Elapsed.TotalMilliseconds / 1000.0);
			Console.WriteLine("Per CYK: {0}ms", sw.Elapsed.TotalMilliseconds / (_numGrammars * preparedSentences.Count));
		}

		[TestMethod]
		[Ignore]
		// non-simplified grammars can't really have probabilities
		public void RandomSimplificationTest() {
			int _maxInputLength = 4;
			int _numNonterminals = 10;
			int _numProductions = 40;
			int _numTrials = 100;

			var randg = new GrammarGenerator();
			var range = Enumerable.Range(0, 5);
			var terminals = new List<Terminal>(range.Select((x) => Terminal.Of("x" + x)));

			var preparedSentences = new List<Sentence>();
			foreach (var target in CFGLibTest.Helpers.CombinationsWithRepetition(terminals, _maxInputLength)) {
				var sentence = new Sentence(target);
				preparedSentences.Add(sentence);
			}

			for (int i = 0; i < _numTrials; i++) {
				var g = randg.NextCNF(_numNonterminals, _numProductions, terminals);
				//var h = g.Clone();
				//h.Simplify();
				//Console.WriteLine(g);
				//Console.WriteLine(h);
				//foreach (var sentence in preparedSentences) {
				//	var chanceg = g.Cyk(sentence);
				//	var chanceh = h.Cyk(sentence);
				//	// Helpers.AssertNear(chanceg, chanceh);
				//}
				// Console.WriteLine("-------------------------------");
			}
		}
	}
}
