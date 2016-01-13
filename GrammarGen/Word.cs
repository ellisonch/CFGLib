using System.Collections.Generic;

namespace GrammarGen {
	internal interface Word {
		Sentence ProduceBy(Grammar grammar);
		bool IsVariable();
	}
}