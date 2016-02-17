using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {
	public class Sppf {
		public List<Children> Families;
		public Nonterminal Nonterminal;
		public Sentence Sentence;

		public Sppf(Nonterminal nonterminal, Sentence sentence, List<Children> families) {
			this.Nonterminal = nonterminal;
			this.Sentence = sentence;
			this.Families = families;
		}
	}
}
