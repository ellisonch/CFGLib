using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib {
	public abstract class BaseProduction {
		private Nonterminal _lhs;
		private int _weight = 1;

		public bool IsEmpty {
			get { return this.Rhs.Count == 0; }
		}

		public Nonterminal Lhs {
			get { return _lhs; }
			protected set { _lhs = value; }
		}

		public int Weight {
			get { return _weight; }
			internal set { _weight = value; }
		}

		public abstract Sentence Rhs {
			get;
		}

		public bool IsSelfLoop {
			get {
				if (this.Rhs.Count != 1) {
					return false;
				}
				var rword = this.Rhs[0];
				return Lhs == rword;
			}
		}

		public override string ToString() {
			var lhss = this.Lhs.ToString();
			var rhss = this.Rhs.ToString();

			return lhss + " → " + rhss;
		}

		internal abstract BaseProduction Clone();
			// return new Production(_lhs, new Sentence(_rhs), _weight);
	}
}
