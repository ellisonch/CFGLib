using CFGLib.ProductionAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib {
	/// <summary>
	/// This class represents a Production
	/// </summary>
	public class Production {
		private double _weight = 1.0;

		/// <summary>
		/// The left-hand side of the Production (e.g., Lhs -> Rhs)
		/// </summary>
		public Nonterminal Lhs { get; }

		/// <summary>
		/// The right-hand side of the Production (e.g., Lhs -> Rhs)
		/// </summary>
		public Sentence Rhs { get; }

		public Annotations Annotations {
			get;
		}

		private int[] _numNonterminalsBefore = null;
		public int NumNonterminalsBefore(int pos) {
			if (_numNonterminalsBefore == null) {
				_numNonterminalsBefore = new int[Rhs.Count];
				var count = 0;
				for (var i = 0; i < Rhs.Count; i++) {
					_numNonterminalsBefore[i] = count;
					if (Rhs[i].IsNonterminal) {
						count++;
					}
				}
			}
			return _numNonterminalsBefore[pos];
		}

		/// <summary>
		/// Returns a new production.
		/// </summary>
		public Production(Nonterminal lhs, Sentence rhs, double weight = 1.0, Annotations annotations = null) {
			if (lhs == null) {
				throw new ArgumentNullException("Lhs must be non-null");
			}
			if (rhs == null) {
				throw new ArgumentNullException("Rhs must be non-null");
			}
			this.Lhs = lhs;
			Rhs = rhs;
			this.Weight = weight;
			if (annotations == null) {
				annotations = Annotations.Empty;
			}
			Annotations = annotations;
		}

		public Production(string lhsName, Sentence rhs, double weight = 1.0) : this(Nonterminal.Of(lhsName), rhs, weight) {
		}
		public Production(Nonterminal lhs, Word rhsOnlyWord, double weight = 1.0) : this(lhs, new Sentence(rhsOnlyWord), weight) {
		}
		public Production(string lhsName, Word rhsOnlyWord, double weight = 1.0) : this(Nonterminal.Of(lhsName), new Sentence(rhsOnlyWord), weight) {
		}
		
		/// <summary>
		/// The weight of the Production.  Weights are compared to the weights of other productions with the same Lhs to calculate Production probability.
		/// </summary>
		public double Weight {
			get { return _weight; }
			internal set {
				if (double.IsNaN(value)) {
					throw new ArgumentException("Weight should be a number");
				}
				if (double.IsPositiveInfinity(value)) {
					throw new ArgumentException("Weight should be a number");
				}
				if (double.IsNegativeInfinity(value)) {
					throw new ArgumentException("Weight should be a number");
				}
				if (value < 0.0) {
					throw new ArgumentOutOfRangeException("Weights should be positive");
				}
				_weight = value;
			}
		}

		/// <summary>
		/// Is this a Production of the form X -> X?
		/// </summary>
		public bool IsSelfLoop {
			get {
				if (this.Rhs.Count != 1) {
					return false;
				}
				var rword = this.Rhs[0];
				return Lhs == rword;
			}
		}

		/// <summary>
		/// Is this a Production of the form X -> ε?
		/// </summary>
		public bool IsEmpty {
			get { return this.Rhs.Count == 0; }
		}

		public bool IsCnfNonterminal {
			get {
				return Rhs.Count == 2 && Rhs[0].IsNonterminal && Rhs[1].IsNonterminal;
			}
		}

		public bool IsCnfTerminal {
			get {
				return Rhs.Count == 1 && Rhs[0].IsTerminal;
			}
		}

		public override string ToString() {
			return string.Format("{0} → {1} [{2}]", this.Lhs, this.Rhs, this.Weight);
		}
		public string ToStringNoWeight() {
			return string.Format("{0} → {1}", this.Lhs, this.Rhs);
		}

		public string ToCodeString() {
			return string.Format("{0} → {1} [{2}]", this.Lhs, this.Rhs, this.Weight.ToString("R"));
		}

		/// <summary>
		/// Returns a new Production with constituent pieces equivalent to this Production.
		/// The Rhs is a new Sentence, so that any piece of the new Production can be changed without changing the old Production.
		/// </summary>
		internal Production DeepClone() {
			return new Production(this.Lhs, new Sentence(Rhs), this.Weight);
		}

		/// <summary>
		/// Checks whether the productions have the same parts
		/// </summary>
		public bool ValueEquals(Production other) {
			if (this.Lhs != other.Lhs) {
				return false;
			}
			if (!this.Rhs.SequenceEqual(other.Rhs)) {
				return false;
			}
			if (this.Weight != other.Weight) {
				return false;
			}
			return true;
		}

		internal bool IsUnit() {
			if (this.Rhs.Count != 1) {
				return false;
			}
			var rhs = this.Rhs[0];
			if (rhs.IsTerminal) {
				return false;
			}
			return true;
		}
	}
}
