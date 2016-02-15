using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CFGLib;
using System.Collections.Generic;

namespace CFGLibTest.Unit {
	[TestClass]
	public class TestCFGParser {
		[TestMethod]
		public void TestProduction() {
			var actualp = CFGParser.Production(@"<S> -> <X> 'a' <S> [3.0]");
			var expectedp = new DefaultProduction(
				Nonterminal.Of("S"),
				new Sentence {
					Nonterminal.Of("X"),
					Terminal.Of("a"),
					Nonterminal.Of("S")
				},
				3.0
			);
			var unexpectedp = new DefaultProduction(
				Nonterminal.Of("S"),
				new Sentence {
					Terminal.Of("a"),
					Nonterminal.Of("X"),
					Nonterminal.Of("S")
				},
				3.0
			);

			Assert.IsTrue(actualp.ValueEquals(expectedp));
			Assert.IsFalse(actualp.ValueEquals(unexpectedp));
		}
		[TestMethod]
		public void TestProductionWeight() {
			Func<double, Production> prodp = (w) => new DefaultProduction(
				Nonterminal.Of("S"),
				new Sentence { Nonterminal.Of("X") },
				w
			);

			var p1 = CFGParser.Production(@"<S> -> <X> [3]");
			var p2 = CFGParser.Production(@"<S> -> <X> [3.0000]");
			var p3 = CFGParser.Production(@"<S> -> <X> [0.5]");

			Assert.IsTrue(p1.ValueEquals(p2));

			Assert.IsTrue(p1.ValueEquals(prodp(3.0)));
			Assert.IsFalse(p1.ValueEquals(prodp(1.0)));

			Assert.IsTrue(p3.ValueEquals(prodp(0.5)));
		}


		[TestMethod]
		// TODO: add case
		public void TestWeirdSppf01() {
			var g = new Grammar(new List<Production>{
				CFGParser.Production("<X_0> → <X_2> <X_6> [84.663251941866818]"),
				CFGParser.Production("<X_5> → ε [58.744849453561407]"),
				CFGParser.Production("<X_6> → 'x0' [14.931547298064245]"),
				CFGParser.Production("<X_1> → <X_4> 'x3' 'x2' [8.0317742447516771]"),
				CFGParser.Production("<X_2> → <X_6> [31.352648361750251]"),
				CFGParser.Production("<X_5> → 'x2' [70.697925527439423]"),
				CFGParser.Production("<X_4> → ε [95.484905709738328]"),
				CFGParser.Production("<X_2> → <X_2> [1]"),
				CFGParser.Production("<X_2> → <X_1> <X_5> <X_5> <X_1> 'x2' <X_5> <X_0> [76.400903250743127]")
			}, Nonterminal.Of("X_0"));

			Assert.IsTrue(g.Earley(Sentence.FromWords("x0 x0")) > 0);
		}

		[TestMethod]
		public void TestWeirdSppf02() {
			var g = new Grammar(new List<Production>{
				CFGParser.Production("<X_0> → <X_1>"),
				CFGParser.Production("<X_1> → 'x1' 'x2'"),
				CFGParser.Production("<X_1> → ε")
			}, Nonterminal.Of("X_0"));
			var p = g.Earley(Sentence.FromWords("x1 x2"));
			Assert.IsTrue(p > 0);
		}

		[TestMethod]
		public void TestWeirdSppf03() {
			var g = new Grammar(new List<Production>{
				CFGParser.Production("<S> → <X> 'b'"),
				CFGParser.Production("<S> → 'a'"),
				CFGParser.Production("<X> → <S>"),
				CFGParser.Production("<X> → 'x'")
			}, Nonterminal.Of("S"));
			var s = Sentence.FromWords("a b b");
			var p1 = g.ToCNF().Cyk(s);
			var p2 = g.Earley(s);
			Helpers.AssertNear(p1, p2);
		}

		[TestMethod]
		public void TestWeirdSppf04() {
			var g = new Grammar(new List<Production>{
				CFGParser.Production("<X_0> → <X_2> 'x2' 'x0'"),
				CFGParser.Production("<X_2> → <X_0>"),
				CFGParser.Production("<X_0> → 'x1'"),
				CFGParser.Production("<X_2> → 'q'")
			}, Nonterminal.Of("X_0"));
			var s = Sentence.FromWords("x1 x2 x0 x2 x0");
			var p1 = g.ToCNF().Cyk(s);
			var p2 = g.Earley(s);
			Helpers.AssertNear(p1, p2);
		}

		[TestMethod]
		public void TestWeirdSppf05() {
			var g = new Grammar(new List<Production>{
				CFGParser.Production("<X_0> → <X_1>"),
				CFGParser.Production("<X_1> → ε"),
				CFGParser.Production("<X_1> → 'x2' <X_0> <X_0> 'x1'"),
				CFGParser.Production("<X_1> → <X_0>"),
			}, Nonterminal.Of("X_0"));
			var s = Sentence.FromWords("x2 x1");
			var p1 = g.ToCNF().Cyk(s);
			var p2 = g.Earley(s);
			Helpers.AssertNear(p1, p2);
		}

		[TestMethod]
		public void TestWeirdSppf06() {
			var g = new Grammar(new List<Production>{
				CFGParser.Production("<A> → <B>"),
				CFGParser.Production("<B> → <A>"),
				CFGParser.Production("<B> → 'x'"),
			}, Nonterminal.Of("A"));
			var s = Sentence.FromWords("x");
			var p1 = g.ToCNF().Cyk(s);
			var p2 = g.Earley(s);
			Helpers.AssertNear(p1, p2);
		}

		[TestMethod]
		public void TestWeirdSppf07() {
			var g = new Grammar(new List<Production>{
				CFGParser.Production("<A> → <B>"),
				CFGParser.Production("<B> → <C>"),
				CFGParser.Production("<C> → <B>"),
				CFGParser.Production("<B> → <A>"),
				CFGParser.Production("<B> → 'x'"),
			}, Nonterminal.Of("A"));
			var s = Sentence.FromWords("x");
			var p1 = g.ToCNF().Cyk(s);
			var p2 = g.Earley(s);
			Helpers.AssertNear(p1, p2);
		}

		[TestMethod]
		public void TestWeirdSppf08() {
			var g = new Grammar(new List<Production>{
				CFGParser.Production("<S> → <A> 'b'"),
				CFGParser.Production("<S> → <AB>"),
				CFGParser.Production("<A> → 'a'"),
				CFGParser.Production("<AB> → 'a' 'b'"),
			}, Nonterminal.Of("S"));
			var s = Sentence.FromWords("a b");
			var p1 = g.ToCNF().Cyk(s);
			var p2 = g.Earley(s);
			Helpers.AssertNear(p1, p2);
		}

		[TestMethod]
		public void TestWeirdSppf09() {
			var g = new Grammar(new List<Production>{
				CFGParser.Production("<S> → <A> 'b'"),
				CFGParser.Production("<S> → <AB>"),
				CFGParser.Production("<S> → <ABPrime>"),
				CFGParser.Production("<A> → 'a'"),
				CFGParser.Production("<AB> → 'a' 'b'"),
				CFGParser.Production("<ABPrime> → 'a' 'b'"),
			}, Nonterminal.Of("S"));
			var s = Sentence.FromWords("a b");
			var p1 = g.ToCNF().Cyk(s);
			var p2 = g.Earley(s);
			Helpers.AssertNear(p1, p2);
		}

	}
}
