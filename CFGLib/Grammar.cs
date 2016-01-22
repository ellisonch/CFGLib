using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib {
	public class Grammar : BaseGrammar {
		private List<BaseProduction> _productions;

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

		internal override void RemoveProductions(IEnumerable<BaseProduction> toRemove) {
			foreach (var production in toRemove) {
				_productions.Remove(production);
			}
		}

		public Grammar(IEnumerable<BaseProduction> productions, Nonterminal start, bool simplify = true) {
			_productions = new List<BaseProduction>(productions);
			this.Start = start;

			if (simplify) {
				Simplify();
			}

			_table = Helpers.ConstructCache(
				_productions,
				(p) => p.Lhs,
				(p) => p,
				() => (ICollection<BaseProduction>)new List<BaseProduction>(),
				(x, y) => x.Add(y)
			);

			BuildHelpers();
		}

		public CNFGrammar ToCNF(bool simplify = true) {
			var conv = new CFGtoCNF(this);
			return conv.Convert(simplify);
		}
	}
}
