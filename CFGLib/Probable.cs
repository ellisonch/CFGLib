using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib {
	/// <summary>
	/// Represents an item, together with an associated probability
	/// </summary>
	public class Probable<T> {
		public double Probability;
		public T Value;

		public Probable(double v, T s) {
			this.Probability = v;
			this.Value = s;
		}
		public override string ToString() {
			return string.Format("{0}: {1}", Probability, Value);
		}
	}
}
