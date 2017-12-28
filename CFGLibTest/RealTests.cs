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
		private static void EbnfParse(Sentence sentence) {
			var noLayoutSentence = Ebnf.RemoveLayout(sentence, out var layoutSppf);

			var g = Ebnf.Grammar(Nonterminal.Of("Syntax"));
			var earley = new EarleyParser(g);
			var earley2 = new EarleyParser2(g);

			var sppf1 = earley.ParseGetForest(noLayoutSentence);
			Assert.IsNotNull(sppf1);
			var sppf2 = earley2.ParseGetForest(noLayoutSentence);
			Assert.IsNotNull(sppf2);
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
		[TestMethod]
		public void ParseArithmeticEbnf() {
			var sentence = Sentence.FromLetters(Grammars.Properties.Resources.Arithmetic_ebnf);
			EbnfParse(sentence);
		}
		[TestMethod]
		[Ignore]
		public void ParseEbnfEbnf() {
			var sentence = Sentence.FromLetters(Grammars.Properties.Resources.Ebnf);
			EbnfParse(sentence);
		}
	}
}
