using CFGLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsolePlayground {
	// A console app for playing around
	class Program {
		static void Main(string[] args) {
			Benchmark();
			//Readme();
			//8.7s

			var p = CFGParser.Production("<S> -> 'a' [5]");
			Console.WriteLine(p);

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
			var productions = new List<BaseProduction> {
				new Production(
					lhs: Nonterminal.Of("S"),
					rhs: new Sentence { Terminal.Of("a"), Nonterminal.Of("S"), Terminal.Of("a") },
					weight: 20
				),
				new Production(
					Nonterminal.Of("S"),
					new Sentence { Terminal.Of("b"), Nonterminal.Of("S"), Terminal.Of("b") },
					10
				),
				new Production(
					Nonterminal.Of("S"),
					new Sentence { },
					1
				)
			};
			Grammar cfg = new Grammar(productions, Nonterminal.Of("S"));
			Console.WriteLine(cfg);
			CNFGrammar cnf = cfg.ToCNF();
			Console.WriteLine(cnf);
			Console.WriteLine(cnf.Cyk(Sentence.FromLetters("aabb")));
			Console.WriteLine(cnf.Cyk(Sentence.FromLetters("abba")));

			for (int i = 0; i < 5; i++) {
				Console.WriteLine(cnf.ProduceRandom().AsTerminals());
			}
		}
	}
}
