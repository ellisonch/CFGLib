using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using CFGLib;
using System.Linq;

namespace CFGLibTest {
	[TestClass]
	public class RandomTests {
		[TestMethod]
		[Ignore]
		public void RandomAcceptanceTest() {
			int _maxDepth = 10;
			var _numGrammars = 500;
			var _maxTestSentences = 100000;
			var _maxNonterminals = 5;
			var _maxProductions = 10;
			var _maxProductionLength = 5;
			var _numTerminals = 5;

			var randg = new GrammarGenerator();
			var range = Enumerable.Range(0, _numTerminals);
			var terminals = new List<Terminal>(range.Select((x) => Terminal.Of("x" + x)));

			var rand = new Random(0);

			for (int i = 0; i < _numGrammars; i++) {
				var numProductions = rand.Next(_maxProductions) + 1;
				var numNonterminals = rand.Next(_maxNonterminals) + 1;
				var g = randg.NextCFG(numNonterminals, numProductions, _maxProductionLength, terminals);
				var h = g.ToCNF();
				Console.WriteLine(g);
				Console.WriteLine(h);

				var swps = g.ProduceToDepth(_maxDepth, _maxTestSentences);
				foreach (var swp in swps) {
					Assert.IsTrue(h.Accepts(swp.Value));
				}
				Console.WriteLine("-------------------------------");
			}
		}

		[TestMethod]
		[Ignore]
		public void RandomCFGToCNFTest() {
			int _maxDepth = 6;
			var _numGrammars = 200;
			var _maxTestSentences = 100000;
			var _numNonterminals = 5;
			var _numProductions = 6;
			var _maxProductionLength = 4;
			var _numTerminals = 5;

			var randg = new GrammarGenerator();
			var range = Enumerable.Range(0, _numTerminals);
			var terminals = new List<Terminal>(range.Select((x) => Terminal.Of("x" + x)));

			//var preparedSentences = new List<Sentence>();
			//foreach (var target in CFGLibTest.Helpers.CombinationsWithRepetition(terminals, _maxInputLength)) {
			//	var sentence = new Sentence(target);
			//	preparedSentences.Add(sentence);
			//}

			for (int i = 0; i < _numGrammars; i++) {
				var g = randg.NextCFG(_numNonterminals, _numProductions, _maxProductionLength, terminals);
				var h = g.ToCNF();
				Console.WriteLine(g);
				Console.WriteLine(h);

				var swps = g.ProduceToDepth(_maxDepth, _maxTestSentences);
				foreach (var swp in swps) {
					// Console.WriteLine(swp);
					var p1 = swp.Probability;
					var p2 = h.Cyk(swp.Value);
					Assert.IsTrue(p1 <= p2 || Helpers.IsNear(p1, p2));
				}
				Console.WriteLine("-------------------------------");
				//foreach (var sentence in preparedSentences) {
				//	var pg = g.C
				//}
			}
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
				var g = randg.NextCNF(_numNonterminals, _numProductions, terminals, false);
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
