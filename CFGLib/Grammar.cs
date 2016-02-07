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

		public override void RemoveProduction(Production production) {
			_productions.Remove(production);
			Simplify();
		}
		public override void AddProduction(Production production) {
			AddToListWithoutDuplicating(_productions, production);
			InvalidateCaches();
		}

		public Grammar(IEnumerable<Production> productions, Nonterminal start) {
			_productions = new List<Production>(productions);
			this.Start = start;
			
			// if (simplify) {
			SimplifyWithoutInvalidate();
			//}

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
		public CNFGrammar ToCNF() {
			var conv = new CFGtoCNF(this);
			return conv.Convert();
		}

		public override BaseGrammar ShallowClone() {
			var clone = new Grammar(this.Productions, this.Start);
			return clone;
		}
	}
}
