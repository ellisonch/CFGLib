using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib {
	/// <summary>
	/// This class can be used to generate random grammars
	/// </summary>
	public class GrammarGenerator {
		private Random _rand;

		public GrammarGenerator(int seed = 0) {
			_rand = new Random(seed);
		}

		/// <summary>
		/// Generates a new, random generic grammar
		/// </summary>
		/// <returns></returns>
		public Grammar NextCFG(int numNonterminals, int numProductions, int maxProductionLength, IList<Terminal> terminals, bool useNull = true) {
			if (numNonterminals < 1) {
				throw new ArgumentOutOfRangeException("Need at least one nonterminal");
			}
			var start = RandomNonterminal(1);

			var productions = new List<Production>();

			for (int i = 0; i < numProductions; i++) {
				productions.Add(RandomProduction(maxProductionLength, numNonterminals, terminals, useNull));
			}

			return new Grammar(productions, start);
		}

		public Production RandomProduction(int maxProductionLength, int numNonterminals, IList<Terminal> terminals, bool useNull = true) {
			var lhs = RandomNonterminal(numNonterminals);
			var weight = 100 * _rand.NextDouble() + 1.0;
			var productionLength = useNull ? _rand.Next(maxProductionLength + 1) + 0: _rand.Next(maxProductionLength + 0) + 1;
			Sentence rhs = new Sentence();
			for (int i = 0; i < productionLength; i++) {
				if (_rand.Next(2) == 0) {
					rhs.Add(RandomNonterminal(numNonterminals));
				} else {
					rhs.Add(RandomTerminal(terminals));
				}
			}

			return new DefaultProduction(lhs, rhs, weight);
		}

		/// <summary>
		/// Generates a new, random CNF grammar
		/// </summary>
		public CNFGrammar NextCNF(int numNonterminals, int numProductions, IList<Terminal> terminals) {
			if (numNonterminals < 1) {
				throw new ArgumentOutOfRangeException("Need at least one nonterminal");
			}
			var start = RandomNonterminal(1);
			double producesEmptyWeight = 0.0;
			if (numProductions > 0) {
				if (_rand.Next(2) == 1) {
					producesEmptyWeight = 100 * _rand.NextDouble();
					numProductions--;
				}
			}
			var numNontermProductions = _rand.Next(numProductions);
			var numTermProductions = numProductions - numNontermProductions;
			var nt = new List<Production>();
			var t = new List<Production>();

			for (int i = 0; i < numNontermProductions; i++) {
				nt.Add(NextCNFNonterminalProduction(numNonterminals));
			}
			for (int i = 0; i < numTermProductions; i++) {
				var terminal = RandomTerminal(terminals);
				t.Add(NextCNFTerminalProduction(numNonterminals, terminals, terminal));
			}
			return new CNFGrammar(nt, t, producesEmptyWeight, start);
		}

		public Production NextCNFNonterminalProduction(int numNonTerminals, Nonterminal lhs = null) {
			if (lhs == null) {
				lhs = RandomNonterminal(numNonTerminals);
			}
			var rhs1 = RandomNonterminal(numNonTerminals, false);
			var rhs2 = RandomNonterminal(numNonTerminals, false);

			return new CNFNonterminalProduction(lhs, rhs1, rhs2);
		}

		public Production NextCNFTerminalProduction(int numNonterminals, IList<Terminal> terminals, Terminal rhs = null) {
			if (rhs == null) {
				rhs = RandomTerminal(terminals);
			}
			var lhs = RandomNonterminal(numNonterminals);

			return new CNFTerminalProduction(lhs, rhs);
		}

		private Nonterminal RandomNonterminal(int numNonterminals, bool allowStart = true) {
			int num;
			
			if (allowStart) {
				num = _rand.Next(0, numNonterminals);
			} else {
				num = _rand.Next(1, numNonterminals);
			}
			return Nonterminal.Of("X_" + num);
		}
		private Terminal RandomTerminal(IList<Terminal> terminals) {
			return terminals[_rand.Next(terminals.Count)];
		}
	}
}
