using CFGLib.Parsers.Forests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers {
	public abstract class Parser {
		public abstract double GetProbability(Sentence s);
		public bool Accepts(Sentence s) {
			return GetProbability(s) > 0.0;
		}
		public abstract Sppf GetParseForest(Sentence s);
	}
}
