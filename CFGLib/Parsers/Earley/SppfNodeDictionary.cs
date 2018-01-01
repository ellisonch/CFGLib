using CFGLib.Parsers.Sppf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {
	public class SppfNodeDictionary {
		private readonly Dictionary<Word, SppfWord>[] _wordDicts;
		private readonly Dictionary<ValueTuple<DecoratedProduction, int, int>, SppfBranch> _prodDict = new Dictionary<ValueTuple<DecoratedProduction, int, int>, SppfBranch>();

		public SppfNodeDictionary(int maxPos) {
			_wordDicts = new Dictionary<Word, SppfWord>[maxPos];
			for (var i = 0; i < _wordDicts.Length; i++) {
				_wordDicts[i] = new Dictionary<Word, SppfWord>();
			}
		}
		internal SppfWord GetOrSet(Word item, int j, int i) {
			SppfWord y;
			var dict = _wordDicts[j];
			if (!dict.TryGetValue(item, out y)) {
				var newY = new SppfWord(item, j, i);
				dict[item] = newY;
				y = newY;
			}
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

		internal void Clear(int max) {
			_prodDict.Clear();
			for (var i = 0; i <= max; i++) {
				_wordDicts[i].Clear();
			}
		}
	}
}
