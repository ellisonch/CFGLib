using CFGLib.Parsers.Sppf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.ProductionAnnotations.Actioning {
	public static class IdentityActions {
		public static Grammar Annotate(Grammar g) {
			var start = g.Start;
			var productions = g.Productions;
			var newProductions = new List<Production>();
			foreach (var production in productions) {
				var annotations = production.Annotations.AllAnnotations;
				var newAnnotations = new List<IAnnotation>();
				foreach (var annotation in annotations) {
					if (!(annotation is ActionAnnotation)) {
						newAnnotations.Add(annotation);
					}
				}
				var action = new ActionAnnotation(args => {
					var retval = new Sentence();

					foreach (var arg in args) {
						var s = GetSentenceForArg(arg);
						retval.AddRange(s);
					}
					return retval;
				});
				newAnnotations.Add(action);
				var newProduction = new Production(production.Lhs, production.Rhs, production.Weight, new Annotations(newAnnotations));
				newProductions.Add(newProduction);
			}
			return new Grammar(newProductions, start);
		}

		private static Sentence GetSentenceForArg(TraverseResult arg) {
			if (arg.Node is SppfEpsilon) {
				return new Sentence();
			} else if (arg.Node is SppfWord sppfWord) {
				if (sppfWord.Word is Terminal term) {
					return new Sentence(term);
				}				
			}

			return (Sentence)arg.Payload;
		}
	}
}
