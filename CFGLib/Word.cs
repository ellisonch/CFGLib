using System.Collections.Generic;

namespace CFGLib {
	/// <summary>
	/// Words are the constituent pieces of Sentences.  They are either Terminals or Nonterminals
	/// </summary>
	public abstract class Word {
		public abstract bool IsNonterminal {
			get;
		}
		public bool IsTerminal {
			get { return !IsNonterminal; }
		}
	}
}