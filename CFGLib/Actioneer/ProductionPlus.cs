using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Actioneer {
	public class ProductionPlus {
		public Production Production { get; }
		public ParserAction Action { get; }

		public ProductionPlus(Production production, ParserAction parserAction) {
			Production = production;
			Action = parserAction;
		}
	}
}
