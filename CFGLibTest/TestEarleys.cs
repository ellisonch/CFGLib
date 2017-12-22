using CFGLib;
using CFGLib.Parsers.CYK;
using CFGLib.Parsers.Earley;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLibTest {
	[TestClass]
	public class TestEarleys {
		private static void ExecuteTest(Grammar g, string input) {
			var earley1 = new EarleyParser(g);
			var earley2 = new EarleyParser2(g);
			var sentence = Sentence.FromWords(input);

			var p2 = earley1.ParseGetProbability(sentence);
			var p3 = earley2.ParseGetProbability(sentence);
			Helpers.AssertNear(p2, p3);
		}

		[TestMethod]
		public void TestEarleys01() {
			ExecuteTest(new Grammar(new List<Production>{
				CFGParser.Production("<X_0> → <X_5> 'x5' [66.743324284322242]"),
				CFGParser.Production("<X_5> → 'x6' 'x6' [18.445467280897063]")
			}, Nonterminal.Of("X_0")), "x6");
		}


	}
}
