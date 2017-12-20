using CFGLib.Parsers.Forests;
using CFGLib.Parsers.Sppf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {

	/*
	Inspired by
	 * Elizabeth Scott's 2008 paper "SPPF-Style Parsing From Earley Recognisers" (http://dx.doi.org/10.1016/j.entcs.2008.03.044) [ES2008]
	 * John Aycock and Nigel Horspool's 2002 paper "Practical Earley Parsing" (http://dx.doi.org/10.1093/comjnl/45.6.620) [AH2002]
	 * Loup Vaillant's tutorial (http://loup-vaillant.fr/tutorials/earley-parsing/)
	*/

	public class EarleyParser : Parser {
		private readonly BaseGrammar _grammar;
		private static readonly double _probabilityChangePercentage = 1e-15;

		public EarleyParser(BaseGrammar grammar) {
			_grammar = grammar;
		}

		public override double ParseGetProbability(Sentence s) {
			var successes = ComputeSuccesses(s);
			if (successes.Count == 0) {
				return 0.0;
			}

			var internalSppf = ConstructInternalSppf(successes, s);

			return GetProbFromSppf(_grammar, internalSppf);
		}

		internal static double GetProbFromSppf(BaseGrammar _grammar, SppfNode2 internalSppf) {
			var nodeProbs = new Dictionary<SppfNode2, double>();
			var prob = CalculateProbability(_grammar, internalSppf, nodeProbs);

			//PrintForest(internalSppf, nodeProbs);
			//Console.WriteLine();
			//PrintDebugForest(internalSppf, s, nodeProbs);
			//var pp = new PrettyPrinter();
			//internalSppf.Accept(pp);
			//Console.WriteLine(pp.Result);

			return prob;
		}

		public override SppfNode2 ParseGetForest(Sentence s) {
			var successes = ComputeSuccesses(s);
			if (successes.Count == 0) {
				return null;
			}

			var internalSppf = ConstructInternalSppf(successes, s);
			// return SppfToForest(_grammar, internalSppf);
			return internalSppf;
		}

		/// <summary>
		/// Compute E_0 ... E_i as in [ES2008] Sec 4.0
		/// </summary>
		private IList<Item> ComputeSuccesses(Sentence s) {
			// this helps sometimes
			//var incomingTerminals = s.GetAllTerminals();
			//var parseableTerminals = _grammar.GetTerminals();
			//if (!incomingTerminals.IsSubsetOf(parseableTerminals)) {
			//	return new List<Item>();
			//}

			var S = ComputeState(s);
			if (S == null) {
				return new List<Item>();
			}

			return GetSuccesses(S, s);
		}
		private StateSet[] ComputeState(Sentence s) {
			StateSet[] S = new StateSet[s.Count + 1];

			// Initialize S(0)
			S[0] = new StateSet(_grammar.Start);
			foreach (var production in _grammar.ProductionsFrom(_grammar.Start)) {
				var item = new Item(production, 0, 0);
				S[0].Insert(item);
			}

			// outer loop
			for (int stateIndex = 0; stateIndex < S.Length; stateIndex++) {
				var stateprob = S[stateIndex];

				// If there are no items in the current state, we're stuck
				if (stateprob.Count == 0) {
					return null;
				}

				var nextIndex = stateIndex + 1;
				if (nextIndex < S.Length) {
					S[nextIndex] = new StateSet();
				}
				StepState(S, s, stateIndex, stateprob);
			}
			
			// for those items we added magically, make sure they get treated by completion
			for (int stateIndex = 0; stateIndex < S.Length; stateIndex++) {
				foreach (var p in S[stateIndex].MagicItems) {
					foreach (var t in S[stateIndex]) {
						if (t.StartPosition != stateIndex) {
							continue;
						}
						if (t.Production.Lhs != p.PrevWord) {
							continue;
						}
						if (!t.IsComplete()) {
							continue;
						}
						p.AddReduction(stateIndex, t);
					}
				}
			}

			return S;
		}

		private void StepState(StateSet[] S, Sentence s, int stateIndex, StateSet state) {
			Terminal inputTerminal = null;
			if (stateIndex < s.Count) {
				inputTerminal = (Terminal)s[stateIndex];
			}

			// completion + initialization
			for (int itemIndex = 0; itemIndex < state.Count; itemIndex++) {
				var item = state[itemIndex];
				var nextWord = item.NextWord;
				if (nextWord == null) {
					Completion(S, stateIndex, item);
				} else if (nextWord.IsNonterminal) {
					Prediction(S, stateIndex, (Nonterminal)nextWord, item);
				} else {
					Scan(S, stateIndex, item, (Terminal)nextWord, s, inputTerminal);
				}
			}
		}

		private static double CalculateProbability(BaseGrammar _grammar, SppfNode2 sppf, Dictionary<SppfNode2, double> nodeProbs) {
			var nodes = GetAllNodes(sppf);

			var indexToNode = nodes.ToArray();
			var nodeToIndex = new Dictionary<SppfNode2, int>(nodes.Count);
			for (int i = 0; i < indexToNode.Length; i++) {
				nodeToIndex[indexToNode[i]] = i;
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
		
		private static double StepProbability(BaseGrammar _grammar, SppfNode2 node, Dictionary<SppfNode2, int> nodeToIndex, double[] previousEstimates) {
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

		private static HashSet<SppfNode2> GetAllNodes(SppfNode2 sppf) {
			var nodes = new HashSet<SppfNode2>();
			var stack = new Stack<SppfNode2>();

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
		
		private SppfNode2 ConstructInternalSppf(IEnumerable<Item> successes, Sentence s) {
			// var root = new SymbolNode(_grammar.Start, 0, s.Count);
			var root = new SppfWord(_grammar.Start, 0, s.Count);
			var processed = new HashSet<Item>();
			var nodes = new Dictionary<SppfNode2, SppfNode2>();
			nodes[root] = root;
			
			foreach (var success in successes) {
				BuildTree(nodes, processed, root, success);
			}

			return root;
		}

		private void PrintForest(SppfNode2 node, Dictionary<SppfNode2, double> nodeProbs = null, string padding = "", HashSet<SppfNode2> seen = null) {
			if (seen == null) {
				seen = new HashSet<SppfNode2>();
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

			//if (node is IntermediateNode) {
			//	foreach (var family in node.Families) {
			//		if (family.Production != null) {
			//			// throw new Exception();
			//		}
			//	}
			//	if (node.Families.Count > 1) {

			//	}
			//}

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

		// [Sec 4, ES2008]
		private void BuildTree(Dictionary<SppfNode2, SppfNode2> nodes, HashSet<Item> processed, SppfNode2 node, Item item) {
			processed.Add(item);

			var production = item.IsComplete() ? item.Production : null;
			if (item.Production.Rhs.Count == 0) {
				var i = node.EndPosition;
				var v = NewOrExistingNode(nodes, new SppfWord(item.Production.Lhs, i, i));
				//if there is no SPPF node v labeled (A, i, i)
				//create one with child node ϵ
				v.AddFamily(production, new SppfEpsilon(i, i));
				// basically, SymbolNodes with no children have empty children
			} else if (item.CurrentPosition == 1) {
				var prevWord = item.PrevWord;
				if (prevWord.IsTerminal) {
					var a = (Terminal)prevWord;
					var i = node.EndPosition;
					var v = NewOrExistingNode(nodes, new SppfWord(a, i - 1, i));
					node.AddFamily(production, v);
				} else {
					var C = (Nonterminal)prevWord;
					var j = node.StartPosition;
					var i = node.EndPosition;
					var v = NewOrExistingNode(nodes, new SppfWord(C, j, i));
					node.AddFamily(production, v);
					foreach (var reduction in item.Reductions) {
						if (reduction.Label != j) {
							continue;
						}
						var q = reduction.Item;
						if (!processed.Contains(q)) {
							BuildTree(nodes, processed, v, q);
						}
					}
				}
			} else if (item.PrevWord.IsTerminal) {
				var a = (Terminal)item.PrevWord;
				var j = node.StartPosition;
				var i = node.EndPosition;
				var v = NewOrExistingNode(nodes, new SppfWord(a, i - 1, i));
				var dp = ItemToDecoratedProduction(item.Decrement());
				var w = NewOrExistingNode(nodes, new SppfBranch(dp, j, i - 1));
				foreach (var predecessor in item.Predecessors) {
					if (predecessor.Label != i - 1) {
						continue;
					}
					var pPrime = predecessor.Item;
					if (!processed.Contains(pPrime)) {
						BuildTree(nodes, processed, w, pPrime);
					}
				}

				node.AddFamily(production, w, v);
			} else {
				var C = (Nonterminal)item.PrevWord;
				foreach (var reduction in item.Reductions) {
					var l = reduction.Label;
					var q = reduction.Item;
					var j = node.StartPosition;
					var i = node.EndPosition;
					var v = NewOrExistingNode(nodes, new SppfWord(C, l, i));
					if (!processed.Contains(q)) {
						BuildTree(nodes, processed, v, q);
					}
					var dp = ItemToDecoratedProduction(item.Decrement());
					var w = NewOrExistingNode(nodes, new SppfBranch(dp, j, l));
					foreach (var predecessor in item.Predecessors) {
						if (predecessor.Label != l) {
							continue;
						}
						var pPrime = predecessor.Item;
						if (!processed.Contains(pPrime)) {
							BuildTree(nodes, processed, w, pPrime);
						}
					}
					node.AddFamily(production, w, v);
				}
			}
		}

		private DecoratedProduction ItemToDecoratedProduction(Item item) {
			return new DecoratedProduction(item.Production, item.CurrentPosition);
		}

		private T NewOrExistingNode<T>(Dictionary<SppfNode2, SppfNode2> nodes, T node) where T : SppfNode2 {
			SppfNode2 existingNode;
			if (!nodes.TryGetValue(node, out existingNode)) {
				existingNode = node;
				nodes[node] = node;
			}
			node = (T)existingNode;
			return node;
		}

		private IList<Item> GetSuccesses(StateSet[] S, Sentence s) {
			var successes = new List<Item>();
			var lastState = S[s.Count];
			foreach (Item item in lastState) {
				if (!item.IsComplete()) {
					continue;
				}
				if (item.StartPosition != 0) {
					continue;
				}
				if (item.Production.Lhs != _grammar.Start) {
					continue;
				}
				successes.Add(item);
			}
			return successes;
		}

		private void Completion(StateSet[] S, int stateIndex, Item completedItem) {
			var state = S[stateIndex];
			var Si = S[completedItem.StartPosition];
			var toAdd = new List<Item>();
			if (Si.Count > 20) {

			}

			/*
			For each item t = (B ::= τ ·, k) ∈ E_i and each corresponding item q = (D ::= τ · B µ, h) ∈ E_k, if there is no item p = (D ::= τ B · µ, h) ∈ E_i create one.
			Add a reduction pointer labelled k from p to t and, if τ ̸= ϵ, a predecessor pointer labelled k from p to q.

			completedItem is t
			Si is E_k
			item is q
			newItem is p
			*/

			foreach (var item in Si) {
				// make sure it's the same nonterminal
				if (item.NextWord != completedItem.Production.Lhs) {
					continue;
				}
				// for some reason, making sure it's the same prefix (tau) breaks everything.
				// this seems like a bug in [ES2008]
				// TODO: what about if tau2 isn't a suffix of tau1?
				// make sure it's the same prefix
				//var tau1 = completedItem.Production.Rhs.GetRange(0, completedItem.CurrentPosition);
				//var tau2 = item.Production.Rhs.GetRange(0, item.CurrentPosition);
				//if (!tau1.SequenceEqual(tau2)) {
				//	continue;
				//}
				//if (!IsSuffix(tau2, tau1)) {
				//	continue;
				//}
				//if (GatherExcludes(item, completedItem)) {
				//	continue;
				//}

				var newItem = item.Increment();
				newItem.AddReduction(completedItem.StartPosition, completedItem);
				if (item.CurrentPosition != 0) {
					newItem.AddPredecessor(completedItem.StartPosition, item);
				}
				toAdd.Add(newItem);
			}
			foreach (var item in toAdd) {
				state.InsertWithoutDuplicating(item);
			}
		}

		// Just for testing the S + S example
		private bool GatherExcludes(Item item, Item completedItem) {
			if (item.Production.Rhs.Count == 3) {
				if (item.CurrentPosition + 1 == 3) { // +1 since we're going to increment
					if (completedItem.Production.Rhs.Count == 3) {
						return true;
					}
				}
			}
			return false;
		}

		private bool IsSuffix(Sentence possibleSuffix, Sentence list) {
			if (list.Count < possibleSuffix.Count) {
				return false;
			}
			for (var i = 0; i < possibleSuffix.Count; i++) {
				var reverseIndex = list.Count - 1 - i;
				Debug.Assert(reverseIndex >= 0);
				Debug.Assert(reverseIndex < list.Count);

				if (list[reverseIndex] != possibleSuffix[reverseIndex]) {
					return false;
				}
			}
			return true;
		}

		private void Prediction(StateSet[] S, int stateIndex, Nonterminal nonterminal, Item item) {
			// From [ES2008] Sec 4.0.
			// For each item (B::= γ · D δ, k) ∈ E_i and each rule D ::= ρ, (D::= ·ρ, i) is added to E_i.
			
			var state = S[stateIndex];
			// check if we've already predicted this nonterminal in this state, if so, don't
			// this optimization may not always be faster, but should help when there are lots of productions or high ambiguity
			if (!state.PredictedAlreadyAndSet(nonterminal)) {
				var productions = _grammar.ProductionsFrom(nonterminal);

				// insert, but avoid duplicates
				foreach (var production in productions) {
					var newItem = new Item(production, 0, stateIndex);
					// state.InsertWithoutDuplicating(stateIndex, newItem);
					// with the above optimization,
					// prediction can never introduce a duplicate item
					// its current marker is always 0, while completion
					// and scan generate items with nonzero current markers
					state.Insert(newItem);
				}
			}
			// If the thing we're trying to produce is nullable, go ahead and eagerly derive epsilon. [AH2002]
			// Except this trick only works easily when we don't want the full parse tree
			// we save items generated this way to use in completion later
			var probabilityNull = _grammar.NullableProbabilities[nonterminal];
			if (probabilityNull > 0.0) {
				var newItem = item.Increment();
				if (item.CurrentPosition != 0) {
					newItem.AddPredecessor(stateIndex, item);
				}
				var actualNewItem = state.InsertWithoutDuplicating(newItem);
				state.MagicItems.Add(actualNewItem);
			}
		}
		
		private void Scan(StateSet[] S, int stateIndex, Item item, Terminal terminal, Sentence s, Terminal currentTerminal) {
			var state = S[stateIndex];

			if (stateIndex + 1 >= S.Length) {
				return;
			}
			StateSet nextState = S[stateIndex + 1];

			if (currentTerminal == terminal) {
				var newItem = item.Increment();
				if (item.CurrentPosition != 0) {
					newItem.AddPredecessor(stateIndex, item);
				}
				// Scan can never insert a duplicate because it adds items to the next
				// StateSet, but never adds them more than once
				nextState.Insert(newItem);		
			}
		}
	}
}
