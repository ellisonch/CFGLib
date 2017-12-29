using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CFGLib;
using System.Collections.Generic;
using CFGLib.Parsers.Earley;
using CFGLib.Actioneer;
using CFGLib.ProductionAnnotations.Actioning;
using System.Linq;

namespace CFGLibTest {
	[TestClass]
	public class TestTraversal {
		private static void ExecuteTest(Grammar g, string input) {
			var ag = IdentityActions.Annotate(g);
			var earley1 = new EarleyParser(ag);
			var earley2 = new EarleyParser2(ag);
			var sentence = Sentence.FromWords(input);

			var sppf1 = earley1.ParseGetForest(sentence);
			var sppf2 = earley2.ParseGetForest(sentence);
			var t1 = new Traversal(sppf1, ag);
			var t2 = new Traversal(sppf2, ag);
			var r1 = t1.Traverse();
			var r2 = t2.Traverse();

			foreach (var option in r1) {
				var s1 = (Sentence)option.Payload;
				if (!sentence.SequenceEqual(s1)) {
					throw new Exception();
				}
			}

			foreach (var option in r2) {
				var s2 = (Sentence)option.Payload;
				if (!sentence.SequenceEqual(s2)) {
					throw new Exception();
				}
			}

			//var s1 = (Sentence)r1.SingleOrDefault().Payload;
			//var s2 = (Sentence)r2.SingleOrDefault().Payload;

			//Assert.IsTrue(sentence.SequenceEqual(s1));
			//Assert.IsTrue(sentence.SequenceEqual(s2));
		}
		[TestMethod]
		public void TestTraversalAddition() {
			ExecuteTest(new Grammar(new List<Production>{
				CFGParser.Production("<S> → <S> '+' <S>"),
				CFGParser.Production("<S> → '1'")
			}, Nonterminal.Of("S")), "1 + 1 + 1");
		}
		[TestMethod]
		[Ignore]
		public void TestTraversalInfinite() {
			var g = new Grammar(new List<Production>{
				CFGParser.Production("<A> → <B>"),
				CFGParser.Production("<B> → <A>"),
				CFGParser.Production("<B> → 'x'"),
			}, Nonterminal.Of("A"));
			ExecuteTest(g, "x");
		}
	}
}
