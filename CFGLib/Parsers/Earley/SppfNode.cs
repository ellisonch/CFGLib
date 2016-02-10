using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {
	// SPPF nodes are labeled with a triple (x,j,i) 
	// where a_{j+1} ... a_i is a substring matched by x
	internal class SppfNode {
		Sentence x;
		int j;
		int i;
	}
}
