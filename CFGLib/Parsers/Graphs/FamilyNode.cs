using CFGLib.Parsers.Forests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Graphs {
	internal class FamilyNode : INode {
		private Family _family;
		private string _id;
		public int StartPosition { get; set; }
		public int EndPosition { get; set; }

		public string Color {
			get {
				return "white";
			}
		}

		public string Label {
			get {
				return "";
			}
		}

		public string Name {
			get {
				return _id;
			}
		}

		public string Ordering {
			get {
				return "ordering=out";
			}
		}

		public string Shape {
			get {
				return "circle";
			}
		}

		public FamilyNode(Family family, string id) {
			_family = family;
			_id = id;
		}
	}
}
