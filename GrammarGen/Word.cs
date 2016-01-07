using System.Collections.Generic;

namespace GrammarGen {
	public interface Word {
		Sentence ProduceBy(Grammar grammar);
		bool IsVariable();
	}
}