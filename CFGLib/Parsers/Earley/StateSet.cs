using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {
	internal class StateSet {
		private readonly List<Item> _list = new List<Item>();
		private readonly Dictionary<Item, Item> _hash = new Dictionary<Item, Item>(new ItemComparer());
		private readonly HashSet<Nonterminal> _alreadyPredicted = new HashSet<Nonterminal>();
		private readonly List<Item> _magicItems = new List<Item>();

		public List<Item> MagicItems {
			get {
				return _magicItems;
			}
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

		public void Insert(Item item) {

			this.AddUnsafe(item);
		}
		public Item InsertWithoutDuplicating(Item item) {

			Item existingItem = null;
			if (!_hash.TryGetValue(item, out existingItem)) {
				this.AddUnsafe(item);
				return item;
			}
			// TODO: we're adding duplicate predecessors and reductions because we're rerunning the same completions twice
			existingItem.Predecessors.UnionWith(item.Predecessors);
			existingItem.Reductions.UnionWith(item.Reductions);

			return existingItem;
		}

		public override string ToString() {
			var retval = "";
			foreach (var item in _list) {
				retval += item.ToString();
				retval += "\n";
			}
			return retval;
		}

		internal bool PredictedAlreadyAndSet(Nonterminal nonterminal) {
			var predicted = _alreadyPredicted.Contains(nonterminal);
			_alreadyPredicted.Add(nonterminal);
			return predicted;
		}
	}
}
