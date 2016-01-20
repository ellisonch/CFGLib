using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsolePlayground {
	internal static class Helpers {
		// http://stackoverflow.com/questions/1210295/how-can-i-add-an-item-to-a-ienumerablet-collection
		public static IEnumerable<T> Append<T>(this IEnumerable<T> e, T value) {
			foreach (var cur in e) {
				yield return cur;
			}
			yield return value;
		}

		// based on http://stackoverflow.com/questions/25824376/combinations-with-repetitions-c-sharp
		internal static IEnumerable<IEnumerable<T>> CombinationsWithRepetition<T>(IEnumerable<T> input, int length) {
			if (length == 0) {
				// return new List<T>();
				yield return Enumerable.Empty<T>();
				// yield break;
			} else {
				foreach (var i in input) {
					var recurse = CombinationsWithRepetition(input, length - 1);
					foreach (var c in recurse) {
						// yield return i.ToString() + c;
						// var retval = new List<T>(c);
						// retval.Add(i);
						yield return c.Append(i);
					}
				}
			}
		}
	}
}
