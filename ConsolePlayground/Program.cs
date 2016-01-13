using CFGLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsolePlayground {
	// A console app for playing around
	class Program {
		static void Main(string[] args) {
			var productions = new List<Production> {
				new Production(
					Variable.Of("S"),
					new Sentence { Terminal.Of("a"), Variable.Of("S"), Terminal.Of("a") }
				),
				new Production(
					Variable.Of("S"),
					new Sentence { Terminal.Of("b"), Variable.Of("S"), Terminal.Of("b") }
				),
				new Production(
					Variable.Of("S"),
					new Sentence { }
				)
			};

			// S → ASA | aB
			// A → B | S
			// B → b | ε
			//var productions = new List<Production> {
			//	new Production(
			//		Variable.Of("S"),
			//		new Sentence { Variable.Of("A"), Variable.Of("S"), Variable.Of("A") }
			//	),
			//	new Production(
			//		Variable.Of("S"),
			//		new Sentence { Terminal.Of("a"), Variable.Of("B") }
			//	),
			//	new Production(
			//		Variable.Of("A"),
			//		new Sentence { Variable.Of("B") }
			//	),
			//	new Production(
			//		Variable.Of("A"),
			//		new Sentence { Variable.Of("S") }
			//	),
			//	new Production(
			//		Variable.Of("B"),
			//		new Sentence { Terminal.Of("b") }
			//	),
			//	new Production(
			//		Variable.Of("B"),
			//		new Sentence { }
			//	)
			//};

			//S → AbB | C
			//B → AA | AC
			//C → b | c
			//A → a | ε
			//var productions = new List<Production> {
			//	new Production(
			//		Variable.Of("S"),
			//		new Sentence { Variable.Of("A"), Terminal.Of("b"), Variable.Of("B") }
			//	),
			//	new Production(
			//		Variable.Of("S"),
			//		new Sentence { Variable.Of("C") }
			//	),
			//	new Production(
			//		Variable.Of("B"),
			//		new Sentence { Variable.Of("A"), Variable.Of("A") }
			//	),
			//	new Production(
			//		Variable.Of("B"),
			//		new Sentence { Variable.Of("A"), Variable.Of("C") }
			//	),
			//	new Production(
			//		Variable.Of("C"),
			//		new Sentence { Terminal.Of("b") }
			//	),
			//	new Production(
			//		Variable.Of("C"),
			//		new Sentence { Terminal.Of("c") }
			//	),
			//	new Production(
			//		Variable.Of("A"),
			//		new Sentence { Terminal.Of("a") }
			//	),
			//	new Production(
			//		Variable.Of("A"),
			//		new Sentence { }
			//	)
			//};

			//var productions = new List<Production> {
			//	new Production(
			//		Variable.Of("S"),
			//		new Sentence { Variable.Of("NP"), Variable.Of("VP") }
			//	),
			//	new Production(
			//		Variable.Of("NP"),
			//		new Sentence { Variable.Of("N") }
			//	),
			//	new Production(
			//		Variable.Of("NP"),
			//		new Sentence { Variable.Of("Det"), Variable.Of("N") }
			//	),
			//	new Production(
			//		Variable.Of("VP"),
			//		new Sentence { Variable.Of("V") }
			//	),
			//	new Production(
			//		Variable.Of("VP"),
			//		new Sentence { Variable.Of("V"), Variable.Of("PP") }
			//	),
			//	new Production(
			//		Variable.Of("PP"),
			//		new Sentence { Variable.Of("Prep"), Variable.Of("NP") }
			//	),
			//	new Production(
			//		Variable.Of("N"),
			//		new Sentence { Terminal.Of("boy") }
			//	),
			//	new Production(
			//		Variable.Of("N"),
			//		new Sentence { Terminal.Of("cat") }
			//	),
			//	new Production(
			//		Variable.Of("N"),
			//		new Sentence { Terminal.Of("trampoline") }
			//	),
			//	new Production(
			//		Variable.Of("Prep"),
			//		new Sentence { Terminal.Of("of") }
			//	),
			//	new Production(
			//		Variable.Of("Prep"),
			//		new Sentence { Terminal.Of("through") }
			//	),
			//	new Production(
			//		Variable.Of("V"),
			//		new Sentence { Terminal.Of("kicked") }
			//	),
			//	new Production(
			//		Variable.Of("V"),
			//		new Sentence { Terminal.Of("works") }
			//	),
			//	new Production(
			//		Variable.Of("Det"),
			//		new Sentence { Terminal.Of("a") }
			//	),
			//	new Production(
			//		Variable.Of("Det"),
			//		new Sentence { Terminal.Of("the") }
			//	),
			//};

			Grammar g = new Grammar(productions);
			Console.WriteLine(g);

			//CNFGrammar h = g.ToCNF();
			//Console.WriteLine(h);

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

			var sentences = g.ProduceToDepth(6);
			sentences = sentences.FindAll((x) => x.Sentence.Count > 0);

			// var swps = sentences.Select((s) => new SentenceWithProbability(1.0 / sentences.Count, s));

			var sum = 0.0;
			foreach (var swp in sentences) {
				// Console.WriteLine(swp);
				sum += swp.Probability;
			}
			Console.WriteLine("Test set has probability {0}", sum);
			Console.Read();
		}
	}
}
