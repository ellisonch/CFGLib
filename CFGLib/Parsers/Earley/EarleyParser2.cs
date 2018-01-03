using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CFGLib.Actioneer;
using CFGLib.Parsers.Sppf;
using CFGLib.ProductionAnnotations.Gathering;

namespace CFGLib.Parsers.Earley {
	/// <summary>
	/// This parser is based directly on Section 5 of Elizabeth Scott's 2008 paper "SPPF-Style Parsing From Earley Recognisers" (http://dx.doi.org/10.1016/j.entcs.2008.03.044) [ES2008]
	/// </summary>
	public class EarleyParser2 : Parser {
		public EarleyParser2(BaseGrammar grammar) : base(grammar) {
			//_S = Grammar.Start;
		}
		
		public override SppfNode ParseGetForest(Sentence s) {
			var ph = new EarleyParser2Helper(Grammar, s);
			var sppf = ph.ParseGetSppf();
			return sppf;
		}

	}
}
