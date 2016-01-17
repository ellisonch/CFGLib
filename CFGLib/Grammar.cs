using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib {
	public class Grammar : BaseGrammar {
		private List<Production> _productions;
		private Nonterminal _start;

		private Dictionary<Nonterminal, List<Production>> _table = new Dictionary<Nonterminal, List<Production>>();

		internal override IEnumerable<Production> ProductionsFrom(Nonterminal lhs) {
			return _table.LookupEnumerable(lhs);
		}

		public override ISet<Production> Productions {
			get { return new HashSet<Production>(_productions); }
		}

		public override ISet<Terminal> Terminals {
			get {
				return null;
			}
		}
		public override ISet<Nonterminal> Nonterminals {
			get {
				return null;
				// return new HashSet<Nonterminal>(this.Productions.Select((x) =>));
			}
		}

		public override Nonterminal Start {
			get { return _start; }
		}

		public Grammar(List<Production> productions, Nonterminal start) {
			_productions = productions;
			
			foreach (var production in productions) {
				var lhs = production.Lhs;
				List<Production> results;
				if (!_table.TryGetValue(lhs, out results)) {
					results = new List<Production>();
					_table[lhs] = results;
				}
				results.Add(production);
			}

			_start = start;
		}

		public CNFGrammar ToCNF() {
			return CNFGrammar.FromCFG(this);
		}

	}
}
