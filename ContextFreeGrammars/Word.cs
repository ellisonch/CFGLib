using System.Collections.Generic;

namespace ContextFreeGrammars {
	public interface Word {
		Sentence ProduceBy(Grammar grammar);
		bool IsVariable();
	}
}