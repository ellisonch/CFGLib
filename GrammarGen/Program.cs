using ContextFreeGrammars;
using Generation;
using Gutenberg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrammarGen {
	class Program {
		static void Main(string[] args) {
			//var productions = new List<Production> {
			//	new Production(
			//		Variable.Of("S"),
			//		new Sentence { Terminal.Of("a"), Variable.Of("S"), Terminal.Of("a") }
			//	),
			//	new Production(
			//		Variable.Of("S"),
			//		new Sentence { Terminal.Of("b"), Variable.Of("S"), Terminal.Of("b") }
			//	),
			//	new Production(
			//		Variable.Of("S"),
			//		new Sentence { }
			//	)
			//};

			// S → ASA | aB
			// A → B | S
			// B → b | ε
			var productions = new List<Production> {
				new Production(
					Variable.Of("S"),
					new Sentence { Variable.Of("A"), Variable.Of("S"), Variable.Of("A") }
				),
				new Production(
					Variable.Of("S"),
					new Sentence { Terminal.Of("a"), Variable.Of("B") }
				),
				new Production(
					Variable.Of("A"),
					new Sentence { Variable.Of("B") }
				),
				new Production(
					Variable.Of("A"),
					new Sentence { Variable.Of("S") }
				),
				new Production(
					Variable.Of("B"),
					new Sentence { Terminal.Of("b") }
				),
				new Production(
					Variable.Of("B"),
					new Sentence { }
				)
			};

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
			Grammar g = new Grammar(productions);
			Console.WriteLine(g);

			CNFGrammar h = g.ToCNF();
			Console.WriteLine(h);

			var parses = h.Cyk(new Sentence { Terminal.Of("a"), Terminal.Of("b") });
			Console.WriteLine(parses);

			//for (int i = 0; i < 10; i++) {
			//	var history = g.Produce();
			//	Console.WriteLine("-------------");
			//	foreach (var sentence in history) {
			//		Console.WriteLine(sentence);
			//	}
			//}

			//for (int i = 0; i < 5; i++) {
			//	var sentences = g.ProduceToDepth(i);
			//	Console.WriteLine("------Depth {0}------", i);
			//	foreach (var sentence in sentences) {
			//		Console.WriteLine(sentence.AsTerminals());
			//		// Console.WriteLine(sentence);
			//	}
			//}

			// var sentences15 = g.ProduceToDepth(15);
			


			//var mysteriousIsland = new Book(@"D:\prog\GrammarGen\docs\8misl10h");


			var generator = new Generator();
			generator.Generate();

			Console.Read();
		}
	}
}
