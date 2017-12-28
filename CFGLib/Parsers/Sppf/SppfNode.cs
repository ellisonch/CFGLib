using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Sppf {
	public abstract class SppfNode {
		public static int _nextId = 0;
				
		public int StartPosition { get; }
		public int EndPosition { get; }
		private readonly HashSet<SppfFamily> _families = new HashSet<SppfFamily>();
		public readonly int Id = _nextId++;

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

		/// <summary>
		/// Used just to convey production in contracted sppf nodes, for testing
		/// </summary>
		// public Production FakeProduction { get; internal set; }
		
		//internal void AddFamily(Production production) {
		//	var family = new Family2<SppfNode2>(production);
		//	Families.Add(family);
		//}
		internal void AddFamily(Production production, SppfNode v) {
			var family = new SppfFamily(production, v);
			_families.Add(family);
		}

		internal void AddFamily(Production production, SppfNode w, SppfNode v) {
			var family = new SppfFamily(production, w, v);
			_families.Add(family);
		}

		protected abstract string PayloadToString();

		public override string ToString() {
			return string.Format("{0} ({1}, {2})", this.PayloadToString(), StartPosition, EndPosition);
		}
	}
}
