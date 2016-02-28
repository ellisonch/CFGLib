using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Forests {
	public abstract class ForestNode {
		public readonly int StartPosition;
		public readonly int EndPosition;

		internal abstract string ToStringHelper(int level);
	}
}
