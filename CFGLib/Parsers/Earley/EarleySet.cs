using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {
	internal class EarleySet {
		private readonly List<EarleyItem> _items = new List<EarleyItem>();

		public bool IsEmpty {
			get {
				return _items.Count == 0;
			}
		}

		internal void Add(EarleyItem earleyItem) {
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
	}
}
