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
			var retval = InternalOldFromNew(sppf);

			if (retval is SymbolNode sn) {
				return sn;
			}
			throw new Exception();
		}
		private static SppfNode InternalOldFromNew(SppfNode2 sppf) {
			if (sppf.Word != null) {
				return OldFromNewWord(sppf);
			} else {
				throw new Exception();
			}
		}

		private static SymbolNode OldFromNewWord(SppfNode2 sppf) {
			var word = sppf.Word;
			if (word.IsNonterminal) {
				var retval = new SymbolNode(word as Nonterminal, sppf.StartPosition, sppf.EndPosition);
				foreach (var family in sppf.Families) {
					AddFamily(retval, family);
				}
				return retval;
			}
			throw new Exception();
		}

		private static void AddFamily(SymbolNode retval, Family2 family) {
			if (family.Children.Count == 0) {
				if (retval.StartPosition != retval.EndPosition) {
					throw new Exception();
				}
				var node = new EpsilonNode(retval.StartPosition, retval.EndPosition);
				var newFamily = new Family(node);
				retval.AddFamily(newFamily);
			} else if (family.Children.Count == 1) {
				var child1 = family.Children[0];

				var newChild1 = InternalOldFromNew(child1);

				var newFamily = new Family(newChild1);
				retval.AddFamily(newFamily);
			} else if (family.Children.Count == 2) {
				var child1 = family.Children[0];
				var child2 = family.Children[1];

				var newChild1 = InternalOldFromNew(child1);
				var newChild2 = InternalOldFromNew(child2);

				if (newChild1 is InteriorNode iNode) {
					var newFamily = new Family(iNode, newChild2);
					retval.AddFamily(newFamily);
				} else {
					throw new Exception();
				}
			} else {
				throw new Exception();
			}
		}

		//private static SymbolNode OldFromNewProduction(SppfNode2 sppf) {
		//	throw new NotImplementedException();
		//}
	}
}
