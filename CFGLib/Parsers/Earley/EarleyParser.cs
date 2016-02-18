using CFGLib.Parsers.Forests;
using System;
using System.Collections.Generic;
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
		
		public EarleyParser(BaseGrammar grammar) {
			_grammar = grammar;
		}

		public override double ParseGetProbability(Sentence s) {
			var successes = ComputeSuccesses(s);
			if (successes.Count == 0) {
				return 0.0;
			}

			var sppf = ConstructInternalSppf(successes, s);
			AnnotateWithProductions(sppf);

			var nodeProbs = new Dictionary<Node, double>();
			var prob = CalculateProbability(sppf, nodeProbs);
			return prob;
		}

		public override Sppf ParseGetForest(Sentence s) {
			var successes = ComputeSuccesses(s);
			if (successes.Count == 0) {
				return null;
			}

			var internalSppf = ConstructInternalSppf(successes, s);
			AnnotateWithProductions(internalSppf);

			//PrintForest(internalSppf, nodeProbs);
			//PrintDebugForest(internalSppf, s, nodeProbs);

			var sppf = internalSppf.ToSppf(s);
			return sppf;
		}

		private IList<Item> ComputeSuccesses(Sentence s) {
			var S = ComputeState(s);
			if (S == null) {
				return new List<Item>();
			}

			return GetSuccesses(S, s);
		}

		private StateSet[] ComputeState(Sentence s) {
			StateSet[] S = FreshS(s.Count + 1);

			// Initialize S(0)
			foreach (var production in _grammar.ProductionsFrom(_grammar.Start)) {
				var item = new Item(production, 0, 0, 0);
				S[0].Add(item);
			}

			// outer loop
			for (int stateIndex = 0; stateIndex < S.Length; stateIndex++) {
				var state = S[stateIndex];
				Terminal inputTerminal = null;
				if (stateIndex < s.Count) {
					inputTerminal = (Terminal)s[stateIndex];
				}

				// If there are no items in the current state, we're stuck
				if (state.Count == 0) {
					return null;
				}

				// completion
				var prevStateCount = 0;
				while (prevStateCount != state.Count) {
					var startIndex = prevStateCount;
					prevStateCount = state.Count;
					for (int itemIndex = 0; itemIndex < state.Count; itemIndex++) {
						var item = state[itemIndex];
						var nextWord = item.NextWord;
						if (nextWord == null) {
							// if item is new, we definitely want to run completion
							if (itemIndex >= startIndex) {
								Completion(S, stateIndex, item);
								// otherwise, we want to run only when we may have added a matching element to an existing completed item
							} else if (item.StartPosition == stateIndex) {
								Completion(S, stateIndex, item);
							}
						} else if (nextWord.IsNonterminal()) {
							if (itemIndex >= startIndex) {
								Prediction(S, stateIndex, (Nonterminal)nextWord, item);
							}
						} else {
							// Scan(S, stateIndex, item, (Terminal)nextWord, s, inputTerminal);
						}
					}
				}

				// initialization
				for (int itemIndex = 0; itemIndex < state.Count; itemIndex++) {
					var item = state[itemIndex];
					var nextWord = item.NextWord;
					if (nextWord == null) {
						//Completion(S, stateIndex, item);
					} else if (nextWord.IsNonterminal()) {
						//Prediction(S, stateIndex, (Nonterminal)nextWord, item);
					} else {
						Scan(S, stateIndex, item, (Terminal)nextWord, s, inputTerminal);
					}
				}
			}
			return S;
		}

		private double CalculateProbability(SymbolNode sppf, Dictionary<Node, double> nodeProbs) {
			var nodes = GetAllNodes(sppf);

			var indexToNode = nodes.ToArray();
			var nodeToIndex = new Dictionary<Node, int>();
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
					var estimate = StepProbability(node, nodeToIndex, previousEstimates);
					currentEstimates[i] = estimate;

					if (currentEstimates[i] > previousEstimates[i]) {
						throw new Exception("Didn't expect estimates to increase");
					} else if (currentEstimates[i] < previousEstimates[i]) {
						changed = true;
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

		private double StepProbability(Node node, Dictionary<Node, int> nodeToIndex, double[] previousEstimates) {
			if (node.Families.Count == 0) {
				return 1.0;
			}

			var l = node.FamiliesList;
			var familyProbs = new double[l.Count];
			for (int i = 0; i < l.Count; i++) {
				var alternative = l[i];
				
				double prob = GetChildProb(node, i);

				var childrenProbs = l[i].Members.Select((child) => previousEstimates[nodeToIndex[child]]).ToList();

				var childrenProb = childrenProbs.Aggregate(1.0, (p1, p2) => p1 * p2);

				familyProbs[i] = prob * childrenProb;
			}
			var familyProb = familyProbs.Sum();
			if (familyProb > 1) {
				familyProb = 1.0;
			}
			var result = familyProb;

			return result;
		}

		private double GetChildProb(Node node, int i) {
			var production = node.ChildProductions[i];
			var prob = 1.0;
			if (production != null) {
				prob = _grammar.GetProbability(production);
			}

			return prob;
		}

		private static HashSet<Node> GetAllNodes(SymbolNode sppf) {
			var nodes = new HashSet<Node>();
			var stack = new Stack<Node>();

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

		#region annotate
		//TODO this is so horribly terrible. There's got to be a better way of thinking about this structure
		private void AnnotateWithProductions(Node node, HashSet<Node> seen = null, Node parent = null, int place = 0) {
			if (seen == null) {
				seen = new HashSet<Node>();
			}

			if (node is IntermediateNode) {
				var intermediateNode = (IntermediateNode)node;
				var production = intermediateNode.Item.Production;
				if (intermediateNode.Item.CurrentPosition == production.Rhs.Count - 1) {
					parent.AddChild(place, production);
				}
			}
			
			if (seen.Contains(node)) {
				return;
			}
			seen.Add(node);

			var l = node.Families.ToList();
			node.FamiliesList = l;
			if (node.ChildProductions == null) {
				node.ChildProductions = new Production[l.Count];
			}
			for (int i = 0; i < l.Count; i++) {
				var alternative = l[i];

				var members = l[i].Members;
				if (members.Count == 1) {
					var child = members[0];

					AnnotateWithProductionsChildren(node, seen, child, i);
				} else if (members.Count == 2) {
					var left = members[0];
					var right = members[1];

					AnnotateWithProductionsChildren(node, seen, left, right, i);
				} else {
					throw new Exception("Should only be 0--2 children");
				}
			}
		}
		
		private void AnnotateWithProductionsChildren(Node parent, HashSet<Node> seen, Node child, int place) {
			Word parentSymbol = null;
			if (parent is SymbolNode) {
				var symbolParent = (SymbolNode)parent;
				parentSymbol = symbolParent.Symbol;
			} else {
				var intermediateParent = (IntermediateNode)parent;
				if (intermediateParent.Item.CurrentPosition != 1) {
					throw new Exception("Expected to be at beginning of item");
				}
				parentSymbol = intermediateParent.Item.Production.Rhs[0];
			}

			if (child is SymbolNode) {
				var symbolChild = (SymbolNode)child;
				if (symbolChild.Symbol.IsNonterminal()) {
					if (parent is SymbolNode) {
						var production = _grammar.FindProduction((Nonterminal)parentSymbol, new Sentence { symbolChild.Symbol });
						parent.AddChild(place, production);
					}
					AnnotateWithProductions(symbolChild, seen, parent, place);
					return;
				} else {
					if (parentSymbol.IsNonterminal()) {
						var production = _grammar.FindProduction((Nonterminal)parentSymbol, new Sentence { symbolChild.Symbol });
						parent.AddChild(place, production);
						return;
					} else {
						// this is like parent = x o x  with child x
						return;
					}
				}
			} else if (child is IntermediateNode) {
				throw new Exception("Don't handle intermediate");
			} else if (child is EpsilonNode) {
				var production = _grammar.FindProduction((Nonterminal)parentSymbol, new Sentence());
				parent.AddChild(place, production);
				return;
			}
			throw new Exception();
		}

		private void AnnotateWithProductionsChildren(Node parent, HashSet<Node> seen, Node left, Node right, int place) {
			if (!(left is IntermediateNode)) {
				throw new Exception();
			}
			if (!(right is SymbolNode)) {
				throw new Exception();
			}

			AnnotateWithProductions(left, seen, parent, place);
			AnnotateWithProductions(right, seen, parent, place);
		}
#endregion annotate

		private SymbolNode ConstructInternalSppf(IList<Item> successes, Sentence s) {
			var root = new SymbolNode(_grammar.Start, 0, s.Count);
			var processed = new HashSet<Item>();
			var nodes = new Dictionary<Node, Node>();
			nodes[root] = root;
			
			foreach (var success in successes) {
				BuildTree(nodes, processed, root, success);
			}

			return root;
		}

		private void PrintForest(Node node, Dictionary<Node, double> nodeProbs = null, string padding = "", HashSet<Node> seen = null) {
			if (seen == null) {
				seen = new HashSet<Node>();
			}

			var star = "";
			if (node.ChildProductions != null && node.ChildProductions.Length == 2 && node.ChildProductions.All((p) => p == null)) {
				star = "***";
			}

			var nodeProb = "";
			if (nodeProbs != null) {
				nodeProb = " p=" + nodeProbs[node];
			}

			Console.WriteLine("{0}{1}{2}{3}", padding, node, star, nodeProb);

			if (node.Families.Count > 0 && seen.Contains(node)) {
				Console.WriteLine("{0}Already seen this node!", padding);
				return;
			}
			seen.Add(node);

			List<Family> l;
			if (node.FamiliesList != null) {
				l = node.FamiliesList;
			} else {
				l = node.Families.ToList();
			}
			
			for (int i = 0; i < l.Count; i++) {
				var alternative = l[i];
				if (l.Count > 1) {
					Console.WriteLine("{0}Alternative {1}", padding, i);
				}
				foreach (var member in l[i].Members) {
					PrintForest(member, nodeProbs, padding + "  ", seen);
				}
			}
		}

		private void PrintDebugForest(Node node, Sentence s, Dictionary<Node, double> nodeProbs = null, string padding = "", HashSet<Node> seen = null) {
			if (seen == null) {
				seen = new HashSet<Node>();
			}
			
			double? nodeProb = null;
			if (nodeProbs != null) {
				nodeProb = nodeProbs[node];
			}

			string lhs = "";
			if (node is SymbolNode) {
				var symbol = (SymbolNode)node;
				lhs = symbol.Symbol.ToString();
			} else if (node is IntermediateNode) {
				var inter = (IntermediateNode)node;
				lhs = inter.Item.ProductionToString();
			} else if (node is EpsilonNode) {
				lhs = "ε";
			} else {
				throw new Exception();
			}
			string rhs = "";
			if (node is InteriorNode) {
				var interior = (InteriorNode)node;
				rhs = s.GetRange(interior.StartPosition, interior.EndPosition - interior.StartPosition).ToString();
			}

			Console.WriteLine("{0}{1} --> {2} [{4}]\t{3}", padding, lhs, rhs, nodeProb, node.ProductionsToString());

			if (node.Families.Count > 0 && seen.Contains(node)) {
				Console.WriteLine("{0}Already seen this node!", padding);
				return;
			}
			seen.Add(node);

			if (node.Families.Count == 0) {
				return;
			}
			var l = node.FamiliesList;
			for (int i = 0; i < l.Count; i++) {
				var alternative = l[i];
				if (l.Count > 1) {
					Console.WriteLine("{0}Alternative {1}", padding, i);
				}
				foreach (var member in l[i].Members) {
					PrintDebugForest(member, s, nodeProbs, padding + "  ", seen);
				}
			}
		}

		// [Sec 4, ES2008]
		private void BuildTree(Dictionary<Node, Node> nodes, HashSet<Item> processed, InteriorNode node, Item item) {
			// item.Processed = true;
			processed.Add(item);

			if (item.Production.Rhs.Count == 0) {
				var i = node.EndPosition;
				var v = NewOrExistingNode(nodes, new SymbolNode(item.Production.Lhs, i, i));
				//if there is no SPPF node v labelled (A, i, i)
				//create one with child node ϵ
				v.AddFamily(new Family(EpsilonNode.Node));
				// basically, SymbolNodes with no children have empty children
			} else if (item.CurrentPosition == 1) {
				var prevWord = item.PrevWord;
				if (prevWord.IsTerminal()) {
					var a = (Terminal)prevWord;
					var i = node.EndPosition;
					var v = NewOrExistingNode(nodes, new SymbolNode(a, i - 1, i));
					node.AddFamily(new Family(v));
				} else {
					var C = (Nonterminal)prevWord;
					var j = node.StartPosition;
					var i = node.EndPosition;
					var v = NewOrExistingNode(nodes, new SymbolNode(C, j, i));
					node.AddFamily(new Family(v));
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
			} else if (item.PrevWord.IsTerminal()) {
				var a = (Terminal)item.PrevWord;
				var j = node.StartPosition;
				var i = node.EndPosition;
				var v = NewOrExistingNode(nodes, new SymbolNode(a, i - 1, i));
				var w = NewOrExistingNode(nodes, new IntermediateNode(item.Decrement(), j, i - 1));
				foreach (var predecessor in item.Predecessors) {
					if (predecessor.Label != i - 1) {
						continue;
					}
					var pPrime = predecessor.Item;
					if (!processed.Contains(pPrime)) {
						BuildTree(nodes, processed, w, pPrime);
					}
				}

				node.AddFamily(new Family(w, v));
			} else {
				var C = (Nonterminal)item.PrevWord;
				foreach (var reduction in item.Reductions) {
					var l = reduction.Label;
					var q = reduction.Item;
					var j = node.StartPosition;
					var i = node.EndPosition;
					var v = NewOrExistingNode(nodes, new SymbolNode(C, l, i));
					if (!processed.Contains(q)) {
						BuildTree(nodes, processed, v, q);
					}
					var w = NewOrExistingNode(nodes, new IntermediateNode(item.Decrement(), j, l));
					foreach (var predecessor in item.Predecessors) {
						if (predecessor.Label != l) {
							continue;
						}
						var pPrime = predecessor.Item;
						if (!processed.Contains(pPrime)) {
							BuildTree(nodes, processed, w, pPrime);
						}
					}
					node.AddFamily(new Family(w, v));
				}
			}
		}

		private T NewOrExistingNode<T>(Dictionary<Node, Node> nodes, T node) where T : Node {
			Node existingNode;
			if (!nodes.TryGetValue(node, out existingNode)) {
				existingNode = node;
				nodes[node] = node;
			}
			node = (T)existingNode;
			return node;
		}

		private static StateSet[] FreshS(int length) {
			var S = new StateSet[length];

			// Initialize S
			for (int i = 0; i < S.Length; i++) {
				S[i] = new StateSet();
			}

			return S;
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
			var toAdd = new StateSet();
			foreach (var item in Si) {
				// make sure it's the same nonterminal
				if (item.NextWord != completedItem.Production.Lhs) {
					continue;
				}
				var newItem = item.Increment();
				newItem.AddReduction(completedItem.StartPosition, completedItem);
				if (item.CurrentPosition != 0) {
					newItem.AddPredecessor(completedItem.StartPosition, item);
				}
				toAdd.Add(newItem);
			}
			foreach (var item in toAdd) {
				state.InsertWithoutDuplicating(stateIndex, item);
			}
		}
		private void Prediction(StateSet[] S, int stateIndex, Nonterminal nonterminal, Item item) {
			var state = S[stateIndex];
			var productions = _grammar.ProductionsFrom(nonterminal);

			// insert, but avoid duplicates
			foreach (var production in productions) {
				var newItem = new Item(production, 0, stateIndex, stateIndex);
				state.InsertWithoutDuplicating(stateIndex, newItem);
			}

			// If the thing we're trying to produce is nullable, go ahead and eagerly derive epsilon. [AH2002]
			// Except this trick won't work for us, since we want probabilities and the full parse tree
			//if (_grammar.NullableProbabilities[nonterminal] > 0.0) {
			//	var newItem = item.Increment();
			//	// TODO: supposed to add pointers here, but don't know what to add
			//	InsertWithoutDuplicating(state, stateIndex, newItem);
			//}
		}
		
		private void Scan(StateSet[] S, int stateIndex, Item item, Terminal terminal, Sentence s, Terminal currentTerminal) {
			var state = S[stateIndex];

			StateSet nextState = null;
			if (stateIndex + 1 < S.Length) {
				nextState = S[stateIndex + 1];
			} else {
				return;
			}

			if (currentTerminal == terminal) {
				var newItem = item.Increment();
				if (item.CurrentPosition != 0) {
					newItem.AddPredecessor(stateIndex, item);
				}
				nextState.InsertWithoutDuplicating(stateIndex + 1, newItem);		
			}
		}
	}
}
