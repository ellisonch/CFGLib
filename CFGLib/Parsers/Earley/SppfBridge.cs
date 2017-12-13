using CFGLib.Parsers.Forests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {
	internal class SppfBridge {
		public static SymbolNode OldFromNew(SppfNode2 sppf) {
			if (sppf == null) {
				return null;
			}
			var retval = InternalOldFromNew(sppf, new Dictionary<SppfNode2, SppfNode>());

			if (retval is SymbolNode sn) {
				return sn;
			}
			throw new Exception();
		}
		private static SppfNode InternalOldFromNew(SppfNode2 sppf, Dictionary<SppfNode2, SppfNode> cache) {
			if (cache.TryGetValue(sppf, out SppfNode cached)) {
				return cached;
			}
			if (sppf is SppfWord sppfWord) {
				var retval = OldFromNewWord(sppfWord, cache);
				cache[sppf] = retval;
				return retval;
			} else if (sppf is SppfBranch sppfBranch) {
				var retval = OldFromNewProduction(sppfBranch, cache);
				cache[sppf] = retval;
				return retval;
			} else {
				throw new Exception();
			}
		}

		private static SppfNode OldFromNewWord(SppfWord sppf, Dictionary<SppfNode2, SppfNode> cache) {
			var word = sppf.Word;
			if (word.IsNonterminal) {
				InteriorNode retval;
				var inner = new SymbolNode(word as Nonterminal, sppf.StartPosition, sppf.EndPosition);
				//if (sppf.FakeProduction == null) {
					retval = inner;
				//} else {
				//	var item = new Item(sppf.FakeProduction, 1, inner.StartPosition);
				//	retval = new IntermediateNode(item, inner.StartPosition, inner.EndPosition);
				//	var family = new Family(inner);
				//	retval.Families.Add(family);
				//}
				cache[sppf] = inner;
				foreach (var family in sppf.Families) {
					AddFamily(inner, family, cache);
				}
				return retval;
			} else {
				SppfNode retval;
				var inner = new TerminalNode(word as Terminal, sppf.StartPosition,
					sppf.EndPosition);
				// if (sppf.FakeProduction == null) {
					retval = inner;
				//} else {
				//	var item = new Item(sppf.FakeProduction, 1, inner.StartPosition);
				//	retval = new IntermediateNode(item, inner.StartPosition, inner.EndPosition);
				//	var family = new Family(inner);
				//	retval.Families.Add(family);
				//}
				cache[sppf] = inner;
				return retval;
			}
		}
		private static SppfNode OldFromNewProduction(SppfBranch sppf, Dictionary<SppfNode2, SppfNode> cache) {
			var prod = sppf.DecoratedProduction;
			var newItem = new Item(prod.Production, prod.CurrentPosition, sppf.StartPosition);
			var retval = new IntermediateNode(newItem, sppf.StartPosition, sppf.EndPosition);
			cache[sppf] = retval;
			foreach (var family in sppf.Families) {
				AddFamily(retval, family, cache);
			}
			return retval;
		}

		private static void AddFamily(InteriorNode retval, Family2 family, Dictionary<SppfNode2, SppfNode> cache) {
			if (family.Children.Count == 0) {
				if (retval.StartPosition != retval.EndPosition) {
					throw new Exception();
				}
				var node = new EpsilonNode(retval.StartPosition, retval.EndPosition);
				var newFamily = new Family(node);
				retval.Families.Add(newFamily);
			} else if (family.Children.Count == 1) {
				var child1 = family.Children[0];

				var newChild1 = InternalOldFromNew(child1, cache);

				var newFamily = new Family(newChild1);
				retval.Families.Add(newFamily);
			} else if (family.Children.Count == 2) {
				var child1 = family.Children[0];
				var child2 = family.Children[1];

				var newChild1 = InternalOldFromNew(child1, cache);
				var newChild2 = InternalOldFromNew(child2, cache);

				// if (newChild1 is InteriorNode iNode) {
					var newFamily = new Family(newChild1, newChild2);
					retval.Families.Add(newFamily);
				//} else {
				//	throw new Exception();
				//}
			} else {
				throw new Exception();
			}
		}

		//private static SymbolNode OldFromNewProduction(SppfNode2 sppf) {
		//	throw new NotImplementedException();
		//}
	}
}
