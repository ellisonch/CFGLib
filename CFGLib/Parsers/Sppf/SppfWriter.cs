using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Sppf {
	public class SppfWriter {
		public void PrintForest(SppfNode node, Dictionary<SppfNode, double> nodeProbs = null, string padding = "", HashSet<SppfNode> seen = null) {
			if (seen == null) {
				seen = new HashSet<SppfNode>();
			}

			var nodeProb = "";
			if (nodeProbs != null) {
				nodeProb = " p=" + nodeProbs[node];
			}

			Console.WriteLine("{0}{1}{2}", padding, node, nodeProb);

			var l = node.Families;
			var familiesCount = l.Count();

			if (familiesCount > 0 && seen.Contains(node)) {
				Console.WriteLine("{0}Already seen this node!", padding);
				return;
			}
			seen.Add(node);

			var i = 0;
			foreach (var alternative in l) {
				// for (int i = 0; i < l.Count; i++) {
				// var alternative = l[i];
				if (familiesCount > 1) {
					Console.WriteLine("{0}Alternative {1}", padding, i);
				}
				foreach (var member in alternative.Members) {
					PrintForest(member, nodeProbs, padding + "  ", seen);
				}
				i++;
			}
		}
	}
}
