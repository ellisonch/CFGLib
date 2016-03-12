using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib {
	/// <summary>
	/// This class represents a sentence, i.e., a list of terminals and nonterminals.  It is an IList&lt;Word>, ICollection&lt;Word>, and IEnumerable&lt;Word>.
	/// </summary>
	public class Sentence : IList<Word>, ICollection<Word>, IEnumerable<Word> {
		private List<Word> _sentence;

		public Sentence() {
			_sentence = new List<Word>();
		}
		public Sentence(Word w) {
			_sentence = new List<Word> { w };
		}
		public Sentence(IEnumerable<Word> l) {
			_sentence = new List<Word>(l);
		}

		/// <summary>
		/// Create a new sentence where each letter in the given string is treated as a separate terminal
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static Sentence FromLetters(string s) {
			var tokens = s.ToCharArray().Select((c) => c.ToString());
			return FromTokens(tokens);
		}
		/// <summary>
		/// Create a new sentence where each word (separated by a space) in the given string is treated as a separate terminal
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static Sentence FromWords(string s) {
			if (s.Length == 0) {
				return new Sentence();
			}
			var tokens = s.Split(new char[] { ' ' });
			return FromTokens(tokens);
		}
		/// <summary>
		/// Create a new sentence where string in the given list is treated as a separate terminal
		/// </summary>
		/// <param name="tokens"></param>
		/// <returns></returns>
		public static Sentence FromTokens(IEnumerable<string> tokens) {
			var l = new List<Terminal>();
			foreach (var token in tokens) {
				// l.Add(Terminal.Of(c.ToString()));
				l.Add(Terminal.Of(token));
			}
			return new Sentence(l);
		}

		/// <summary>
		/// Append all the words of the given sentence to the end of this sentence
		/// </summary>
		/// <param name="collection"></param>
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
		
		public Sentence GetRange(int index, int count) {
			return new Sentence(_sentence.GetRange(index, count));
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

		public void InsertRange(int index, IEnumerable<Word> collection) {
			_sentence.InsertRange(index, collection);
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

			return string.Join(" ", _sentence.Select((s) => s.ToString()));
		}

		public bool OnlyTerminals() {
			foreach (var word in _sentence) {
				if (word.IsNonterminal) {
					return false;
				}
			}
			return true;
		}
		public bool OnlyNonterminals() {
			foreach (var word in _sentence) {
				if (word.IsTerminal) {
					return false;
				}
			}
			return true;
		}

		public string AsTerminals(string separator = "") {
			if (!OnlyTerminals()) {
				throw new Exception("Can only use AsTerminals() on sentences that only contain terminals");
			}
			var resultList = new List<string>();
			foreach (var word in _sentence) {
				var terminal = (Terminal)word;
				resultList.Add(terminal.Name);
			}
			if (resultList.Count == 0) {
				return "ε";
			}
			return string.Join(separator, resultList);
		}
		
		public bool ContainsNonterminal() {
			foreach (var c in this) {
				if (c.IsNonterminal) {
					return true;
				}
			}
			return false;
		}

		public bool ContainsTerminal() {
			foreach (var c in this) {
				if (c.IsTerminal) {
					return true;
				}
			}
			return false;
		}

		public ISet<Terminal> GetAllTerminals() {
			var terminals = new HashSet<Terminal>();
			foreach (var word in this) {
				if (!word.IsNonterminal) {
					terminals.Add((Terminal)word);
				}
			}
			return terminals;
		}
	}
}
