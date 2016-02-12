using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {
	internal abstract class Node {
		public readonly HashSet<Family> Families = new HashSet<Family>();
	}
}
