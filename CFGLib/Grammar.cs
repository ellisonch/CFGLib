using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib {
	public class Grammar : BaseGrammar {
		private List<BaseProduction> _productions;
		private Nonterminal _start;

		private Dictionary<Nonterminal, List<BaseProduction>> _table = new Dictionary<Nonterminal, List<BaseProduction>>();

		internal override IEnumerable<BaseProduction> ProductionsFrom(Nonterminal lhs) {
			return _table.LookupEnumerable(lhs);
		}

		public override IEnumerable<BaseProduction> Productions {
			get { return _productions; }
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

		public Grammar(IEnumerable<BaseProduction> productions, Nonterminal start) {
			_productions = new List<BaseProduction>(productions);
			
			foreach (var production in productions) {
				var lhs = production.Lhs;
				List<BaseProduction> results;
				if (!_table.TryGetValue(lhs, out results)) {
					results = new List<BaseProduction>();
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
