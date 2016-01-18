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
			//var productions = new List<BaseProduction> {
			//	new Production(
			//		Nonterminal.Of("S"),
			//		new Sentence { Terminal.Of("a"), Nonterminal.Of("S"), Terminal.Of("a") }
			//	),
			//	new Production(
			//		Nonterminal.Of("S"),
			//		new Sentence { Terminal.Of("b"), Nonterminal.Of("S"), Terminal.Of("b") }
			//	),
			//	new Production(
			//		Nonterminal.Of("S"),
			//		new Sentence { }
			//	)
			//};

			// S → ASA | aB
			// A → B | S
			// B → b | ε
			//var productions = new List<Production> {
			//	new Production(
			//		Nonterminal.Of("S"),
			//		new Sentence { Nonterminal.Of("A"), Nonterminal.Of("S"), Nonterminal.Of("A") }
			//	),
			//	new Production(
			//		Nonterminal.Of("S"),
			//		new Sentence { Terminal.Of("a"), Nonterminal.Of("B") }
			//	),
			//	new Production(
			//		Nonterminal.Of("A"),
			//		new Sentence { Nonterminal.Of("B") }
			//	),
			//	new Production(
			//		Nonterminal.Of("A"),
			//		new Sentence { Nonterminal.Of("S") }
			//	),
			//	new Production(
			//		Nonterminal.Of("B"),
			//		new Sentence { Terminal.Of("b") }
			//	),
			//	new Production(
			//		Nonterminal.Of("B"),
			//		new Sentence { }
			//	)
			//};

			//S → AbB | C
			//B → AA | AC
			//C → b | c
			//A → a | ε
			//var productions = new List<Production> {
			//	new Production(
			//		Nonterminal.Of("S"),
			//		new Sentence { Nonterminal.Of("A"), Terminal.Of("b"), Nonterminal.Of("B") }
			//	),
			//	new Production(
			//		Nonterminal.Of("S"),
			//		new Sentence { Nonterminal.Of("C") }
			//	),
			//	new Production(
			//		Nonterminal.Of("B"),
			//		new Sentence { Nonterminal.Of("A"), Nonterminal.Of("A") }
			//	),
			//	new Production(
			//		Nonterminal.Of("B"),
			//		new Sentence { Nonterminal.Of("A"), Nonterminal.Of("C") }
			//	),
			//	new Production(
			//		Nonterminal.Of("C"),
			//		new Sentence { Terminal.Of("b") }
			//	),
			//	new Production(
			//		Nonterminal.Of("C"),
			//		new Sentence { Terminal.Of("c") }
			//	),
			//	new Production(
			//		Nonterminal.Of("A"),
			//		new Sentence { Terminal.Of("a") }
			//	),
			//	new Production(
			//		Nonterminal.Of("A"),
			//		new Sentence { }
			//	)
			//};

			//var productions = new List<Production> {
			//	new Production(
			//		Nonterminal.Of("S"),
			//		new Sentence { Nonterminal.Of("NP"), Nonterminal.Of("VP") }
			//	),
			//	new Production(
			//		Nonterminal.Of("NP"),
			//		new Sentence { Nonterminal.Of("N") }
			//	),
			//	new Production(
			//		Nonterminal.Of("NP"),
			//		new Sentence { Nonterminal.Of("Det"), Nonterminal.Of("N") }
			//	),
			//	new Production(
			//		Nonterminal.Of("VP"),
			//		new Sentence { Nonterminal.Of("V") }
			//	),
			//	new Production(
			//		Nonterminal.Of("VP"),
			//		new Sentence { Nonterminal.Of("V"), Nonterminal.Of("PP") }
			//	),
			//	new Production(
			//		Nonterminal.Of("PP"),
			//		new Sentence { Nonterminal.Of("Prep"), Nonterminal.Of("NP") }
			//	),
			//	new Production(
			//		Nonterminal.Of("N"),
			//		new Sentence { Terminal.Of("boy") }
			//	),
			//	new Production(
			//		Nonterminal.Of("N"),
			//		new Sentence { Terminal.Of("cat") }
			//	),
			//	new Production(
			//		Nonterminal.Of("N"),
			//		new Sentence { Terminal.Of("trampoline") }
			//	),
			//	new Production(
			//		Nonterminal.Of("Prep"),
			//		new Sentence { Terminal.Of("of") }
			//	),
			//	new Production(
			//		Nonterminal.Of("Prep"),
			//		new Sentence { Terminal.Of("through") }
			//	),
			//	new Production(
			//		Nonterminal.Of("V"),
			//		new Sentence { Terminal.Of("kicked") }
			//	),
			//	new Production(
			//		Nonterminal.Of("V"),
			//		new Sentence { Terminal.Of("works") }
			//	),
			//	new Production(
			//		Nonterminal.Of("Det"),
			//		new Sentence { Terminal.Of("a") }
			//	),
			//	new Production(
			//		Nonterminal.Of("Det"),
			//		new Sentence { Terminal.Of("the") }
			//	),
			//};

			//Grammar g = new Grammar(productions, Nonterminal.Of("S"));
			//Console.WriteLine(g);

			//CNFGrammar h = g.ToCNF();
			//Console.WriteLine(h);

			//var randg = new CNFRandom();
			//for (int numProductions = 0; numProductions < 100; numProductions += 5) {
			//	for (int numNonterminals = 0; numNonterminals < 200; numNonterminals += 5) {
			//		for (int numTerminals = 1; numTerminals < 200; numTerminals += 5) {
			//			var range = Enumerable.Range(0, numTerminals);
			//			var terminals = new List<Terminal>(range.Select((x) => Terminal.Of("x" + x)));
			//			var rg = randg.Next(numNonterminals, numProductions, terminals);
			//			Console.WriteLine(rg);
			//		}
			//	}
			//}
			//for (int i = 0; i < 5; i++) {
			//	var rg = randg.Next(5, 2, new List<Terminal> { Terminal.Of("a"), Terminal.Of("b") });
			//	Console.WriteLine(rg);
			//}


			//var parses = h.Cyk(new Sentence { Terminal.Of("a"), Terminal.Of("b") });
			//Console.WriteLine(parses);

			//for (int i = 0; i < 10; i++) {
			//	var history = g.Produce();
			//	Console.WriteLine("-------------");
			//	foreach (var sentence in history) {
			//		Console.WriteLine(sentence);
			//	}
			//}

			//for (int i = 0; i < 10; i++) {
			//	var sentences = g.ProduceToDepth(i);
			//	Console.WriteLine("------Depth {0}------", i);
			//	foreach (var sentence in sentences) {
			//		Console.WriteLine(sentence.AsTerminals());
			//		// Console.WriteLine(sentence);
			//	}
			//}

			//var sentences = g.ProduceToDepth(6);
			//sentences = sentences.FindAll((x) => x.Sentence.Count > 0);

			// var swps = sentences.Select((s) => new SentenceWithProbability(1.0 / sentences.Count, s));

			//var sum = 0.0;
			//foreach (var swp in sentences) {
			//	// Console.WriteLine(swp);
			//	sum += swp.Probability;
			//}
			//Console.WriteLine("Test set has probability {0}", sum);








				var randg = new CNFRandom();

				int _maxNonterminals = 8;
				int _maxProductions = 10;
				int _maxTerminals = 20;
				int _step = 1;

				for (int numProductions = 0; numProductions < _maxProductions; numProductions += _step) {
					for (int numNonterminals = 1; numNonterminals < _maxNonterminals; numNonterminals += _step) {
						for (int numTerminals = 1; numTerminals < _maxTerminals; numTerminals += _step) {
							var range = Enumerable.Range(0, numTerminals);
							var terminals = new List<Terminal>(range.Select((x) => Terminal.Of("x" + x)));
							Console.WriteLine("{0}, {1}, {2}", numNonterminals, numProductions, numTerminals);
							var rg = randg.Next(numNonterminals, numProductions, terminals);
							TestGrammar(rg);
						}
					}
				}


				//var ut = new CFGLibTest.UnitTests();
				//ut.TestCYK02();


				//var range = Enumerable.Range(0, 1);
				//var rg = randg.Next(1, 12, new List<Terminal>(range.Select((x) => Terminal.Of("x" + x))));
				//Console.WriteLine(rg);
				//Console.WriteLine("Producing...");
				//var sw = Stopwatch.StartNew();
				//var swps = rg.ProduceToDepth(3);
				//sw.Stop();
				//Console.WriteLine("Done in {0}s", sw.Elapsed.TotalMilliseconds/1000);

				Console.Read();
		}

		private static void TestGrammar(CNFGrammar rg) {
			Console.WriteLine("=====================");
			for (int i = 0; i < 5; i++) {
				var swps = rg.ProduceToDepth(i);
				Console.WriteLine("------Depth {0}------", i);
				foreach (var swp in swps) {
					var actual = rg.Cyk(swp.Sentence);
					var expected = swp.Probability;
					if (actual < expected) {
						Console.WriteLine("{0}, {1}", actual, expected);
						Console.WriteLine(rg);
						Console.WriteLine(swp);

					}
					// Console.WriteLine(swp.Sentence.AsTerminals());
					// Console.WriteLine(sentence);
				}
			}
		}
	}
}
