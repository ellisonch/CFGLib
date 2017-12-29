using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Graphs {
	internal struct Edge {
		public readonly IGraphNode Left;
		public readonly IGraphNode Right;

		public readonly Production Label;

		public Edge(IGraphNode left, IGraphNode right, Production label) {
			Left = left;
			Right = right;
			Label = label;
		}
	}
}
