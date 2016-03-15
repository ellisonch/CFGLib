using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsolePlayground {
	internal static class DotRunner {
		public static void Run(string dot, string outputFilename) {
			File.WriteAllText(outputFilename, dot);

			// assumes dot.exe is on the path:
			var args = string.Format(@"{0} -Tpdf -O", outputFilename);
			var proc = System.Diagnostics.Process.Start("dot.exe", args);
			proc.WaitForExit();
		}
	}
}
