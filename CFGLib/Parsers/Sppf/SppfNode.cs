using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Sppf {
	public abstract class SppfNode {
		public int StartPosition { get; }
		public int EndPosition { get; }
		private readonly HashSet<SppfFamily> _families = new HashSet<SppfFamily>();

		internal IEnumerable<SppfFamily> Families {
			get {
				return _families;
			}
		}
		
		public SppfNode(int startPos, int endPos) {
			if (startPos > endPos) {
				throw new Exception();
			}
			StartPosition = startPos;
			EndPosition = endPos;
		}
		
		internal void AddFamily(Production production, SppfNode v) {
			var family = new SppfFamilySingle(production, v);
			_families.Add(family);
		}
		
		internal void AddFamily(Production production, SppfNode w, SppfNode v) {
			var family = new SppfFamilyDouble(production, w, v);
			_families.Add(family);
		}

		protected abstract string PayloadToString();

		public override string ToString() {
			return string.Format("{0} ({1}, {2})", this.PayloadToString(), StartPosition, EndPosition);
		}
	}
}
