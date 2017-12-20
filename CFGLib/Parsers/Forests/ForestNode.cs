using CFGLib.Parsers.Forests.ForestVisitors;
using CFGLib.Parsers.Graphs;
using CFGLib.Parsers.Sppf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Forests {
	public abstract class ForestNode {
		public readonly int StartPosition;
		public readonly int EndPosition;
		
		public ForestNode(int startPosition, int endPosition) {
			StartPosition = startPosition;
			EndPosition = endPosition;
		}

		internal abstract string ToStringHelper(int level, HashSet<SppfNode2> visited);

		internal abstract string ToStringSelf();
		
		internal abstract bool Accept(IForestVisitor visitor);

		internal static ForestInternal SppfToForest(SppfNode2 internalSppf) {
			if (!(internalSppf is SppfWord sppfWord)) {
				throw new Exception();
			}
			if (!sppfWord.Word.IsNonterminal) {
				throw new Exception();
			}
			var nonterminal = (Nonterminal)sppfWord.Word;

			return new ForestInternal(internalSppf, nonterminal);
		}
	}
}
