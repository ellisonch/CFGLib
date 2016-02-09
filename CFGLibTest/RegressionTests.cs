using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using CFGLib;

namespace CFGLibTest {
	[TestClass]
	public class RegressionTests {
		[TestMethod]
		public void TestHugeWeights() {
			var productions = new List<Production> {
				CFGParser.Production(@"<S> -> <A> <B> [3000000000]"),
				CFGParser.Production(@"<S> -> <C> <A> [3000000000]"),
				CFGParser.Production(@"<S> -> ε [3000000000]"),
				CFGParser.Production(@"<A> -> 'a'"),
				CFGParser.Production(@"<B> -> 'b'"),
				CFGParser.Production(@"<C> -> 'c'"),
			};
			var g = new CNFGrammar(productions, Nonterminal.Of("S"));

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
			CNFGrammar h = new CNFGrammar(productions, Nonterminal.Of("S"));

			Helpers.IsNear(0.0, h.Cyk(Sentence.FromLetters("a")));
		}

		[TestMethod]
		public void TestFreshNames() {
			var productions = new List<Production> {
				CFGParser.Production(@"<S> -> 'a'"),
				CFGParser.Production(@"<X_0> -> 'b'"),
			};
			Grammar g = new Grammar(productions, Nonterminal.Of("S"));
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

		[TestMethod]
		public void TestParsing01() {
			var g = new Grammar(new List<Production>{
				CFGParser.Production("<S> → <X> 'x'"),
				CFGParser.Production("<S> → ε"),
				CFGParser.Production("<X> → <S> <S>"),
			}, Nonterminal.Of("S"));
			var h = g.ToCNF();

			var s = Sentence.FromWords("x");
			Assert.IsTrue(g.Earley(s) > 0.0);
			Assert.IsTrue(h.Accepts(s));
		}

		[TestMethod]
		public void TestParsing02() {
			var g = new Grammar(new List<Production>{
				CFGParser.Production("<X_9> → 'x3' <X_4> <X_9> [69.71867415901211]"),
				CFGParser.Production("<X_6> → 'x4' [43.169519673180545]"),
				CFGParser.Production("<X_0> → 'x0' 'x3' <X_6> <X_9> <X_9> [95.5660355475573]"),
				CFGParser.Production("<X_5> → <X_9> 'x1' 'x0' 'x1' 'x3' <X_2> [35.638882444537657]"),
				CFGParser.Production("<X_1> → 'x4' 'x3' 'x1' 'x1' <X_9> <X_8> [60.963767072169006]"),
				CFGParser.Production("<X_9> → <X_6> [96.869668710916145]"),
				CFGParser.Production("<X_8> → 'x1' <X_0> 'x0' <X_2> <X_2> [10.412202848779131]"),
				CFGParser.Production("<X_4> → ε [89.394112460498746]"),
				CFGParser.Production("<X_4> → <X_8> 'x2' <X_5> 'x1' [41.46934854261081]"),
				CFGParser.Production("<X_2> → ε [28.04076097674703]"),
				CFGParser.Production("<X_8> → ε [55.798571558109757]"),
				CFGParser.Production("<X_0> → 'x2' 'x2' 'x3' <X_6> [48.407048357374521]"),
				CFGParser.Production("<X_0> → <X_1> 'x3' 'x2' [82.3935272774629]"),
				CFGParser.Production("<X_1> → <X_8> <X_1> <X_2> [68.051246746932733]")
			}, Nonterminal.Of("X_0"));
			var h = g.ToCNF();

			var s = Sentence.FromWords("x3 x2");
			var pearley = g.Earley(s);
			var pcyk = h.Cyk(s);

			var earley = pearley > 0.0;
			var cyk = pcyk > 0.0;

			Assert.IsFalse(earley);
			Assert.IsFalse(cyk);
		}
	}
}
