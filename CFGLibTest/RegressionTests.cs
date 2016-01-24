﻿using System;
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
		public void TestCFGToCNFBadProb01() {
			// S -> aSa | bSb | ε
			var productions = new List<BaseProduction> {
				CFGParser.Production(@"<S> -> 'a' <S> 'a' [1]"),
				CFGParser.Production(@"<S> -> 'b' <S> 'b' [3]"),
				CFGParser.Production(@"<S> -> ε [4]"),
			};
			Grammar g = new Grammar(productions, Nonterminal.Of("S"));
			CNFGrammar h = g.ToCNF();

			Helpers.AssertNear(0.5, h.Cyk(Sentence.FromLetters("")));
			Helpers.AssertNear((1.0 / 8) * 0.5 , h.Cyk(Sentence.FromLetters("aa")));
			Helpers.AssertNear((3.0 / 8) * 0.5, h.Cyk(Sentence.FromLetters("bb")));
			Helpers.AssertNear((1.0 / 8) * (3.0 / 8) * 0.5, h.Cyk(Sentence.FromLetters("abba")));
		}

		[TestMethod]
		public void TestMissingStart() {
			var productions = new List<BaseProduction> {
				CFGParser.Production(@"<X_0> -> <X_0> <X_0>"),
				CFGParser.Production(@"<X_0> -> 'a'"),
			};
			Grammar g = new Grammar(productions, Nonterminal.Of("S"));
			CNFGrammar h = g.ToCNF();
		}

		[TestMethod]
		public void TestFreshNames() {
			var productions = new List<BaseProduction> {
				CFGParser.Production(@"<S> -> 'a'"),
				CFGParser.Production(@"<X_0> -> 'b'"),
			};
			Grammar g = new Grammar(productions, Nonterminal.Of("S"), false);
			CNFGrammar h = g.ToCNF();

			Assert.IsTrue(h.Accepts(Sentence.FromLetters("a")));
			Assert.IsFalse(h.Accepts(Sentence.FromLetters("b")));
		}

		[TestMethod]
		[ExpectedException(typeof(Exception), "A bad production string was allowed")]
		public void TestParserFailure() {
			var fails = CFGParser.Production(@"<X_0> -> X_0 X_0");
		}
	}
}