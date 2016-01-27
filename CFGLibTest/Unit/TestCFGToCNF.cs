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

		[TestMethod]
		public void TestToCNF02() {
			var productions = new List<BaseProduction> {
				CFGParser.Production(@"<S> -> 'a' <B> <B>"),
				CFGParser.Production(@"<B> -> 'b'"),
				CFGParser.Production(@"<B> -> ε"),
			};
			Grammar g = new Grammar(productions, Nonterminal.Of("S"));
			CNFGrammar h = g.ToCNF();

			var pa = h.Cyk(Sentence.FromLetters("a"));
			var pab = h.Cyk(Sentence.FromLetters("ab"));
			var pabb = h.Cyk(Sentence.FromLetters("abb"));

			Assert.IsTrue(pa > 0.0);
			Assert.IsTrue(pab > 0.0);
			Assert.IsTrue(pabb > 0.0);
			Helpers.IsNear(1.0, pa + pab + pabb);
		}

		[TestMethod]
		public void TestGetNullable01() {
			PrivateType cfgToCnf = new PrivateType(typeof(CFGtoCNF));
			
			var productions = new HashSet<BaseProduction> {
				CFGParser.Production("<X> -> <B> <B>"),
				CFGParser.Production("<B> -> 'b'"),
				CFGParser.Production("<B> -> ε"),
			};

			var result = (Dictionary<Nonterminal, double>)cfgToCnf.InvokeStatic("GetNullable", new object[] { productions });
			
			Assert.IsTrue(result.Count == 2);
			Assert.IsTrue(result[Nonterminal.Of("B")] == 0.5);
			Assert.IsTrue(result[Nonterminal.Of("X")] == 0.25);
		}

		[TestMethod]
		public void TestGetNullable02() {
			PrivateType cfgToCnf = new PrivateType(typeof(CFGtoCNF));
			var productions = new HashSet<BaseProduction> {
				CFGParser.Production("<A> -> <A> <B>"),
				CFGParser.Production("<A> -> ε"),
				CFGParser.Production("<B> -> 'b'"),
				CFGParser.Production("<B> -> ε"),
			};

			var result = (Dictionary<Nonterminal, double>)cfgToCnf.InvokeStatic("GetNullable", new object[] { productions });

			Assert.IsTrue(result.Count == 2);
			Assert.IsTrue(result[Nonterminal.Of("A")] == 1.0 / 3.0);
			Assert.IsTrue(result[Nonterminal.Of("B")] == 0.5);
		}

		[TestMethod]
		public void TestGetNullable03() {
			PrivateType cfgToCnf = new PrivateType(typeof(CFGtoCNF));

			var productions = new HashSet<BaseProduction> {
				CFGParser.Production("<A> -> <B> <C>"),
				CFGParser.Production("<B> -> <C>"),
				CFGParser.Production("<B> -> 'b'"),
				CFGParser.Production("<B> -> ε"),
				CFGParser.Production("<C> -> <B>"),
				CFGParser.Production("<C> -> 'c'"),
				CFGParser.Production("<C> -> ε"),
			};

			var result = (Dictionary<Nonterminal, double>)cfgToCnf.InvokeStatic("GetNullable", new object[] { productions });

			Assert.IsTrue(result.Count == 3);
			Assert.IsTrue(result[Nonterminal.Of("A")] == 0.25);
			Assert.IsTrue(result[Nonterminal.Of("B")] == 0.5);
			Assert.IsTrue(result[Nonterminal.Of("C")] == 0.5);
		}

		[TestMethod]
		public void TestGetNullable04() {
			PrivateType cfgToCnf = new PrivateType(typeof(CFGtoCNF));

			var productions = new HashSet<BaseProduction> {
				CFGParser.Production("<A> -> <A> <A>"),
				CFGParser.Production("<A> -> ε"),
				CFGParser.Production("<A> -> 'a'"),
			};

			var result = (Dictionary<Nonterminal, double>)cfgToCnf.InvokeStatic("GetNullable", new object[] { productions });

			Assert.IsTrue(result.Count == 1);
			Assert.IsTrue(result[Nonterminal.Of("A")] == 0.381966);
		}

		[TestMethod]
		public void TestToCNF03() {
			var productions = new HashSet<BaseProduction> {
				CFGParser.Production("<A> -> <B> <C>"),
				CFGParser.Production("<B> -> <C>"),
				CFGParser.Production("<B> -> 'b'"),
				CFGParser.Production("<B> -> ε"),
				CFGParser.Production("<C> -> <B>"),
				CFGParser.Production("<C> -> 'c'"),
				CFGParser.Production("<C> -> ε"),
			};

			Grammar g = new Grammar(productions, Nonterminal.Of("A"));
			CNFGrammar h = g.ToCNF();

			Helpers.AssertNear(0.25, h.Cyk(Sentence.FromLetters("")));
			Helpers.AssertNear(0.25, h.Cyk(Sentence.FromLetters("b")));
			Helpers.AssertNear(0.25, h.Cyk(Sentence.FromLetters("c")));
			Helpers.AssertNear(0.140625, h.Cyk(Sentence.FromLetters("bc")));
			Helpers.AssertNear(0.046875, h.Cyk(Sentence.FromLetters("cc")));
			Helpers.AssertNear(0.046875, h.Cyk(Sentence.FromLetters("bb")));
			Helpers.AssertNear(0.015625, h.Cyk(Sentence.FromLetters("cb")));
		}

		[TestMethod]
		public void TestToCNF04() {
			var productions = new HashSet<BaseProduction> {
				CFGParser.Production("<A> -> <A> <B>"),
				CFGParser.Production("<A> -> ε"),
				CFGParser.Production("<B> -> 'b'"),
				CFGParser.Production("<B> -> ε"),
			};

			Grammar g = new Grammar(productions, Nonterminal.Of("A"));
			CNFGrammar h = g.ToCNF();

			var third = 1.0 / 3.0;

			Helpers.AssertNear(0.5 + third * 0.5, h.Cyk(Sentence.FromLetters("")));
			Helpers.AssertNear(0.222222, h.Cyk(Sentence.FromLetters("b")));
		}

		[TestMethod]
		public void TestCNFNoNull01() {
			var productions = new HashSet<BaseProduction> {
				CFGParser.Production("<A> -> <A> <B>"),
				CFGParser.Production("<A> -> <B>"),
				CFGParser.Production("<A> -> 'a'"),
				CFGParser.Production("<B> -> <A>"),
				CFGParser.Production("<B> -> 'b'"),
			};

			Grammar g = new Grammar(productions, Nonterminal.Of("A"));
			CNFGrammar h = g.ToCNF();

			Helpers.AssertNear(0, h.Cyk(Sentence.FromLetters("")));
			Helpers.AssertNear(0.83967276, h.Cyk(Sentence.FromLetters("a")));
			Helpers.AssertNear(0.0763439, h.Cyk(Sentence.FromLetters("b")));
			Helpers.AssertNear(0.06462378, h.Cyk(Sentence.FromLetters("ab")));
			Helpers.AssertNear(0.00585872, h.Cyk(Sentence.FromLetters("bb")));
			Helpers.AssertNear(0.00538844, h.Cyk(Sentence.FromLetters("aa")));
		}

		[TestMethod]
		public void TestCNFNoNull02() {
			var productions = new HashSet<BaseProduction> {
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
			var productions = new HashSet<BaseProduction> {
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
			var productions = new HashSet<BaseProduction> {
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
			var productions = new HashSet<BaseProduction> {
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
	}
}
