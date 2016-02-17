using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {
	internal class StateSet {
		List<Item> _list = new List<Item>();
		Dictionary<Item, Item> _hash = new Dictionary<Item, Item>(new ItemComparer());
		
		public List<Item>.Enumerator GetEnumerator() {
			return _list.GetEnumerator();
		}

		// Should only be used during initialization!
		internal void Add(Item item) {
			if (_hash.ContainsKey(item)) {
				throw new Exception("Duplicate item found when using Add()");
			}
			AddUnsafe(item);
		}

		private void AddUnsafe(Item item) {
			_hash[item] = item;
			_list.Add(item);
		}

		public int Count {
			get {
				return _list.Count;
			}
		}

		public Item this[int index] {
			get {
				return _list[index];
			}
			set {
				_list[index] = value;
			}
		}

		public void InsertWithoutDuplicating(int stateIndex, Item item) {
			// the endPosition should always equal the stateIndex of the state it resides in
			item.EndPosition = stateIndex;

			// var existingItem = _list.Find(equalityCheck);
			Item existingItem = null;
			if (!_hash.TryGetValue(item, out existingItem)) {
				this.AddUnsafe(item);
			} else {
				// TODO: we're adding duplicate predecessors and reductions because we're rerunning the same completions twice
				existingItem.Predecessors.AddRange(item.Predecessors);
				existingItem.Reductions.AddRange(item.Reductions);
			}
		}
	}
}
