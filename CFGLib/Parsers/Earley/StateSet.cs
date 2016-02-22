using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {
	internal class StateSet {
		private readonly List<Item> _items;
		private readonly Dictionary<Item, Item> _seenItems;
		private readonly HashSet<Nonterminal> _nonterminalsPredicted;
		private readonly List<Item> _magicItems;

		public List<Item> MagicItems {
			get {
				return _magicItems;
			}
		}

		public int Count {
			get {
				return _items.Count;
			}
		}

		public Item this[int index] {
			get {
				return _items[index];
			}
			set {
				_items[index] = value;
			}
		}

		public StateSet(Nonterminal predictionSeedNonterminal = null) {
			_items = new List<Item>();
			_seenItems = new Dictionary<Item, Item>(new ItemComparer());
			if (predictionSeedNonterminal != null) {
				_nonterminalsPredicted = new HashSet<Nonterminal> { predictionSeedNonterminal };
			} else {
				_nonterminalsPredicted = new HashSet<Nonterminal>();
			}
			_magicItems = new List<Item>();
		}

		public List<Item>.Enumerator GetEnumerator() {
			return _items.GetEnumerator();
		}
		
		private void AddUnsafe(Item item) {
			_seenItems[item] = item;
			_items.Add(item);
		}

		public void Insert(Item item) {
			this.AddUnsafe(item);
		}
		public Item InsertWithoutDuplicating(Item item) {
			Item existingItem = null;
			if (!_seenItems.TryGetValue(item, out existingItem)) {
				this.AddUnsafe(item);
				return item;
			}
			// TODO: we're adding duplicate predecessors and reductions because we're rerunning the same completions twice when we have magic predictions
			//if (existingItem.AddedFrom != "Completion" && existingItem.AddedFrom != "PredictionMagic") {
			//	throw new Exception();
			//}
			//if (item.AddedFrom != "Completion" && item.AddedFrom != "PredictionMagic") {
			//	throw new Exception();
			//}
			existingItem.Predecessors.UnionWith(item.Predecessors);
			existingItem.Reductions.UnionWith(item.Reductions);

			return existingItem;
		}

		public override string ToString() {
			var retval = "";
			foreach (var item in _items) {
				retval += item.ToString();
				retval += "\n";
			}
			return retval;
		}

		internal bool PredictedAlreadyAndSet(Nonterminal nonterminal) {
			var predicted = _nonterminalsPredicted.Contains(nonterminal);
			_nonterminalsPredicted.Add(nonterminal);
			return predicted;
		}
	}
}
