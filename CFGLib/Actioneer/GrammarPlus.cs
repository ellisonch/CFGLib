using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Actioneer {
	public class GrammarPlus {
		private readonly Grammar _grammar;
		private readonly List<ProductionPlus> _productions;
		private readonly Dictionary<Production, IParserAction> _actions = new Dictionary<Production, IParserAction>();

		public GrammarPlus(Grammar grammar, List<ProductionPlus> productions) {
			_grammar = grammar;
			_productions = productions;
			foreach (var production in productions) {
				_actions[production.Production] = production.Action;
			}
		}

		internal bool TryGetValue(Production production, out IParserAction action) {
			return _actions.TryGetValue(production, out action);
		}
	}
}
