using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CFGLib;
using System.Collections.Generic;
using System.Linq;

namespace CFGLibTest.Unit {
	[TestClass]
	public class TestCFGToCNF {
		[TestMethod]
		public void TestNullate01() {
			PrivateType cfgToCnf = new PrivateType(typeof(CFGtoCNF));
			
			var production = CFGParser.Production("<S> -> <A> 'b' <B>");
			var nullableSet = new HashSet<Nonterminal> { Nonterminal.Of("A"), Nonterminal.Of("B") };

			var actualList = (List<BaseProduction>)cfgToCnf.InvokeStatic("Nullate", new object[] { production, nullableSet });
			var actual = new HashSet<string>(actualList.Select((p) => p.ToString()));
			var expected = new HashSet<string> {
				CFGParser.Production("<S> -> <A> 'b' <B>").ToString(),
				CFGParser.Production("<S> -> <A> 'b'").ToString(),
				CFGParser.Production("<S> -> 'b' <B>").ToString(),
				CFGParser.Production("<S> -> 'b'").ToString(),
			};
			Assert.IsTrue(actual.SetEquals(expected));
		}
	}
}
