using CFGLib.Parsers.Forests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Sppf {
	internal class EpsilonNode : LeafNode {
		// private static EpsilonNode _node = new EpsilonNode();

		//public static EpsilonNode Node {
		//	get {
		//		return _node;
		//	}
		//}
		public EpsilonNode(int start, int end) : base(start, end) {
		}
		
		//public override string ToString() {
		//	return string.Format("(ε, ){0}", ProductionsToString());
		//}

		public override Sentence GetSentence() {
			return new Sentence();
		}
	}
}
