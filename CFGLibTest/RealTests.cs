using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CFGLib;
using Grammars;
using CFGLib.Parsers.Earley;
using CFGLib.Parsers.CYK;

namespace CFGLibTest {
	[TestClass]
	public class RealTests {
		private static void BnfParse(Sentence sentence) {
			var g = Bnf.Grammar();
			//var h = g.ToCNF(); // too much memory
			var earley = new EarleyParser(g);
			// var cyk = new CykParser(h);
			var earley2 = new EarleyParser2(g);

			// var p1 = cyk.ParseGetProbability(sentence);
			var p2 = earley.ParseGetProbability(sentence);
			var p3 = earley2.ParseGetProbability(sentence);
			// Helpers.AssertNear(p1, p2);
			Helpers.AssertNear(p2, p3);
			Assert.IsTrue(p2 > 0.0);
		}

		[TestMethod]
		public void ParseAddition() {
			var sentence = Sentence.FromLetters(Grammars.Properties.Resources.Addition);
			BnfParse(sentence);
		}
		[TestMethod]
		public void ParseArithmetic() {
			var sentence = Sentence.FromLetters(Grammars.Properties.Resources.Arithmetic);
			BnfParse(sentence);
		}
		[TestMethod]
		public void ParseBnf() {
			var sentence = Sentence.FromLetters(Grammars.Properties.Resources.Bnf);
			BnfParse(sentence);
		}
	}
}
