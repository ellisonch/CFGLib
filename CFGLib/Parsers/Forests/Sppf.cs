using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Forests {
	public class Sppf {
		public List<Children> Families = new List<Children>();
		public Nonterminal Nonterminal;
		public Sentence Sentence;

		public Sppf(Nonterminal nonterminal, Sentence sentence) {
			this.Nonterminal = nonterminal;
			this.Sentence = sentence;
		}

		public override string ToString() {
			return ToStringHelper("", new HashSet<Sppf>());
		}

		internal string ToStringHelper(string padding, HashSet<Sppf> seen) {
			var result = "";
			result += string.Format("{0}{1} --> {2}\n", padding, Nonterminal, Sentence);
			if (seen.Contains(this)) {
				result += string.Format("{0}  This node was seen before\n", padding);
				return result;
			}
			seen.Add(this);

			// foreach (var children in Families) {
			for (int i = 0; i < Families.Count; i++) {
				var children = Families[i];
				if (Families.Count > 1) {
					result += string.Format("{0}Alternative {1}\n", padding, i);
				}
				result += children.ToStringHelper(padding + "  ", seen);
			}
			return result;
		}
	}
}
