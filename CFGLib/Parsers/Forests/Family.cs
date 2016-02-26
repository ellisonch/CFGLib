using CFGLib.Parsers.Forests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Forests {
	public class Family {
		private readonly List<SppfNode> _nodes = new List<SppfNode>();
		public Production Production { get; internal set; }

		public IList<SppfNode> Members {
			get {
				return _nodes;
			}
		}

		internal Family(SppfNode node1) {
			_nodes.Add(node1);
		}
		internal Family(InteriorNode node1, SppfNode node2) {
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
