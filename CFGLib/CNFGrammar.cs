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

		internal double ProbabilityNull {
			get {
				if (_emptyProductions.Count == 0) {
					return 0.0;
				}
				return GetProbability(_emptyProductions.First());
			}
		}

		private Cache<Dictionary<Terminal, ICollection<CNFTerminalProduction>>> _reverseTerminalProductions;
		private Cache<Dictionary<Nonterminal, ICollection<CNFNonterminalProduction>>> _ntProductionsByNonterminal;
		private Cache<Dictionary<Nonterminal, ICollection<CNFTerminalProduction>>> _tProductionsByNonterminal;

		internal ICollection<CNFTerminalProduction> ProductionsProductingTerminal(Terminal terminal) {
			ICollection<CNFTerminalProduction> result;
			if (!_reverseTerminalProductions.Value.TryGetValue(terminal, out result)) {
				result = new Collection<CNFTerminalProduction>();
			}
			return result;				
		}
		
		internal List<CNFNonterminalProduction> NonterminalProductions {
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

		public double Cyk(Sentence s) {
			var cyk = new CYKNS.Cyk(this);
			return cyk.GetProbability(s);
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
