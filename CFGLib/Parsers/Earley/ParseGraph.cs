using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {
	internal struct ParseGoal {
		public readonly Item Item;
		public readonly int ItemOffset;
		public readonly int SentenceOffset;
		public readonly List<ParseGoal> Parents;
		public readonly List<ParseGoal> Children;

		public ParseGoal(Item item, int itemOffset, int sentenceOffset) {
			Item = item;
			ItemOffset = itemOffset;
			SentenceOffset = sentenceOffset;
			Parents = new List<ParseGoal>();
			Children = new List<ParseGoal>();
		}
		public override string ToString() {
			string retval = "";
			retval += string.Format("Goal({0}, {1}, {2})", Item, ItemOffset, SentenceOffset);
			return retval;
		}
		public string ToString(Sentence s) {
			string retval = "";
			var s1 = s.GetRange(0, SentenceOffset).AsTerminals();
			var s2 = s.GetRange(SentenceOffset, s.Count - SentenceOffset).AsTerminals();
			retval += string.Format("Goal({0}, {3}, \"{1}\" | \"{2}\")", Item, s1, s2, ItemOffset);
			return retval;
		}
	}

	internal class ParseGoalComparer : IEqualityComparer<ParseGoal> {
		public bool Equals(ParseGoal left, ParseGoal right) {
			return left.Item == right.Item
				&& left.ItemOffset == right.ItemOffset
				&& left.SentenceOffset == right.SentenceOffset
			;
		}

		public int GetHashCode(ParseGoal item) {
			return new { item.Item, item.ItemOffset, item.SentenceOffset }.GetHashCode();
		}
	}

	internal class ParseGraph {
		private readonly StateSet[] S;
		private readonly Sentence s;

		public ParseGraph(StateSet[] S, Sentence s) {
			this.S = S;
			this.s = s;
		}

		public void DFS(Item item) {
			// var offset = 0;
			Console.WriteLine("Looking for path for {0}", item);
			// FindPaths(reversedS, success, offset, s);

			var stack = new Stack<ParseGoal>();
			var initialGoal = new ParseGoal(item, 0, 0);
			stack.Push(initialGoal);
			var seenGoals = new HashSet<ParseGoal>(new ParseGoalComparer());

			while (stack.Count != 0) {
				var goal = stack.Pop();

				// If we've already handled this once, don't need to handle it again
				if (seenGoals.Contains(goal)) {
					Console.WriteLine("Already seen {0}", goal);
					continue;
				}
				seenGoals.Add(goal);

				Console.WriteLine("Seeking {0}", goal.ToString(s));

				// if we make it to the end, success!
				if (goal.ItemOffset >= goal.Item.Production.Rhs.Count) {
					Console.WriteLine("Hit the end of {0}!", goal);
					AddToParents(goal);
					continue;
				}

				var word = goal.Item.Production.Rhs[goal.ItemOffset];
				ParseGoal? nextGoal = null;
				if (word.IsNonterminal) {
					var nonterminal = (Nonterminal)word;
					var candidates = FindMatching(goal.SentenceOffset, nonterminal);
					foreach (var candidate in candidates) {
						Console.WriteLine("Considering {0}", candidate);
						var newv = new ParseGoal(candidate, 0, goal.SentenceOffset);
						stack.Push(newv);
						newv.Parents.Add(goal);
					}
					nextGoal = new ParseGoal(goal.Item, goal.ItemOffset + 1, goal.SentenceOffset);
				} else {
					var terminal = (Terminal)word;
					if (s[goal.SentenceOffset] == terminal) {
						Console.WriteLine("Terminals {0} and {1} match!", s[goal.SentenceOffset], terminal);
					} else {
						Console.WriteLine("Terminals {0} and {1} DON'T match :(", s[goal.SentenceOffset], terminal);
						continue;
					}
					nextGoal = new ParseGoal(goal.Item, goal.ItemOffset + 1, goal.SentenceOffset + 1);
				}
				if (nextGoal != null) {
					stack.Push(nextGoal.Value);
					nextGoal.Value.Parents.Add(goal);
				}
			}

		}

		private void AddToParents(ParseGoal goal) {
			foreach (var parent in goal.Parents) {
				parent.Children.Add(goal);
			}
		}

		private IEnumerable<Item> FindMatching(int offset, Nonterminal needle) {
			var state = S[offset];
			var result = new List<Item>();

			foreach (var item in state) {
				if (item.Production.Lhs == needle) {
					result.Add(item);
				}
			}

			return result;
		}

	}
}
