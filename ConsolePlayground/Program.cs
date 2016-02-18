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

			// testp.TestParsing05();
			// testp.TestWeirdSppf06();
			// testp.TestWeirdSppf07();

			// Console.Read();

			var g = new Grammar(new List<Production>{
				CFGParser.Production("<X_0> → <X_1>"),
				CFGParser.Production("<X_1> → ε"),
				CFGParser.Production("<X_1> → 'x2' <X_0> <X_0> 'x1'"),
				CFGParser.Production("<X_1> → <X_0>"),
			}, Nonterminal.Of("X_0"));

			var ep = new EarleyParser(g);
			var sppf = ep.ParseGetForest(Sentence.FromWords("x2 x1"));
			Console.WriteLine();
			Console.WriteLine(sppf);
			// Console.WriteLine(sppf.ToStringHelper("", new HashSet<Sppf>()));
			Console.WriteLine();

			// Readme.Do();

			//var p = CFGParser.Production("<S> -> 'a' [5]");
			//Console.WriteLine(p);

			var rt = new CFGLibTest.RandomTests();
			var sw = Stopwatch.StartNew();
			// rt.RandomSimplificationTest();
			// rt.RandomCFGToCNFTest();
			// rt.RandomParsingTest(10);
			rt.RandomParsingTest(10);
			sw.Stop();
			Console.WriteLine("Elapsed: {0}s", sw.Elapsed.TotalMilliseconds / 1000.0);

			
			Console.WriteLine("Finished!");
			Console.Read();
		}
	}
}
