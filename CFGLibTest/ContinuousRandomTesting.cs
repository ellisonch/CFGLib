using CFGLib;
using CFGLib.Parsers.CYK;
using CFGLib.Parsers.Earley;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLibTest {
	public class ContinuousRandomTesting {
		private readonly Random _r;
		private readonly int _maxNonterminals;
		private readonly int _maxTerminals;
		private readonly int _maxProductions;
		private readonly int _maxProductionLength;
		private readonly int _maxInputLength;
		private readonly int _numRandomSentences;
		private readonly GrammarGenerator _gramGen;

		private readonly StreamWriter _parseErrorFile;

		public ContinuousRandomTesting(
			int maxNonterminals,
			int maxTerminals,
			int maxProductions,
			int maxProductionLength,
			int maxInputLength,
			int numRandomSentences,
			int seed
		) {
			_maxNonterminals = maxNonterminals;
			_maxTerminals = maxTerminals;
			_maxProductions = maxProductions;
			_maxProductionLength = maxProductionLength;
			_maxInputLength = maxInputLength;
			_numRandomSentences = numRandomSentences;
			_r = new Random(seed);
			_gramGen = new GrammarGenerator(_r);

			_parseErrorFile = new System.IO.StreamWriter(@"parseErrors.txt");
		}

		public void Run(int rounds = -1) {
			int fail = 0;
			int total = 0;
			int timeouts = 0;
			var overallSw = Stopwatch.StartNew();
			var sw = Stopwatch.StartNew();
			while (rounds == -1 || rounds-- > 0) {
				var task = Task.Run(() => ProcessOneGrammar());
				if (task.Wait(TimeSpan.FromSeconds(20))) {
					if (task.Result) {
						fail++;
					}
				} else { 
					timeouts++;
				}
				total++;
				if (total % 50 == 0) {
					Console.WriteLine("{0} / {1} / {2} fail / timeout / total in {3}ms", fail, timeouts, total, sw.Elapsed.TotalMilliseconds);
					sw.Restart();
				}
			}
			Console.WriteLine("{0} / {1} / {2} fail / timeout / total in {3}ms", fail, timeouts, total, overallSw.Elapsed.TotalMilliseconds);
		}

		private bool ProcessOneGrammar() {
			var (g, terminals) = NextGrammar();
			var h = g.ToCNF();
			// Console.WriteLine(g.Productions.Count());
			var preparedSentences = new List<Sentence>();
			for (int length = 0; length <= _maxInputLength; length++) {
				var combinations = CFGLibTest.Helpers.CombinationsWithRepetition(terminals, length);
				foreach (var target in combinations) {
					var sentence = new Sentence(target);
					preparedSentences.Add(sentence);
				}
			}

			AddRandomSentences(preparedSentences, terminals);
			var uniquifySentences = new Dictionary<string, Sentence>();
			foreach (var sentence in preparedSentences) {
				uniquifySentences[sentence.AsTerminals()] = sentence;
			}
			preparedSentences = uniquifySentences.Values.ToList();

			// Console.WriteLine("Parsing sentences...");
			EarleyParser earley1;
			EarleyParser2 earley2;
			CykParser cyk;
			try {
				earley1 = new EarleyParser(g);
				earley2 = new EarleyParser2(g);
				cyk = new CykParser(h);
			} catch {
				Report(g);
				return true;
			}
			
			foreach (var sentence in preparedSentences) {
				try {
					var p1 = earley1.ParseGetProbability(sentence);
					var p2 = earley2.ParseGetProbability(sentence);
					var p3 = cyk.ParseGetProbability(sentence);
					if (!Helpers.IsNear(p1, p2)) {
						throw new Exception();
					}
					if (!Helpers.IsNear(p1, p3)) {
						throw new Exception();
					}
				} catch {
					Report(g, sentence);
					return true;
					// throw new RandomTestException(e, g, sentence);
				}
			}
			return false;
		}

		private void AddRandomSentences(IList<Sentence> preparedSentences, IList<Terminal> terminals) {
			for (var i = 0; i < _numRandomSentences; i++) {
				var length = _r.Next(_maxInputLength + 1, _maxInputLength * 4);
				var sentenceArray = new Terminal[length];
				for (var pos = 0; pos < length; pos++) {
					sentenceArray[pos] = terminals[_r.Next(0, terminals.Count)];
				}
				var sentence = new Sentence(sentenceArray);
				preparedSentences.Add(sentence);
			}
		}

		private void Report(Grammar g, Sentence sentence = null) {
			//Console.WriteLine("Offending grammar:");
			//Console.WriteLine(g.ToCodeString());
			//Console.WriteLine("Offending sentence:");
			//Console.WriteLine(sentence);
			if (sentence == null) {
				sentence = Sentence.FromLetters("");
			}
			var test = string.Format("ExecuteTest({0}, \"{1}\");", g.ToCodeString(), sentence.AsTerminals(" "));
			_parseErrorFile.WriteLine(test);
			_parseErrorFile.WriteLine("-------------------------------------------");
			_parseErrorFile.Flush();
		}

		private (Grammar, List<Terminal>) NextGrammar() {
			var numNonterminals = _r.Next(1, _maxNonterminals);
			var numProductions = _r.Next(1, _maxProductions);
			var maxProductionLength = _maxProductionLength;
			var numTerminals = _r.Next(1, _maxTerminals);

			var range = Enumerable.Range(0, numTerminals);
			var terminals = new List<Terminal>(range.Select((x) => Terminal.Of("x" + x)));

			Grammar g = null;
			for (int i = 0; i < 200; i++) {
				g = _gramGen.NextCFG(numNonterminals, numProductions, maxProductionLength, terminals, true);
				if (g.Productions.Count() > 0) {
					return (g, terminals);
				}
			}
			return (g, terminals);
		}
	}
}
