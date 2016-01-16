using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib {
	public class CNFGrammar {
		private List<CNFNonterminalProduction> _nonterminalProductions = new List<CNFNonterminalProduction>();
		private List<CNFTerminalProduction> _terminalProductions = new List<CNFTerminalProduction>();
		// private bool _producesEmpty = false;
		// TODO: it's possible for the likelihood to be so low as to appear to be 0, but not actually be 
		private int _producesEmptyWeight = 0;
		private Variable _start;

		private Dictionary<Terminal, ISet<CNFTerminalProduction>> _reverseTerminalProductions;
		private Dictionary<Variable, ISet<CNFNonterminalProduction>> _ntProductionsByVariable;
		private Dictionary<Variable, ISet<CNFTerminalProduction>> _tProductionsByVariable;


		public List<CNFNonterminalProduction> NonterminalProductions {
			get { return _nonterminalProductions; }
		}

		public List<CNFTerminalProduction> TerminalProductions {
			get { return _terminalProductions; }
		}

		public Variable Start {
			get { return _start; }
			set { _start = value; }
		}

		private CNFGrammar() {
		}

		public CNFGrammar(List<CNFNonterminalProduction> nt, List<CNFTerminalProduction> t, int producesEmptyWeight, Variable start) {
			_nonterminalProductions = nt;
			_terminalProductions = t;
			_producesEmptyWeight = producesEmptyWeight;
			_start = start;

			BuildLookups();
		}

		// TODO probably doesn't preserve weights
		//public CNFGrammar(Grammar grammar) {
		//	CFGtoCNF.Convert(grammar)
		//}
		public static CNFGrammar FromCFG(Grammar grammar) {
			var conv = new CFGtoCNF(grammar);
			return conv.Convert();
		}

		//private Dictionary<Terminal, ISet<CNFTerminalProduction>> ReverseTerminalLookups() {

		//}

		/// <summary>
		/// Returns new CNFGrammar containing new immediate data structures, but reusing the same underlying productions
		/// </summary>
		/// <returns></returns>
		public CNFGrammar ShallowClone() {
			var newGrammar = new CNFGrammar();
			newGrammar._nonterminalProductions.AddRange(_nonterminalProductions);
			newGrammar._terminalProductions.AddRange(_terminalProductions);
			newGrammar._start = _start;
			newGrammar._producesEmptyWeight = _producesEmptyWeight;
			// _nonterminalProductions = grammar._nonterminalProductions

			// newGrammar.BuildLookups();

			return newGrammar;
		}

		private void BuildLookups() {
			_reverseTerminalProductions = Helpers.ConstructCache(
				_terminalProductions,
				(p) => p.Rhs,
				(p) => p
			);
			_ntProductionsByVariable = Helpers.ConstructCache(
				_nonterminalProductions,
				(p) => p.Lhs,
				(p) => p
			);
			_tProductionsByVariable = Helpers.ConstructCache(
				_terminalProductions,
				(p) => p.Lhs,
				(p) => p
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
				return GetProbability(_start, _producesEmptyWeight);
			}

			// TODO: don't need to do this every time, just every time there's a change
			// TODO can maybe detect changes by using a dirty flag that bubbles up
			BuildLookups(); 
			// var reverseTerminalProductions = ReverseTerminalLookups();

			List<Variable> nonterminals_R = new List<Variable>(GetNonterminals());
			Dictionary<Variable, int> RToJ = new Dictionary<Variable, int>();
			for (int i = 0; i < nonterminals_R.Count; i++) {
				var R = nonterminals_R[i];
				RToJ[R] = i;
			}

			// var P = new bool[s.Count, s.Count, nonterminals_R.Count];
			var P = new double[s.Count, s.Count, nonterminals_R.Count];
			for (int i = 0; i < s.Count; i++) {
				var a_i = (Terminal)s[i];
				ISet<CNFTerminalProduction> yields_a_i;
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
							var R_B = production.Rhs[0];
							var R_C = production.Rhs[1];
							var A = RToJ[R_A];
							var B = RToJ[R_B];
							var C = RToJ[R_C];
							var probThis = GetProbability(production);

							var pleft = P[k - 1, j - 1, B];
							var pright = P[i - k - 1, j + k - 1, C];
							//if (pleft && pright) {
							//	P[i - 1, j - 1, A] = true;
							//}
							P[i - 1, j - 1, A] += pleft * pright * probThis;
						}
					}
				}
			}

			return P[s.Count - 1, 0, RToJ[_start]];
		}

		public bool Accepts(Sentence s) {
			return Cyk(s) > 0;
		}

		private double GetProbability(CNFProduction target) {
			return GetProbability(target.Lhs, target.Weight);
		}
		private double GetProbability(Variable nonterminal, int weight) {
			int weightTotal = 0;

			// var nts = _ntProductionsByVariable[target.Lhs];
			var nts = _ntProductionsByVariable.LookupEnumerable(nonterminal);
			foreach (var production in nts) {
				weightTotal += production.Weight;
			}

			// var ts = _tProductionsByVariable[target.Lhs];
			var ts = _tProductionsByVariable.LookupEnumerable(nonterminal);
			foreach (var production in ts) {
				weightTotal += production.Weight;
			}

			if (_start == nonterminal) {
				weightTotal += _producesEmptyWeight;
			}

			return (double)weight / weightTotal;
		}

		private HashSet<Variable> GetNonterminals() {
			var results = new HashSet<Variable>();

			foreach (var production in _nonterminalProductions) {
				results.Add(production.Lhs);
				results.Add(production.Rhs[0]);
				results.Add(production.Rhs[1]);
			}
			foreach (var production in _terminalProductions) {
				results.Add(production.Lhs);
			}
			results.Add(_start);

			return results;
		}

		public override string ToString() {
			var retval = "CNFGrammar(" + _start + "){\n";

			foreach (var production in _nonterminalProductions) {
				var prob = GetProbability(production);
				retval += string.Format("  {1:0.00e+000}: {0}\n", production.ToString(), prob);
			}
			foreach (var production in _terminalProductions) {
				var prob = GetProbability(production);
				retval += string.Format("  {1:0.00e+000}: {0}\n", production.ToString(), prob);
			}
			if (_producesEmptyWeight > 0) {
				var prob = GetProbability(_start, _producesEmptyWeight);
				retval += string.Format("  {1:0.00e+000}: {0} → ε\n", _start, prob);
			}
			retval += "\n}\n";
			return retval;
		}
	}
}
