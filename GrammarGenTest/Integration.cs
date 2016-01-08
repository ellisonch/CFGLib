using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ContextFreeGrammars;

namespace GrammarGenTest {
	[TestClass]
	public class Integration {
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
			Grammar g = new Grammar(productions);
			CNFGrammar h = g.ToCNF();

			Assert.IsFalse(h.Cyk(Sentence.FromLetters("ab")));
			Assert.IsFalse(h.Cyk(Sentence.FromLetters("abc")));
			Assert.IsFalse(h.Cyk(Sentence.FromLetters("aaa")));
			Assert.IsFalse(h.Cyk(Sentence.FromLetters("abbba")));

			Assert.IsTrue(h.Cyk(Sentence.FromLetters("")));
			Assert.IsTrue(h.Cyk(Sentence.FromLetters("aa")));
			Assert.IsTrue(h.Cyk(Sentence.FromLetters("bb")));
			Assert.IsTrue(h.Cyk(Sentence.FromLetters("abba")));
			Assert.IsTrue(h.Cyk(Sentence.FromLetters("baab")));
			Assert.IsTrue(h.Cyk(Sentence.FromLetters("aaaa")));
			Assert.IsTrue(h.Cyk(Sentence.FromLetters("bbbb")));
			Assert.IsTrue(h.Cyk(Sentence.FromLetters("aaabbabbabbaaa")));
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
			
			var g = new CNFGrammar(nonterminalProductions, terminalProductions, false, Variable.Of("X_0"));

			
			Assert.IsFalse(g.Cyk(Sentence.FromLetters("abc")));
			Assert.IsFalse(g.Cyk(Sentence.FromLetters("ab")));
			Assert.IsFalse(g.Cyk(Sentence.FromLetters("ba")));
			Assert.IsFalse(g.Cyk(Sentence.FromLetters("aabb")));
			Assert.IsFalse(g.Cyk(Sentence.FromLetters("abba")));

			Assert.IsTrue(g.Cyk(Sentence.FromLetters("a")));
			Assert.IsTrue(g.Cyk(Sentence.FromLetters("b")));
		}
	}
}
