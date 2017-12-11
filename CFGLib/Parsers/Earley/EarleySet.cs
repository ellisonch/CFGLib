using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {
	internal class EarleySet : IEnumerable<EarleyItem> {
		private readonly List<EarleyItem> _items = new List<EarleyItem>();

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

		public EarleyItem this[int i] {
			get { return _items[i]; }
		}

		public EarleySet() { }

		public EarleySet(EarleySet earleySet) {
			foreach (var item in earleySet._items) {
				this.Add(item);
			}
		}

		internal void Add(EarleyItem earleyItem) {
			if (earleyItem == null) {
				throw new ArgumentNullException();
			}
			_items.Add(earleyItem);
		}

		// TODO: Need to use hash
		public bool Contains(EarleyItem item) {
			return _items.Contains(item);
		}

		internal EarleyItem TakeOne() {
			if (IsEmpty) {
				throw new Exception();
			}
			var item = _items[_items.Count - 1];
			_items.RemoveAt(_items.Count - 1);
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
