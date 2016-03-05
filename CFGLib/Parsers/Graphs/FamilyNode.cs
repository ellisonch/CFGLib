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
		public int Rank { get; set; }

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

		public string Other {
			get {
				return "fixedsize=true width=0.25 height=0.25";
			}
		}

		public FamilyNode(Family family, string id, int rank) {
			_family = family;
			_id = id;
			Rank = rank;
		}
	}
}
