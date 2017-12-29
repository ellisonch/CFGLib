using System;
using System.Collections.Generic;

namespace CFGLib.ProductionAnnotations.Actioning {
	public interface IParserAction {
		object Act(TraverseResult[] args);
	}
	public class ActionAnnotation : IParserAction, IAnnotation {
		private readonly Func<TraverseResult[], object> _action;

		public ActionAnnotation(Func<TraverseResult[], object> p) {
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