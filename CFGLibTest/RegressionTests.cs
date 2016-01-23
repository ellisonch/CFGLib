using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using CFGLib;

namespace CFGLibTest {
	[TestClass]
	public class RegressionTests {
		[TestMethod]
		public void TestHugeWeights() {
			// S -> aSa | bSb | ε
			var ntproductions = new List<BaseProduction> {
				CFGParser.Production(@"<S> -> <A> <B> [3000000000]"),
				CFGParser.Production(@"<S> -> <C> <A> [3000000000]"),
			};
			var tproductions = new List<BaseProduction> {
				CFGParser.Production(@"<A> -> 'a'"),
				CFGParser.Production(@"<B> -> 'b'"),
				CFGParser.Production(@"<C> -> 'c'"),
			};
			var g = new CNFGrammar(ntproductions, tproductions, 3000000000, Nonterminal.Of("S"));

			Helpers.AssertNear(1.0 / 3.0, g.Cyk(Sentence.FromLetters("")));
			Helpers.AssertNear(1.0 / 3.0, g.Cyk(Sentence.FromLetters("ab")));
			Helpers.AssertNear(1.0 / 3.0, g.Cyk(Sentence.FromLetters("ca")));
		}

		[TestMethod]
		[Ignore]
		public void TestCFGToCNFBadProb01() {
			// S -> aSa | bSb | ε
			var productions = new List<BaseProduction> {
				CFGParser.Production(@"<S> -> 'a' <S> 'a'"),
				CFGParser.Production(@"<S> -> 'b' <S> 'b'"),
				CFGParser.Production(@"<S> -> ε"),
			};
			Grammar g = new Grammar(productions, Nonterminal.Of("S"));
			CNFGrammar h = g.ToCNF();

			Assert.IsFalse(h.Accepts(Sentence.FromLetters("ab")));
			Assert.IsFalse(h.Accepts(Sentence.FromLetters("abc")));
			Assert.IsFalse(h.Accepts(Sentence.FromLetters("aaa")));
			Assert.IsFalse(h.Accepts(Sentence.FromLetters("abbba")));

			Assert.IsTrue(h.Accepts(Sentence.FromLetters("")));
			Assert.IsTrue(h.Accepts(Sentence.FromLetters("aa")));
			Assert.IsTrue(h.Accepts(Sentence.FromLetters("bb")));
			Assert.IsTrue(h.Accepts(Sentence.FromLetters("abba")));
			Assert.IsTrue(h.Accepts(Sentence.FromLetters("baab")));
			Assert.IsTrue(h.Accepts(Sentence.FromLetters("aaaa")));
			Assert.IsTrue(h.Accepts(Sentence.FromLetters("bbbb")));
			Assert.IsTrue(h.Accepts(Sentence.FromLetters("aaabbabbabbaaa")));
		}
	}
}
