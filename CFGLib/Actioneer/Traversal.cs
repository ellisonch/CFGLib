using CFGLib;
using CFGLib.Parsers.Forests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Actioneer {
	public class Traversal {
		private readonly ForestInternal _forestRoot;
		private readonly InteriorNode _root;
		private readonly Sentence _input;
		private readonly Dictionary<Production, ParserAction> _actions;

		public Traversal(ForestInternal root, Sentence input, Dictionary<Production, ParserAction> actions) {
			_forestRoot = root;
			_root = root.InternalNode;
			_input = input;
			_actions = actions;
		}

		public void Traverse() {
			Traverse(_root, 0);
		}

		private void Traverse(SppfNode node, int level) {
			var ni = node as InteriorNode;
			if (ni != null) {
				TraverseInternal(ni, level);
				return;
			}
			//var nl = node as ForestLeaf;
			//if (nl != null) {
			//	TraverseInternal(nl, level);
			//	return;
			//}

			throw new ArgumentException(string.Format("Unhandled case {0}", node.GetType().Name));
		}

		private void TraverseInternal(InteriorNode node, int level) {
			var start = node.StartPosition;
			var length = node.EndPosition - node.StartPosition;
			var sub = _input.GetRange(start, length);
			
			var str = string.Format("Internal(({1}, {2})) {3}", 0, node.StartPosition, node.EndPosition, sub).Indent(level);
			Console.WriteLine(str);

			//foreach (var option in node.Options) {
			//	TraverseOption(option, level + 1);
			//}
		}
		//private void TraverseInternal(ForestLeaf leaf, int level) {
		//	var start = leaf.StartPosition;
		//	var length = leaf.EndPosition - leaf.StartPosition;
		//	var sub = _input.GetRange(start, length);

		//	var str = string.Format("Leaf({0}, {1}) {2}", leaf.StartPosition, leaf.EndPosition, sub).Indent(level);
		//	Console.WriteLine(str);
		//}

		//private void TraverseOption(ForestOption option, int level) {
		//	ParserAction action;
		//	if (!_actions.TryGetValue(option.Production, out action)) {
		//		action = new ParserAction((strList) => "");
		//	}
		//	var str = string.Format("Option({0})", option.Production).Indent(level);
		//	Console.WriteLine(str);
		//	foreach (var childSet in option.Children()) {
		//		if (childSet.Length == 1) {
		//			TraverseChildSingle(childSet[0], level + 1);
		//		} else if (childSet.Length == 2) {
		//			TraverseChildDouble((ForestInternal)childSet[0], childSet[1], level + 1);
		//		} else {
		//			throw new Exception();
		//		}
		//		//foreach (var child in childSet) {
		//		// Traverse(child, level + 1);
		//		//}
		//	}
		//}

		//private void TraverseChildDouble(ForestInternal node1, ForestNode node2, int level) {
		//	Traverse(node1, level + 1);
		//	Traverse(node2, level + 1);
		//}

		//private void TraverseChildSingle(ForestNode node, int level) {
		//	Traverse(node, level + 1);
		//}
	}
}
