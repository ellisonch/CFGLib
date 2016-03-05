using CFGLib.Parsers.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Forests {
	internal abstract class InteriorNode : SppfNode {
		private HashSet<Family> _familiesInternal = new HashSet<Family>(); // used during construction only
		private readonly List<Family> _families = new List<Family>();

		internal override IList<Family> Families {
			get {
				return _families;
			}
		}
		
		protected InteriorNode(int startPosition, int endPosition) : base(startPosition, endPosition) {
		}

		internal void AddFamily(Family family) {
			_familiesInternal.Add(family);
		}
		internal override void FinishFamily() {
			if (_familiesInternal != null) {
				_families.Clear();
				_families.AddRange(_familiesInternal);
				_familiesInternal = null;
			}
		}

		internal void AddChild(int i, Production production) {
			if (i >= _families.Count) {
				throw new Exception();
			}
			if (_families[i].Production != null) {
				if (production != _families[i].Production) {
					throw new Exception();
				}
			}
			_families[i].Production = production;
		}

		internal override void GetGraphHelper(Graph g, SppfNodeNode myNode, HashSet<InteriorNode> visited) {
			if (visited.Contains(this)) {
				return;
			}
			visited.Add(this);

			// foreach (var family in Families) {
			for (int i = 0; i < Families.Count; i++) {
				var family = Families[i];
				Production singletonProduction = null;
				INode prevNode;
				if (Families.Count == 1) {
					prevNode = myNode;
					singletonProduction = Families[0].Production;
				} else {
					prevNode = new FamilyNode(family, myNode.Node.Id + "-" + i, myNode.Rank + 1);
					// g.AddEdge(myNode, prevNode, family.Production);
					g.AddEdge(myNode, prevNode);
				}
				foreach (var child in family.Members) {
					var childNode = new SppfNodeNode(child, prevNode.Rank + 1);
					// var childNode = g.GetNode(child, prevNode.Rank + 1);
					// g.AddEdge(prevNode, childNode, singletonProduction);
					g.AddEdge(prevNode, childNode);
					child.GetGraphHelper(g, childNode, visited);
				}
			}
		}
	}
}
