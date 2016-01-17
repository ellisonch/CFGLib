using System.Collections.Generic;

namespace CFGLib {
	public interface Word {
		Sentence ProduceBy(BaseGrammar grammar);
		bool IsNonterminal();
	}
}