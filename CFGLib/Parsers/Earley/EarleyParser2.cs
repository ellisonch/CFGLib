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
			return new ForestInternal(sppf, sppf.Symbol);
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
		private SymbolNode ParseGetSppf(Sentence a) {
			if (a.Count == 0) {
				throw new ArgumentException("Not sure how to handle empty yet");
			}
			// E_0, ..., E_n, R, Q′, V = ∅
			EarleySet[] E = new EarleySet[a.Count + 1]; // need +1?
			for (var i = 0; i < E.Length; i++) {
				E[i] = new EarleySet();
			}
			EarleySet Q = new EarleySet();
			EarleySet QPrime = new EarleySet();
			EarleySet R = new EarleySet();
			EarleySet H = new EarleySet();
			HashSet<SppfNode2> V = new HashSet<SppfNode2>();


			//for all (S ::= α) ∈ P {
			//	if α ∈ Σ_N, add (S ::= · α, 0, null) to E_0
			//	if α = a_1 α′, add (S ::= · α, 0, null) to Q′
			//}
			foreach (var production in _grammar.Productions) {
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
				H = new EarleySet();
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
						// for all (C ::= δ) ∈ P {
						foreach (var production in _grammar.ProductionsFrom(C)) {
							// if δ ∈ Σ N and (C ::= ·δ,i,null) ̸∈ E i {				
							if (InSigma(production.Rhs)) {
								// add(C::= ·δ, i, null) to E_i and R }
								var newItem = new EarleyItem(new DecoratedProduction(production, 0), i, null);
								if (!E[i].Contains(newItem)) {
									E[i].Add(newItem);
									R.Add(newItem);
								}
							}
							// if δ = a i + 1 δ ′ { add(C::= ·δ, i, null) to Q }
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
			}

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
