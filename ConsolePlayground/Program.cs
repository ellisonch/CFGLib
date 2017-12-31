using CFGLib;
using CFGLib.Parsers.CYK;
using CFGLib.Parsers.Earley;
using CFGLibTest;
using CFGLibTest.Unit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CFGLib.Actioneer;
using CFGLib.Parsers.Sppf;
using Grammars;
using CFGLib.ProductionAnnotations.Actioning;
using CFGLib.ProductionAnnotations;
using CFGLib.ProductionAnnotations.Precedencing;
using CFGLib.ProductionAnnotations.Gathering;

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

			//DebugGrammar();

			// var testp = new TestTraversal();
			// var sppf = testp.TestTraversal04();

			//var g = new Grammar(new List<Production>{
			//	CFGParser.Production("<X_0> → 'x1' <X_1> <X_1> <X_1> 'x0' 'x0' 'x1' [35.561233564541318]"),
			//	CFGParser.Production("<X_1> → 'x0' 'x1' <X_1> <X_0> 'x0' <X_0> <X_1> 'x0' 'x1' [93.742161775353438]"),
			//	CFGParser.Production("<X_0> → 'x0' <X_1> [21.769176176176025]"),
			//	CFGParser.Production("<X_1> → ε [15.296797436800226]"),
			//	CFGParser.Production("<X_0> → <X_1> <X_1> <X_1> 'x0' <X_0> <X_0> 'x1' [27.142061933009913]"),
			//	CFGParser.Production("<X_1> → 'x1' <X_1> 'x1' 'x1' <X_0> [61.381177142439959]"),
			//	CFGParser.Production("<X_1> → <X_0> 'x1' 'x0' <X_1> [43.405714859443584]"),
			//	CFGParser.Production("<X_1> → 'x1' <X_1> <X_1> <X_0> <X_1> <X_1> [62.132346960311914]"),
			//	CFGParser.Production("<X_1> → <X_1> 'x0' 'x0' 'x1' 'x0' 'x1' [86.420178335821333]"),
			//	CFGParser.Production("<X_0> → 'x0' <X_1> <X_1> 'x1' 'x0' 'x1' [49.638692241459474]"),
			//	CFGParser.Production("<X_0> → ε [79.0508000767095]"),
			//	CFGParser.Production("<X_1> → <X_1> <X_1> <X_0> <X_1> <X_0> 'x0' 'x0' 'x1' [83.5624104973685]"),
			//	CFGParser.Production("<X_1> → <X_1> 'x1' 'x0' [82.773570404282566]"),
			//	CFGParser.Production("<X_1> → 'x0' 'x1' 'x0' 'x1' <X_1> 'x0' [78.397427464554752]"),
			//	CFGParser.Production("<X_0> → 'x0' <X_1> <X_1> <X_1> <X_0> 'x0' 'x0' 'x1' 'x0' 'x0' [49.294450411710166]"),
			//	CFGParser.Production("<X_0> → 'x1' 'x0' 'x1' 'x0' 'x0' 'x1' <X_1> <X_1> 'x0' <X_1> [23.409070712704711]"),
			//	CFGParser.Production("<X_0> → 'x0' 'x1' <X_1> <X_0> 'x0' <X_1> 'x0' <X_1> 'x0' [20.120124363862036]")
			//}, Nonterminal.Of("X_0"));
			//var input = Sentence.FromWords("x1 x0 x0 x0 x0 x1 x1 x1 x1 x1 x1 x1 x0 x0 x1 x1 x1 x1 x1 x1 x0 x0 x1");
			//var sppf = (new EarleyParser2(g)).ParseGetForest(input);
			//DotRunner.Run(DotBuilder.GetRawDot(sppf), "oom");

			// BnfPlay();
			// ParserGenerator();
			// EbnfPlay();
			// EbnfBench();
			// VisitorPlay();
			//TraversePlay();

			(new ContinuousRandomTesting(4, 5, 10, 5, 6, 1000, 18)).Run();

			//Benchmark();
			// BenchmarkBison();

			#region junk 
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

			#endregion

			Console.WriteLine("Finished!");
			Console.Read();
		}

		private static void TraversePlay() {
			var g = new Grammar(new List<Production>{
				CFGParser.Production("<A> → <B>"),
				CFGParser.Production("<B> → <A>"),
				CFGParser.Production("<B> → 'x'"),
			}, Nonterminal.Of("A"));
			g = IdentityActions.Annotate(g);
			
			var earley2 = new EarleyParser2(g);
			var sentence = Sentence.FromWords("x");
			var sppf2 = earley2.ParseGetForest(sentence);
			DotRunner.Run(DotBuilder.GetRawDot(sppf2), "infinite");

			var t2 = new Traversal(sppf2, g);
			var r2 = t2.Traverse();
			//foreach (var option in r2) {
			//	var s2 = (Sentence)option.Payload;
			//	if (!sentence.SequenceEqual(s2)) {
			//		throw new Exception();
			//	}
			//}
		}

		private static void EbnfPlay() {
			var input = Sentence.FromLetters(Grammars.Properties.Resources.Ebnf_bench);
			Sentence inputNoLayout = Ebnf.RemoveLayout(input, out var layoutSppf);

			//DotRunner.Run(DotBuilder.GetRawDot(layoutSppf), "arithmetic_ebnf_layout");
			Console.WriteLine(inputNoLayout.AsTerminals());
			var layoutGrammar = Ebnf.Grammar(Nonterminal.Of("Syntax"));
			var earley = new EarleyParser2(layoutGrammar);

			var sppf = earley.ParseGetForest(inputNoLayout);
			if (sppf == null) {
				throw new Exception();
			}

			//DotRunner.Run(DotBuilder.GetRawDot(sppf), "arithmetic_ebnf");

			//var traversal = new Traversal(sppf, input, layoutGrammar);
			//var result = traversal.Traverse();
			//if (result.Count() != 1) {
			//	throw new Exception();
			//}
			//var inputNoLayout = new Sentence((List<Terminal>)result.First().Payload);
			//return inputNoLayout;

			//Console.WriteLine(inputNoLayout);
		}

		private static void EbnfBench() {
			var sw = Stopwatch.StartNew();
			var input = Sentence.FromLetters(Grammars.Properties.Resources.Ebnf_bench);
			Sentence inputNoLayout = Ebnf.RemoveLayout(input, out var layoutSppf);
			var ms1 = sw.Elapsed.TotalMilliseconds;
			Console.WriteLine("Layout: {0:0.#}ms", ms1);

			sw.Restart();

			var grammar = Ebnf.Grammar(Nonterminal.Of("Syntax"));
			var earley = new EarleyParser2(grammar);
			var sppf = earley.ParseGetForest(inputNoLayout);
			var ms2 = sw.Elapsed.TotalMilliseconds;
			if (sppf == null) {
				throw new Exception();
			}
			Console.WriteLine("Parse: {0:0.#}ms", ms2);
			Console.WriteLine("Total: {0:0.#}ms", ms1 + ms2);
		}

		private static void BnfPlay() {
			var bnf = Bnf.Grammar();
			var earley = new EarleyParser2(bnf);

			var sentence1 = Sentence.FromLetters(Grammars.Properties.Resources.Arithmetic);
			// var sentence1 = Sentence.FromLetters(Grammars.Properties.Resources.Bnf);
			// var sentence2 = Sentence.FromLetters("<S> ::= <S> '+' <S>\r\n<S> ::= '1'\r\n");
			// if (!sentence1.Equals(sentence2)) { 			}
			// int index = sentence1.Zip(sentence2, (c1, c2) => c1 == c2).TakeWhile(b => b).Count() + 1;
			var sppf = earley.ParseGetForest(sentence1);
			if (sppf == null) {
				throw new Exception();
			}
			DotRunner.Run(DotBuilder.GetRawDot(sppf), "bnfPlay");
		}

		private static void ParserGenerator() {
			var g = Ebnf.Grammar(Nonterminal.Of("SyntaxLayout"));
			var earley = new EarleyParser2(g);

			var sentence1 = Sentence.FromLetters(Grammars.Properties.Resources.Arithmetic_ebnf);
			// var sentence1 = Sentence.FromLetters(Grammars.Properties.Resources.Bnf);
			// var sentence2 = Sentence.FromLetters("<S> ::= <S> '+' <S>\r\n<S> ::= '1'\r\n");
			// if (!sentence1.Equals(sentence2)) { 			}
			// int index = sentence1.Zip(sentence2, (c1, c2) => c1 == c2).TakeWhile(b => b).Count() + 1;

			var sppf = earley.ParseGetForest(sentence1);
			if (sppf == null) {
				throw new Exception();
			}

			DotRunner.Run(DotBuilder.GetRawDot(sppf), "arithmetic_ebnf");

			//var traversal = new Traversal(sppf, sentence1, g);
			//var result = traversal.Traverse();
			//if (result.Count() != 1) {
			//	throw new Exception();
			//}
			//var generatedGrammar = new Grammar((IEnumerable<Production>)result.First().Payload, Nonterminal.Of("S"));
			//Console.WriteLine(generatedGrammar);
		}

		private static void DebugGrammar() {
			BaseGrammar g = new Grammar(new List<Production>{
				CFGParser.Production("<S> → ε"),
			}, Nonterminal.Of("S"));
			var sentence = Sentence.FromWords("1 + 1 + 1");
			var grammar = AdditionGrammar(argList => string.Format("({0} + {1})", argList[0].Payload, argList[2].Payload));
			g = grammar;
			var earley = new EarleyParser(g);
			var earley2 = new EarleyParser2(g);
			//DotRunner.Run(earley.ParseGetForest(sentence).GetRawDot(), "testEarleyOld");
			//DotRunner.Run(earley2.ParseGetForest(sentence).GetRawDot(), "testEarleyNew");
			DotRunner.Run(DotBuilder.GetRawDot(earley.ParseGetForest(sentence)), "testEarleyOld");
			DotRunner.Run(DotBuilder.GetRawDot(earley2.ParseGetForest(sentence)), "testEarleyNew");
			// DotRunner.Run(DotBuilder.GetFlattenedDot(earley2.ParseGetForest(sentence)), "testEarleyFlat");
			

			// var prob0 = earley.ParseGetProbability(sentence);
			var prob = earley2.ParseGetProbability(sentence);
		}

		private static void BenchmarkBison() {
			Console.WriteLine("Benching...");
			var inputs = new List<Tuple<string, long, int>>();
			for (var i = 1; i <= 280; i++) {
				inputs.Add(Tuple.Create(AdditionInput(i) + ":", (long)i, i));
			}

			ProcessStartInfo startInfo = new ProcessStartInfo();
			startInfo.UseShellExecute = false;
			startInfo.RedirectStandardOutput = true;
			startInfo.RedirectStandardInput = true;
			startInfo.FileName = @"..\..\..\comparison\vars.exe";
			Process process = new Process();
			process.StartInfo = startInfo;
			

			foreach (var inputPair in inputs) {
				var input = inputPair.Item1;
				var expectedResult = inputPair.Item2;
				var i = inputPair.Item3;

				process.Start();
				var sw = Stopwatch.StartNew();
				process.StandardInput.WriteLine(input);
				process.StandardInput.Close();
				var result = process.StandardOutput.ReadLine();
				sw.Stop();
				var resultLong = Convert.ToInt64(result);
				if (resultLong != expectedResult) {
					throw new Exception();
				}
				// process.Kill();
				Console.WriteLine("{0},{1}", i, sw.Elapsed.TotalMilliseconds);
				process.WaitForExit();
			}
		}

		private static void Benchmark() {
			Console.WriteLine("Benching...");
			var inputs = new List<Tuple<Sentence, long, int>>();
			// for (var i = 80; i < 105; i++) { // 13336ms
			// for (var i = 1; i < 500; i += 1) {
			// for (var i = 170; i < 195; i++) { // 10755ms after gather in sppf
			// for (var i = 170; i < 195; i++) { // 9203ms after hash change
			// for (var i = 170; i < 195; i++) { // 2703ms after doing gather earlier
			// for (var i = 170; i < 195; i++) {
			// for (var i = 751; i < 752; i++) {
			// for (var i = 95; i < 130; i++) { // new; 15385
			for (var i = 120; i < 160; i++) { // new; 10649ms
			// for (var i = 300; i < 350; i++) { // new; 
				inputs.Add(Tuple.Create(Sentence.FromWords(AdditionInput(i)), (long)i, i));
			}
			var gp = AdditionGrammar(argList => (long)argList[0].Payload + (long)argList[2].Payload);
			var ep = new EarleyParser2(gp);

			// var totalSw = Stopwatch.StartNew();
			double totalMs = 0;
			foreach (var inputPair in inputs) {
				var input = inputPair.Item1;
				var expectedResult = inputPair.Item2;
				var i = inputPair.Item3;

				var time = MinTime(3, ep, gp, input);
				totalMs += time;

				Console.WriteLine("{0}, {1}", i, time);
			}
			Console.WriteLine("Done in {0}ms (prev 3686ms)", (int)totalMs);

			foreach (var kvp in EarleyParser2._stats.Data) {
				Console.WriteLine("{0}, {1}", kvp.Key, kvp.Value);
			}
		}

		private static double MinTime(int times, EarleyParser2 ep, BaseGrammar grammar, Sentence input) {
			double fastest = double.MaxValue;

			var sw = new Stopwatch();
			for (var i = 0; i < times; i++) {
				sw.Restart();
				// var sppf = ep.ParseGetRawSppf(input);
				var sppf = ep.ParseGetForest(input);

				//var trav = new Traversal(sppf, input, gp);
				//var resultList = trav.Traverse();
				//if (resultList.Count() != 1) {
				//	throw new Exception();
				//}
				//var result = (long)resultList.First().Payload;
				//if (result != expectedResult) {
				//	throw new Exception();
				//}
				var elapsed = sw.Elapsed.TotalMilliseconds;
				if (elapsed < fastest) {
					fastest = elapsed;
				}
			}
			return fastest;
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

			var ep = new EarleyParser2(ex3);
			var input = Sentence.FromLetters("abbb");
			var sppf = ep.ParseGetForest(input);

			// var sppf3 = ex3.ParseGetForest(Sentence.FromLetters("abbb"));
			var dot = DotBuilder.GetRawDot(sppf);
			//System.IO.File.WriteAllText(@"D:\prog\ContextFreeGrammars\ConsolePlayground\bin\Debug\ex3dot.dot", dot);
			DotRunner.Run(dot, "example3");
		}

		private static BaseGrammar AdditionGrammar<T>(Func<TraverseResult[], T> func) {
			var p1 = new Production(
				Nonterminal.Of("S"), 
				new Sentence {
					Nonterminal.Of("S"),
					Terminal.Of("+"),
					Nonterminal.Of("S")
				},
				1,
				new Annotations(new List<IAnnotation> {
					new PrecedenceAnnotation(5),
					new ActionAnnotation(argList => func(argList)),
					new GatherAnnotation(new GatherOption[]{ GatherOption.SameOrLower, GatherOption.StrictlyLower })
				})
			);
			var nums = new List<Production> {
				// CFGParser.Production("<S> → '0'"),
				new Production(
					Nonterminal.Of("S"),
					new Sentence {
						Terminal.Of("1"),
					},
					1,
					new Annotations(new List<IAnnotation> {
						new PrecedenceAnnotation(0),
						new ActionAnnotation(x => Convert.ToInt64(x[0].Payload)),
						new GatherAnnotation(new GatherOption[]{ })
					})
				),
				//CFGParser.Production("<S> → '2'"),
				//CFGParser.Production("<S> → '3'"),
				//CFGParser.Production("<S> → '4'"),
				//CFGParser.Production("<S> → '5'"),
				//CFGParser.Production("<S> → '6'"),
				//CFGParser.Production("<S> → '7'"),
				//CFGParser.Production("<S> → '8'"),
				//CFGParser.Production("<S> → '9'"),
			};
			var g = new Grammar(new List<Production>{
				p1,
			}.Concat(nums), Nonterminal.Of("S"));
			
			return g;
		}

		private static void VisitorPlay() {
			var gp = AdditionGrammar(argList => string.Format("({0} + {1})", argList[0].Payload, argList[2].Payload));
			
			//var actions2 = new Dictionary<Production, IParserAction> {
			//	[p1] = new ParserAction<long>((argList) => (long)(argList[0].Payload) + (long)(argList[2].Payload)),
			//};
			//var termAction2 = new ParserAction<long>(x => Convert.ToInt64(x[0].Payload));
			//foreach (var num in nums) {
			//	actions2[num] = termAction2;
			//}

			var ep = new EarleyParser2(gp);

			var inputString = AdditionInput(5);
			var input = Sentence.FromWords(inputString);
			var sppf = ep.ParseGetForest(input);

			var rawdot = DotBuilder.GetRawDot(sppf);
			DotRunner.Run(rawdot, "newSppf");

			Console.WriteLine(sppf.ToString());
			Console.WriteLine();

			Console.WriteLine("Starting Traversal...");
			var trav = new Traversal(sppf, gp);
			var resultList = trav.Traverse();
			Console.WriteLine("-----------------");
			foreach (var result in resultList) {
				Console.WriteLine(result.Payload);
			}

			//Console.WriteLine("-----------------");
			//foreach (var result in new Traversal(sppf, input, gp).Traverse()) {
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
