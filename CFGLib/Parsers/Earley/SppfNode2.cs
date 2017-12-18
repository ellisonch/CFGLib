using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {
	public abstract class SppfNode2 {
		public int StartPosition { get; }
		public int EndPosition { get; }

		public SppfNode2(int startPos, int endPos) {
			if (startPos > endPos) {
				throw new Exception();
			}
			StartPosition = startPos;
			EndPosition = endPos;
		}

		internal HashSet<Family2<SppfNode2>> Families { get; } = new HashSet<Family2<SppfNode2>>();

		/// <summary>
		/// Used just to convey production in contracted sppf nodes, for testing
		/// </summary>
		// public Production FakeProduction { get; internal set; }
		
		//internal void AddFamily(Production production) {
		//	var family = new Family2<SppfNode2>(production);
		//	Families.Add(family);
		//}
		internal void AddFamily(Production production, SppfNode2 v) {
			var family = new Family2<SppfNode2>(production, v);
			Families.Add(family);
		}

		internal void AddFamily(Production production, SppfNode2 w, SppfNode2 v) {
			var family = new Family2<SppfNode2>(production, w, v);
			Families.Add(family);
		}
	}
}
