using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CFGLib;
using System.Collections.Generic;
using System.Linq;

namespace CFGLibTest.Unit {
	[TestClass]
	public class TestCFGToCNF {
		[TestMethod]
		public void TestNullate01() {
			PrivateType cfgToCnf = new PrivateType(typeof(CFGtoCNF));
			
			var production = CFGParser.Production("<S> -> <A> 'b' <B> [1]");
			var nullableDictionary = new Dictionary<Nonterminal, double> {
				{ Nonterminal.Of("A"), 0.5 },
				{ Nonterminal.Of("B"), 0.2 }
			};

			var actualList = (List<BaseProduction>)cfgToCnf.InvokeStatic("Nullate", new object[] { production, nullableDictionary });
			var actual = new HashSet<string>(actualList.Select((p) => p.ToString()));
			var expected = new HashSet<string> {
				CFGParser.Production("<S> -> <A> 'b' <B> [0.4]").ToString(),
				CFGParser.Production("<S> -> <A> 'b' [0.1]").ToString(),
				CFGParser.Production("<S> -> 'b' <B> [0.4]").ToString(),
				CFGParser.Production("<S> -> 'b' [0.1]").ToString(),
			};
			Assert.IsTrue(actual.SetEquals(expected));
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
			Helpers.AssertNear((1.0 / 8) * 0.5, h.Cyk(Sentence.FromLetters("aa")));
			Helpers.AssertNear((3.0 / 8) * 0.5, h.Cyk(Sentence.FromLetters("bb")));
			Helpers.AssertNear((1.0 / 8) * (3.0 / 8) * 0.5, h.Cyk(Sentence.FromLetters("abba")));
		}

		[TestMethod]
		public void TestCFGToCNFBadProb02() {
			// S -> aSa | bSb | ε
			var productions = new List<BaseProduction> {
				CFGParser.Production(@"<S> -> 'a' <X> 'a' [1]"),
				CFGParser.Production(@"<S> -> 'c' [1]"),
				CFGParser.Production(@"<X> -> 'b' [1]"),
				CFGParser.Production(@"<X> -> ε [3]"),
			};
			Grammar g = new Grammar(productions, Nonterminal.Of("S"));
			CNFGrammar h = g.ToCNF();

			Helpers.AssertNear(0.0, h.Cyk(Sentence.FromLetters("")));
			Helpers.AssertNear(0.5, h.Cyk(Sentence.FromLetters("c")));
			Helpers.AssertNear((3.0 / 4) * 0.5, h.Cyk(Sentence.FromLetters("aa")));
			Helpers.AssertNear((1.0 / 4) * 0.5, h.Cyk(Sentence.FromLetters("aba")));
		}

		[TestMethod]
		public void TestUnitProductions01() {
			var productions = new List<BaseProduction> {
				CFGParser.Production(@"<S> -> <A> [3]"),
				CFGParser.Production(@"<S> -> <B> [1]"),
				CFGParser.Production(@"<A> -> 'a'"),
				CFGParser.Production(@"<B> -> 'b'"),
			};
			Grammar g = new Grammar(productions, Nonterminal.Of("S"));
			CNFGrammar h = g.ToCNF();

			Helpers.AssertNear(0.0, h.Cyk(Sentence.FromLetters("")));
			Helpers.AssertNear(0.75, h.Cyk(Sentence.FromLetters("a")));
			Helpers.AssertNear(0.25, h.Cyk(Sentence.FromLetters("b")));
		}

		[TestMethod]
		public void TestToCNF01() {
			var productions = new List<BaseProduction> {
				CFGParser.Production(@"<X_0> -> 'x3' <X_0> [23]"),
				CFGParser.Production(@"<X_0> -> ε [85]"),
			};
			Grammar g = new Grammar(productions, Nonterminal.Of("X_0"));
			CNFGrammar h = g.ToCNF();

			Helpers.AssertNear(85.0 / 108.0, h.Cyk(Sentence.FromLetters("")));
			Helpers.AssertNear((23.0 / 108.0) * (85.0 / 108.0), h.Cyk(Sentence.FromWords("x3")));
			Helpers.AssertNear((23.0 / 108.0) * (23.0 / 108.0) * (85.0 / 108.0), h.Cyk(Sentence.FromWords("x3 x3")));
		}
	}
}
