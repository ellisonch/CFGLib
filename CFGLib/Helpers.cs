using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib {
	internal static class Helpers {
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
		
		/// <summary>
		/// Returns a bitmask just large enough to cover the given number
		/// </summary>
		/// <param name="n"></param>
		/// <returns></returns>
		private static ulong NextHighestBitmask(ulong n) {
			if (n == 0UL) {
				return 0UL;
			}
			n--;
			n |= n >> 1;
			n |= n >> 2;
			n |= n >> 4;
			n |= n >> 8;
			n |= n >> 16;
			n |= n >> 32;

			return n;
		}
	}
	internal class Boxed<T> {
		public T Value;
		public Boxed(T value) {
			Value = value;
		}
	}
}
