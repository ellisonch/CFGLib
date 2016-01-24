using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLibTest {
	public static class Helpers {
		public static void AssertNear(double expected, double actual) {
			double tolerance = Math.Abs(expected * 0.00001);
			double diff = Math.Abs(expected - actual);
			if (diff <= tolerance) {
				return;
			}
			Assert.Fail(string.Format("Expected {0}, but got {1}", expected, actual));
		}

		// Used with permission from Jared Parsons
		// http://stackoverflow.com/questions/1210295/how-can-i-add-an-item-to-a-ienumerablet-collection
		public static IEnumerable<T> Append<T>(this IEnumerable<T> e, T value) {
			foreach (var cur in e) {
				yield return cur;
			}
			yield return value;
		}

		// used with permission from Carsten König
		// based on http://stackoverflow.com/questions/25824376/combinations-with-repetitions-c-sharp
		public static IEnumerable<IEnumerable<T>> CombinationsWithRepetition<T>(IEnumerable<T> input, int length) {
			if (length == 0) {
				yield return Enumerable.Empty<T>();
			} else {
				foreach (var i in input) {
					var recurse = CombinationsWithRepetition(input, length - 1);
					foreach (var c in recurse) {
						yield return c.Append(i);
					}
				}
			}
		}
	}
}
