using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CFGLib;
using System.Collections.Generic;
using CFGLib.Parsers.Earley;
using CFGLib.Actioneer;

namespace CFGLibTest {
	[TestClass]
	public class TestTraversal {
		private static void ExecuteTest(Grammar g, string input) {
			var earley1 = new EarleyParser(g);
			var earley2 = new EarleyParser2(g);
			var sentence = Sentence.FromWords(input);

			var sppf1 = earley1.ParseGetForest(sentence);
			var sppf2 = earley2.ParseGetForest(sentence);
			var t1 = new Traversal(sppf1, g);
			var t2 = new Traversal(sppf2, g);
			t1.Traverse();
			t2.Traverse();
		}
		[TestMethod]
		public void TestAddition() {
			ExecuteTest(new Grammar(new List<Production>{
				CFGParser.Production("<S> → <S> '+' <S>"),
				CFGParser.Production("<S> → '1'")
			}, Nonterminal.Of("S")), "1 + 1");
		}
	}
}
