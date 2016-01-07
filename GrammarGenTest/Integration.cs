using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using GrammarGen;

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
	}
}
