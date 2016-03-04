using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Graphs {
	internal struct ChildNode : INode {
		public readonly Sentence Sentence;
		public int StartPosition { get; set; }
		public int EndPosition { get; set; }
		public readonly string Id;

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
		public ChildNode(Sentence rhs, int startPosition, int endPosition, string id) : this() {
			Sentence = rhs;
			StartPosition = startPosition;
			EndPosition = endPosition;
			Id = id;
		}
	}
}
