using CFGLib.Parsers.Forests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Graphs {
	internal class SppfNodeNode : INode {
		public readonly SppfNode Node;
		public int Rank { get; set; }
		private Family _theFamily;
		public Family TheFamily {
			get {
				return _theFamily;
			}
			set {
				if (_theFamily != null) {
					throw new Exception();
				}
				_theFamily = value;
			}
		}

		public string Other {
			get {
				return "";
			}
		}
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
				var production = TheFamily?.Production;
				var productionString = string.Format("\nr:{0}", production);
				productionString = null;
				return string.Format("{0}{1}", Node.ToStringSimple(), productionString);
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
		public SppfNodeNode(SppfNode node, int rank) {
			Node = node;
			Rank = rank;
		}
	}
}
