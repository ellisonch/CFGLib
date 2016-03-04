using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Graphs {
	internal interface INode {
		string Label { get; }
		string Name { get; }
		string Shape { get; }
		string Color { get; }
		string Ordering { get; }
		int StartPosition { get; }
		int EndPosition { get; }
	}
}
