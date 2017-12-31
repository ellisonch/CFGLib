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
		public static void CheckTraversal(Grammar g, Sentence sentence, CFGLib.Parsers.Sppf.SppfNode sppf) {
			if (sppf == null) {
				return;
			}
			var t = new Traversal(sppf, g);
			TraverseResultCollection r = null;
			r = t.Traverse();
			foreach (var option in r) {
				if (!(option.Payload is Sentence)) {

				}
				var sgen = (Sentence)option.Payload;
				if (!sentence.SequenceEqual(sgen)) {
					throw new Exception();
				}
			}
		}

		private static void ExecuteTest(Grammar g, string input) {
			var ag = IdentityActions.Annotate(g);
			var earley1 = new EarleyParser(ag);
			var earley2 = new EarleyParser2(ag);
			var sentence = Sentence.FromWords(input);

			var sppf1 = earley1.ParseGetForest(sentence);
			var sppf2 = earley2.ParseGetForest(sentence);

			CheckTraversal(ag, sentence, sppf1);
			CheckTraversal(ag, sentence, sppf2);
		}
		[TestMethod]
		public void TestTraversalAddition() {
			ExecuteTest(new Grammar(new List<Production>{
				CFGParser.Production("<S> → <S> '+' <S>"),
				CFGParser.Production("<S> → '1'")
			}, Nonterminal.Of("S")), "1 + 1 + 1");
		}
		[TestMethod]
		[ExpectedException(typeof(TraversalLoopException))]
		public void TestTraversalInfinite() {
			var g = new Grammar(new List<Production>{
				CFGParser.Production("<A> → <B>"),
				CFGParser.Production("<B> → <A>"),
				CFGParser.Production("<B> → 'x'"),
			}, Nonterminal.Of("A"));
			ExecuteTest(g, "x");
		}
		[TestMethod]
		public void TestTraversalLinear() {
			var g = new Grammar(new List<Production>{
				CFGParser.Production("<S> → <A>"),
				CFGParser.Production("<S> → <B>"),
				CFGParser.Production("<S> → <C>"),
				CFGParser.Production("<A> → '1'"),
				CFGParser.Production("<B> → '1'"),
				CFGParser.Production("<C> → '1'")
			}, Nonterminal.Of("S"));
			ExecuteTest(g, "1");
		}
		[TestMethod]
		public void TestTraversalEmpty() {
			var g = new Grammar(new List<Production>{
				CFGParser.Production("<S> → <A>"),
				CFGParser.Production("<A> → ε"),
			}, Nonterminal.Of("S"));
			ExecuteTest(g, "");
		}

		[TestMethod]
		public void TestTraversal01() {
			ExecuteTest(new Grammar(new List<Production>{
				CFGParser.Production("<X_0> → ε [115.49728913674936]"),
				CFGParser.Production("<X_0> → 'x0' 'x0' <X_0> 'x0' 'x0' [32.857227595456521]")
			}, Nonterminal.Of("X_0")), "ε");
		}

		[TestMethod]
		public void TestTraversal02() {
			ExecuteTest(new Grammar(new List<Production>{
				CFGParser.Production("<X_0> → 'x0' [91.822083917829246]")
			}, Nonterminal.Of("X_0")), "x0");
		}

		[TestMethod]
		public void TestTraversal03() {
			ExecuteTest(new Grammar(new List<Production>{
				CFGParser.Production("<X_0> → <X_1> <X_0> <X_1> 'x1' 'x1' <X_0> <X_0> <X_0> 'x0' 'x1' [53.389437636541871]"),
				CFGParser.Production("<X_0> → <X_1> [11.500305104302385]"),
				CFGParser.Production("<X_0> → <X_0> <X_0> <X_0> 'x0' [90.413361106726043]"),
				CFGParser.Production("<X_1> → 'x0' [13.006726633760485]")
			}, Nonterminal.Of("X_0")), "x0");
		}
		
		[TestMethod]
		public void TestTraversal04() {
			ExecuteTest(new Grammar(new List<Production>{
				CFGParser.Production("<X_0> → 'x0' <X_0> [53.221112815766176]"),
				CFGParser.Production("<X_0> → ε [172.512627911527]"),
				CFGParser.Production("<X_0> → 'x0' 'x0' <X_0> <X_0> [52.465847087775288]"),
				CFGParser.Production("<X_0> → <X_0> <X_0> <X_0> 'x0' 'x0' [72.139131426410344]"),
				CFGParser.Production("<X_0> → <X_0> <X_0> 'x0' 'x0' 'x0' [87.645744548479911]")
			}, Nonterminal.Of("X_0")), "x0 x0 x0 x0 x0 x0 x0 x0 x0 x0 x0 x0 x0 x0");
		}

		[TestMethod]
		public void TestTraversal05() {
			ExecuteTest(new Grammar(new List<Production>{
				CFGParser.Production("<S> → <S> <S>"),
				CFGParser.Production("<S> → 'x'"),
			}, Nonterminal.Of("S")), "x x x x x x x x x x x x x x x x");
		}
		
	}
}
