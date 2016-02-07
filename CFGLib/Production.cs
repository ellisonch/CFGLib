using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib {
	/// <summary>
	/// This class represents an abstract Production
	/// </summary>
	public abstract class Production {
		private Nonterminal _lhs;
		private double _weight = 1.0;

		/// <summary>
		/// Returns a new production.
		/// We use a New() method because this class is abstract.
		/// </summary>
		public static Production New(Nonterminal lhs, Sentence rhs, double weight = 1.0) {
			return new DefaultProduction(lhs, rhs, weight);
		}

		/// <summary>
		/// The left-hand side of the Production (e.g., Lhs -> Rhs)
		/// </summary>
		public Nonterminal Lhs {
			get { return _lhs; }
			protected set { _lhs = value; }
		}

		/// <summary>
		/// The weight of the Production.  Weights are compared to the weights of other productions with the same Lhs to calculate Production probability.
		/// </summary>
		public double Weight {
			get { return _weight; }
			internal set {
				if (value < 0.0) {
					throw new ArgumentOutOfRangeException("Weights must be non-negative");
				}
				if (double.IsNaN(value)) {
					throw new ArgumentException("Weights need to be numbers");
				}
				_weight = value;
			}
		}

		/// <summary>
		/// The right-hand side of the Production (e.g., Lhs -> Rhs)
		/// </summary>
		public abstract Sentence Rhs {
			get;
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

		public override string ToString() {
			return string.Format("{0} → {1} [{2}]", this.Lhs, this.Rhs, this.Weight);
		}

		public string ToCodeString() {
			return string.Format("{0} → {1} [{2}]", this.Lhs, this.Rhs, this.Weight.ToString("R"));
		}

		/// <summary>
		/// Returns a new Production with constituent pieces equivalent to this Production.
		/// The Rhs is a new Sentence, so that any piece of the new Production can be changed without changing the old Production.
		/// </summary>
		/// <returns></returns>
		internal abstract Production DeepClone();

		/// <summary>
		/// Checks whether the productions have the same parts
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
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
	}
}
