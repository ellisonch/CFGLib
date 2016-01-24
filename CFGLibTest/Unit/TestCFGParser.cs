using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CFGLib;

namespace CFGLibTest.Unit {
	[TestClass]
	public class TestCFGParser {
		[TestMethod]
		public void TestProduction() {
			var actualp = CFGParser.Production(@"<S> -> <X> 'a' <S> [3.0]");
			var expectedp = new Production(
				Nonterminal.Of("S"),
				new Sentence {
					Nonterminal.Of("X"),
					Terminal.Of("a"),
					Nonterminal.Of("S")
				},
				3.0
			);
			var unexpectedp = new Production(
				Nonterminal.Of("S"),
				new Sentence {
					Terminal.Of("a"),
					Nonterminal.Of("X"),
					Nonterminal.Of("S")
				},
				3.0
			);

			Assert.IsTrue(actualp.ValueEquals(expectedp));
			Assert.IsFalse(actualp.ValueEquals(unexpectedp));
		}
	}
}
