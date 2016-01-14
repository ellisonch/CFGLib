using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using CFGLib;

namespace CFGLibTest {
	[TestClass]
	public class IntegrationTests {
		[TestMethod]
		public void TestMethod1() {
			// S -> aSa | bSb | ε
			var productions = new List<Production> {
				new Production(
					Variable.Of("S"),
					new Sentence { Terminal.Of("a"), Variable.Of("S"), Terminal.Of("a") }
				),
				new Production(
					Variable.Of("S"),
					new Sentence { Terminal.Of("b"), Variable.Of("S"), Terminal.Of("b") }
				),
				new Production(
					Variable.Of("S"),
					new Sentence { }
				)
			};
			Grammar g = new Grammar(productions, Variable.Of("S"));
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

		[TestMethod]
		public void TestMethod2() {
			//CNFGrammar(Var(X_0)){
			//	Var(X_0) → Var(X_0) Var(X_1)
			//	Var(X_2) → Trm(b)
			//	Var(X_0) → Trm(b)
			//	Var(X_0) → Trm(a)
			//	Var(X_2) → Trm(a)
			//}
			var terminalProductions = new List<CNFTerminalProduction> {
				new CNFTerminalProduction(Variable.Of("X_2"), Terminal.Of("b")),
				new CNFTerminalProduction(Variable.Of("X_0"), Terminal.Of("b")),
				new CNFTerminalProduction(Variable.Of("X_0"), Terminal.Of("a")),
				new CNFTerminalProduction(Variable.Of("X_2"), Terminal.Of("a")),
			};
			var nonterminalProductions = new List<CNFNonterminalProduction> {
				new CNFNonterminalProduction(Variable.Of("X_0"), Variable.Of("X_0"), Variable.Of("X_1"))
			};
			
			var g = new CNFGrammar(nonterminalProductions, terminalProductions, 0.0, Variable.Of("X_0"));

			
			Assert.IsFalse(g.Accepts(Sentence.FromLetters("abc")));
			Assert.IsFalse(g.Accepts(Sentence.FromLetters("ab")));
			Assert.IsFalse(g.Accepts(Sentence.FromLetters("ba")));
			Assert.IsFalse(g.Accepts(Sentence.FromLetters("aabb")));
			Assert.IsFalse(g.Accepts(Sentence.FromLetters("abba")));

			Assert.IsTrue(g.Accepts(Sentence.FromLetters("a")));
			Assert.IsTrue(g.Accepts(Sentence.FromLetters("b")));
		}

		[TestMethod]
		public void TestMethod3() {
			// http://www.cs.columbia.edu/~mcollins/courses/nlp2011/notes/pcfgs.pdf

			var nonterminalProductions = new List<CNFNonterminalProduction> {
				new CNFNonterminalProduction(
					Variable.Of("S"),
					Variable.Of("NP"), Variable.Of("VP"),
					10
				),
				new CNFNonterminalProduction(
					Variable.Of("VP"),
					Variable.Of("Vt"), Variable.Of("NP"),
					8
				),
				new CNFNonterminalProduction(
					Variable.Of("VP"),
					Variable.Of("VP"), Variable.Of("PP"),
					2
				),
				new CNFNonterminalProduction(
					Variable.Of("NP"),
					Variable.Of("DT"), Variable.Of("NN"),
					8
				),
				new CNFNonterminalProduction(
					Variable.Of("NP"),
					Variable.Of("NP"), Variable.Of("PP"),
					2
				),
				new CNFNonterminalProduction(
					Variable.Of("PP"),
					Variable.Of("IN"), Variable.Of("NP")
				),
			};
			var terminalProductions = new List<CNFTerminalProduction> {
				new CNFTerminalProduction(
					Variable.Of("Vt"),
					Terminal.Of("saw")
				),
				new CNFTerminalProduction(
					Variable.Of("NN"),
					Terminal.Of("man"),
					1
				),
				new CNFTerminalProduction(
					Variable.Of("NN"),
					Terminal.Of("woman"),
					1
				),
				new CNFTerminalProduction(
					Variable.Of("NN"),
					Terminal.Of("telescope"),
					3
				),
				new CNFTerminalProduction(
					Variable.Of("NN"),
					Terminal.Of("dog"),
					5
				),
				new CNFTerminalProduction(
					Variable.Of("DT"),
					Terminal.Of("the")
				),
				new CNFTerminalProduction(
					Variable.Of("IN"),
					Terminal.Of("with"),
					6
				),
				new CNFTerminalProduction(
					Variable.Of("IN"),
					Terminal.Of("in"),
					4
				),
			};
			var h = new CNFGrammar(nonterminalProductions, terminalProductions, 0.0, Variable.Of("S"));
			// CNFGrammar h = g.ToCNF();

			Assert.IsFalse(h.Accepts(Sentence.FromWords("saw the dog")));
			Assert.IsFalse(h.Accepts(Sentence.FromWords("with the telescope")));

			Assert.IsTrue(h.Accepts(Sentence.FromWords("the man saw the dog with the telescope")));

			// the man (0.8 * 0.1) = 0.08
			// saw the dog (0.8 * 0.8 * 0.5) = 0.32
			AssertNear(0.0256, h.Cyk(Sentence.FromWords("the man saw the dog")));

			// S -> NP VP
			// NP PP Vt NP // 0.2 * 0.8
			// DT NN PP saw DT NN // 0.8 * 0.8
			// the woman PP saw the dog // 0.1 * 0.5
			// the woman IN NP saw the dog
			// the woman with DT NN saw the dog // 0.6 * 0.8
			// the woman with the telescope saw the dog // 0.3
			// == 0.00073728
			AssertNear(0.00073728, h.Cyk(Sentence.FromWords("the woman with the telescope saw the dog")));
		}

		private static void AssertNear(double a, double b) {
			double tolerance = Math.Abs(a * 0.00001);
			double diff = Math.Abs(a - b);
			if (diff <= tolerance) {
				return;
			}
			Assert.Fail(string.Format("{0} is not near {1}", b, a));
		}
	}
}
