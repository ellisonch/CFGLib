using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib {
	public interface IDirtyable {
		void SetDirty();
	}
	// this is a trick to get around the lack of Class<T> type inference in C#
	internal static class Cache {
		public static Cache<TKey, TValue> Create<TKey, TValue, T2, TElm>(
            Func<IEnumerable<TElm>> getInputListOfElements,
			Func<TElm, TKey> getKeyFromElement,
			Func<TElm, T2> getValueFromElement,
			Func<TValue> newEnumerable,
			Action<TValue, T2> updateStored
		) where TValue : class {
			return Cache<TKey, TValue>.ConstructCache(getInputListOfElements, getKeyFromElement, getValueFromElement, newEnumerable, updateStored);
		}
	}
	internal class Cache<TKey, TValue> : IDirtyable where TValue : class {
		private bool _dirty = true;
		private Dictionary<TKey, TValue> _table;
		private Func<TValue> _defaultValue;
		private Func<Dictionary<TKey, TValue>> _build;

		private Cache(Func<Dictionary<TKey, TValue>> buildCommand, Func<TValue> defaultValue) {
			_build = buildCommand;
			_defaultValue = defaultValue;
		}
		
		public TValue this[TKey key] {
			get {
				if (_dirty) {
					Build();
				}
				TValue result;
				if (!_table.TryGetValue(key, out result)) {
					return _defaultValue();
				}
				return result;
			}
		}

		private void Build() {
			_table = _build();
			_dirty = false;
		}

		public void SetDirty() {
			_dirty = true;
		}

		internal static Cache<TKey, TValue> ConstructCache<T2, TElm>(
			Func<IEnumerable<TElm>> getInputListOfElements,
			Func<TElm, TKey> getKeyFromElement,
			Func<TElm, T2> getValueFromElement,
			Func<TValue> newEnumerable,
			Action<TValue, T2> updateStored
		) {
			Func<Dictionary<TKey, TValue>> build = () => {
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
			};
			return new Cache<TKey, TValue>(build, newEnumerable);
			// return dict;
		}
		//public static Cache<TKey, TValue> ConstructCacheValue<T2, TElm>(
		//	IEnumerable<TElm> inputListOfElements,
		//	Func<TElm, TKey> getKeyFromElement,
		//	Func<TElm, T2> getValueFromElement,
		//	Func<TValue> newEnumerable,
		//	Func<TValue, T2, TValue> updateStored
		//) {
		//	var dict = new Dictionary<TKey, TValue>();
		//	foreach (var production in inputListOfElements) {
		//		var key = getKeyFromElement(production);
		//		var value = getValueFromElement(production);
		//		TValue result;
		//		if (!dict.TryGetValue(key, out result)) {
		//			result = newEnumerable();
		//			dict[key] = result;
		//		}
		//		dict[key] = updateStored(result, value);
		//	}
		//	return new Cache<TKey, TValue>(dict);
		//}
	}
}
