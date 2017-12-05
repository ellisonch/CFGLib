using System.Collections;
using System.Collections.Generic;

namespace CFGLib.Actioneer {
	public class TraverseResultCollection : IEnumerable<TraverseResult> {
		private readonly IEnumerable<TraverseResult> _results;
		public TraverseResultCollection(IEnumerable<TraverseResult> results) {
			_results = results;
		}
		public IEnumerator<TraverseResult> GetEnumerator() {
			return _results.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}