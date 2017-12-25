using CFGLib.ProductionAnnotations.Actioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.ProductionAnnotations.Gathering {
	public class GatherAnnotation : IAnnotation {
		public GatherOption[] Value { get; }

		public GatherAnnotation(GatherOption[] gathers) {
			Value = gathers;
		}

		public bool Supports(BaseGrammar annotatedGrammar, Production production, TraverseResult[] oneSet) {
			var gathers = this.Value;
			var precedence = production.Annotations.Precedence;
			if (precedence == null) {
				return true;
			}
			var nti = 0;
			for (int i = 0; i < production.Rhs.Count; i++) {
				var wordi = production.Rhs[i];
				if (wordi.IsTerminal) {
					continue;
				}
				var gather = gathers[nti];
				var arg = oneSet[i];
				if (arg.Production == null) {
					continue;
				}
				var subPrecedence = arg.Production.Annotations.Precedence;
				if (subPrecedence == null) {
					continue;
				}
				switch (gather) {
					case GatherOption.SameOrLower:
						if (subPrecedence.Value > precedence.Value) {
							return false;
						}
						break;
					case GatherOption.StrictlyLower:
						if (subPrecedence.Value >= precedence.Value) {
							return false;
						}
						break;
				}
				nti++;
			}
			return true;
		}
	}
}
