using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Forests {
	public class ForestInternal : ForestNode {
		private readonly InteriorNode _node;
		private readonly Nonterminal _nonterminal;

		//private readonly List<ForestLeaf> _leafChildren;
		private readonly List<ForestOption> _options = new List<ForestOption>();

		//public List<ForestLeaf> LeafChildren {
		//	get {
		//		return _leafChildren;
		//	}
		//}
		//public List<ForestOptions> SymbolChildren {
		//	get {
		//		return _symbolChildren;
		//	}
		//}
		public List<ForestOption> Options {
			get {
				return _options;
			}
		}

		public Nonterminal Nonterminal{
			get {
				return _nonterminal;
			}
		}

		internal ForestInternal(InteriorNode node, Nonterminal nonterminal) {
			_node = node;
			_nonterminal = nonterminal;

			_options = ForestOption.BuildOptions(node.Families);
		}

		public override string ToString() {
			return ToStringHelper(0);
		}
		internal override string ToStringHelper(int level) {

			var retval = "";

			retval += string.Format("{0}\n", Nonterminal).Indent(2 * level);
			// foreach (var option in Options) {
			for (var i = 0; i < Options.Count; i++) {
				var option = Options[i];
				if (Options.Count > 1) {
					retval += string.Format("Alternative {0}:\n", i).Indent(2 * level);
				}
				retval += option.ToStringHelper(level + 1);
			}
			
			//int leafIndex = 0;
			//int symbolIndex = 0;
			//for (int i = 0; i < _localSentence.Count; i++) {
			//	var word = _localSentence[i];
			//	if (word.IsTerminal) {
			//		var leaf = LeafChildren[leafIndex];
			//		leafIndex++;
			//		retval += leaf.ToStringHelper(level);
			//	} else {
			//		var symbol = SymbolChildren[symbolIndex];
			//		symbolIndex++;
			//		retval += symbol.ToStringHelper(level);
			//	}
			//}

			return retval;
		}
	}
}
