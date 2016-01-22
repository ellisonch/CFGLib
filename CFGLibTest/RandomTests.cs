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
		public void RandomCFGToCNFTest() {
			int _maxInputLength = 4;
			var randg = new GrammarGenerator();
			var range = Enumerable.Range(0, 5);
			var terminals = new List<Terminal>(range.Select((x) => Terminal.Of("x" + x)));

			var preparedSentences = new List<Sentence>();
			foreach (var target in CFGLibTest.Helpers.CombinationsWithRepetition(terminals, _maxInputLength)) {
				var sentence = new Sentence(target);
				preparedSentences.Add(sentence);
			}

			for (int i = 0; i < 10; i++) {
				var g = randg.NextCFG(5, 10, 4, terminals);
				var h = g.ToCNF();
				Console.WriteLine(g);
				Console.WriteLine(h);
				Console.WriteLine("-------------------------------");
				//foreach (var sentence in preparedSentences) {
				//	var pg = g.C
				//}
			}

		}
	}
}
