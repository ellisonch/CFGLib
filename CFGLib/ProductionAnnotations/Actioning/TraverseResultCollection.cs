using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CFGLib.ProductionAnnotations.Actioning {
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

		public override string ToString() {
			return string.Join(", ", _results.Select((x) => x.ToString()));
		}
	}
}