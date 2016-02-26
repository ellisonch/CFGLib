using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Forests {
	public abstract class InteriorNode : SppfNode {
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
	}
}
