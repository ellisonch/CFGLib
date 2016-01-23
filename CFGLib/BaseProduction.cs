using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib {
	/// <summary>
	/// This class represents an abstract Production
	/// </summary>
	public abstract class BaseProduction {
		private Nonterminal _lhs;
		private int _weight = 1;
		
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
		public int Weight {
			get { return _weight; }
			internal set {
				if (value < 1) {
					throw new ArgumentOutOfRangeException("Weights must be positive");
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
			var lhss = this.Lhs.ToString();
			var rhss = this.Rhs.ToString();

			return lhss + " → " + rhss;
		}
		
		/// <summary>
		/// Returns a new Production with constituent pieces equivalent to this Production.
		/// The Rhs is a new Sentence, so that any piece of the new Production can be changed without changing the old Production.
		/// </summary>
		/// <returns></returns>
		internal abstract BaseProduction DeepClone();
	}
}
