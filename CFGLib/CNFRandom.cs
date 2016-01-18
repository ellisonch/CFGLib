using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib {
	public class CNFRandom {
		private Random _rand = new Random(0);

		public CNFGrammar Next(int numNonTerminals, int numProductions, List<Terminal> terminals) {
			if (numNonTerminals < 1) {
				throw new ArgumentOutOfRangeException("Need at least one nonterminal");
			}
			var start = RandomNonterminal(1);
			int producesEmptyWeight = 0;
			if (numProductions > 0) {
				if (_rand.Next(2) == 1) {
					producesEmptyWeight = _rand.Next(100);
					numProductions--;
				}
			}
			var numNontermProductions = _rand.Next(numProductions);
			var numTermProductions = numProductions - numNontermProductions;
			var nt = new List<CNFNonterminalProduction>();
			var t = new List<CNFTerminalProduction>();
			//if (_rand.Next(2) == 0) {
			//	nt.Add(RandomNTProduction(numNonTerminals, start));
			//} else {
			//	nt.Add(RandomNTProduction(numNonTerminals, start));
			//}
			for (int i = 0; i < numNontermProductions; i++) {
				nt.Add(RandomNTProduction(numNonTerminals));
			}
			for (int i = 0; i < numTermProductions; i++) {
				var terminal = RandomTerminal(terminals);
				t.Add(RandomTProduction(numNonTerminals, terminals, terminal));
			}
			return new CNFGrammar(nt, t, producesEmptyWeight, start);
		}

		private CNFNonterminalProduction RandomNTProduction(int numNonTerminals, Nonterminal lhs = null) {
			if (lhs == null) {
				lhs = RandomNonterminal(numNonTerminals);
			}
			var rhs1 = RandomNonterminal(numNonTerminals, false);
			var rhs2 = RandomNonterminal(numNonTerminals, false);

			return new CNFNonterminalProduction(lhs, rhs1, rhs2);
		}

		private CNFTerminalProduction RandomTProduction(int numNonTerminals, List<Terminal> terminals, Terminal rhs = null) {
			if (rhs == null) {
				rhs = RandomTerminal(terminals);
			}
			var lhs = RandomNonterminal(numNonTerminals);

			return new CNFTerminalProduction(lhs, rhs);
		}

		private Nonterminal RandomNonterminal(int numNonTerminals, bool allowStart = true) {
			int num;
			
			if (allowStart) {
				num = _rand.Next(0, numNonTerminals);
			} else {
				num = _rand.Next(1, numNonTerminals);
			}
			return Nonterminal.Of("X_" + num);
		}
		private Terminal RandomTerminal(List<Terminal> terminals) {
			return terminals[_rand.Next(terminals.Count)];
		}
	}
}
