using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextFreeGrammars {
	interface CNFProduction {
		Variable Lhs {
			get;
		}
		int Weight {
			get;
		}
	}
}
