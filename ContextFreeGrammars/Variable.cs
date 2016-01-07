using System;
using System.Collections.Generic;

namespace ContextFreeGrammars {
	public class Variable : Word {
		private static Dictionary<string, Variable> _history = new Dictionary<string, Variable>();

		private readonly string _name;

		private Variable(string name) {
			_name = name;
		}

		public static Variable Of(string v) {
			Variable variable;
			if (!_history.TryGetValue(v, out variable)) {
				variable = new Variable(v);
				_history[v] = variable;
			}
			return variable;
		}

		public override string ToString() {
			return string.Format("Var({0})", _name);
		}

		public Sentence ProduceBy(Grammar grammar) {
			return grammar.ProduceVariable(this);
		}
		public bool IsVariable() {
			return true;
		}
	}
}