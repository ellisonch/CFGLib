using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib {
	public class CNFGrammar : BaseGrammar {
		private List<CNFNonterminalProduction> _nonterminalProductions = new List<CNFNonterminalProduction>();
		private List<CNFTerminalProduction> _terminalProductions = new List<CNFTerminalProduction>();
		
		private List<Production> _emptyProductions = new List<Production>();

		private int EmptyProductionWeight {
			get {
				if (_emptyProductions.Count == 0) {
					return 0;
				} else {
					return _emptyProductions.First().Weight;
				}
			}
		}

		private Dictionary<Terminal, ICollection<CNFTerminalProduction>> _reverseTerminalProductions;
		private Dictionary<Nonterminal, ICollection<CNFNonterminalProduction>> _ntProductionsByNonterminal;
		private Dictionary<Nonterminal, ICollection<CNFTerminalProduction>> _tProductionsByNonterminal;


		protected List<CNFNonterminalProduction> NonterminalProductions {
			get { return _nonterminalProductions; }
		}

		protected List<CNFTerminalProduction> TerminalProductions {
			get { return _terminalProductions; }
		}


		internal override IEnumerable<BaseProduction> ProductionsFrom(Nonterminal lhs) {
			IEnumerable<BaseProduction> list1 = _ntProductionsByNonterminal.LookupEnumerable(lhs);
			IEnumerable<BaseProduction> list2 = _tProductionsByNonterminal.LookupEnumerable(lhs);

			var result = list1.Concat(list2);
			if (lhs == this.Start) {
				result = result.Concat(_emptyProductions);
			}
			return result;
		}

		public override IEnumerable<BaseProduction> Productions {
			get {
				IEnumerable<BaseProduction> list1 = _nonterminalProductions;
				IEnumerable<BaseProduction> list2 = _terminalProductions;
				return list1.Concat(list2).Concat(_emptyProductions);
			}
		}

		internal override void RemoveProductions(IEnumerable<BaseProduction> toRemove) {
			foreach (var production in toRemove) {
				if (production.Lhs == this.Start && production.Rhs.Count == 0) {
					throw new Exception("Don't handle removing empty production in CNF yet");
				} else if (production is CNFNonterminalProduction) {
					var ntprod = (CNFNonterminalProduction)production;
					_nonterminalProductions.Remove(ntprod);
				} else {
					var tprod = (CNFTerminalProduction)production;
					_terminalProductions.Remove(tprod);
				}
			}
		}

		private CNFGrammar() {
		}

		public CNFGrammar(IEnumerable<BaseProduction> nt, IEnumerable<BaseProduction> t, int producesEmptyWeight, Nonterminal start, bool simplify = true) {
			_nonterminalProductions = new List<CNFNonterminalProduction>();
			foreach (var production in nt) {
				_nonterminalProductions.Add(new CNFNonterminalProduction(production));
			}
			_terminalProductions = new List<CNFTerminalProduction>();
			foreach (var production in t) {
				_terminalProductions.Add(new CNFTerminalProduction(production));
			}
			if (producesEmptyWeight > 0) {
				_emptyProductions.Add(new Production(start, new Sentence(), producesEmptyWeight));
			}
			this.Start = start;
			
			if (simplify) {
				Simplify();
			}

			BuildLookups();
			BuildHelpers();
		}

		private void BuildLookups() {
			_reverseTerminalProductions = Helpers.ConstructCache(
				_terminalProductions,
				(p) => p.SpecificRhs,
				(p) => p,
				() => (ICollection<CNFTerminalProduction>)new HashSet<CNFTerminalProduction>(),
				(x, y) => x.Add(y)
			);
			_ntProductionsByNonterminal = Helpers.ConstructCache(
				_nonterminalProductions,
				(p) => p.Lhs,
				(p) => p,
				() => (ICollection<CNFNonterminalProduction>)new HashSet<CNFNonterminalProduction>(),
				(x, y) => x.Add(y)
			);
			_tProductionsByNonterminal = Helpers.ConstructCache(
				_terminalProductions,
				(p) => p.Lhs,
				(p) => p,
				() => (ICollection<CNFTerminalProduction>)new HashSet<CNFTerminalProduction>(),
				(x, y) => x.Add(y)
			);
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
		public double Cyk(Sentence s) {
			if (s.Count == 0) {
				return GetProbability(this.Start, EmptyProductionWeight);
			}

			// TODO: don't need to do this every time, just every time there's a change
			// TODO can maybe detect changes by using a dirty flag that bubbles up
			BuildLookups();
			// var reverseTerminalProductions = ReverseTerminalLookups();

			List<Nonterminal> nonterminals_R = new List<Nonterminal>(GetNonterminals());
			Dictionary<Nonterminal, int> RToJ = new Dictionary<Nonterminal, int>();
			for (int i = 0; i < nonterminals_R.Count; i++) {
				var R = nonterminals_R[i];
				RToJ[R] = i;
			}

			// var P = new bool[s.Count, s.Count, nonterminals_R.Count];
			var P = new double[s.Count, s.Count, nonterminals_R.Count];
			for (int i = 0; i < s.Count; i++) {
				var a_i = (Terminal)s[i];
				ICollection<CNFTerminalProduction> yields_a_i;
				if (!_reverseTerminalProductions.TryGetValue(a_i, out yields_a_i)) {
					// the grammar can't possibly produce this string if it doesn't know a terminal
					return 0.0;
				}
				foreach (var production in yields_a_i) {
					var j = RToJ[production.Lhs];
					P[0, i, j] += GetProbability(production);
				}
			}

			for (int i = 2; i <= s.Count; i++) {
				for (int j = 1; j <= s.Count - i + 1; j++) {
					for (int k = 1; k <= i - 1; k++) {
						// Console.WriteLine("i, j, k = {0:00}, {1:00}, {2:00}", i, j, k);
						foreach (var production in _nonterminalProductions) {
							var R_A = production.Lhs;
							var R_B = production.SpecificRhs[0];
							var R_C = production.SpecificRhs[1];
							var A = RToJ[R_A];
							var B = RToJ[R_B];
							var C = RToJ[R_C];
							var probThis = GetProbability(production);

							var pleft = P[k - 1, j - 1, B];
							var pright = P[i - k - 1, j + k - 1, C];
							P[i - 1, j - 1, A] += pleft * pright * probThis;
						}
					}
				}
			}
			// PrintP(s, P);

			return P[s.Count - 1, 0, RToJ[this.Start]];
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

		public bool Accepts(Sentence s) {
			return Cyk(s) > 0;
		}

		public CNFGrammar Clone() {
			return new CNFGrammar(this.NonterminalProductions, this.TerminalProductions, EmptyProductionWeight, this.Start, false);
		}
	}
}
