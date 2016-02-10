using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.CYKNS {
	internal struct LocalCykProduction {
		public int A;
		public int B;
		public int C;
		public double Probability;
		public LocalCykProduction(int A, int B, int C, double Probability) {
			this.A = A;
			this.B = B;
			this.C = C;
			this.Probability = Probability;
		}
	}
}
