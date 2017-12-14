using System;
using System.Runtime.Serialization;
using CFGLib;

namespace CFGLibTest {
	[Serializable]
	internal class RandomTestException : Exception {
		public readonly Grammar g;
		public readonly Sentence sentence;

		public RandomTestException(Exception e, Grammar g, Sentence sentence) : base("Random", e) {
			this.g = g;
			this.sentence = sentence;
		}
	}
}