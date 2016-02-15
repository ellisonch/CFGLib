using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {
	internal class EpsilonNode : Node {
		private static EpsilonNode _node = new EpsilonNode();

		public static EpsilonNode Node {
			get {
				return _node;
			}
		}
		private EpsilonNode() {

		}
		public override string ToString() {
			return string.Format("(ε){0}", ProductionsToString());
		}
	}
}
