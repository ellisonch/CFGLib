using CFGLib.Parsers.Forests;
using CFGLib.Parsers.Sppf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Graphs {
	internal struct ChildNode : IGraphNode {
		public readonly Sentence Sentence;
		public int StartPosition { get; set; }
		public int EndPosition { get; set; }
		public int Rank { get; set; }
		public readonly string Id;
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

		public string Name {
			get {
				// return string.Format("\"{0} ({1}, {2}) {3}\"", Sentence, StartPosition, EndPosition, Id);
				return string.Format("{0}", Id);
			}
		}
		public string Label {
			get {
				// return string.Format("{0} ({1}, {2}) {3}", Sentence, StartPosition, EndPosition, Id);
				return string.Format("{0} ({1}, {2})", Sentence, StartPosition, EndPosition);
			}
		}
		public string Shape {
			get {
				return "box";
			}
		}
		public string Color {
			get {
				return "white";
			}
		}
		public string Ordering {
			get {
				return "ordering=out";
			}
		}
		public ChildNode(Sentence rhs, int startPosition, int endPosition, string id, int rank) : this() {
			Sentence = rhs;
			StartPosition = startPosition;
			EndPosition = endPosition;
			Id = id;
			Rank = rank;
		}
	}
}
