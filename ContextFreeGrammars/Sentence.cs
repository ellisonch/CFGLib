using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextFreeGrammars {
	public class Sentence : IList<Word>, ICollection<Word>, IEnumerable<Word> {
		private List<Word> _sentence;

		public Sentence() {
			_sentence = new List<Word>();
		}
		public Sentence(IEnumerable<Word> l) {
			_sentence = new List<Word>(l);
		}

		public static Sentence FromLetters(string s) {
			var l = new List<Terminal>();
			foreach (var c in s) {
				l.Add(Terminal.Of(c.ToString()));
			}
			return new Sentence(l);
		}

		public void AddRange(Sentence collection) {
			_sentence.AddRange(collection);
		}

		public Word this[int index] {
			get {
				return _sentence[index];
			}

			set {
				_sentence[index] = value;
			}
		}

		public int Count {
			get {
				return _sentence.Count;
			}
		}

		public bool IsReadOnly {
			get {
				return false;
			}
		}

		public void Add(Word item) {
			_sentence.Add(item);
		}

		public void Clear() {
			_sentence.Clear();
		}

		public bool Contains(Word item) {
			return _sentence.Contains(item);
		}

		public void CopyTo(Word[] array, int arrayIndex) {
			_sentence.CopyTo(array, arrayIndex);
		}

		public IEnumerator<Word> GetEnumerator() {
			return _sentence.GetEnumerator();
		}

		public int IndexOf(Word item) {
			return _sentence.IndexOf(item);
		}

		public void Insert(int index, Word item) {
			_sentence.Insert(index, item);
		}

		public bool Remove(Word item) {
			return _sentence.Remove(item);
		}

		public void RemoveAt(int index) {
			_sentence.RemoveAt(index);
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return _sentence.GetEnumerator();
		}

		public override string ToString() {
			if (Count == 0) {
				return "ε";
			}

			var retval = "";
			foreach (var c in _sentence) {
				retval += c.ToString() + " ";
			}
			return retval;
		}

		public bool OnlyTerminals() {
			var hasVariable = false;
			foreach (var word in _sentence) {
				if (word.IsVariable()) {
					hasVariable = true;
					break;
				}
			}
			return !hasVariable;
		}

		public string AsTerminals() {
			if (!OnlyTerminals()) {
				throw new Exception("Can only use AsTerminals() on sentences that only contain terminals");
			}
			var result = "";
			foreach (var word in _sentence) {
				var terminal = (Terminal)word;
				result += terminal.Name + " ";
			}
			return result;
		}

		public static ISet<Terminal> GetAllTerminals(IEnumerable<Sentence> sentences) {
			var terminals = new HashSet<Terminal>();
			foreach (var sentence in sentences) {
				foreach (var word in sentence) {
					if (!word.IsVariable()) {
						terminals.Add((Terminal)word);
					}
				}
			}
			return terminals;
		}
	}
}
