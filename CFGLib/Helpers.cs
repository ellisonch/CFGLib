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
		
		public static Dictionary<T1, ICollection<T2>> ConstructCache<T1, T2, TElm>(
			IEnumerable<TElm> inputListOfElements,
			Func<TElm, T1> getKeyFromElement,
			Func<TElm, T2> getValueFromElement,
			Func<ICollection<T2>> newEnumerable
		) {
			var cache = new Dictionary<T1, ICollection<T2>>();
			foreach (var production in inputListOfElements) {
				ICollection<T2> result;
				if (!cache.TryGetValue(getKeyFromElement(production), out result)) {
					result = newEnumerable();
					cache[getKeyFromElement(production)] = result;
				}
				result.Add(getValueFromElement(production));
			}
			return cache;
		}
	}
}
