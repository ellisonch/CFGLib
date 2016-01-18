using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CFGLib;
using System.Linq;

namespace CFGLibTest {
	/// <summary>
	/// Summary description for UnitTests
	/// </summary>
	[TestClass]
	public class UnitTests {
		[TestMethod]
		public void TestProduceToDepth() {
			var g = new CNFGrammar(
				Enumerable.Empty<CNFNonterminalProduction>(),
				Enumerable.Empty<CNFTerminalProduction>(),
				0,
				Nonterminal.Of("S")
			);
			Assert.IsTrue(g.ProduceToDepth(1).Count == 0);
			Assert.IsTrue(g.ProduceToDepth(2).Count == 0);
		}

		[TestMethod]
		public void TestCYK01() {
			var nonterminalProductions = new List<CNFNonterminalProduction> {
				new CNFNonterminalProduction(
					Nonterminal.Of("S"),
					Nonterminal.Of("X"), Nonterminal.Of("X"),
					2
				),
				new CNFNonterminalProduction(
					Nonterminal.Of("X"),
					Nonterminal.Of("X"), Nonterminal.Of("X"),
					2
				)
			};
			var terminalProductions = new List<CNFTerminalProduction> {
				new CNFTerminalProduction(
					Nonterminal.Of("S"),
					Terminal.Of("a"),
					8
				),
				new CNFTerminalProduction(
					Nonterminal.Of("X"),
					Terminal.Of("a"),
					8
				)
			};

			var g = new CNFGrammar(
				nonterminalProductions,
				terminalProductions,
				0,
				Nonterminal.Of("S")
			);

			// g.PrintProbabilities(5000000, 0.2);

			Helpers.AssertNear(0.8, g.Cyk(Sentence.FromLetters("a")));
			Helpers.AssertNear(0.128, g.Cyk(Sentence.FromLetters("aa")));
			Helpers.AssertNear(0.04096, g.Cyk(Sentence.FromLetters("aaa")));
			Helpers.AssertNear(0.016384, g.Cyk(Sentence.FromLetters("aaaa")));
			Helpers.AssertNear(0.007340032, g.Cyk(Sentence.FromLetters("aaaaa")));
		}
	}
}
