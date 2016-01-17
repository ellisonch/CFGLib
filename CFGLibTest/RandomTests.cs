using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using CFGLib;
using System.Linq;

namespace CFGLibTest {
	[TestClass]
	public class RandomTests {
		int _maxProductions = 20;
		int _maxNonterminals = 20;
		int _maxTerminals = 20;
		int _step = 5;

		[TestMethod]
		public void RandomClimbing() {
			var randg = new CNFRandom();

			for (int numProductions = 0; numProductions < _maxProductions; numProductions += _step) {
				for (int numNonterminals = 0; numNonterminals < _maxNonterminals; numNonterminals += _step) {
					for (int numTerminals = 1; numTerminals < _maxTerminals; numTerminals += _step) {
						var range = Enumerable.Range(0, numTerminals);
						var terminals = new List<Terminal>(range.Select((x) => Terminal.Of("x" + x)));
						var rg = randg.Next(numNonterminals, numProductions, terminals);
						TestGrammar(rg);
					}
				}
			}
		}

		private void TestGrammar(CNFGrammar rg) {
			for (int i = 0; i < 10; i++) {
				var swps = rg.ProduceToDepth(i);
				Console.WriteLine("------Depth {0}------", i);
				foreach (var swp in swps) {
					Console.WriteLine(swp.Sentence.AsTerminals());
					// Console.WriteLine(sentence);
				}
			}
		}
	}
}
