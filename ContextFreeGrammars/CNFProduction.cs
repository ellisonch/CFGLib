using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextFreeGrammars {
	public interface CNFProduction {
		Variable Lhs {
			get;
		}
		int Weight {
			get;
			set;
		}
	}
}
