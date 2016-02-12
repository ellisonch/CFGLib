using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {
	internal class Family {
		private readonly List<Node> _nodes = new List<Node>();
		public IList<Node> Members {
			get {
				return _nodes;
			}
		}
		
		public Family(Node node1) {
			_nodes.Add(node1);
		}
		public Family(InteriorNode node1, InteriorNode node2) {
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
