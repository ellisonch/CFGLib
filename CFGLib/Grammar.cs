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

		private Dictionary<Nonterminal, ICollection<BaseProduction>> _table = new Dictionary<Nonterminal, ICollection<BaseProduction>>();

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

		internal override void RemoveProductions(IEnumerable<BaseProduction> toRemove) {
			foreach (var production in toRemove) {
				_productions.Remove(production);
			}
		}

		public Grammar(IEnumerable<BaseProduction> productions, Nonterminal start) {
			_productions = new List<BaseProduction>(productions);
			_start = start;

			RemoveDuplicates();

			_table = Helpers.ConstructCache(
				_productions,
				(p) => p.Lhs,
				(p) => p,
				() => new List<BaseProduction>()
			);
		}

		public CNFGrammar ToCNF() {
			return CNFGrammar.FromCFG(this);
		}
	}
}
