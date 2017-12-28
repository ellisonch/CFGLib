using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {
	internal class EarleySet : IEnumerable<EarleyItem> {
		private readonly List<EarleyItem> _items = new List<EarleyItem>();
		private readonly HashSet<EarleyItem> _hashedItems = new HashSet<EarleyItem>();
		private readonly Dictionary<Nonterminal, List<EarleyItem>> _nonterminalCache = new Dictionary<Nonterminal, List<EarleyItem>>();

		public bool IsEmpty {
			get {
				return _items.Count == 0;
			}
		}

		public int Count {
			get {
				return _items.Count;
			}
		}

		//public EarleyItem this[int i] {
		//	get { return _items[i]; }
		//}

		public IEnumerable<EarleyItem> FixedEnum() {
			var count = _items.Count;
			for (var i = 0; i < count; i++) {
				yield return _items[i];
			}
		}
		public IEnumerable<EarleyItem> ItemsAtNonterminal(Nonterminal nonterminal) {
			if (!_nonterminalCache.TryGetValue(nonterminal, out var list)) {
				yield break;
			}
			var count = list.Count;
			for (var i = 0; i < count; i++) {
				var item = list[i];
				if (item.DecoratedProduction.NextWord != nonterminal) {
					continue;
				}
				yield return item;
			}
		}

		public EarleySet() { }

		public EarleySet(EarleySet earleySet) {
			foreach (var item in earleySet._items) {
				this.Add(item);				
			}
		}

		internal bool Add(EarleyItem earleyItem) {
			if (earleyItem == null) {
				throw new ArgumentNullException();
			}
			
			if (_hashedItems.Add(earleyItem)) {
				_items.Add(earleyItem);
				var nextWord = earleyItem.DecoratedProduction.NextWord;
				if (nextWord is Nonterminal nt) {
					if (!_nonterminalCache.TryGetValue(nt, out var ntList)) {
						ntList = new List<EarleyItem>();
						_nonterminalCache[nt] = ntList;
					}
					ntList.Add(earleyItem);
				}
				return true;
			}
			return false;
		}
		
		//public bool Contains(EarleyItem item) {
		//	return _hashedItems.Contains(item);
		//}

		internal EarleyItem TakeOne() {
			if (IsEmpty) {
				throw new Exception();
			}
			var item = _items[_items.Count - 1];
			_items.RemoveAt(_items.Count - 1);
			_hashedItems.Remove(item);
			return item;
		}

		public IEnumerator<EarleyItem> GetEnumerator() {
			return _items.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
