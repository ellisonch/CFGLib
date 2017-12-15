using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {
	public class SppfNodeDictionary {
		private Dictionary<ValueTuple<Word, int, int>, SppfWord> _wordDict = new Dictionary<ValueTuple<Word, int, int>, SppfWord>();
		private Dictionary<ValueTuple<DecoratedProduction, int, int>, SppfBranch> _prodDict = new Dictionary<ValueTuple<DecoratedProduction, int, int>, SppfBranch>();

		//internal SppfNode2 this[ValueTuple<ValueTuple<Word, DecoratedProduction>, int, int> key] {
		//	set {
		//		_dict[key] = value;
		//	}
		//}

		//// public TValue this[TKey key] { get; set; }
		//internal bool TryGetValue((ValueTuple<Word, DecoratedProduction>, int, int) tup, out SppfNode2 v) {
		//	return _dict.TryGetValue(tup, out v);
		//}

		internal SppfWord GetOrSet(Word item, int j, int i) {
			var tup = ValueTuple.Create(item, j, i);
			SppfWord y;
			if (!_wordDict.TryGetValue(tup, out y)) {
				var newY = new SppfWord(item, j, i);
				_wordDict[tup] = newY;
				y = newY;
			}
			return y;
		}
		internal SppfBranch GetOrSet(DecoratedProduction item, int j, int i) {
			var tup = ValueTuple.Create(item, j, i);
			SppfBranch y;
			if (!_prodDict.TryGetValue(tup, out y)) {
				var newY = new SppfBranch(item, j, i);
				_prodDict[tup] = newY;
				y = newY;
			}
			return y;
		}
	}
}
