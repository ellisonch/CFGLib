using CFGLib.Parsers.Forests;
using CFGLib.Parsers.Sppf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers {
	public abstract class Parser {
		public abstract double ParseGetProbability(Sentence s);
		public bool Accepts(Sentence s) {
			return ParseGetProbability(s) > 0.0;
		}
		public abstract SppfNode2 ParseGetForest(Sentence s);
	}
}
