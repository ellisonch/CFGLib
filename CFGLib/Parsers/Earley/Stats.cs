using System.Collections.Generic;
using System.Diagnostics;

namespace CFGLib.Parsers.Earley {
	public class Stats {
		public Dictionary<string, long> Data {
			get;
		} = new Dictionary<string, long>();

		[Conditional("DEBUG")]
		public void AddCount(string name) {
			if (!Data.ContainsKey(name)) {
				Data[name] = 0;
			}
			Data[name]++;
		}
	}
}