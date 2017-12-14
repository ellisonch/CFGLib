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

		internal bool Add(EarleyItem earleyItem) {
			if (earleyItem == null) {
				throw new ArgumentNullException();
			}
			
			if (_hashedItems.Add(earleyItem)) {
				_items.Add(earleyItem);
				return true;
			}
			return false;
		}
		
		public bool Contains(EarleyItem item) {
			return _hashedItems.Contains(item);
		}

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
