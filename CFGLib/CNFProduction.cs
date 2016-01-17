using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib {
	public interface CNFProduction {
		Nonterminal Lhs {
			get;
		}
		int Weight {
			get;
			set;
		}
	}
}
