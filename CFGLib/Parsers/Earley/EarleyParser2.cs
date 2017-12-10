using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CFGLib.Parsers.Forests;
using CFGLib.Actioneer;

namespace CFGLib.Parsers.Earley {
	public class EarleyParser2 : Parser {
		private readonly BaseGrammar _grammar;

		public EarleyParser2(BaseGrammar grammar) {
			_grammar = grammar;
		}

		public override ForestInternal ParseGetForest(Sentence s) {
			var sppf = ParseGetSppf(s);
			throw new NotImplementedException();
			// return new ForestInternal(sppf, sppf.Symbol);
		}

		public override double ParseGetProbability(Sentence s) {
			throw new NotImplementedException();
		}

		//public TraverseResultCollection Traverse(Sentence input) {
		//	var sppf = ParseGetSppf(input);

		//	var trav = new Traversal(sppf, input, gp);
		//	var resultList = trav.Traverse();
		//}

		// [Sec 5, ES2008]
		private SppfNode2 ParseGetSppf(Sentence a) {
			if (a.Count == 0) {
				throw new ArgumentException("Not sure how to handle empty yet");
			}
			var S = _grammar.Start;

			// E_0, ..., E_n, R, Q′, V = ∅
			EarleySet[] E = new EarleySet[a.Count + 1]; // need +1?
			for (var i = 0; i < E.Length; i++) {
				E[i] = new EarleySet();
			}
			EarleySet Q = new EarleySet();
			EarleySet QPrime = new EarleySet();
			EarleySet R = new EarleySet();
			var H = new Dictionary<Nonterminal, SppfNode2>();
			HashSet<SppfNode2> V = new HashSet<SppfNode2>();
			
			// for all (S ::= α) ∈ P {
			foreach (var production in _grammar.ProductionsFrom(S)) {
				var alpha = production.Rhs;
				var potentialItem = new EarleyItem(new DecoratedProduction(production, 0), 0, null);
				var potentialItem2 = new EarleyItem(new DecoratedProduction(production, 0), 0, null);

				// if α ∈ Σ_N, add (S ::= · α, 0, null) to E_0
				if (InSigma(alpha)) {
					E[0].Add(potentialItem);
				}

				// if α = a_1 α′, add(S::= · α, 0, null) to Q′
				// TODO: not sure how to handle this when a is ε
				var a1 = a.First();
				if (alpha.First() == a1) {
					QPrime.Add(potentialItem);
				}
			}

			// for 0 ≤ i ≤ n {
			for (var i = 0; i <= a.Count; i++) {
				// H = ∅, R = E_i , Q = Q ′
				H = new Dictionary<Nonterminal, SppfNode2>();
				R = new EarleySet(E[i]);
				Q = QPrime;

				// Q′ = ∅
				QPrime = new EarleySet();

				// while R ̸= ∅ {
				while (!R.IsEmpty) {
					// remove an element, Λ say, from R
					var Λ = R.TakeOne();
					// if Λ = (B ::= α · Cβ, h, w) {
					if (Λ.NextWord is Nonterminal C) {
						var β = Λ.Tail;
						// for all (C ::= δ) ∈ P {
						foreach (var production in _grammar.ProductionsFrom(C)) {
							// if δ ∈ Σ_N and (C ::= ·δ, i, null) ̸∈ E_i {
							var δ = production.Rhs;
							var newItem = new EarleyItem(new DecoratedProduction(production, 0), i, null);
							if (InSigma(δ)) {
								// add (C::= ·δ, i, null) to E_i and R }
								if (!E[i].Contains(newItem)) {
									E[i].Add(newItem);
									R.Add(newItem);
								}
							} else {
								// if δ = a_i+1 δ ′ { add (C ::= ·δ, i, null) to Q }
								var aNext = a[i + 1];
								if (δ.First() == aNext) {
									Q.Add(newItem);
								}
							}
						}

						// if ((C,v) ∈ H) {
						if (H.TryGetValue(C, out SppfNode2 v)) {
							// let y = MAKE_NODE(B ::= αC · β, h, i, w, v, V)

							var h = Λ.StartPosition;
							var w = Λ.SppfNode;
							var productionAdvanced = Λ.DecoratedProduction.Increment();
							var y = MakeNode(productionAdvanced, h, i, w, v, V);

							var newItem = new EarleyItem(productionAdvanced, h, y);
							// if β ∈ Σ N and (B ::= αC · β, h, y) ̸∈ E_i {
							if (InSigma(β)) {
								if (!E[i].Contains(newItem)) {
									// add(B::= αC · β, h, y) to E i and R }
									E[i].Add(newItem);
									R.Add(newItem);
								}
							} else {
								// if β = a_i+1 β ′ { add (B ::= αC · β,h,y) to Q } } }
								var aNext = a[i + 1];
								if (β.First() == aNext) {
									Q.Add(newItem);
								}
							}
						}
					}
					// if Λ = (D ::= α·, h, w) {
					else if (Λ.NextWord == null) {
						throw new NotImplementedException();
					} else {
						throw new Exception("Didn't expect a terminal");
					}
				}

				V = new HashSet<SppfNode2>();
				// create an SPPF node v labelled(a_i+1, i, i + 1)
				SppfNode2 v2 = null;
				throw new NotImplementedException();

				// while Q ̸= ∅ {
				while (!Q.IsEmpty) {
					// remove an element, Λ = (B ::= α · a_i+1 β, h, w) say, from Q
					var Λ = Q.TakeOne();
					if (Λ.NextWord != a[i + 1]) {
						throw new Exception();
					}
					var h = Λ.StartPosition;
					var w = Λ.SppfNode;
					var β = Λ.Tail;

					// let y = MAKE NODE(B ::= α a_i+1 · β, h, i + 1, w, v, V)
					var productionAdvanced = Λ.DecoratedProduction.Increment();
					var y = MakeNode(productionAdvanced, h, i + 1, w, v2, V);

					var newItem = new EarleyItem(productionAdvanced, h, y);
					// if β ∈ Σ_N { add (B ::= α a_i+1 · β, h, y) to E_i+1 }
					if (InSigma(β)) {
						E[i + 1].Add(newItem);
					}

					 // if β = a_i+2 β′ { add (B ::= α a_i+1 · β, h, y) to Q′ }
					else {
						var aNextNext = a[i + 2];
						if (β.First() == aNextNext) {
							QPrime.Add(newItem);
						}
					}
				}
			}

			// if (S ::= τ ·, 0, w) ∈ E_n return w
			// else return failure
			foreach (var item in E[a.Count]) {
				if (item.DecoratedProduction.Production.Lhs != S) {
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
			return null;
		}

		private SppfNode2 MakeNode(DecoratedProduction decoratedProduction, int j, int i, SppfNode2 w, SppfNode2 v, HashSet<SppfNode2> V) {
			throw new NotImplementedException();
		}

		private bool InSigma(Sentence alpha) {
			if (alpha.Count == 0) {
				return true;
			}
			if (alpha.First().IsNonterminal) {
				return true;
			}
			return false;
		}
	}
}
