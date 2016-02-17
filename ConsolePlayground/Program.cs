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
			var tr = new RegressionTests();
			var testp = new TestParsing();

			testp.TestParsing14();
			// testp.TestWeirdSppf06();
			// testp.TestWeirdSppf07();
			// t.TestToCNF01();
			// tp.TestToCNF06();
			// t.TestAccepts01();
			// tr.TestParsing01();
			// tp.TestGetNullable01();
			// tp.TestGetNullable01();
			// Experimental.Test();

			// Console.Read();

			
			//var g = new Grammar(new List<Production>{
			//	CFGParser.Production("<S> → <A>"),
			//	CFGParser.Production("<A> → <S> 'x' <S>"),
			//	CFGParser.Production("<A> → ε")
			//}, Nonterminal.Of("S"));
			//var probs = g.EstimateProbabilities(100000);
			//foreach (var entry in probs) {
			//	var key = entry.Key;
			//	var value = entry.Value;
			//	if (key.Length <= 1) {
			//	Console.WriteLine("{0}: {1}", key, value);
			//	}
			//}
			//var h = g.ToCNF();
			//Sentence s;

			//s = Sentence.FromLetters("bb");
			//Console.WriteLine("Ear = {0}, CYK = {1}", g.Earley(s), h.Cyk(s));
			//// g.Earley(Sentence.FromLetters("bbb"));

			// Console.Read();

			//var productions = new HashSet<Production> {
			//	CFGParser.Production("<S> → <S> <T>"),
			//	CFGParser.Production("<S> → 'a'"),
			//	CFGParser.Production("<B> → ε"),
			//	CFGParser.Production("<T> → 'a' <B>"),
			//	CFGParser.Production("<T> → 'a'"),
			//	// CFGParser.Production("<S> → ε"),
			//};
			//var productions = new HashSet<Production> {
			//	CFGParser.Production("<Sum> → <Sum> '+' <Product>"),
			//	//CFGParser.Production("<Sum> → <Sum> '-' <Product>"),
			//	CFGParser.Production("<Sum> → <Product>"),
			//	CFGParser.Production("<Product> → <Product> '*' <Factor>"),
			//	//CFGParser.Production("<Product> → <Product> '/' <Factor>"),
			//	CFGParser.Production("<Product> → <Factor>"),
			//	CFGParser.Production("<Factor> → '(' <Sum> ')'"),
			//	CFGParser.Production("<Factor> → <Number>"),
			//	CFGParser.Production("<Number> → '0' <Number>"),
			//	CFGParser.Production("<Number> → '0'"),
			//};
			//var productions = new HashSet<Production> {
			//	CFGParser.Production("<A> → ε"),
			//	CFGParser.Production("<A> → <B>"),
			//	CFGParser.Production("<B> → <A>"),
			//};



			// g.Earley(Sentence.FromLetters("aa"));
			// g.Earley(Sentence.FromLetters("bbbb"));
			// g.Earley(Sentence.FromLetters("bbb"));
			// g.Earley(Sentence.FromLetters("b"));
			// Console.Read();
			// g.Earley(Sentence.FromWords("0 + ( 0 * 0 + 0 )"));
			// g.Earley(Sentence.FromWords(""));

			//Console.WriteLine(g);
			//g.AddProduction(CFGParser.Production("<S> → <S> <S>"));
			//Console.WriteLine(g);
			//var neww = CFGParser.Production("<S> → 'w'");
			//g.AddProduction(neww);
			//Console.WriteLine(g);
			//g.RemoveProduction(neww);
			//Console.WriteLine(g);

			//var sentences = g.ProduceToDepth(5);

			//// var probs = g.EstimateProbabilities(1000000);
			//var probs = g.EstimateProbabilities(1000000);
			//foreach (var entry in probs) {
			//	var key = entry.Key;
			//	var value = entry.Value;
			//	// if (key.Length <= 2) {
			//		Console.WriteLine("{0}: {1}", key, value);
			//	// }
			//}
			//Console.Read();


			// Console.WriteLine(g.EstimateProbabilityNull(Nonterminal.Of("S"), 1000000));
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


			//var productions = new HashSet<Production> {
			//	CFGParser.Production("<S> -> <S> <S>"),
			//	CFGParser.Production("<S> -> 'x'"),
			//	CFGParser.Production("<S> -> ε"),
			//};
			//Grammar g = new Grammar(productions, Nonterminal.Of("S"));
			//Stopwatch sw = Stopwatch.StartNew();
			//var sentences = g.ProduceToDepth(4);
			//foreach (var sentence in sentences) {
			//	Console.WriteLine(sentence.Value.AsTerminals());
			//}
			//sw.Stop();
			//Console.WriteLine("Elapsed: {0}s", sw.Elapsed.TotalMilliseconds / 1000.0);

			// Comparer();

			//var probs = h.EstimateProbabilities(100000);
			//foreach (var entry in probs) {
			//	var key = entry.Key;
			//	var value = entry.Value;
			//	// if (key.Length <= 2) {
			//	Console.WriteLine("{0}: {1}", key, value);
			//	// }
			//}
			// Console.Read();

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

			//Benchmark();


			// Readme();
			//8.7s

			//var p = CFGParser.Production("<S> -> 'a' [5]");
			//Console.WriteLine(p);

			var rt = new CFGLibTest.RandomTests();
			var sw = Stopwatch.StartNew();
			// rt.RandomSimplificationTest();
			// rt.RandomCFGToCNFTest();
			// rt.RandomParsingTest(10);
			rt.RandomParsingTest(1000);
			sw.Stop();
			Console.WriteLine("Elapsed: {0}s", sw.Elapsed.TotalMilliseconds / 1000.0);


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

		static void Compare() {
			var productions = new List<Production> {
				Production.New(
					Nonterminal.Of("S"),
					new Sentence { Terminal.Of("a"), Nonterminal.Of("S"), Terminal.Of("a") }
				),
				Production.New(
					Nonterminal.Of("S"),
					new Sentence { Terminal.Of("b"), Nonterminal.Of("S"), Terminal.Of("b") }
				),
				Production.New(
					Nonterminal.Of("S"),
					new Sentence { }
				)
			};
			Grammar g0 = new Grammar(productions, Nonterminal.Of("S"));
			var g = g0.ToCNF();
			Console.WriteLine(g.ToCodeString());

			var h = new CNFGrammar(new List<Production>{
				CFGParser.Production("<X_3> → <X_4> <X_5> [1]"),
				CFGParser.Production("<X_0> → <X_5> <X_5> [2.2]"),
				CFGParser.Production("<X_4> → <X_2> <X_1> [1]"),
				CFGParser.Production("<X_4> → <X_5> <X_3> [1]"),
				CFGParser.Production("<X_0> → <X_1> <X_1> [2.112]"),
				CFGParser.Production("<X_2> → <X_1> <X_4> [1]"),
				CFGParser.Production("<X_4> → <X_5> <X_5> [1.2]"),
				CFGParser.Production("<X_0> → <X_2> <X_1> [3.2]"),
				CFGParser.Production("<X_0> → <X_5> <X_3> [3.24]"),
				CFGParser.Production("<X_4> → <X_1> <X_1> [1.2]"),
				CFGParser.Production("<X_1> → 'a' [1]"),
				CFGParser.Production("<X_5> → 'b' [1]")
			}, Nonterminal.Of("X_0"));
			Console.WriteLine(h.ToCodeString());

			// CNFGrammar h = g.ToCNF();
			// Console.WriteLine(h);

			for (int i = 0; i < 8; i += 2) {
				var combinations = Helpers.CombinationsWithRepetition(new List<string> { "a", "b" }, i);

				foreach (var combination in combinations) {
					var s = Sentence.FromTokens(combination);
					var p1 = g.Cyk(s);
					var p2 = h.Cyk(s);
					Console.WriteLine("{0} {1} : {2}", p1.ToString("F5"), p2.ToString("F5"), s.AsTerminals());
				}
			}
		}

		static void Benchmark() {
			// CFGParser.Production(@"<X> -> <X0> <X1> 'asdf as_-""fw' <Z23X>");
			int _numNonterminals = 10;
			int _numProductions = 100;
			int _numTerminals = 5;
			int _maxLength = 5;
			int _numGrammars = 300;

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
			int count = 0;
			foreach (var g in preparedGrammars) {
				// Console.WriteLine(g);
				Console.Write("{0}, ", count);
				count++;
				foreach (var sentence in preparedSentences) {
					var chance = g.Cyk(sentence);
					// Console.WriteLine("{0}: {1}", sentence, chance);
				}
			}
			sw.Stop();
			Console.WriteLine();
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

			//var probs = cfg.EstimateProbabilities(1000000);
			//foreach (var entry in probs) {
			//	var key = entry.Key;
			//	var value = entry.Value;
			//	if (key.Length <= 4) {
			//	Console.WriteLine("{0}: {1}", key, value);
			//	}
			//}

			// Print out the new CNF grammar
			Console.WriteLine(cnf);
			// Does this grammar accept the string "aabb"?
			Console.WriteLine("aabb: {0}", cnf.Cyk(Sentence.FromLetters("aabb")));
			// How about "abba"?
			Console.WriteLine("abba: {0}", cnf.Cyk(Sentence.FromLetters("abba")));

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
