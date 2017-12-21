using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grammars {
	public static class Addition {
		public static string Raw {
			get {
				return @"<S> ::= <S> '+' <S>
<S> ::= '1'
";
			}
		}
	}
}
