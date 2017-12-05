using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Actioneer {
	public class GrammarPlus {
		private readonly Grammar _grammar;
		private readonly List<ProductionPlus> _productions;
		// private readonly Dictionary<Production, IParserAction> _actions = new Dictionary<Production, IParserAction>();
		private readonly Dictionary<Production, ProductionPlus> _lookup = new Dictionary<Production, ProductionPlus>();

		public GrammarPlus(Grammar grammar, List<ProductionPlus> productions) {
			_grammar = grammar;
			_productions = productions;
			foreach (var production in productions) {
				_lookup[production.Production] = production;
			}
		}

		internal bool TryGetValue(Production production, out ProductionPlus productionPlus) {
			return _lookup.TryGetValue(production, out productionPlus);
		}
	}
}
