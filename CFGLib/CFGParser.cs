using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CFGLib {
	public class CFGParser {
		private static Regex _productionRegex = null;

		private static Regex ProductionRegex {
			get {
				if (_productionRegex == null) {
					var arrow = @"->";
					var variablePattern = @"[a-zA-Z_][a-zA-Z0-9_]*";
					var terminalPattern = @"[^']+";
					var lhsPattern = string.Format(@"<(?<lhs>{0})>", variablePattern);
					var rhsPattern = string.Format(@"(?:\s+(?:<(?<nt>{0})>|'(?<t>{1})'|ε))*", variablePattern, terminalPattern);
					var regexString = string.Format(@"^\s*{0}\s+{1}{2}\s*$", lhsPattern, arrow, rhsPattern);
					_productionRegex = new Regex(regexString);
				}
				return _productionRegex;
			}
		}

		// <X> -> <X> 'a' <X>
		public static BaseProduction Production(string s) {
			var match = ProductionRegex.Match(s);
			
			var lhsMatch = match.Groups["lhs"];
			var ntMatch = match.Groups["nt"];
			var tMatch = match.Groups["t"];
			
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
			Console.WriteLine(rhs);
			var lhs = Nonterminal.Of(lhsMatch.Value);

			var retval = new Production(lhs, rhs);
			return retval;
		}
	}
}
