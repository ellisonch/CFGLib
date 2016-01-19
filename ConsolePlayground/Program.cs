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
			CFGParser.Production(@"<X> -> <X0> <X1> 'asdf as_-""fw' <Z23X>");

			// Readme();

			//var ut = new CFGLibTest.UnitTests();
			//ut.TestCYK02();

			Console.Read();
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
			CNFGrammar cnf = cfg.ToCNF();
			Console.WriteLine(cnf.Cyk(Sentence.FromLetters("aabb")));
			Console.WriteLine(cnf.Cyk(Sentence.FromLetters("abba")));

			for (int i = 0; i < 5; i++) {
				Console.WriteLine(cnf.ProduceRandom().AsTerminals());
			}
		}
	}
}
