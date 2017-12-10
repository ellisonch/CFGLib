using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {
	internal class Family2 {
		private List<SppfNode2> Children { get; }

		public Family2(SppfNode2 v) {
			Children = new List<SppfNode2> { v };
		}
		public Family2(SppfNode2 w, SppfNode2 v) {
			Children = new List<SppfNode2> { w, v };
		}

		internal class Family {
			private readonly List<SppfNode2> _nodes = new List<SppfNode2>();
			public Production Production { get; internal set; }

			public IList<SppfNode2> Members {
				get {
					return _nodes;
				}
			}

			internal Family(SppfNode2 node1) {
				_nodes.Add(node1);
			}
			internal Family(SppfNode2 node1, SppfNode2 node2) {
				_nodes.Add(node1);
				_nodes.Add(node2);
			}

			public override int GetHashCode() {
				var first = _nodes.ElementAtOrDefault(0);
				var second = _nodes.ElementAtOrDefault(1);

				return new { first, second }.GetHashCode();
			}

			public override bool Equals(Object other) {
				if (other == null) {
					return false;
				}
				var localOther = other as Family;
				if (localOther == null) {
					return false;
				}

				return _nodes.SequenceEqual(localOther._nodes);
			}
		}
	}
}
