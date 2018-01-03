using CFGLib.Parsers.Sppf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Graphs {
	internal class SppfNodeNode : IGraphNode {
		public readonly SppfNode Node;
		private readonly int _id;
		public int Rank { get; set; }
		private SppfFamily _theFamily;
		public SppfFamily TheFamily {
			get {
				return _theFamily;
			}
			set {
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
				return string.Format("{0}", _id);
				// return string.Format("{0}", &Node);
			}
		}
		public string Label {
			get {
				// return string.Format("{0} {1}", Node.ToStringSelf(), Id);
				string productionString = null;
				if (TheFamily == null) {
					productionString = null;
				} else {
					var production = TheFamily.Production;
					productionString = string.Format("\nr:{0}", production);
					productionString = null;
				}
				
				return string.Format("{0}{1}", Node.ToString(), productionString);
			}
		}
		public string Shape {
			get {
				return "oval";
			}
		}
		public string Color {
			get {
				if (Node is SppfEpsilon) {
					return "yellow";
				} else if (Node is SppfWord sppfWord) {
					if (sppfWord.Word.IsTerminal) {
						return "yellow";
					}
				}
				return "white";
			}
		}
		public string Ordering {
			get {
				return "";
			}
		}
		public SppfNodeNode(SppfNode node, int rank, int id) {
			Node = node;
			Rank = rank;
			_id = id;
		}
	}
}
