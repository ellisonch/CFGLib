using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Actioneer {
	public class ProductionPlus {
		public Production Production { get; }
		public ParserAction Action { get; }
		public double Precedence { get; }
		public GatherOption[] Gathers { get; }

		public ProductionPlus(Production production, ParserAction parserAction, double precedence, GatherOption[] gathers) {
			Production = production;
			Action = parserAction;
			Precedence = precedence;
			Gathers = gathers;
		}

		internal bool Supports(GrammarPlus annotatedGrammar, TraverseResult[] oneSet) {
			var gathers = this.Gathers;
			var nti = 0;
			for (int i = 0; i < this.Production.Rhs.Count; i++) {
				var wordi = this.Production.Rhs[i];
				if (wordi.IsTerminal) {
					continue;
				}
				var gather = gathers[nti];
				var arg = oneSet[i];
				if (arg.Production == null) {
					continue;
				}
				if (annotatedGrammar.TryGetValue(arg.Production, out ProductionPlus subProduction)) {
					switch (gather) {
						case GatherOption.SameOrLower:
							if (subProduction.Precedence > this.Precedence) {
								return false;
							}
							break;
						case GatherOption.StrictlyLower:
							if (subProduction.Precedence >= this.Precedence) {
								return false;
							}
							break;
					}
				}
				nti++;
			}
			return true;
		}
	}
}
