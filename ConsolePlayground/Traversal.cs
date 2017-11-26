using CFGLib;
using CFGLib.Parsers.Forests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsolePlayground {
	public class Traversal {
		private readonly ForestInternal _root;
		private readonly Sentence _input;
		private readonly Dictionary<Production, ParserAction> _actions;

		public Traversal(ForestInternal root, Sentence input, Dictionary<Production, ParserAction> actions) {
			_root = root;
			_input = input;
			_actions = actions;
		}

		public void Traverse() {
			Traverse(_root, 0);
		}

		private void Traverse(ForestNode node, int level) {
			var ni = node as ForestInternal;
			if (ni != null) {
				TraverseInternal(ni, level);
				return;
			}
			var nl = node as ForestLeaf;
			if (nl != null) {
				TraverseInternal(nl, level);
				return;
			}

			throw new ArgumentException(string.Format("Unhandled case {0}", node.GetType().Name));
		}

		private void TraverseInternal(ForestInternal node, int level) {
			var start = node.StartPosition;
			var length = node.EndPosition - node.StartPosition;
			var sub = _input.GetRange(start, length);

			var str = string.Format("Internal({0} ({1}, {2})) {3}", node.Nonterminal, node.StartPosition, node.EndPosition, sub).Indent(level);
			Console.WriteLine(str);

			foreach (var option in node.Options) {
				TraverseOption(option, level + 1);
			}
		}
		private void TraverseInternal(ForestLeaf leaf, int level) {
			var start = leaf.StartPosition;
			var length = leaf.EndPosition - leaf.StartPosition;
			var sub = _input.GetRange(start, length);

			var str = string.Format("Leaf({0}, {1}) {2}", leaf.StartPosition, leaf.EndPosition, sub).Indent(level);
			Console.WriteLine(str);
		}

		private void TraverseOption(ForestOption option, int level) {
			var str = string.Format("Option({0})", option.Production).Indent(level);
			Console.WriteLine(str);
			foreach (var childSet in option.Children()) {
				foreach (var child in childSet) {
					Traverse(child, level + 1);
				}
			}
		}
	}
}
