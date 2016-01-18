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
		public void TestCYK() {
			var nonterminalProductions = new List<CNFNonterminalProduction> {
				new CNFNonterminalProduction(
					Nonterminal.Of("S"),
					Nonterminal.Of("S"), Nonterminal.Of("S"),
					2
				)
			};
			var terminalProductions = new List<CNFTerminalProduction> {
				new CNFTerminalProduction(
					Nonterminal.Of("S"),
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

			Helpers.AssertNear(0.0032768, g.Cyk(Sentence.FromLetters("aaaa")));
		}
	}
}
