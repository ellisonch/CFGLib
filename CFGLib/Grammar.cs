using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib {
	/// <summary>
	/// Represents a concrete (probabilistic) context free grammar
	/// </summary>
	public class Grammar : BaseGrammar {
		private List<Production> _productions;

		private Cache<Dictionary<Nonterminal, ICollection<Production>>> _table;

		internal override IEnumerable<Production> ProductionsFrom(Nonterminal lhs) {
			return _table.Value.LookupEnumerable(lhs);
		}

		public override IEnumerable<Production> Productions {
			get { return _productions; }
		}

		internal override void RemoveProductions(IEnumerable<Production> toRemove) {
			foreach (var production in toRemove) {
				_productions.Remove(production);
			}
		}

		public Grammar(IEnumerable<Production> productions, Nonterminal start, bool simplify = true) {
			_productions = new List<Production>(productions);
			this.Start = start;

			RemoveDuplicates();
			if (simplify) {
				SimplifyWithoutInvalidate();
			}

			_table = Cache.Create(() => Helpers.BuildLookup(
				() => _productions,
				(p) => p.Lhs,
				(p) => p,
				() => (ICollection<Production>)new List<Production>(),
				(x, y) => x.Add(y)
			));
			this.Caches.Add(_table);

			BuildHelpers();
		}

		/// <summary>
		/// Returns a new grammar that is the CNF equivalent of this grammar.
		/// WARNING: currently this does not always preserve probabilities!
		/// </summary>
		/// <param name="simplify"></param>
		/// <returns></returns>
		public CNFGrammar ToCNF(bool simplify = true) {
			var conv = new CFGtoCNF(this);
			return conv.Convert(simplify);
		}
	}
}
