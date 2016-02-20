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

namespace ConsolePlayground {
	/// <summary>
	/// A console app for playing around.
	/// To use CFGLib, you only need the CFGLib project.
	/// This project is just to make my own testing and debugging easier.
	/// </summary>
	class Program {
		static void Main(string[] args) {
			//var rand = new Random(0);
			//Experimental.TestSolver(rand);
			// RandomTests.RandomJacobianTest();

			var t = new TestCFGToCNF();
			var tp = new TestCFGToCNFEmptyProb();
			var tr = new RegressionTests();
			var testp = new TestParsing();

			testp.TestParsing21();
			// testp.TestWeirdSppf06();
			// testp.TestWeirdSppf07();
			
			// Console.Read();

			//var g = new Grammar(new List<Production>{
			//	CFGParser.Production("<S> → <A> <A>"),
			//	CFGParser.Production("<A> → 'a' <A>"),
			//	CFGParser.Production("<A> → ε")
			//}, Nonterminal.Of("S"));
			//var ests = g.EstimateProbabilities(10000);
			//foreach (var est in ests) {
			//	Console.WriteLine("{0}: {1}", est.Key, est.Value);
			//}

			//var ep = new EarleyParser(g);
			//var sppf = ep.ParseGetForest(Sentence.FromWords("x2 x1"));
			//Console.WriteLine();
			//Console.WriteLine(sppf);
			//// Console.WriteLine(sppf.ToStringHelper("", new HashSet<Sppf>()));
			//Console.WriteLine();

			// Readme.Do();

			//var p = CFGParser.Production("<S> -> 'a' [5]");
			//Console.WriteLine(p);

			var rt = new CFGLibTest.RandomTests();
			var sw = Stopwatch.StartNew();
			rt.RandomParsingTest(50000, 4, 3, 5, 4, 6);
			rt.RandomParsingTest(500, 10, 5, 30, 8, 6);
			rt.RandomParsingTest(1, 10, 5, 50, 8, 6);
			sw.Stop();
			Console.WriteLine("Elapsed: {0}s", sw.Elapsed.TotalMilliseconds / 1000.0);

			
			Console.WriteLine("Finished!");
			Console.Read();
		}
	}
}
