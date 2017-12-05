using System;
using System.Collections.Generic;

namespace CFGLib.Actioneer {
	public interface IParserAction {
		object Act(TraverseResult[] args);
	}
	public class ParserAction<T> : IParserAction {
		private Func<TraverseResult[], T> _action;

		public ParserAction(Func<TraverseResult[], T> p) {
			_action = p;
		}

		public T Act(TraverseResult[] args) {
			return _action(args);
		}

		object IParserAction.Act(TraverseResult[] args) {
			return Act(args);
		}
	}
}