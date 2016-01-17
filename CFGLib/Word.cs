using System.Collections.Generic;

namespace CFGLib {
	public interface Word {
		Sentence ProduceBy(Grammar grammar);
		bool IsNonterminal();
	}
}