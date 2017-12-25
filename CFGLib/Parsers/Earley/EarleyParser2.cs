using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CFGLib.Actioneer;
using CFGLib.Parsers.Sppf;
using CFGLib.ProductionAnnotations.Gathering;

namespace CFGLib.Parsers.Earley {
	/// <summary>
	/// This parser is based directly on Section 5 of Elizabeth Scott's 2008 paper "SPPF-Style Parsing From Earley Recognisers" (http://dx.doi.org/10.1016/j.entcs.2008.03.044) [ES2008]
	/// </summary>
	public class EarleyParser2 : Parser {
		private readonly BaseGrammar _grammar;
		public static Stats _stats = new Stats();
		private readonly Nonterminal _S;

		private Sentence _a;
		private EarleySet[] _E;
		private EarleySet _QPrime = new EarleySet();

		public EarleyParser2(BaseGrammar grammar) {
			_grammar = grammar;

			_S = _grammar.Start;
		}
		
		public override SppfNode ParseGetForest(Sentence s) {
			var sppf = ParseGetSppf(s);
			return sppf;
		}

		public override double ParseGetProbability(Sentence s) {
			var sppf = ParseGetSppf(s);
			if (sppf == null) {
				return 0.0;
			}
			// var oldSppf = SppfBridge.OldFromNew(sppf);
			// EarleyParser.AnnotateWithProductions(_grammar, oldSppf);
			var prob = ProbabilityCalculator.GetProbFromSppf(_grammar, sppf);
			return prob;
		}

		//public TraverseResultCollection Traverse(Sentence input) {
		//	var sppf = ParseGetSppf(input);

		//	var trav = new Traversal(sppf, input, gp);
		//	var resultList = trav.Traverse();
		//}

		// [Sec 5, ES2008]
		// I've taken the liberty of adjusting the indices of a to start from 0
		private SppfNode ParseGetSppf(Sentence a) {
			//if (a.Count == 0) {
			//	throw new ArgumentException("Not sure how to handle empty yet");
			//}

			SppfNode._nextId = 0;

			_a = a;
			// E_0, ..., E_n, R, Q′, V = ∅
			_E = new EarleySet[a.Count + 1]; // need +1?
			for (var i = 0; i < _E.Length; i++) {
				_E[i] = new EarleySet();
			}

			Initialize();

			MainLoop();

			return Finish();
		}

		private SppfNode Finish() {
			// if (S ::= τ ·, 0, w) ∈ E_n return w
			foreach (var item in _E[_a.Count]) {
				if (item.DecoratedProduction.Production.Lhs != _S) {
					continue;
				}
				if (item.StartPosition != 0) {
					continue;
				}
				if (!item.DecoratedProduction.AtEnd) {
					continue;
				}
				return item.SppfNode;
				// TODO: could there be others?
			}
			// else return failure
			return null;
		}

		private void MainLoop() {
			var V = new SppfNodeDictionary(0);

			// for 0 ≤ i ≤ n {
			for (var i = 0; i < _E.Length; i++) {
				// H = ∅, R = E_i , Q = Q ′
				var H = new Dictionary<Nonterminal, SppfNode>();
				var R = new EarleySet(_E[i]);
				var Q = _QPrime;

				// Q′ = ∅
				_QPrime = new EarleySet();

				ProcessR(V, i, H, R, Q);

				V = ProcessQ(i, Q);
			}
		}

		private void ProcessR(SppfNodeDictionary V, int i, Dictionary<Nonterminal, SppfNode> H, EarleySet R, EarleySet Q) {
			// while R ̸= ∅ {
			while (!R.IsEmpty) {
				// remove an element, Λ say, from R
				var Λ = R.TakeOne();
				var nextWord = Λ.DecoratedProduction.NextWord;
				// if Λ = (B ::= α · Cβ, h, w) {
				if (nextWord is Nonterminal C) {
					Predict(V, i, H, R, Q, Λ, C);
				}
				// if Λ = (D ::= α·, h, w) {
				else if (nextWord == null) {
					Complete(V, i, H, R, Q, Λ);
				} else {
					throw new Exception("Didn't expect a terminal");
				}
			}
		}

		private SppfNodeDictionary ProcessQ(int i, EarleySet Q) {
			var V = new SppfNodeDictionary(i + 1);
			if (i < _a.Count) {
				// create an SPPF node v labelled(a_i, i, i + 1)
				var v2 = new SppfWord(_a[i], i, i + 1);

				// while Q ̸= ∅ {
				while (!Q.IsEmpty) {
					// remove an element, Λ = (B ::= α · a_i β, h, w) say, from Q
					// TODO: the statement above seems to imply all elements of Q have that form, but this doesn't seem to happen.  Skip them if they don't?
					var Λ = Q.TakeOne();
					if (Λ.DecoratedProduction.NextWord != _a[i]) {
						throw new Exception();
						// continue;
					}
					var h = Λ.StartPosition;
					var w = Λ.SppfNode;
					var β0 = Λ.DecoratedProduction.TailFirst;

					// let y = MAKE NODE(B ::= α a_i · β, h, i + 1, w, v, V)
					var productionAdvanced = Λ.DecoratedProduction.Increment();
					var y = MakeNode(productionAdvanced, h, i + 1, w, v2, V);

					var newItem = new EarleyItem(productionAdvanced, h, y);
					// if β ∈ Σ_N { add (B ::= α a_i · β, h, y) to E_i+1 }
					if (PrefixInSigma(β0)) {
						_E[i + 1].Add(newItem);
					}

					// if β = a_i+1 β′ { add (B ::= α a_i · β, h, y) to Q′ }
					else {
						if (i + 1 < _a.Count) {
							var aNext = _a[i + 1];
							if (β0 == aNext) {
								_QPrime.Add(newItem);
							}
						}
					}
				}
			}

			return V;
		}

		// if Λ = (B ::= α · Cβ, h, w) {
		private void Predict(SppfNodeDictionary V, int i, Dictionary<Nonterminal, SppfNode> H, EarleySet R, EarleySet Q, EarleyItem Λ, Nonterminal C) {
			//var β = Λ.Tail;
			var w = Λ.SppfNode;
			var h = Λ.StartPosition;
			var β0 = Λ.DecoratedProduction.TailFirst;
			// for all (C ::= δ) ∈ P {
			foreach (var production in _grammar.ProductionsFrom(C)) {
				// if δ ∈ Σ_N and (C ::= ·δ, i, null) ̸∈ E_i {
				var δ0 = production.Rhs.FirstOrDefault();
				var newItem = new EarleyItem(new DecoratedProduction(production, 0), i, null);
				if (PrefixInSigma(δ0)) {
					// add (C ::= ·δ, i, null) to E_i and R }
					if (_E[i].Add(newItem)) {
						R.Add(newItem);
					}
				} else {
					// if δ = a_i δ′ { add (C ::= · δ, i, null) to Q }
					if (i < _a.Count) {
						var aCurr = _a[i];
						if (δ0 == aCurr) {
							Q.Add(newItem);
						}
					}
				}
			}

			// if ((C, v) ∈ H) {
			if (H.TryGetValue(C, out SppfNode v)) {
				// let y = MAKE_NODE(B ::= αC · β, h, i, w, v, V)
				var productionAdvanced = Λ.DecoratedProduction.Increment();
				var y = MakeNode(productionAdvanced, h, i, w, v, V);

				var newItem = new EarleyItem(productionAdvanced, h, y);
				// if β ∈ Σ N and (B ::= αC · β, h, y) ̸∈ E_i {
				if (PrefixInSigma(β0)) {
					// add(B::= αC · β, h, y) to E_i and R }
					if (_E[i].Add(newItem)) {
						R.Add(newItem);
					}
				} else {
					// if β = a_i β′ { add (B ::= αC · β, h, y) to Q } } }
					if (i < _a.Count) {
						var aCurr = _a[i];
						if (β0 == aCurr) {
							Q.Add(newItem);
						}
					}
				}
			}
		}

		// if Λ = (D ::= α·, h, w) {
		private void Complete(SppfNodeDictionary V, int i, Dictionary<Nonterminal, SppfNode> H, EarleySet R, EarleySet Q, EarleyItem Λ) {
			var D = Λ.DecoratedProduction.Production.Lhs;
			var w = Λ.SppfNode;
			var h = Λ.StartPosition;

			// if w = null {
			if (w == null) {
				// if there is no node v ∈ V labelled (D, i, i) create one
				var v = V.GetOrSet(D, i, i);
				// set w = v
				w = v;
				Λ.SppfNode = v;

				// if w does not have family (ϵ) add one
				w.AddFamily(Λ.DecoratedProduction.Production, new SppfEpsilon(i, i));
			}

			// if h = i { add (D, w) to H }
			if (h == i) {
				if (H.TryGetValue(D, out var oldw)) {
					if (w != oldw) {
						throw new Exception("Different D, w in H");
					}
				} else {
					H[D] = w;
				}
			}

			var eh = _E[h];
			// for all (A ::= τ · Dδ, k, z) in E_h {
			for (var itemi = 0; itemi < eh.Count; itemi++) {
				var item = eh[itemi];
				if (item.DecoratedProduction.NextWord != D) {
					continue;
				}
				LinkCompletedChildWithParent(V, i, item, Λ, R, Q, w);
			}
		}

		private void LinkCompletedChildWithParent(SppfNodeDictionary V, int i, EarleyItem parent, EarleyItem child, EarleySet R, EarleySet Q, SppfNode w) {
			var k = parent.StartPosition;
			var z = parent.SppfNode;
			// var δ = item.Tail;
			var δ0 = parent.DecoratedProduction.TailFirst;

			// Λ is child, item is parent
			var gatherExcludes = GatherExcludes(parent, child);
			if (gatherExcludes) {
				return;
			}

			// let y = MAKE NODE(A ::= τD · δ, k, i, z, w, V)			
			var productionAdvanced = parent.DecoratedProduction.Increment();
			var y = MakeNode(productionAdvanced, k, i, z, w, V);
			var newItem = new EarleyItem(productionAdvanced, k, y);

			// if δ ∈ Σ_N and (A ::= τD · δ, k, y) ̸∈ E_i {
			if (PrefixInSigma(δ0)) {
				// add (A ::= τD · δ, k, y) to E_i and R
				if (_E[i].Add(newItem)) {
					R.Add(newItem);
				}
			} else {
				// if δ = a_i δ′ { add (A ::= τD · δ, k, y) to Q } }
				if (i < _a.Count) {
					var aCurr = _a[i];
					if (δ0 == aCurr) {
						Q.Add(newItem);
					}
				}
			}
		}

		private void Initialize() {
			// for all (S ::= α) ∈ P {
			foreach (var production in _grammar.ProductionsFrom(_S)) {
				var α0 = production.Rhs.FirstOrDefault();
				var potentialItem = new EarleyItem(new DecoratedProduction(production, 0), 0, null);

				// if α ∈ Σ_N, add (S ::= · α, 0, null) to E_0
				if (PrefixInSigma(α0)) {
					_E[0].Add(potentialItem);
				}
				// if α = a_0 α′, add (S ::= · α, 0, null) to Q′
				else {
					if (_a.Count > 0) {
						var a1 = _a.First();
						if (α0 == a1) {
							_QPrime.Add(potentialItem);
						}
					}
				}
			}
		}

		private bool GatherExcludes(EarleyItem parent, EarleyItem child) {
			var parentProduction = parent.DecoratedProduction.Production;
			var parentGathers = parentProduction.Annotations.Gather;
			if (parentGathers == null) {
				return false;
			}
			var parentPrecedence = parentProduction.Annotations.Precedence;
			if (parentPrecedence == null) {
				return false;
			}
			var childPrecedence = child.DecoratedProduction.Production.Annotations.Precedence;
			if (childPrecedence == null) {
				return false;
			}
			// S + * S [* is at 2]
			//var subSentence = parent.DecoratedProduction.Production.Rhs.Take(parent.DecoratedProduction.CurrentPosition);
			//var numNonterminals = subSentence.Count(word => word.IsNonterminal);
			var numNonterminals = parentProduction.NumNonterminalsBefore(parent.DecoratedProduction.CurrentPosition);
			var parentGather = parentGathers.Value[numNonterminals];
			switch (parentGather) {
				case GatherOption.SameOrLower:
					if (childPrecedence.Value > parentPrecedence.Value) {
						return true;
					}
					break;
				case GatherOption.StrictlyLower:
					if (childPrecedence.Value >= parentPrecedence.Value) {
						return true;
					}
					break;
			}
			
			return false;
		}

		// MAKE_NODE(B ::= αx · β, j, i, w, v, V) {
		private static SppfNode MakeNode(DecoratedProduction decoratedProduction, int j, int i, SppfNode w, SppfNode v, SppfNodeDictionary V) {
			SppfNode y;
			Production production = null;
			// if β = ϵ { let s = B } else { let s = (B ::= αx · β) }
			if (decoratedProduction.AtEnd) {
				// s = ValueTuple.Create<Word, DecoratedProduction>(decoratedProduction.Production.Lhs, null);
				y = V.GetOrSet(decoratedProduction.Production.Lhs, j, i);
				production = decoratedProduction.Production;
			} else {
				// s = ValueTuple.Create<Word, DecoratedProduction>(null, decoratedProduction);
				y = V.GetOrSet(decoratedProduction, j, i);
			}

			//SppfNode2 y;
			//// if α = ϵ and β ̸= ϵ { let y = v }
			//// TODO: got rid of contraction to make it easy to run old code
			////if (α.Count == 0 && β.Count != 0) {
			//if (false) {
			//	//y = v;

			//	//var tup = new Tuple<Word, DecoratedProduction>(null, decoratedProduction);
			//	//y = new SppfNode2(tup, j, i)
			//	//y = v;
			//	//if (y.FakeProduction != null) {
			//	//	if (y.FakeProduction != decoratedProduction.Production) {
			//	//		throw new Exception("Different production for contracted node");
			//	//	}
			//	//}
			//	//y.FakeProduction = decoratedProduction.Production;
			//} else {
			//	// if there is no node y ∈ V labelled (s,j,i) create one and add it to V
			//	// var tup = ValueTuple.Create(s, j, i);

			//	if (s.Item1 != null) {
			//		y = V.GetOrSet(s.Item1, j, i);
			//	} else {
			//		y = V.GetOrSet(s.Item2, j, i);
			//	}

			//	//if (!V.TryGetValue(tup, out y)) {
			//	//	SppfNode2 newY;
			//	//	if (tup.Item1.Item1 != null) {
			//	//		newY = new SppfWord(tup.Item1.Item1, j, i);
			//	//	} else {
			//	//		newY = new SppfBranch(tup.Item1.Item2, j, i);
			//	//	}
			//	//	V[tup] = newY;
			//	//	y = newY;
			//	//}

			//}

			// if w = null and y does not have a family of children (v) add one
			if (w == null) {
				y.AddFamily(production, v);
			}
			// if w ̸= null and y does not have a family of children(w, v) add one
			else {
				y.AddFamily(production, w, v);
			}

			return y;
		}

		private bool PrefixInSigma(Word alpha) {
			if (alpha == null) {
				return true;
			}
			if (alpha.IsNonterminal) {
				return true;
			}
			return false;
		}
	}
}
