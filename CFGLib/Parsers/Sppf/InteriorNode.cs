using CFGLib.Parsers.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Sppf {
	internal abstract class InteriorNode : SppfNode {
		protected InteriorNode(int startPosition, int endPosition) : base(startPosition, endPosition) {
		}
	}
}
