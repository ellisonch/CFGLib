using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Forests.ForestVisitors {
	public class DefaultVisitor : IForestVisitor {
		private readonly ForestInternal _root;

		public DefaultVisitor(ForestInternal root) {
			if (root == null) {
				throw new ArgumentNullException();
			}
			_root = root;
		}

		protected ForestInternal Root {
			get {
				return _root;
			}
		}

		public virtual bool Visit(ForestLeaf node) {
			return true;
		}

		public virtual bool Visit(ForestInternal node) {
			foreach (var option in node.Options) {
				foreach (var children in option.Children()) {
					foreach (var child in children) {
						child.Accept(this);
					}
				}
			}
			return true;
		}
	}
}
