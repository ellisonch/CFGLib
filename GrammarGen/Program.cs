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
			Grammar g = new Grammar(productions);
			Console.WriteLine(g);

			for (int i = 0; i < 10; i++) {
				var history = g.Produce();
				Console.WriteLine("-------------");
				foreach (var sentence in history) {
					Console.WriteLine(sentence);
				}
			}

			for (int i = 0; i < 5; i++) {
				var sentences = g.ProduceToDepth(i);
				Console.WriteLine("------Depth {0}------", i);
				foreach (var sentence in sentences) {
					Console.WriteLine(sentence.AsTerminals());
					// Console.WriteLine(sentence);
				}
			}

			// var sentences15 = g.ProduceToDepth(15);
			


			var mysteriousIsland = new Book(@"D:\prog\GrammarGen\docs\8misl10h");


			var generator = new Generator();
			generator.Generate();

			Console.Read();
		}
	}
}
