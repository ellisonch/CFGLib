using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CFGLib;
using System.Linq;

namespace CFGLibTest.Unit {
	/// <summary>
	/// Summary description for UnitTests
	/// </summary>
	[TestClass]
	public class UnitTests {
		[TestMethod]
		public void TestProduceToDepth() {
			var g = new CNFGrammar(
				Enumerable.Empty<Production>(),
				Nonterminal.Of("S")
			);
			Assert.IsTrue(g.ProduceToDepth(1).Count == 0);
			Assert.IsTrue(g.ProduceToDepth(2).Count == 0);
		}

		[TestMethod]
		public void TestCYK01() {
			var productions = new List<Production> {
				Production.New(
					Nonterminal.Of("S"),
					new Sentence { Nonterminal.Of("X"), Nonterminal.Of("X") },
					2
				),
				Production.New(
					Nonterminal.Of("X"),
					new Sentence { Nonterminal.Of("X"), Nonterminal.Of("X") },
					2
				),
				Production.New(
					Nonterminal.Of("S"),
					new Sentence { Terminal.Of("a") },
					8
				),
				Production.New(
					Nonterminal.Of("X"),
					new Sentence { Terminal.Of("a") },
					8
				)
			};

			var g = new CNFGrammar(productions, Nonterminal.Of("S"));
			
			Helpers.AssertNear(0.8, g.Cyk(Sentence.FromLetters("a")));
			Helpers.AssertNear(0.128, g.Cyk(Sentence.FromLetters("aa")));
			Helpers.AssertNear(0.04096, g.Cyk(Sentence.FromLetters("aaa")));
			Helpers.AssertNear(0.016384, g.Cyk(Sentence.FromLetters("aaaa")));
			Helpers.AssertNear(0.007340032, g.Cyk(Sentence.FromLetters("aaaaa")));
		}

		[TestMethod]
		public void TestDisjointProbability() {
			var l = new double[] { 0.2, 0.3, 0.4 };
			var dp = CFGLib.Helpers.DisjointProbability(l);

			var truedp = (0.2 + 0.3 + 0.4)
				- (0.2 * 0.3) - (0.2 * 0.4) - (0.3 * 0.4)
				+ (0.2 * 0.3 * 0.4);

			Helpers.AssertNear(truedp, dp);
		}

		[TestMethod]
		public void TestBadProduction() {
			Helpers.AssertThrows<ArgumentException>(() => Production.New(Nonterminal.Of("S"), new Sentence(), double.PositiveInfinity));
			Helpers.AssertThrows<ArgumentException>(() => Production.New(Nonterminal.Of("S"), new Sentence(), double.NegativeInfinity));
			Helpers.AssertThrows<ArgumentException>(() => Production.New(Nonterminal.Of("S"), new Sentence(), double.NaN));
		}
	}
}
