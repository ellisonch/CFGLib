using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CFGLib;
using System.Collections.Generic;
using System.Linq;

namespace CFGLibTest.Unit {
	[TestClass]
	public class TestCFGToCNFEmptyProb {
		[TestMethod]
		public void TestNullate01() {
			PrivateType cfgToCnf = new PrivateType(typeof(CFGtoCNF));

			var production = CFGParser.Production("<S> -> <A> 'b' <B> [1]");
			var nullableDictionary = new Dictionary<Nonterminal, double> {
				{ Nonterminal.Of("A"), 0.5 },
				{ Nonterminal.Of("B"), 0.2 }
			};

			var actualList = (List<Production>)cfgToCnf.InvokeStatic("Nullate", new object[] { production, nullableDictionary });
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
			var productions = new List<Production> {
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
			var productions = new List<Production> {
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
		public void TestToCNF01() {
			var productions = new List<Production> {
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
			var productions = new List<Production> {
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
			Helpers.AssertNear(1.0, pa + pab + pabb);
		}

		[TestMethod]
		public void TestToCNF03() {
			var productions = new HashSet<Production> {
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
			var productions = new HashSet<Production> {
				CFGParser.Production("<A> -> <A> <B>"),
				CFGParser.Production("<A> -> ε"),
				CFGParser.Production("<B> -> 'b'"),
				CFGParser.Production("<B> -> ε"),
			};

			Grammar g = new Grammar(productions, Nonterminal.Of("A"));
			CNFGrammar h = g.ToCNF();

			var third = 1.0 / 3.0;

			Helpers.AssertNear(0.5 + third * 0.5, h.Cyk(Sentence.FromLetters("")));
			Helpers.AssertNear(third * 2.0 / 3.0, h.Cyk(Sentence.FromLetters("b")));
		}

		[TestMethod]
		public void TestToCNF05() {
			Grammar g = new Grammar(new List<Production>{
				CFGParser.Production("<X_0> → 'x4' <X_4> [16.517998587115667]"),
				CFGParser.Production("<X_4> → 'x3' [49.290950734303777]"),
				CFGParser.Production("<X_0> → 'x4' 'x1' [23.628313965456705]")
			}, Nonterminal.Of("X_0"));
			CNFGrammar h = g.ToCNF();
	
			var first = 16.517998587115667;
			var third = 23.628313965456705;
			var sum = first + third;
			Helpers.AssertNear(third / sum, h.Cyk(Sentence.FromWords("x4 x1")));
			Helpers.AssertNear(first / sum , h.Cyk(Sentence.FromWords("x4 x3")));
		}

		[TestMethod]
		public void TestToCNF06() {
			Grammar g = new Grammar(new List<Production>{
				CFGParser.Production("<S> → ε"),
				CFGParser.Production("<S> → 'x' <A>")
			}, Nonterminal.Of("S"));
			CNFGrammar h = g.ToCNF();

			Helpers.AssertNear(1.0, h.Cyk(Sentence.FromWords("")));
		}

		[TestMethod]
		public void TestToCNF07() {
			Grammar g = new Grammar(new List<Production>{
				CFGParser.Production("<X_0> → 'x1' 'x4' <X_2> <X_6> [48.024797111295534]"),
				CFGParser.Production("<X_1> → <X_0> 'x0' [28.859845304796398]"),
				CFGParser.Production("<X_4> → 'x4' 'x4' 'x4' <X_3> <X_6> [32.396577754708275]"),
				CFGParser.Production("<X_4> → ε [46.519217869974312]"),
				CFGParser.Production("<X_0> → 'x4' 'x0' 'x3' 'x3' [90.918005973993814]"),
				CFGParser.Production("<X_6> → <X_4> <X_0> 'x3' <X_3> [31.319837867431264]"),
				CFGParser.Production("<X_2> → <X_2> [72.917730323932]"),
				CFGParser.Production("<X_3> → <X_1> [36.901735786302822]"),
				CFGParser.Production("<X_6> → ε [40.092343899464396]"),
				CFGParser.Production("<X_2> → 'x1' [16.801839537826293]"),
				CFGParser.Production("<X_0> → ε [136.892430380868]")
			}, Nonterminal.Of("X_0"));
			CNFGrammar h = g.ToCNF();

			// Helpers.AssertNear(1.0, h.Cyk(Sentence.FromWords("")));
		}

		[TestMethod]
		public void TestGetNullable01() {
			PrivateType cfgToCnf = new PrivateType(typeof(CFGtoCNF));

			var productions = new HashSet<Production> {
				CFGParser.Production("<X> -> <B> <B>"),
				CFGParser.Production("<B> -> 'b'"),
				CFGParser.Production("<B> -> ε"),
			};

			var result = (Dictionary<Nonterminal, double>)cfgToCnf.InvokeStatic("GetNullable", new object[] { productions });

			Assert.IsTrue(result.Count == 2);
			Helpers.AssertNear(0.5, result[Nonterminal.Of("B")]);
			Helpers.AssertNear(0.25, result[Nonterminal.Of("X")]);
		}

		[TestMethod]
		public void TestGetNullable02() {
			PrivateType cfgToCnf = new PrivateType(typeof(CFGtoCNF));
			var productions = new HashSet<Production> {
				CFGParser.Production("<A> -> <A> <B>"),
				CFGParser.Production("<A> -> ε"),
				CFGParser.Production("<B> -> 'b'"),
				CFGParser.Production("<B> -> ε"),
			};

			var result = (Dictionary<Nonterminal, double>)cfgToCnf.InvokeStatic("GetNullable", new object[] { productions });

			Assert.IsTrue(result.Count == 2);
			Helpers.AssertNear(2.0 / 3.0, result[Nonterminal.Of("A")]);
			Helpers.AssertNear(0.5, result[Nonterminal.Of("B")]);
		}

		[TestMethod]
		public void TestGetNullable03() {
			PrivateType cfgToCnf = new PrivateType(typeof(CFGtoCNF));

			var productions = new HashSet<Production> {
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
			Helpers.AssertNear(0.25, result[Nonterminal.Of("A")]);
			Helpers.AssertNear(0.5, result[Nonterminal.Of("B")]);
			Helpers.AssertNear(0.5, result[Nonterminal.Of("C")]);
		}

		[TestMethod]
		public void TestGetNullable04() {
			PrivateType cfgToCnf = new PrivateType(typeof(CFGtoCNF));

			var productions = new HashSet<Production> {
				CFGParser.Production("<A> -> <A> <A>"),
				CFGParser.Production("<A> -> ε"),
				CFGParser.Production("<A> -> 'a'"),
			};

			var result = (Dictionary<Nonterminal, double>)cfgToCnf.InvokeStatic("GetNullable", new object[] { productions });

			Assert.IsTrue(result.Count == 1);
			var phiRecip = 2.0 / (1.0 + Math.Sqrt(5));
			Helpers.AssertNear(1.0 - phiRecip, result[Nonterminal.Of("A")]);
		}

		[TestMethod]
		public void TestGetNullable05() {
			PrivateType cfgToCnf = new PrivateType(typeof(CFGtoCNF));

			var productions = new HashSet<Production> {
				CFGParser.Production("<S> -> <A>"),
				CFGParser.Production("<S> -> <B>"),
				CFGParser.Production("<A> -> ε"),
				CFGParser.Production("<A> -> 'a'"),
				CFGParser.Production("<A> -> <B>"),
				CFGParser.Production("<B> -> ε"),
				CFGParser.Production("<B> -> 'b'"),
				CFGParser.Production("<B> -> <A>"),
			};

			var result = (Dictionary<Nonterminal, double>)cfgToCnf.InvokeStatic("GetNullable", new object[] { productions });

			Assert.IsTrue(result.Count == 3);
			Helpers.AssertNear(0.5, result[Nonterminal.Of("S")]);
			Helpers.AssertNear(0.5, result[Nonterminal.Of("A")]);
			Helpers.AssertNear(0.5, result[Nonterminal.Of("B")]);
		}

		[TestMethod]
		public void TestGetNullable06() {
			PrivateType cfgToCnf = new PrivateType(typeof(CFGtoCNF));

			var productions = new HashSet<Production> {
				CFGParser.Production("<S> -> ε"),
				CFGParser.Production("<S> -> <S> <S> <S>"),
			};

			var result = (Dictionary<Nonterminal, double>)cfgToCnf.InvokeStatic("GetNullable", new object[] { productions });

			Assert.IsTrue(result.Count == 1);
			Helpers.AssertNear(1, result[Nonterminal.Of("S")]);
		}

		[TestMethod]
		public void TestGetNullable07() {
			PrivateType cfgToCnf = new PrivateType(typeof(CFGtoCNF));

			var productions = new HashSet<Production> {
				CFGParser.Production("<S> -> 'x'"),
				CFGParser.Production("<S> -> <S> <S> <S>"),
			};

			var result = (Dictionary<Nonterminal, double>)cfgToCnf.InvokeStatic("GetNullable", new object[] { productions });

			Assert.IsTrue(result.Count == 1);
			Helpers.AssertNear(0, result[Nonterminal.Of("S")]);
		}

		[TestMethod]
		public void TestGetNullable08() {
			PrivateType cfgToCnf = new PrivateType(typeof(CFGtoCNF));

			var productions = new HashSet<Production> {
				CFGParser.Production("<S> -> 'x'"),
				CFGParser.Production("<S> -> ε"),
				CFGParser.Production("<S> -> <S> <S> <S>"),
			};

			var result = (Dictionary<Nonterminal, double>)cfgToCnf.InvokeStatic("GetNullable", new object[] { productions });

			Assert.IsTrue(result.Count == 1);
			Helpers.AssertNear(0.34729635533386069770343325353862959, result[Nonterminal.Of("S")]);
		}

	}
}
