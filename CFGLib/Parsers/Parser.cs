using CFGLib.Parsers.Earley;
using CFGLib.Parsers.Sppf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers {
	public abstract class Parser {
		public Parser(BaseGrammar grammar) {
			Grammar = grammar;
		}

		protected BaseGrammar Grammar { get; }

		public virtual double ParseGetProbability(Sentence s) {
			var sppf = ParseGetForest(s);
			if (sppf == null) {
				return 0.0;
			}
			var prob = ProbabilityOfSppf(sppf);
			return prob;
		}

		public double ProbabilityOfSppf(SppfNode sppf) {
			return ProbabilityCalculator.GetProbFromSppf(Grammar, sppf);
		}

		public bool Accepts(Sentence s) {
			return ParseGetProbability(s) > 0.0;
		}
		public abstract SppfNode ParseGetForest(Sentence s);
	}
}
