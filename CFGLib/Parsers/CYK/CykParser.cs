using CFGLib.Parsers.Forests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.CYK {
	public class CykParser : Parser {
		private readonly CNFGrammar _grammar;

		public CykParser(CNFGrammar grammar) {
			_grammar = grammar;
		}

		public override Sppf GetParseForest(Sentence s) {
			throw new NotImplementedException();
		}

		// https://en.wikipedia.org/wiki/CYK_algorithm
		//let the input be a string S consisting of n characters: a1 ... an.
		//let the grammar contain r nonterminal symbols R1 ... Rr.
		//This grammar contains the subset Rs which is the set of start symbols.
		//let P[n,n,r] be an array of booleans. Initialize all elements of P to false.
		//for each i = 1 to n
		//  for each unit production Rj -> ai
		//	set P[1,i,j] = true
		//for each i = 2 to n -- Length of span
		//  for each j = 1 to n-i+1 -- Start of span
		//	for each k = 1 to i-1 -- Partition of span
		//	  for each production RA -> RB RC
		//		if P[k,j,B] and P[i-k,j+k,C] then set P[i,j,A] = true
		//if any of P[n,1,x] is true (x is iterated over the set s, where s are all the indices for Rs) then
		//  S is member of language
		//else
		//  S is not member of language
		/// <summary>
		/// Returns the probability that this grammar generated the given sentence
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public override double GetProbability(Sentence s) {
			if (s.Count == 0) {
				return _grammar.ProbabilityNull;
			}

			var nonterminals_R = _grammar.GetNonterminals();
			var RToJ = BuildRToJ(nonterminals_R);

			var P = new double[s.Count, s.Count, nonterminals_R.Count];
			var shouldPunt = CykFillInBase(s, P, RToJ);
			if (shouldPunt) {
				return 0.0;
			}

			var localProductionList = BuildLocalCYKProductionList(RToJ);

			for (int i = 2; i <= s.Count; i++) {
				for (int j = 1; j <= s.Count - i + 1; j++) {
					for (int k = 1; k <= i - 1; k++) {
						foreach (var production in localProductionList) {
							var A = production.A;
							var B = production.B;
							var C = production.C;
							var probThis = production.Probability;

							var pleft = P[k - 1, j - 1, B];
							var pright = P[i - k - 1, j + k - 1, C];
							P[i - 1, j - 1, A] += pleft * pright * probThis;
						}
					}
				}
			}

			return P[s.Count - 1, 0, RToJ[_grammar.Start]];
		}

		private bool CykFillInBase(Sentence s, double[,,] P, Dictionary<Nonterminal, int> RToJ) {
			for (int i = 0; i < s.Count; i++) {
				var a_i = (Terminal)s[i];
				var yields_a_i = _grammar.ProductionsProductingTerminal(a_i);
				if (yields_a_i.Count == 0) {
					// the grammar can't possibly produce this string if it doesn't know a terminal
					return true;
				}
				foreach (var production in yields_a_i) {
					var j = RToJ[production.Lhs];
					P[0, i, j] += _grammar.GetProbability(production);
				}
			}
			return false;
		}

		private Dictionary<Nonterminal, int> BuildRToJ(ISet<Nonterminal> nonterminals_R) {
			Dictionary<Nonterminal, int> RToJ = new Dictionary<Nonterminal, int>();
			// for (int i = 0; i < nonterminals_R.Count; i++) {
			var nonterminalIndex = 0;
			foreach (var R in nonterminals_R) {
				// var R = nonterminals_R[nonterminalIndex];
				RToJ[R] = nonterminalIndex;
				nonterminalIndex++;
			}
			return RToJ;
		}

		/// <summary>
		/// Returns a representation of the nt productions that is efficient for CYK
		/// </summary>
		/// <param name="RToJ"></param>
		/// <returns></returns>
		// TODO: can maybe improve this by using an array of arrays; keep a separate array for each LHS
		private IEnumerable<LocalCykProduction> BuildLocalCYKProductionList(Dictionary<Nonterminal, int> RToJ) {
			var retval = new LocalCykProduction[_grammar.NonterminalProductions.Count];
			for (var i = 0; i < _grammar.NonterminalProductions.Count; i++) {
				var production = _grammar.NonterminalProductions[i];
				var R_A = production.Lhs;
				var R_B = production.SpecificRhs[0];
				var R_C = production.SpecificRhs[1];
				var A = RToJ[R_A];
				var B = RToJ[R_B];
				var C = RToJ[R_C];
				var probThis = _grammar.GetProbability(production);
				retval[i] = new LocalCykProduction(A, B, C, probThis);
			}
			return retval;
		}

		private static void PrintP(Sentence s, double[,,] P) {
			Console.WriteLine("--------P--------");
			for (var i = 0; i < s.Count; i++) {
				var word = s[i];
				Console.Write("{0,-10}", word);
			}
			Console.WriteLine();
			for (var i = 0; i < s.Count; i++) {
				for (var j = 0; j < s.Count; j++) {
					Console.Write("{0,-10}", P[i, j, 0]);
				}
				Console.WriteLine();
			}
			Console.WriteLine();
			Console.WriteLine("--------P--------");
		}
	}
}
