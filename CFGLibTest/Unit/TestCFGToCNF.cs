using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CFGLib;
using System.Collections.Generic;
using System.Linq;

namespace CFGLibTest.Unit {
	[TestClass]
	public class TestCFGToCNF {
		[TestMethod]
		public void TestUnitProductions01() {
			var productions = new List<Production> {
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
		public void TestCNFNoNull01() {
			var productions = new HashSet<Production> {
				CFGParser.Production("<A> -> <A> <B>"),
				CFGParser.Production("<A> -> <B>"),
				CFGParser.Production("<A> -> 'a'"),
				CFGParser.Production("<B> -> <A>"),
				CFGParser.Production("<B> -> 'b'"),
			};

			Grammar g = new Grammar(productions, Nonterminal.Of("A"));
			CNFGrammar h = g.ToCNF();

			Helpers.AssertNear(0, h.Cyk(Sentence.FromLetters("")));
			Helpers.AssertNear(0.4, h.Cyk(Sentence.FromLetters("a")));
			Helpers.AssertNear(0.2, h.Cyk(Sentence.FromLetters("b")));
			Helpers.AssertNear(0.096, h.Cyk(Sentence.FromLetters("ab")));
			Helpers.AssertNear(0.048, h.Cyk(Sentence.FromLetters("bb")));
			Helpers.AssertNear(0.032, h.Cyk(Sentence.FromLetters("aa")));
			Helpers.AssertNear(0.016, h.Cyk(Sentence.FromLetters("ba")));
		}

		[TestMethod]
		public void TestCNFNoNull02() {
			var productions = new HashSet<Production> {
				CFGParser.Production("<A> -> <A>"),
				CFGParser.Production("<A> -> 'a'"),
				CFGParser.Production("<A> -> 'b'"),
			};

			Grammar g = new Grammar(productions, Nonterminal.Of("A"));
			CNFGrammar h = g.ToCNF();

			Helpers.AssertNear(0, h.Cyk(Sentence.FromLetters("")));
			Helpers.AssertNear(0.5, h.Cyk(Sentence.FromLetters("a")));
			Helpers.AssertNear(0.5, h.Cyk(Sentence.FromLetters("b")));
		}
		[TestMethod]
		public void TestCNFNoNull03() {
			var productions = new HashSet<Production> {
				CFGParser.Production("<S> -> <A>"),
				CFGParser.Production("<S> -> <B>"),
				CFGParser.Production("<A> -> <B>"),
				CFGParser.Production("<B> -> <A>"),
				CFGParser.Production("<A> -> 'a'"),
				CFGParser.Production("<B> -> 'b'"),
			};

			Grammar g = new Grammar(productions, Nonterminal.Of("S"));
			CNFGrammar h = g.ToCNF();

			Helpers.AssertNear(0, h.Cyk(Sentence.FromLetters("")));
			Helpers.AssertNear(0.5, h.Cyk(Sentence.FromLetters("a")));
			Helpers.AssertNear(0.5, h.Cyk(Sentence.FromLetters("b")));
		}

		[TestMethod]
		public void TestAccepts01() {
			var productions = new HashSet<Production> {
				CFGParser.Production("<S> -> 'x2' <A> <A>"),
				CFGParser.Production("<S> -> ε"),
				CFGParser.Production("<A> -> 'x4' <S> 'x2'"),
				CFGParser.Production("<A> -> ε"),
			};
			Grammar g = new Grammar(productions, Nonterminal.Of("S"));
			CNFGrammar h = g.ToCNF();

			Assert.IsTrue(h.Accepts(Sentence.FromLetters("")));
			Assert.IsTrue(h.Accepts(Sentence.FromWords("x2")));
			Assert.IsTrue(h.Accepts(Sentence.FromWords("x2 x4 x2")));
			Assert.IsTrue(h.Accepts(Sentence.FromWords("x2 x4 x2 x4 x2")));
		}

		[TestMethod]
		public void TestAccepts02() {
			var productions = new HashSet<Production> {
				CFGParser.Production("<X_0> -> <X_0> 'x4' <X_0> 'x0'"),
				CFGParser.Production("<X_0> -> <X_0> <X_0> 'x2' <X_0> 'x3'"),
				CFGParser.Production("<X_0> -> <X_0> 'x1' <X_0>"),
				CFGParser.Production("<X_0> -> <X_0> 'x1' 'x1' 'x1' 'x3'"),
				CFGParser.Production("<X_0> -> ε"),
			};
			Grammar g = new Grammar(productions, Nonterminal.Of("X_0"));
			CNFGrammar h = g.ToCNF();

			Assert.IsTrue(h.Accepts(Sentence.FromLetters("")));
			Assert.IsTrue(h.Accepts(Sentence.FromWords("x4 x0")));
			Assert.IsTrue(h.Accepts(Sentence.FromWords("x4 x0 x4 x2 x3 x0")));
		}

		[TestMethod]
		public void TestAccepts03() {
			var productions = new HashSet<Production> {
				CFGParser.Production("<S> -> <S> <S> <S>"),
				CFGParser.Production("<S> -> ε"),
				CFGParser.Production("<S> -> 'x2' 'x0'"),
				CFGParser.Production("<S> -> 'x0' <S>"),
				CFGParser.Production("<S> -> 'x4'"),
				CFGParser.Production("<S> -> <S> 'x0' 'x4'"),
				CFGParser.Production("<S> -> 'x3' <S> 'x3' <S> 'x0'"),
				CFGParser.Production("<S> -> 'x2' <S> <S> <S> <S>"),
				CFGParser.Production("<S> -> <S> <S> <S> <S> <S>"),
				CFGParser.Production("<S> -> 'x0' <S> <S>"),
				CFGParser.Production("<S> -> <S>"),
				CFGParser.Production("<S> -> 'x0' <S> <S> 'x1'"),
				CFGParser.Production("<S> -> 'x3' 'x2' 'x1'"),
				CFGParser.Production("<S> -> 'x0' 'x0' 'x2'"),
			};
			Grammar g = new Grammar(productions, Nonterminal.Of("S"));
			CNFGrammar h = g.ToCNF();

			Assert.IsTrue(h.Accepts(Sentence.FromLetters("")));
			Assert.IsTrue(h.Accepts(Sentence.FromWords("x4")));
			Assert.IsTrue(h.Accepts(Sentence.FromWords("x4 x0 x4")));
		}

		[TestMethod]
		public void TestAccepts04() {
			var productions = new HashSet<Production> {
				CFGParser.Production("<X_2> -> <X_3> <X_1>"),
				CFGParser.Production("<X_3> -> <X_1> <X_1>"),
				CFGParser.Production("<X_0> -> 'x2' 'x0' <X_3> <X_2> <X_2>"),
				CFGParser.Production("<X_2> -> 'x0' 'x3' <X_1> 'x0' 'x0'"),
				CFGParser.Production("<X_3> -> <X_1>"),
				CFGParser.Production("<X_2> -> <X_1> 'x4'"),
				CFGParser.Production("<X_1> -> ε"),
				CFGParser.Production("<X_0> -> <X_2>"),
				CFGParser.Production("<X_1> -> 'x4' <X_2> <X_1> 'x0' 'x1'"),
				CFGParser.Production("<X_1> -> <X_2> 'x0' 'x1' <X_2> <X_2>"),
				CFGParser.Production("<X_2> -> 'x1' <X_2> 'x3'"),
				CFGParser.Production("<X_1> -> <X_3> <X_0> <X_2> <X_3>"),
			};
			var g = new Grammar(productions, Nonterminal.Of("X_0"));
			var h = g.ToCNF();

			Assert.IsTrue(h.Accepts(Sentence.FromWords("x2 x0 x0 x3 x0 x0 x4")));
			Assert.IsTrue(h.Accepts(Sentence.FromWords("x2 x0 x4 x4")));
			Assert.IsTrue(h.Accepts(Sentence.FromWords("x0 x3 x0 x0")));
			Assert.IsTrue(h.Accepts(Sentence.FromWords("x2 x0")));
			Assert.IsTrue(h.Accepts(Sentence.FromWords("x4")));
		}

		[TestMethod]
		public void TestAccepts05() {
			var productions = new HashSet<Production> {
				CFGParser.Production("<X_2> -> <X_1> <X_0>"),
				CFGParser.Production("<X_1> -> <X_2> <X_1> <X_3> 'x2'"),
				CFGParser.Production("<X_3> -> <X_0> <X_0> <X_1> <X_3>"),
				CFGParser.Production("<X_3> -> ε"),
				CFGParser.Production("<X_2> -> <X_0> <X_1> <X_3> <X_1> <X_3>"),
				CFGParser.Production("<X_2> -> <X_1> <X_2> <X_2> <X_0>"),
				CFGParser.Production("<X_0> -> <X_3> 'x3'"),
				CFGParser.Production("<X_2> -> ε"),
				CFGParser.Production("<X_0> -> <X_2> <X_1>"),
				CFGParser.Production("<X_2> -> <X_0> <X_1> <X_2>"),
				CFGParser.Production("<X_1> -> <X_3> <X_3>"),
				CFGParser.Production("<X_3> -> 'x3' 'x4'"),
				CFGParser.Production("<X_3> -> <X_3> 'x4'"),
				CFGParser.Production("<X_1> -> 'x0' 'x4' 'x0' <X_2> <X_0>"),
			};
			var g = new Grammar(productions, Nonterminal.Of("X_0"));
			var h = g.ToCNF();

			Assert.IsTrue(h.Accepts(Sentence.FromWords("")));
			Assert.IsTrue(h.Accepts(Sentence.FromWords("x3")));
			Assert.IsTrue(h.Accepts(Sentence.FromWords("x3 x4 x3")));
			Assert.IsTrue(h.Accepts(Sentence.FromWords("x4 x3")));
			Assert.IsTrue(h.Accepts(Sentence.FromWords("x3 x4 x4 x3")));
			Assert.IsTrue(h.Accepts(Sentence.FromWords("x3 x4")));
			Assert.IsTrue(h.Accepts(Sentence.FromWords("x3 x4 x3 x4")));
		}

		[TestMethod]
		public void TestAccepts06() {
			var productions = new HashSet<Production> {
				CFGParser.Production("<X_3> -> ε"),
				CFGParser.Production("<X_2> -> ε"),
				CFGParser.Production("<X_0> -> <X_2> <X_1>"),
				CFGParser.Production("<X_1> -> <X_3> <X_3>"),
			};
			var g = new Grammar(productions, Nonterminal.Of("X_0"));
			var h = g.ToCNF();

			Assert.IsTrue(h.Accepts(Sentence.FromWords("")));
		}

		[TestMethod]
		public void TestAccepts07() {
			var productions = new HashSet<Production> {
				CFGParser.Production("<A> -> 'a'"),
				CFGParser.Production("<A> -> <B>"),
				CFGParser.Production("<B> -> 'b'"),
				CFGParser.Production("<B> -> <A>"),
			};
			var g = new Grammar(productions, Nonterminal.Of("A"));
			var h = g.ToCNF();

			Assert.IsTrue(h.Accepts(Sentence.FromWords("a")));
			Assert.IsTrue(h.Accepts(Sentence.FromWords("b")));
		}

		[TestMethod]
		public void TestAccepts08() {
			var productions = new HashSet<Production> {
				CFGParser.Production("<S> -> <S> <S>"),
				CFGParser.Production("<S> -> ε"),
			};
			Grammar g = new Grammar(productions, Nonterminal.Of("S"));
			CNFGrammar h = g.ToCNF();

			Assert.IsTrue(h.Accepts(Sentence.FromLetters("")));
			//Assert.IsTrue(h.Accepts(Sentence.FromWords("x4")));
			//Assert.IsTrue(h.Accepts(Sentence.FromWords("x4 x0 x4")));
		}

		[TestMethod]
		public void TestAccepts09() {
			var productions = new HashSet<Production> {
				CFGParser.Production("<S> -> ε"),
				// CFGParser.Production("<S> -> 'x'"),
				CFGParser.Production("<S> -> <S> <S> <S>"),
			};
			Grammar g = new Grammar(productions, Nonterminal.Of("S"));
			CNFGrammar h = g.ToCNF();

			Assert.IsTrue(h.Accepts(Sentence.FromLetters("")));
		}

		[TestMethod]
		public void TestProbabilityUnit01() {
			var productions = new HashSet<Production> {
				CFGParser.Production("<A> -> 'a'"),
				CFGParser.Production("<A> -> <B>"),
				CFGParser.Production("<B> -> 'b'"),
				CFGParser.Production("<B> -> <A>"),
			};
			var g = new Grammar(productions, Nonterminal.Of("A"));
			var h = g.ToCNF();

			Helpers.IsNear(2.0 / 3, h.Cyk(Sentence.FromLetters("a")));
			Helpers.IsNear(1.0 / 3, h.Cyk(Sentence.FromLetters("b")));
		}
	}
}
