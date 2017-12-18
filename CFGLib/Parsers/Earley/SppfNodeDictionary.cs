using CFGLib.Parsers.Sppf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {
	public class SppfNodeDictionary {
		// private Dictionary<ValueTuple<Word, int>, SppfWord>[] _wordDicts;
		private Dictionary<Word, SppfWord>[] _wordDicts;
		private Dictionary<ValueTuple<DecoratedProduction, int, int>, SppfBranch> _prodDict = new Dictionary<ValueTuple<DecoratedProduction, int, int>, SppfBranch>();

		public SppfNodeDictionary(int maxPos) {
			// _wordDicts = new Dictionary<ValueTuple<Word, int>, SppfWord>[currentI + 1];
			_wordDicts = new Dictionary<Word, SppfWord>[maxPos + 1];
			for (var i = 0; i < _wordDicts.Length; i++) {
				// _wordDicts[i] = new Dictionary<(Word, int), SppfWord>();
				_wordDicts[i] = new Dictionary<Word, SppfWord>();
			}
		}

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
			//if (j > _currenti) {
			//	throw new Exception();
			//}
			// var tup = ValueTuple.Create(item, j);
			SppfWord y;
			var dict = _wordDicts[j];
			if (!dict.TryGetValue(item, out y)) {
				var newY = new SppfWord(item, j, i);
				dict[item] = newY;
				y = newY;
			}
			//if (dict.Count > 1) {
			//	throw new Exception();
			//}
			if (i != y.EndPosition) {
				throw new Exception(string.Format("Invalid assumption; need to include {0} in hash", nameof(i)));
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
