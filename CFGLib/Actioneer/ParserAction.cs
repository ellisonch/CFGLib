using System;
using System.Collections.Generic;

namespace CFGLib.Actioneer {
	public interface IParserAction {
		object Act(TraverseResult[] args);
	}
	public class ParserAction : IParserAction {
		private Func<TraverseResult[], object> _action;

		public ParserAction(Func<TraverseResult[], object> p) {
			_action = p;
		}

		public object Act(TraverseResult[] args) {
			return _action(args);
		}

		object IParserAction.Act(TraverseResult[] args) {
			return Act(args);
		}
	}
}