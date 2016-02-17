using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {
	public class Children {
		public Production Production;
		public Sppf Left;
		public Sppf Right;

		public Children(Production production, IEnumerable<Sppf> children) {
			Production = production;
			if (children.Count() > 2) {
				throw new Exception();
			}
			Left = children.ElementAtOrDefault(0);
			Right = children.ElementAtOrDefault(1);
		}
	}
}
