using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.ProductionAnnotations.Precedencing {
	public class PrecedenceAnnotation : IAnnotation {
		public double Value { get; }
		public PrecedenceAnnotation(double value) {
			Value = value;
		}
	}
}
