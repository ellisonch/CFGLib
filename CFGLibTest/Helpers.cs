using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLibTest {
	internal class Helpers {
		public static void AssertNear(double a, double b) {
			double tolerance = Math.Abs(a * 0.00001);
			double diff = Math.Abs(a - b);
			if (diff <= tolerance) {
				return;
			}
			Assert.Fail(string.Format("{0} is not near {1}", b, a));
		}
	}
}
