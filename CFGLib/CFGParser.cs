using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CFGLib {
	/// <summary>
	/// This class helps turn strings into grammars.
	/// </summary>
	public class CFGParser {
		private static Regex _productionRegex = null;

		private static void InitRegex() {
			var arrow = @"->";
			var variablePattern = @"[a-zA-Z_][a-zA-Z0-9_]*";
			var terminalPattern = @"[^']+";
			var lhsPattern = string.Format(@"<(?<lhs>{0})>", variablePattern);
			var rhsPattern = string.Format(@"(?:\s+(?:<(?<nt>{0})>|'(?<t>{1})'|ε))*", variablePattern, terminalPattern);
			var probabilityPattern = string.Format(@"(?:\s+\[(?<weight>[0-9]*\.?[0-9]*)\])?");
			var regexString = string.Format(@"^\s*{0}\s+{1}{2}{3}\s*$", lhsPattern, arrow, rhsPattern, probabilityPattern);
			_productionRegex = new Regex(regexString);
		}

		private static Regex ProductionRegex {
			get {
				if (_productionRegex == null) {
					InitRegex();
				}
				return _productionRegex;
			}
		}

		/// <summary>
		/// Turns a string like
		/// &lt;X> -> &lt;X> 'a' &lt;X>
		/// into a production.
		/// Nonterminals must be surrounded by angled brackets, terminals must be surrounded by single quotes, and everything must be separated by spaces.
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static BaseProduction Production(string s) {
			var match = ProductionRegex.Match(s);
			if (!match.Success) {
				throw new Exception("Didn't find valid string");
			}

			var lhsMatch = match.Groups["lhs"];
			var ntMatch = match.Groups["nt"];
			var tMatch = match.Groups["t"];
			var weightMatch = match.Groups["weight"];

			if (!lhsMatch.Success) {
				throw new Exception("Didn't find LHS");
			}

			double weight;
			if (!double.TryParse(weightMatch.Value, out weight)) {
				weight = 1.0;
			}
			
			var rhsList = new SortedList<int, Word>();

			foreach (Capture capture in ntMatch.Captures) {
				var word = Nonterminal.Of(capture.Value);
				rhsList.Add(capture.Index, word);
			}
			foreach (Capture capture in tMatch.Captures) {
				var word = Terminal.Of(capture.Value);
				rhsList.Add(capture.Index, word);
			}
			var rhs = new Sentence(rhsList.Values);
			var lhs = Nonterminal.Of(lhsMatch.Value);

			var retval = new Production(lhs, rhs, weight);
			return retval;
		}
	}
}
