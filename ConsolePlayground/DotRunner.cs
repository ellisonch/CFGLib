using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsolePlayground {
	internal static class DotRunner {
		public static void Run(string dot, string baseFilename) {
			var dotFilename = string.Format("{0}.dot", baseFilename);
			File.WriteAllText(dotFilename, dot);

			// assumes dot.exe is on the path:
			// var args = string.Format(@"{0} -Tpdf -O", baseOutputFilename);
			var args = string.Format(@"{0} -Tpdf -o{1}.pdf", dotFilename, baseFilename);

			var proc = new Process() {
				StartInfo = new ProcessStartInfo("dot.exe", args) {
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					UseShellExecute = false,
				}
			};
			if (!proc.Start()) {
				throw new Exception();
			}

			// var proc = System.Diagnostics.Process.Start(, args);
			proc.WaitForExit();
			if (proc.ExitCode != 0) {
				var stdout = proc.StandardOutput.ReadToEnd();
				var stderr = proc.StandardError.ReadToEnd();
				throw new Exception();
			}
		}
	}
}
