using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Forests.ForestVisitors {
	public interface IForestVisitor {
		bool Visit(ForestInternal forestInternal);
		bool Visit(ForestLeaf forestInternal);
	}
}
