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
		[TestMethod]
		public void TestProductionWeight() {
			Func<double, BaseProduction> prodp = (w) => new Production(
				Nonterminal.Of("S"),
				new Sentence { Nonterminal.Of("X") },
				w
			);

			var p1 = CFGParser.Production(@"<S> -> <X> [3]");
			var p2 = CFGParser.Production(@"<S> -> <X> [3.0000]");
			var p3 = CFGParser.Production(@"<S> -> <X> [0.5]");

			Assert.IsTrue(p1.ValueEquals(p2));

			Assert.IsTrue(p1.ValueEquals(prodp(3.0)));
			Assert.IsFalse(p1.ValueEquals(prodp(1.0)));

			Assert.IsTrue(p3.ValueEquals(prodp(0.5)));
		}
	}
}
