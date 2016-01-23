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
		private List<BaseProduction> _productions;

		private Cache<Dictionary<Nonterminal, ICollection<BaseProduction>>> _table;

		internal override IEnumerable<BaseProduction> ProductionsFrom(Nonterminal lhs) {
			return _table.Value.LookupEnumerable(lhs);
		}

		public override IEnumerable<BaseProduction> Productions {
			get { return _productions; }
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
				SimplifyWithoutInvalidate();
			}

			_table = Cache.Create(() => Helpers.BuildLookup(
				() => _productions,
				(p) => p.Lhs,
				(p) => p,
				() => (ICollection<BaseProduction>)new List<BaseProduction>(),
				(x, y) => x.Add(y)
			));
			this.Caches.Add(_table);

			BuildHelpers();
		}

		// TODO: preserve probabilities
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
