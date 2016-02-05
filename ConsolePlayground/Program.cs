using CFGLib;
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
			// t.TestToCNF01();
			// t.TestAccepts09();
			t.TestAccepts02();
			// tp.TestGetNullable01();

			// Experimental.Test();

			//var productions = new HashSet<Production> {
			//	CFGParser.Production("<A> -> <A> <B>"),
			//	CFGParser.Production("<A> -> <B>"),
			//	CFGParser.Production("<A> -> 'a'"),
			//	CFGParser.Production("<A> -> ε"),
			//	CFGParser.Production("<B> -> <A>"),
			//	CFGParser.Production("<B> -> 'b'"),
			//	CFGParser.Production("<B> -> ε"),
			//};
			//var g = new Grammar(productions, Nonterminal.Of("A"));
			//var probs = g.EstimateProbabilities(5000000);

			//foreach (var entry in probs) {
			//	var key = entry.Key;
			//	var value = entry.Value;
			//	if (key.Length <= 2) {
			//		Console.WriteLine("{0}: {1}", key, value);
			//	}
			//}
			// Console.WriteLine(g.EstimateProbabilityNull(Nonterminal.Of("A"), 100000));
			//Console.WriteLine(g.EstimateProbabilityNull(Nonterminal.Of("B"), 10000));
			//Console.WriteLine(g);
			////var swps = g.ProduceToDepth(6, 100000);
			////foreach (var swp in swps) {
			////	Console.WriteLine(swp);
			////}
			//var h = g.ToCNF();
			//Console.WriteLine(h);
			//Console.WriteLine(h.Accepts(Sentence.FromWords("")));

			//var q = new CNFGrammar(new HashSet<BaseProduction> { }, new HashSet<BaseProduction> { }, 1, Nonterminal.Of("S"), true);
			//Console.WriteLine(q.Accepts(Sentence.FromWords("")));

			//var rt = new CFGLibTest.RandomTests();
			//var sw = Stopwatch.StartNew();
			//// rt.RandomAcceptanceTest();
			//rt.RandomCFGToCNFTest();
			//sw.Stop();
			//Console.WriteLine("Elapsed: {0}s", sw.Elapsed.TotalMilliseconds / 1000.0);

			//var productions = new HashSet<BaseProduction> {
			//	CFGParser.Production("<S> -> <A>"),
			//	CFGParser.Production("<S> -> <B>"),
			//	CFGParser.Production("<A> -> <B>"),
			//	CFGParser.Production("<B> -> <A>"),
			//	CFGParser.Production("<A> -> 'a'"),
			//	CFGParser.Production("<B> -> 'b'"),
			//};

			//Grammar g = new Grammar(productions, Nonterminal.Of("S"));
			//CNFGrammar h = g.ToCNF(false);
			//Console.WriteLine(g);
			//Console.WriteLine(h);

			// g.EstimateProbabilities(1000000);

			//var t = new CFGLibTest.RandomTests();
			//var sw = Stopwatch.StartNew();
			//t.RandomCFGToCNFTest();
			//sw.Stop();
			//Console.WriteLine("Elapsed: {0}s", sw.Elapsed.TotalMilliseconds / 1000.0);

			// CFGParser.Production("<X> -> <Y>");

			// Benchmark();
			// Readme();
			//8.7s

			//var p = CFGParser.Production("<S> -> 'a' [5]");
			//Console.WriteLine(p);

			//var rt = new CFGLibTest.RandomTests();
			//var sw = Stopwatch.StartNew();
			//// rt.RandomSimplificationTest();
			//rt.RandomCFGToCNFTest();
			//sw.Stop();
			//Console.WriteLine("Elapsed: {0}s", sw.Elapsed.TotalMilliseconds / 1000.0);


			//var randg = new GrammarGenerator();
			//var range = Enumerable.Range(0, 5);
			//var terminals = new List<Terminal>(range.Select((x) => Terminal.Of("x" + x)));
			//var g = randg.NextCFG(5, 10, 4, terminals);
			//Console.WriteLine(g);

			//var ut = new CFGLibTest.UnitTests();
			//ut.TestCYK02();

			Console.WriteLine("Finished!");
			Console.Read();
		}

		static void Benchmark() {
			// CFGParser.Production(@"<X> -> <X0> <X1> 'asdf as_-""fw' <Z23X>");
			int _numNonterminals = 10;
			int _numProductions = 100;
			int _numTerminals = 5;
			int _maxLength = 4;
			int _numGrammars = 80;

			var range = Enumerable.Range(0, _numTerminals);
			var terminals = new List<Terminal>(range.Select((x) => Terminal.Of("x" + x)));
			var preparedSentences = new List<Sentence>();
			for (int length = 1; length <= _maxLength; length++) {
				foreach (var target in CFGLibTest.Helpers.CombinationsWithRepetition(terminals, length)) {
					var sentence = new Sentence(target);
					preparedSentences.Add(sentence);
				}
			}

			var randg = new GrammarGenerator();
			var preparedGrammars = new List<CNFGrammar>();
			for (int i = 0; i < _numGrammars; i++) {
				var g = randg.NextCNF(_numNonterminals, _numProductions, terminals);
				// Console.WriteLine(g);
				// g.PrintProbabilities(2, 3);
				preparedGrammars.Add(g);
			}

			var sw = Stopwatch.StartNew();
			foreach (var g in preparedGrammars) {
				// Console.WriteLine(g);
				foreach (var sentence in preparedSentences) {
					var chance = g.Cyk(sentence);
					// Console.WriteLine("{0}: {1}", sentence, chance);
				}
			}
			sw.Stop();
			Console.WriteLine("Elapsed: {0}s", sw.Elapsed.TotalMilliseconds / 1000.0);
			Console.WriteLine("Per CYK: {0}ms", sw.Elapsed.TotalMilliseconds / (_numGrammars * preparedSentences.Count));
		}
		
		static void Readme() {
			// S -> aSa | bSb | ε
			var productions = new List<Production> {
			// construct productions by passing arguments...
			Production.New(
				lhs: Nonterminal.Of("S"),
				rhs: new Sentence { Terminal.Of("a"), Nonterminal.Of("S"), Terminal.Of("a") },
				weight: 20
			),
			// or from a string...
			CFGParser.Production(@"<S> -> 'b' <S> 'b' [10]"),
			CFGParser.Production(@"<S> -> ε [5]"),
};
			var cfg = new Grammar(productions, Nonterminal.Of("S"));
			var cnf = cfg.ToCNF();
			// Does this grammar accept the string "aabb"?
			Console.WriteLine(cnf.Accepts(Sentence.FromLetters("aabb")));
			// How about "abba"?
			Console.WriteLine(cnf.Accepts(Sentence.FromLetters("abba")));

			for (int i = 0; i < 5; i++) {
				Console.WriteLine(cfg.ProduceRandom().AsTerminals());
			}

			var sentences = cfg.ProduceToDepth(3);
			foreach (var sentence in sentences) {
				Console.WriteLine(sentence.Value.AsTerminals());
			}

			var gg = new GrammarGenerator(1);
			var terminals = new List<Terminal> { Terminal.Of("a"), Terminal.Of("b") };
			var randGram = gg.NextCFG(
				numNonterminals: 4,
				numProductions: 10,
				maxProductionLength: 4,
				terminals: terminals
			);
			Console.WriteLine(randGram);
		}
	}
}
