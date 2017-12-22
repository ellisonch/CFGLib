using CFGLib.Parsers.Sppf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {
	internal static class ProbabilityCalculator {
		private static readonly double _probabilityChangePercentage = 1e-15;

		private static HashSet<SppfNode> GetAllNodes(SppfNode sppf) {
			var nodes = new HashSet<SppfNode>();
			var stack = new Stack<SppfNode>();

			stack.Push(sppf);
			while (stack.Count > 0) {
				var node = stack.Pop();
				if (nodes.Contains(node)) {
					continue;
				}
				nodes.Add(node);

				foreach (var family in node.Families) {
					foreach (var child in family.Members) {
						stack.Push(child);
					}
				}
			}

			return nodes;
		}

		public static double GetProbFromSppf(BaseGrammar _grammar, SppfNode internalSppf) {
			var nodeProbs = new Dictionary<SppfNode, double>();
			var prob = CalculateProbability(_grammar, internalSppf, nodeProbs);

			return prob;
		}

		private static double CalculateProbability(BaseGrammar _grammar, SppfNode sppf, Dictionary<SppfNode, double> nodeProbs) {
			var nodes = GetAllNodes(sppf);

			var indexToNode = nodes.ToArray();
			var nodeToIndex = new Dictionary<SppfNode, int>(nodes.Count);
			for (int i = 0; i < indexToNode.Length; i++) {
				var node = indexToNode[i];
				nodeToIndex[node] = i;
			}

			var previousEstimates = Enumerable.Repeat(1.0, indexToNode.Length).ToArray();
			var currentEstimates = new double[indexToNode.Length];

			//for (var i = 0; i < indexToNode.Length; i++) {
			//	Console.WriteLine("{0,-40}: {1}", indexToNode[i], previousEstimates[i]);
			//}

			bool changed = true;
			while (changed == true) {
				changed = false;

				Array.Clear(currentEstimates, 0, currentEstimates.Length);

				for (var i = 0; i < indexToNode.Length; i++) {
					var node = indexToNode[i];
					var estimate = StepProbability(_grammar, node, nodeToIndex, previousEstimates);
					currentEstimates[i] = estimate;

					if (currentEstimates[i] > previousEstimates[i]) {
						throw new Exception("Didn't expect estimates to increase");
					} else if (currentEstimates[i] < previousEstimates[i]) {
						var diff = previousEstimates[i] - currentEstimates[i];
						var tolerance = _probabilityChangePercentage * previousEstimates[i];
						if (diff > _probabilityChangePercentage) {
							changed = true;
						}
					}
				}

				//Console.WriteLine("--------------------------");
				//for (var i = 0; i < indexToNode.Length; i++) {
				//	Console.WriteLine("{0,-40}: {1}", indexToNode[i], currentEstimates[i]);
				//}

				Helpers.Swap(ref previousEstimates, ref currentEstimates);
			}

			for (var i = 0; i < indexToNode.Length; i++) {
				nodeProbs[indexToNode[i]] = currentEstimates[i];
			}

			return currentEstimates[nodeToIndex[sppf]];
		}

		private static double StepProbability(BaseGrammar _grammar, SppfNode node, Dictionary<SppfNode, int> nodeToIndex, double[] previousEstimates) {
			var l = node.Families;
			var familyCount = l.Count();

			if (familyCount == 0) {
				return 1.0;
			}

			var familyProbs = new double[familyCount];
			var i = 0;
			foreach (var alternative in l) {
				// for (int i = 0; i < familyCount; i++) {
				// var alternative = l[i];

				double prob = GetChildProb(_grammar, alternative.Production);

				//var childrenProbs = l[i].Members.Select((child) => previousEstimates[nodeToIndex[child]]);
				//var childrenProb = childrenProbs.Aggregate(1.0, (p1, p2) => p1 * p2);
				var childrenProb = 1.0;
				foreach (var child in alternative.Members) {
					var index = nodeToIndex[child];
					var estimate = previousEstimates[index];
					childrenProb *= estimate;
				}

				familyProbs[i] = prob * childrenProb;

				i++;
			}
			var familyProb = familyProbs.Sum();
			if (familyProb > 1) {
				familyProb = 1.0;
			}
			var result = familyProb;

			return result;
		}

		private static double GetChildProb(BaseGrammar _grammar, Production production) {
			// var production = alternative.Production;
			var prob = 1.0;
			if (production != null) {
				prob = _grammar.GetProbability(production);
			}

			return prob;
		}
	}
}
