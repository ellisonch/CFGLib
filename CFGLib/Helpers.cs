using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib {
	internal static class Helpers {
		public static IEnumerable<T> LookupEnumerable<TKey, T>	(
			this IDictionary<TKey, ISet<T>> dictionary,
			TKey key
		) {
			ISet<T> retval;
			if (dictionary.TryGetValue(key, out retval)) {
				return retval;
			}
			return Enumerable.Empty<T>();
		}

		public static IEnumerable<T> LookupEnumerable<TKey, T>(
			this IDictionary<TKey, List<T>> dictionary,
			TKey key
		) {
			List<T> retval;
			if (dictionary.TryGetValue(key, out retval)) {
				return retval;
			}
			return Enumerable.Empty<T>();
		}
		
		public static Dictionary<T1, ISet<T2>> ConstructCache<T1, T2, T3>(
			IEnumerable<T3> terminalProductions,
			Func<T3, T1> getKey,
			Func<T3, T2> getValue
		) {
			var cache = new Dictionary<T1, ISet<T2>>();
			foreach (var production in terminalProductions) {
				ISet<T2> result;
				if (!cache.TryGetValue(getKey(production), out result)) {
					result = new HashSet<T2>();
					cache[getKey(production)] = result;
				}
				result.Add(getValue(production));
			}
			return cache;
		}
	}
}
