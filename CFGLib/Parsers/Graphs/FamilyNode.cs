using CFGLib.Parsers.Forests;
using CFGLib.Parsers.Sppf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Graphs {
	internal class FamilyNode : INode {
		private SppfFamily<SppfNode> _family;
		private string _id;
		public int StartPosition { get; set; }
		public int EndPosition { get; set; }
		public int Rank { get; set; }
		private SppfFamily<SppfNode> _theFamily;
		public SppfFamily<SppfNode> TheFamily {
			get {
				return _theFamily;
			}
			set {
				_theFamily = value;
			}
		}

		public string Color {
			get {
				return "white";
			}
		}

		public string Label {
			get {
				return "";
				
				//var production = _family.Production;
				//var fakeProduction = _family.Members[0].FakeProduction;
				//return string.Format("\n{0}\n{1}", production, fakeProduction);
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

		public FamilyNode(SppfFamily<SppfNode> family, string id, int rank) {
			_family = family;
			_id = id;
			Rank = rank;
		}
	}
}
