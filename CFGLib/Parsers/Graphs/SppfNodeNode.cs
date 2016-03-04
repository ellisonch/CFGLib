using CFGLib.Parsers.Forests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Graphs {
	internal class SppfNodeNode : INode {
		public readonly SppfNode Node;

		public int StartPosition {
			get { return Node.StartPosition; }
		}
		public int EndPosition {
			get { return Node.EndPosition; }
		}

		public string Name {
			get {
				// return string.Format("\"{0} {1}\"", Node.ToStringSelf(), Id);
				return string.Format("{0}", Node.Id);
				// return string.Format("{0}", &Node);
			}
		}
		public string Label {
			get {
				// return string.Format("{0} {1}", Node.ToStringSelf(), Id);
				return string.Format("{0}", Node.ToStringSimple());
			}
		}
		public string Shape {
			get {
				return "oval";
			}
		}
		public string Color {
			get {
				if (Node is LeafNode) {
					return "yellow";
				} else {
					return "white";
				}
			}
		}
		public string Ordering {
			get {
				return "";
			}
		}
		public SppfNodeNode(SppfNode node) {
			Node = node;
		}
	}
}
