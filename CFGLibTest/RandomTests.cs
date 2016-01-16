using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using CFGLib;
using System.Linq;

namespace CFGLibTest {
	[TestClass]
	public class RandomTests {
		[TestMethod]
		public void RandomClimbing() {
			var randg = new CNFRandom();

			for (int numProductions = 0; numProductions < 100; numProductions += 5) {
				for (int numNonterminals = 0; numNonterminals < 200; numNonterminals += 5) {
					for (int numTerminals = 1; numTerminals < 200; numTerminals += 5) {
						var range = Enumerable.Range(0, numTerminals);
						var terminals = new List<Terminal>(range.Select((x) => Terminal.Of("x" + x)));
						var rg = randg.Next(numNonterminals, numProductions, terminals);
						TestGrammar(rg);
					}
				}
			}
		}

		private void TestGrammar(CNFGrammar rg) {
			//for (int i = 0; i < 10; i++) {
			//	var sentences = rg.ProduceToDepth(i);
			//	Console.WriteLine("------Depth {0}------", i);
			//	foreach (var sentence in sentences) {
			//		Console.WriteLine(sentence.AsTerminals());
			//		// Console.WriteLine(sentence);
			//	}
			//}
		}
	}
}
