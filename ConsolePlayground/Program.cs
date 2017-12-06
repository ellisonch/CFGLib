using CFGLib;
using CFGLib.Parsers.CYK;
using CFGLib.Parsers.Earley;
using CFGLib.Parsers.Forests;
using CFGLibTest;
using CFGLibTest.Unit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CFGLib.Actioneer;

namespace ConsolePlayground {
	/// <summary>
	/// A console app for playing around.
	/// To use CFGLib, you only need the CFGLib project.
	/// This project is just to make my own testing and debugging easier.
	/// </summary>
	class Program {
		static void Main(string[] args) {
			Console.OutputEncoding = System.Text.Encoding.UTF8;
			//var rand = new Random(0);
			//Experimental.TestSolver(rand);
			// RandomTests.RandomJacobianTest();

			// PaperExamples();

			VisitorPlay();

			//var g = new Grammar(new List<Production>{
			//	CFGParser.Production("<S> → <X>"),
			//	CFGParser.Production("<X> → <X> <X> <X>"),
			//	CFGParser.Production("<X> → 'x'"),
			//	CFGParser.Production("<X> → ε"),
			//}, Nonterminal.Of("S"));

			//var h = g.ToCNF();
			//Console.WriteLine(g.ToCodeString());
			//Console.WriteLine(h.ToCodeString());

			//var t = new TestCFGToCNF();
			//var tp = new TestCFGToCNFEmptyProb();
			//var tr = new RegressionTests();
			//var testp = new TestParsing();

			// testp.TestParsing21();
			// testp.TestWeirdSppf06();
			// testp.TestWeirdSppf07();

			// Console.Read();

			//var g = new Grammar(new List<Production>{
			//	CFGParser.Production("<S> → 'x' <X>"),
			//	CFGParser.Production("<S> → <X> 'x'"),
			//	CFGParser.Production("<S> → 'x' 'x'"),
			//	CFGParser.Production("<X> → 'x'"),
			//}, Nonterminal.Of("S"));
			//var g = new Grammar(new List<Production>{
			//	CFGParser.Production("<S> → <S> <S> <S>"),
			//	CFGParser.Production("<S> → 'x'"),
			//	CFGParser.Production("<S> → ε"),
			//}, Nonterminal.Of("S"));

			//var g = new Grammar(new List<Production>{
			//	CFGParser.Production("<S> → <S> <S>"),
			//	CFGParser.Production("<S> → 'b'"),
			//	CFGParser.Production("<S> → ε"),
			//}, Nonterminal.Of("S"));

			//var g = new Grammar(new List<Production>{
			//	CFGParser.Production("<S> → <S> '+' <S>"),
			//	// CFGParser.Production("<S> → <S> '*' <S>"),
			//	// CFGParser.Production("<S> → [0-9]+"),
			//	CFGParser.Production("<S> → '0'"),
			//	// CFGParser.Production("<S> → '2'"),
			//}, Nonterminal.Of("S"));
			//var ests = g.EstimateProbabilities(10000);
			//foreach (var est in ests) {
			//	Console.WriteLine("{0}: {1}", est.Key, est.Value);
			//}

			// 0 + 123 * 72

			//var ep = new EarleyParser(g);
			//var sppf = ep.ParseGetForest(Sentence.FromWords("0 + 0 + 0"));

			//// var sppf = ep.ParseGetForest(Sentence.FromWords("x x"));
			//// var sppf = ep.ParseGetForest(Sentence.FromWords("b"));
			////Console.WriteLine();
			//Console.WriteLine(sppf);
			//// var dot = ForestHelpers.ToDot(sppf);
			
			//var rawdot = sppf.GetRawDot();
			//DotRunner.Run(rawdot, "rawGraph");

			//var dot = sppf.ToDot();
			//DotRunner.Run(dot, "addition");

			// var dotShared = ForestHelpers.ToDot(sppf, true);
			//var dotShared = sppf.ToDot(true);
			//DotRunner.Run(dotShared, "additionShared");

			//var pp = new PrettyPrinter();
			//sppf.Accept(pp);
			//Console.WriteLine(pp.Result);

			//// Console.WriteLine(sppf.ToStringHelper("", new HashSet<Sppf>()));
			//Console.WriteLine();
			// Readme.Do();
			//var p = CFGParser.Production("<S> -> 'a' [5]");
			//Console.WriteLine(p);

			//Console.Read();
			//return;

			//var rt = new CFGLibTest.RandomTests();
			//var sw = Stopwatch.StartNew();
			//// rt.RandomParsingTest(50000, 4, 3, 5, 4, 6, 1);
			//// rt.RandomParsingTest(500, 10, 5, 30, 8, 6);
			//rt.RandomParsingTest(1, 10, 5, 50, 8, 6);
			//sw.Stop();
			//Console.WriteLine("Elapsed: {0}s", sw.Elapsed.TotalMilliseconds / 1000.0);

			
			Console.WriteLine("Finished!");
			Console.Read();
		}

		// from http://dx.doi.org/10.1016/j.entcs.2008.03.044
		private static void PaperExamples() {
			var ex3 = new Grammar(new List<Production>{
				CFGParser.Production("<S> → <A> <T>"),
				CFGParser.Production("<S> → 'a' <T>"),
				CFGParser.Production("<A> → 'a'"),
				CFGParser.Production("<A> → <B> <A>"),
				CFGParser.Production("<B> → ε"),
				CFGParser.Production("<T> → 'b' 'b' 'b'"),
			}, Nonterminal.Of("S"));
			var sppf3 = ex3.ParseGetForest(Sentence.FromLetters("abbb"));
			DotRunner.Run(sppf3.GetRawDot(), "example3");
		}

		private static GrammarPlus AdditionGrammar() {
			var p1 = CFGParser.Production("<S> → <S> '+' <S>");
			var nums = new List<Production> {
				CFGParser.Production("<S> → '0'"),
				CFGParser.Production("<S> → '1'"),
				CFGParser.Production("<S> → '2'"),
				CFGParser.Production("<S> → '3'"),
				CFGParser.Production("<S> → '4'"),
				CFGParser.Production("<S> → '5'"),
				CFGParser.Production("<S> → '6'"),
				CFGParser.Production("<S> → '7'"),
				CFGParser.Production("<S> → '8'"),
				CFGParser.Production("<S> → '9'"),
			};
			var g = new Grammar(new List<Production>{
				p1,
			}.Concat(nums), Nonterminal.Of("S"));

			//var h = g.ToCNF();
			Console.WriteLine(g.ToCodeString());
			Console.WriteLine();

			var actions = new List<ProductionPlus> {
				// new Dictionary<Production, IParserAction> {
				new ProductionPlus(p1, new ParserAction((argList) => string.Format("({0} + {1})", argList[0].Payload, argList[2].Payload)), 5, new GatherOption[]{ GatherOption.SameOrLower, GatherOption.StrictlyLower }),
			};
			var termAction1 = new ParserAction(x => Convert.ToInt64(x[0].Payload));
			foreach (var num in nums) {
				actions.Add(new ProductionPlus(num, termAction1, 0, new GatherOption[] { }));
				// actions[num] = ;
			}
			var gp = new GrammarPlus(g, actions);
			return gp;
		}

		private static void VisitorPlay() {
			var gp = AdditionGrammar();
			
			//var actions2 = new Dictionary<Production, IParserAction> {
			//	[p1] = new ParserAction<long>((argList) => (long)(argList[0].Payload) + (long)(argList[2].Payload)),
			//};
			//var termAction2 = new ParserAction<long>(x => Convert.ToInt64(x[0].Payload));
			//foreach (var num in nums) {
			//	actions2[num] = termAction2;
			//}

			var ep = new EarleyParser(gp.Grammar);

			var inputString = AdditionInput(3);
			var input = Sentence.FromWords(inputString);
			var sppf = ep.ParseGetForest(input);

			//var rawdot = sppf.ToDot();
			//DotRunner.Run(rawdot, "rawGraph");

			Console.WriteLine(sppf.ToString());
			Console.WriteLine();

			Console.WriteLine("Starting Traversal...");
			var trav = new Traversal(sppf, input, gp);
			var resultList = trav.Traverse();
			Console.WriteLine("-----------------");
			foreach (var result in resultList) {
				Console.WriteLine(result.Payload);
			}

			//Console.WriteLine("-----------------");
			//foreach (var result in new Traversal(sppf, input, actions2).Traverse()) {
			//	Console.WriteLine(result.Payload);
			//}

		}

		private static string AdditionInput(int count) {
			if (count == 0) {
				return "";
			}

			var retval = "1";
			while (count > 1) {
				retval += " + 1";
				count--;
			}
			return retval;
		}
	}
}
