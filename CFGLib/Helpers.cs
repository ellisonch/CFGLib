using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("CFGLibTest")]
namespace CFGLib {
	public static class Helpers {
		public static IEnumerable<T> LookupEnumerable<TKey, T>(
			this IDictionary<TKey, ICollection<T>> dictionary,
			TKey key
		) {
			ICollection<T> retval;
			if (dictionary.TryGetValue(key, out retval)) {
				return retval;
			}
			return Enumerable.Empty<T>();
		}

		internal static Dictionary<TKey, TValue> BuildLookup<TKey, TValue, T2, TElm>(
			Func<IEnumerable<TElm>> getInputListOfElements,
			Func<TElm, TKey> getKeyFromElement,
			Func<TElm, T2> getValueFromElement,
			Func<TValue> newEnumerable,
			Action<TValue, T2> updateStored
		) where TValue : class {
			var dict = new Dictionary<TKey, TValue>();
			foreach (var production in getInputListOfElements()) {
				var key = getKeyFromElement(production);
				var value = getValueFromElement(production);
				TValue result;
				if (!dict.TryGetValue(key, out result)) {
					result = newEnumerable();
					dict[key] = result;
				}
				updateStored(result, value);
			}
			return dict;
		}
		
		public static ulong Sum<T>(this IEnumerable<T> source, Func<T, ulong> selector) {
			if (source == null) {
				throw new ArgumentNullException("Cannot sum a null list");
			}
			var sum = 0UL;
			foreach (var item in source) {
				var number = selector(item);
				sum = checked(sum + number);
			}
			return sum;
		}

		public static void Swap<T>(ref T a, ref T b) {
			var temp = a;
			a = b;
			b = temp;
		}

		public static int RemoveMany<T>(this List<T> source, IEnumerable<T> toRemove) {
			var count = 0;
			foreach (var thing in toRemove) {
				var success = source.Remove(thing);
				if (success) {
					count++;
				}
			}
			return count;
		}

		// Used with permission from Jared Parsons
		// http://stackoverflow.com/questions/1210295/how-can-i-add-an-item-to-a-ienumerablet-collection
		public static IEnumerable<T> Append<T>(this IEnumerable<T> e, T value) {
			foreach (var cur in e) {
				yield return cur;
			}
			yield return value;
		}

		// based on http://stackoverflow.com/questions/25824376/combinations-with-repetitions-c-sharp
		// used with permission from Carsten König
		public static IEnumerable<IEnumerable<T>> CombinationsWithoutRepetition<T>(IEnumerable<T> input, int length) {
			if (length == 0) {
				yield return Enumerable.Empty<T>();
			} else {
				int count = 0;
				foreach (var i in input) {
					count++;
					var recurse = CombinationsWithoutRepetition(input.Skip(count), length - 1);
					foreach (var c in recurse) {
						yield return c.Append(i);
					}
				}
			}
		}

		// all 1 at a time - all 2 at a time + all 3 at a time - all 4 at a time...
		// assumes all events are independent, so we can multiply groups
		public static double DisjointProbability(IList<double> probs) {
			double sign = 1.0;
			var result = 0.0;

			if (probs.Count == 0) {
				return 1.0;
			}

			for (int i = 1; i <= probs.Count; i++) {
				foreach (var combination in Helpers.CombinationsWithoutRepetition(probs, i)) {
					var product = combination.Aggregate(sign, (p, q) => p * q);
					result += product;
				}
				sign *= -1;
			}

			return result;
		}

		public static string Indent(this string str, int count) {
			return new string(' ', count) + str;
		}
	}
	internal class Boxed<T> {
		public T Value;
		public Boxed(T value) {
			Value = value;
		}
	}
}
