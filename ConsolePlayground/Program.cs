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

		private static void VisitorPlay() {
			var p1 = CFGParser.Production("<S> → <S> '+' <S>");
			var p2 = CFGParser.Production("<S> → '0'");
			var g = new Grammar(new List<Production>{
				p1,
				p2,
			}, Nonterminal.Of("S"));

			//var h = g.ToCNF();
			Console.WriteLine(g.ToCodeString());
			Console.WriteLine();

			var actions = new Dictionary<Production, ParserAction>();
			actions[p1] = new ParserAction((argList) => string.Format("({0} + {1})", argList[0], argList[1]));
			actions[p2] = new ParserAction((argList) => "0");

			//Console.WriteLine(h.ToCodeString());

			//var t = new TestCFGToCNF();
			//var tp = new TestCFGToCNFEmptyProb();
			//var tr = new RegressionTests();
			//var testp = new TestParsing();
			var ep = new EarleyParser(g);

			var inputString = "0 + 0 + 0";
			var input = Sentence.FromWords(inputString);
			var sppf = ep.ParseGetForest(input);

			//var rawdot = sppf.ToDot();
			//DotRunner.Run(rawdot, "rawGraph");

			Console.WriteLine(sppf.ToString());
			Console.WriteLine();

			var trav = new Traversal(sppf, input, actions);
			trav.Traverse();
		}
	}
}
