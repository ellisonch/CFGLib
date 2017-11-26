using System;
using System.Collections.Generic;

namespace ConsolePlayground {
	public class ParserAction {
		private Func<IList<string>, string> _action;

		public ParserAction(Func<IList<string>, string> p) {
			_action = p;
		}
	}
}