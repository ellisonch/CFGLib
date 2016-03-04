using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Graphs {
	internal struct Edge {
		public readonly INode Left;
		public readonly INode Right;

		public Production Label;

		public Edge(INode left, INode right, Production label) {
			Left = left;
			Right = right;
			Label = label;
		}
	}
}
