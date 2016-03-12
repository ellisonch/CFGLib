using CFGLib;
using CFGLib.Parsers.Earley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsolePlayground {
	// Some code to help generate the readme
	internal class Readme {
		public static void Do() {
			// S -> aSa | bSb | ε
			var productions = new List<Production> {
				// construct productions by passing arguments...
				new Production(
					lhs: Nonterminal.Of("S"),
					rhs: new Sentence { Terminal.Of("a"), Nonterminal.Of("S"), Terminal.Of("a") },
					weight: 20
				),
				// or from a string...
				CFGParser.Production(@"<S> -> 'b' <S> 'b' [10]"),
				CFGParser.Production(@"<S> -> ε [5]"),
			};
			var cfg = new Grammar(productions, Nonterminal.Of("S"));
			// var cnf = cfg.ToCNF();

			//var probs = cfg.EstimateProbabilities(1000000);
			//foreach (var entry in probs) {
			//	var key = entry.Key;
			//	var value = entry.Value;
			//	if (key.Length <= 4) {
			//	Console.WriteLine("{0}: {1}", key, value);
			//	}
			//}

			// Print out the new CNF grammar
			// Console.WriteLine(cnf);

			var ep = new EarleyParser(cfg);
			// var cp = new CykParser(cnf);

			// Does this grammar accept the string "aabb"?
			Console.WriteLine("aabb: {0}", ep.ParseGetProbability(Sentence.FromLetters("aabb")));
			// How about "abba"?
			Console.WriteLine("abba: {0}", ep.ParseGetProbability(Sentence.FromLetters("abba")));

			Console.WriteLine(ep.ParseGetForest(Sentence.FromLetters("abba")));

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
