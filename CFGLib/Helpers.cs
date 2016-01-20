using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib {
	internal static class Helpers {
		public static IEnumerable<T> LookupEnumerable<TKey, T> (
			this IDictionary<TKey, ICollection<T>> dictionary,
			TKey key
		) {
			ICollection<T> retval;
			if (dictionary.TryGetValue(key, out retval)) {
				return retval;
			}
			return Enumerable.Empty<T>();
		}
		
		public static Dictionary<T1, TStored> ConstructCache<T1, T2, TElm, TStored>(
			IEnumerable<TElm> inputListOfElements,
			Func<TElm, T1> getKeyFromElement,
			Func<TElm, T2> getValueFromElement,
			Func<TStored> newEnumerable,
			Action<TStored, T2> updateStored
		) where TStored : class {
			var cache = new Dictionary<T1, TStored>();
			foreach (var production in inputListOfElements) {
				var key = getKeyFromElement(production);
				var value = getValueFromElement(production);
				TStored result;
				if (!cache.TryGetValue(key, out result)) {
					result = newEnumerable();
					cache[key] = result;
				}
				updateStored(result, value);
			}
			return cache;
		}
		public static Dictionary<T1, TStored> ConstructCacheValue<T1, T2, TElm, TStored>(
			IEnumerable<TElm> inputListOfElements,
			Func<TElm, T1> getKeyFromElement,
			Func<TElm, T2> getValueFromElement,
			Func<TStored> newEnumerable,
			Func<TStored, T2, TStored> updateStored
		) {
			var cache = new Dictionary<T1, TStored>();
			foreach (var production in inputListOfElements) {
				var key = getKeyFromElement(production);
				var value = getValueFromElement(production);
				TStored result;
				if (!cache.TryGetValue(key, out result)) {
					result = newEnumerable();
					cache[key] = result;
				}
				cache[key] = updateStored(result, value);
			}
			return cache;
		}
	}
}
