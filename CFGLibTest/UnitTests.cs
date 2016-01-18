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
	}
}
