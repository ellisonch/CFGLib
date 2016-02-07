using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib {
	/// <summary>
	/// Represents a concrete (probabilistic) context free grammar in Chomsky normal form (CNF)
	/// </summary>
	public class CNFGrammar : BaseGrammar {
		private List<CNFNonterminalProduction> _nonterminalProductions = new List<CNFNonterminalProduction>();
		private List<CNFTerminalProduction> _terminalProductions = new List<CNFTerminalProduction>();
		
		private List<Production> _emptyProductions = new List<Production>();

		private double EmptyProductionWeight {
			get {
				if (_emptyProductions.Count == 0) {
					return 0.0;
				} else {
					return _emptyProductions.First().Weight;
				}
			}
		}

		private Cache<Dictionary<Terminal, ICollection<CNFTerminalProduction>>> _reverseTerminalProductions;
		private Cache<Dictionary<Nonterminal, ICollection<CNFNonterminalProduction>>> _ntProductionsByNonterminal;
		private Cache<Dictionary<Nonterminal, ICollection<CNFTerminalProduction>>> _tProductionsByNonterminal;


		private List<CNFNonterminalProduction> NonterminalProductions {
			get { return _nonterminalProductions; }
		}

		private List<CNFTerminalProduction> TerminalProductions {
			get { return _terminalProductions; }
		}


		internal override IEnumerable<Production> ProductionsFrom(Nonterminal lhs) {
			IEnumerable<Production> list1 = _ntProductionsByNonterminal.Value.LookupEnumerable(lhs);
			IEnumerable<Production> list2 = _tProductionsByNonterminal.Value.LookupEnumerable(lhs);

			var result = list1.Concat(list2);
			if (lhs == this.Start) {
				result = result.Concat(_emptyProductions);
			}
			return result;
		}

		public override IEnumerable<Production> Productions {
			get {
				IEnumerable<Production> list1 = _nonterminalProductions;
				IEnumerable<Production> list2 = _terminalProductions;
				return list1.Concat(list2).Concat(_emptyProductions);
			}
		}

		// TODO: should make sure the empty production is the actual empty production
		// TODO: should error if the production doesn't exist
		public override void RemoveProduction(Production production) {
			if (production.Lhs == this.Start && production.Rhs.Count == 0) {
				if (_emptyProductions.Count > 0) {
					_emptyProductions.Clear();
				} else {
					throw new Exception("No production to remove");
				}
			} else if (production is CNFNonterminalProduction) {
				var ntprod = (CNFNonterminalProduction)production;
				_nonterminalProductions.Remove(ntprod);
			} else {
				// TODO: might not actually be a terminal production
				var tprod = (CNFTerminalProduction)production;
				_terminalProductions.Remove(tprod);
			}
			InvalidateCaches();
		}
		public override void AddProduction(Production production) {
			if (production.Lhs == this.Start && production.Rhs.Count == 0) {
				if (_emptyProductions.Count > 0) {
					_emptyProductions.First().Weight += production.Weight;
				} else {
					_emptyProductions.Add(production);
				}
			} else if (production is CNFNonterminalProduction) {
				var ntprod = (CNFNonterminalProduction)production;
				AddToListWithoutDuplicating(_nonterminalProductions, ntprod);
			} else if (production is CNFTerminalProduction) {
				var tprod = (CNFTerminalProduction)production;
				AddToListWithoutDuplicating(_terminalProductions, tprod);
			} else {
				// TODO: should look into the production and see if we can convert
				throw new Exception("You can't add that kind of production to this grammar");
			}
			InvalidateCaches();
		}

		private CNFGrammar() {
		}

		public CNFGrammar(IEnumerable<Production> productions, Nonterminal start) {
			this.Start = start;

			foreach (var production in productions) {
				if (production.Lhs == start && production.Rhs.Count == 0) {
					if (production.Weight == 0.0) {
						continue;
					}
					if (_emptyProductions.Count == 0) {
						_emptyProductions.Add(production);
					} else {
						_emptyProductions.First().Weight += production.Weight;
					}
				} else if (production.Rhs.OnlyNonterminals()) {
					_nonterminalProductions.Add(new CNFNonterminalProduction(production));
				} else {
					_terminalProductions.Add(new CNFTerminalProduction(production));
				}
			}

			SimplifyWithoutInvalidate();
			BuildLookups();
			BuildHelpers();
		}

		private void BuildLookups() {
			_reverseTerminalProductions = Cache.Create(() => Helpers.BuildLookup(
				() => _terminalProductions,
				(p) => p.SpecificRhs,
				(p) => p,
				() => (ICollection<CNFTerminalProduction>)new HashSet<CNFTerminalProduction>(),
				(x, y) => x.Add(y)
			));
			this.Caches.Add(_reverseTerminalProductions);

			_ntProductionsByNonterminal = Cache.Create(() => Helpers.BuildLookup(
				() => _nonterminalProductions,
				(p) => p.Lhs,
				(p) => p,
				() => (ICollection<CNFNonterminalProduction>)new HashSet<CNFNonterminalProduction>(),
				(x, y) => x.Add(y)
			));
			this.Caches.Add(_ntProductionsByNonterminal);

			_tProductionsByNonterminal = Cache.Create(() => Helpers.BuildLookup(
				() => _terminalProductions,
				(p) => p.Lhs,
				(p) => p,
				() => (ICollection<CNFTerminalProduction>)new HashSet<CNFTerminalProduction>(),
				(x, y) => x.Add(y)
			));
			this.Caches.Add(_tProductionsByNonterminal);
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
		public double Cyk(Sentence s) {
			if (s.Count == 0) {
				if (_emptyProductions.Count == 0) {
					return 0.0;
				}
				return GetProbability(_emptyProductions.First());
			}

			var nonterminals_R = GetNonterminals();
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
			
			return P[s.Count - 1, 0, RToJ[this.Start]];
		}

		private bool CykFillInBase(Sentence s, double[,,] P, Dictionary<Nonterminal, int> RToJ) {
			for (int i = 0; i < s.Count; i++) {
				var a_i = (Terminal)s[i];
				ICollection<CNFTerminalProduction> yields_a_i;
				if (!_reverseTerminalProductions.Value.TryGetValue(a_i, out yields_a_i)) {
					// the grammar can't possibly produce this string if it doesn't know a terminal
					return true;
				}
				foreach (var production in yields_a_i) {
					var j = RToJ[production.Lhs];
					P[0, i, j] += GetProbability(production);
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

		private struct LocalCYKProduction {
			public int A;
			public int B;
			public int C;
			public double Probability;
			public LocalCYKProduction(int A, int B, int C, double Probability) {
				this.A = A;
				this.B = B;
				this.C = C;
				this.Probability = Probability;
			}
		}
		/// <summary>
		/// Returns a representation of the nt productions that is efficient for CYK
		/// </summary>
		/// <param name="RToJ"></param>
		/// <returns></returns>
		// TODO: can maybe improve this by using an array of arrays; keep a separate array for each LHS
		private IEnumerable<LocalCYKProduction> BuildLocalCYKProductionList(Dictionary<Nonterminal, int> RToJ) {
			var retval = new LocalCYKProduction[_nonterminalProductions.Count];
			for (var i = 0; i < _nonterminalProductions.Count; i++) {
				var production = _nonterminalProductions[i];
				var R_A = production.Lhs;
				var R_B = production.SpecificRhs[0];
				var R_C = production.SpecificRhs[1];
				var A = RToJ[R_A];
				var B = RToJ[R_B];
				var C = RToJ[R_C];
				var probThis = GetProbability(production);
				retval[i] = new LocalCYKProduction(A, B, C, probThis);
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

		/// <summary>
		/// Returns whether this grammar accepts the given sentence
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public bool Accepts(Sentence s) {
			return Cyk(s) > 0;
		}

		public override BaseGrammar ShallowClone() {
			var clone = new CNFGrammar(this.Productions, this.Start);
			return clone;
		}
	}
}
