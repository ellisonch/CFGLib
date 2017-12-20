using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CFGLib.Parsers.Forests;
using CFGLib.Parsers.Sppf;
using CFGLib.Parsers.Sppf.Old;

namespace CFGLib.Parsers.Graphs {
	internal interface INode {
		string Label { get; }
		string Name { get; }
		string Shape { get; }
		string Color { get; }
		string Other { get; }
		string Ordering { get; }
		int Rank { get; }
		int StartPosition { get; }
		int EndPosition { get; }
		Family2<SppfNode2> TheFamily { get; set; }
	}
}
