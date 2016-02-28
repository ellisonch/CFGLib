using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Forests {
	public class ForestLeaf : ForestNode {
		private readonly Terminal _terminal;

		internal ForestLeaf(TerminalNode terminalChild) {
			_terminal = terminalChild.Terminal;
		}

		internal override string ToStringHelper(int level) {
			var retval = "";
			retval += string.Format("{0}\n", _terminal.ToString()).Indent(2 * level);
			return retval;
		}
	}
}
