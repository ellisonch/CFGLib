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
			var productions = new List<BaseProduction> {
				new Production(
					Nonterminal.Of("S"),
					new Sentence { Terminal.Of("a"), Nonterminal.Of("S"), Terminal.Of("a") }
				),
				new Production(
					Nonterminal.Of("S"),
					new Sentence { Terminal.Of("b"), Nonterminal.Of("S"), Terminal.Of("b") }
				),
				new Production(
					Nonterminal.Of("S"),
					new Sentence { }
				)
			};

			Grammar g = new Grammar(productions, Nonterminal.Of("S"));
			Console.WriteLine(g);

			CNFGrammar h = g.ToCNF();
			Console.WriteLine(h);
			

			//var ut = new CFGLibTest.UnitTests();
			//ut.TestCYK02();

			Console.Read();
		}
	}
}
