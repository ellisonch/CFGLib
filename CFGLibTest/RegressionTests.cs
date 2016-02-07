using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using CFGLib;

namespace CFGLibTest {
	[TestClass]
	public class RegressionTests {
		[TestMethod]
		public void TestHugeWeights() {
			var ntproductions = new List<Production> {
				CFGParser.Production(@"<S> -> <A> <B> [3000000000]"),
				CFGParser.Production(@"<S> -> <C> <A> [3000000000]"),
			};
			var tproductions = new List<Production> {
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
		public void TestMissingStart01() {
			var productions = new List<Production> {
				CFGParser.Production(@"<X_0> -> <X_0> <X_0>"),
				CFGParser.Production(@"<X_0> -> 'a'"),
			};
			Grammar g = new Grammar(productions, Nonterminal.Of("S"));
			CNFGrammar h = g.ToCNF();
		}

		[TestMethod]
		public void TestMissingStart02() {
			var productions = new List<Production> {
				CFGParser.Production(@"<X_0> -> 'a'"),
			};
			CNFGrammar h = new CNFGrammar(new List<Production>(), productions, 0.0, Nonterminal.Of("S"), false);

			Helpers.IsNear(0.0, h.Cyk(Sentence.FromLetters("a")));
		}

		[TestMethod]
		public void TestFreshNames() {
			var productions = new List<Production> {
				CFGParser.Production(@"<S> -> 'a'"),
				CFGParser.Production(@"<X_0> -> 'b'"),
			};
			Grammar g = new Grammar(productions, Nonterminal.Of("S"), false);
			CNFGrammar h = g.ToCNF();

			Assert.IsTrue(h.Accepts(Sentence.FromLetters("a")));
			Assert.IsFalse(h.Accepts(Sentence.FromLetters("b")));
		}

		[TestMethod]
		public void TestParserFailure() {
			Helpers.AssertThrows<Exception>(() =>
				CFGParser.Production(@"<X_0> -> X_0 X_0")
			);
		}
	}
}
