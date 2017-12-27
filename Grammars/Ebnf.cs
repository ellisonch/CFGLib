using CFGLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grammars {
	// This grammar is based on ISO/IEC 14977 : 1996(E)
	// In particular, the structure was borrowed from Section 8.1 "The Syntax of Extended BNF"
	public class Ebnf {
		private static readonly List<char> _lettersUc = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToList();

		public static Grammar Grammar(Nonterminal start) {
			// var start = Nonterminal.Of("Syntax");
			var g = new Grammar(Productions(), start);
			return g;
		}
		private static IEnumerable<Production> Productions() {
			var retval = Letter();
			retval = retval.Concat(DecimalDigit());
			retval = retval.Concat(BasicSymbols());
			retval = retval.Concat(SecondPart());
			retval = retval.Concat(ThirdPart());
			retval = retval.Concat(FinalPart());

			return retval;
		}

		private static IEnumerable<Production> BasicSymbols() {
			return new List<Production> {
				new Production("ConcatenateSymbol", Terminal.Of(",")),
				new Production("DefiningSymbol", Terminal.Of("=")),
				new Production("DefinitionSeparatorSymbol", Terminal.Of("|")),
				new Production("DefinitionSeparatorSymbol", Terminal.Of("/")),
				new Production("DefinitionSeparatorSymbol", Terminal.Of("!")),
				new Production("EndCommentSymbol", Terminal.Of("*)")),
				new Production("EndGroupSymbol", Terminal.Of(")")),
				new Production("EndOptionSymbol", Terminal.Of("]")),
				new Production("EndOptionSymbol", Terminal.Of("/)")),
				new Production("EndRepeatSymbol", Terminal.Of("}")),
				new Production("EndRepeatSymbol", Terminal.Of(":)")),
				new Production("ExceptSymbol", Terminal.Of("-")),
				new Production("FirstQuoteSymbol", Terminal.Of("'")),
				new Production("RepetitionSymbol", Terminal.Of("*")),
				new Production("SecondQuoteSymbol", Terminal.Of("\"")),
				new Production("SpecialSequenceSymbol", Terminal.Of("?")),
				new Production("StartCommentSymbol", Terminal.Of("(*")),
				new Production("StartGroupSymbol", Terminal.Of("(")),
				new Production("StartOptionSymbol", Terminal.Of("[")),
				new Production("StartOptionSymbol", Terminal.Of("(/")),
				new Production("StartRepeatSymbol", Terminal.Of("{")),
				new Production("StartRepeatSymbol", Terminal.Of("(:")),
				new Production("TerminatorSymbol", Terminal.Of(";")),
				new Production("TerminatorSymbol", Terminal.Of(".")),

				new Production("SpaceCharacter", Terminal.Of(" ")),
				new Production("HorizontalTabulationCharacter", Terminal.Of("\t")),
				new Production("NewLine", Terminal.Of("\n")),
				new Production("NewLine", Terminal.Of("\r\n")),
				new Production("VerticalTabulationCharacter", Terminal.Of("\v")),
				new Production("FormFeed", Terminal.Of("\f")),
			}.Concat(
				":+_%@&#$<>\\^`~".Select((x) => new Production("OtherCharacter", Terminal.Of(x.ToString())))
			);
		}

		private static IEnumerable<Production> SecondPart() {
			return new List<Production> {
				new Production("TerminalCharacterBasic", Nonterminal.Of("CommentlessTerminal")),
				new Production("TerminalCharacterBasic", Nonterminal.Of("Letter")),
				new Production("TerminalCharacterBasic", Nonterminal.Of("DecimalDigit")),
				new Production("TerminalCharacterBasic", Nonterminal.Of("EndCommentSymbol")),
				new Production("TerminalCharacterBasic", Nonterminal.Of("StartCommentSymbol")),
				new Production("TerminalCharacterBasic", Nonterminal.Of("OtherCharacter")),

				new Production("TerminalCharacter", Nonterminal.Of("TerminalCharacterBasic")),
				new Production("TerminalCharacter", Nonterminal.Of("FirstQuoteSymbol")),
				new Production("TerminalCharacter", Nonterminal.Of("SecondQuoteSymbol")),
				new Production("TerminalCharacter", Nonterminal.Of("SpecialSequenceSymbol")),

				new Production("GapFreeSymbol", Nonterminal.Of("TerminalCharacterBasic")),
				new Production("GapFreeSymbol", Nonterminal.Of("TerminalString")),

				new Production("TerminalString", new Sentence {
					Nonterminal.Of("FirstQuoteSymbol"),
					Nonterminal.Of("FirstTerminalCharacterList1"),
					Nonterminal.Of("FirstQuoteSymbol"),
				}),
				new Production("TerminalString", new Sentence {
					Nonterminal.Of("SecondQuoteSymbol"),
					Nonterminal.Of("SecondTerminalCharacterList1"),
					Nonterminal.Of("SecondQuoteSymbol"),
				}),

				new Production("FirstTerminalCharacter", Nonterminal.Of("TerminalCharacterBasic")),
				new Production("FirstTerminalCharacter", Nonterminal.Of("SecondQuoteSymbol")),

				new Production("SecondTerminalCharacter", Nonterminal.Of("TerminalCharacterBasic")),
				new Production("SecondTerminalCharacter", Nonterminal.Of("FirstQuoteSymbol")),

				new Production("GapSeparator", Nonterminal.Of("SpaceCharacter")),
				new Production("GapSeparator", Nonterminal.Of("HorizontalTabulationCharacter")),
				new Production("GapSeparator", Nonterminal.Of("NewLine")),
				new Production("GapSeparator", Nonterminal.Of("VerticalTabulationCharacter")),
				new Production("GapSeparator", Nonterminal.Of("FormFeed")),

				// TODO
				new Production("SyntaxLayout", new Sentence {
					Nonterminal.Of("GapSeparatorList0"),
					Nonterminal.Of("SymbolWithOptionalGapList1"),
				}),

				new Production("SymbolWithOptionalGap", new Sentence {
					Nonterminal.Of("GapFreeSymbol"),
					Nonterminal.Of("GapSeparatorList0"),
				}),

			}.Concat(
				MakeList("FirstTerminalCharacter", 1)
			).Concat(
				MakeList("SecondTerminalCharacter", 1)
			).Concat(
				MakeList("GapSeparator", 0)
			).Concat(
				MakeList("SymbolWithOptionalGap", 1)
			);
		}

		public static IEnumerable<Production> ThirdPart() {
			return new List<Production> {
				new Production("CommentlessTerminal", Nonterminal.Of("ConcatenateSymbol")),
				new Production("CommentlessTerminal", Nonterminal.Of("DefiningSymbol")),
				new Production("CommentlessTerminal", Nonterminal.Of("DefinitionSeparatorSymbol")),
				new Production("CommentlessTerminal", Nonterminal.Of("EndGroupSymbol")),
				new Production("CommentlessTerminal", Nonterminal.Of("EndOptionSymbol")),
				new Production("CommentlessTerminal", Nonterminal.Of("EndRepeatSymbol")),
				new Production("CommentlessTerminal", Nonterminal.Of("ExceptSymbol")),
				new Production("CommentlessTerminal", Nonterminal.Of("RepetitionSymbol")),
				new Production("CommentlessTerminal", Nonterminal.Of("StartGroupSymbol")),
				new Production("CommentlessTerminal", Nonterminal.Of("StartOptionSymbol")),
				new Production("CommentlessTerminal", Nonterminal.Of("StartRepeatSymbol")),
				new Production("CommentlessTerminal", Nonterminal.Of("TerminatorSymbol")),

				new Production("CommentlessSymbol", Nonterminal.Of("CommentlessTerminal")),
				new Production("CommentlessSymbol", Nonterminal.Of("MetaIdentifier")),
				new Production("CommentlessSymbol", Nonterminal.Of("Integer")),
				new Production("CommentlessSymbol", Nonterminal.Of("TerminalString")),
				new Production("CommentlessSymbol", Nonterminal.Of("SpecialSequence")),

				new Production("Integer", Nonterminal.Of("DecimalDigitList1")),

				new Production("MetaIdentifier", new Sentence {
					Nonterminal.Of("Letter"),
					Nonterminal.Of("MetaIdentifierCharacterList0"),
				}),

				new Production("MetaIdentifierCharacter", Nonterminal.Of("Letter")),
				new Production("MetaIdentifierCharacter", Nonterminal.Of("DecimalDigit")),

				new Production("SpecialSequence", new Sentence {
					Nonterminal.Of("SpecialSequenceSymbol"),
					Nonterminal.Of("SpecialSequenceCharacterList0"),
					Nonterminal.Of("SpecialSequenceSymbol"),
				}),

				new Production("SpecialSequenceCharacter", Nonterminal.Of("TerminalCharacterBasic")),
				new Production("SpecialSequenceCharacter", Nonterminal.Of("FirstQuoteSymbol")),
				new Production("SpecialSequenceCharacter", Nonterminal.Of("SecondQuoteSymbol")),

				new Production("CommentSymbol", Nonterminal.Of("BracketedTextualComment")),
				new Production("CommentSymbol", Nonterminal.Of("OtherCharacter")),
				new Production("CommentSymbol", Nonterminal.Of("CommentlessSymbol")),

				new Production("BracketedTextualComment", new Sentence {
					Nonterminal.Of("StartCommentSymbol"),
					Nonterminal.Of("CommentSymbolList0"),
					Nonterminal.Of("EndCommentSymbol"),
				}),

				// TODO
				//new Production("Syntax", new Sentence {
				//	Nonterminal.Of("BracketedTextualCommentList0"),
				//	Nonterminal.Of("CommentlessSymbolWithOptionalCommentList1"),
				//}),

				new Production("CommentlessSymbolWithOptionalComment", new Sentence {
					Nonterminal.Of("CommentlessSymbol"),
					Nonterminal.Of("BracketedTextualCommentList0"),
				}),

			}.Concat(
				MakeList("DecimalDigit", 1)
			).Concat(
				MakeList("MetaIdentifierCharacter", 0)
			).Concat(
				MakeList("SpecialSequenceCharacter", 0)
			).Concat(
				MakeList("CommentSymbol", 0)
			).Concat(
				MakeList("BracketedTextualComment", 0)
			).Concat(
				MakeList("CommentlessSymbolWithOptionalComment", 1)
			);
		}

		public static IEnumerable<Production> FinalPart() {
			return new List<Production> {
				new Production("Syntax", Nonterminal.Of("SyntaxRuleList1")),

				new Production("SyntaxRule", new Sentence {
					Nonterminal.Of("MetaIdentifier"),
					Nonterminal.Of("DefiningSymbol"),
					Nonterminal.Of("DefinitionsList"),
					Nonterminal.Of("TerminatorSymbol"),
				}),

				new Production("DefinitionsList", new Sentence {
					Nonterminal.Of("SingleDefinition"),
					Nonterminal.Of("SeparatedDefinitionList0"),
				}),

				new Production("SeparatedDefinition", new Sentence {
					Nonterminal.Of("DefinitionSeparatorSymbol"),
					Nonterminal.Of("SingleDefinition"),
				}),

				new Production("SingleDefinition", new Sentence {
					Nonterminal.Of("SyntacticTerm"),
					Nonterminal.Of("ConcatenatedSyntacticTermList0"),
				}),

				new Production("ConcatenatedSyntacticTerm", new Sentence {
					Nonterminal.Of("ConcatenateSymbol"),
					Nonterminal.Of("SyntacticTerm"),
				}),

				new Production("SyntacticTerm", new Sentence {
					Nonterminal.Of("SyntacticFactor"),
				}),
				// TODO
				//new Production("SyntacticTerm", new Sentence {
				//	Nonterminal.Of("SyntacticFactor"),
				//	Nonterminal.Of("ExceptSymbol"),
				//	Nonterminal.Of("SyntacticException"),
				//}),

				// TODO
				/*
					(* see 4.7 *) syntactic exception
					= ? a syntactic-factor that could be replaced
					by a syntactic-factor containing no
					meta-identifiers
					? ;
				*/
				new Production("SyntacticFactor", new Sentence {
					Nonterminal.Of("Integer"),
					Nonterminal.Of("RepetitionSymbol"),
					Nonterminal.Of("SyntacticPrimary"),
				}),
				new Production("SyntacticFactor", new Sentence {
					Nonterminal.Of("SyntacticPrimary"),
				}),

				new Production("SyntacticPrimary", Nonterminal.Of("OptionalSequence")),
				new Production("SyntacticPrimary", Nonterminal.Of("RepeatedSequence")),
				new Production("SyntacticPrimary", Nonterminal.Of("GroupedSequence")),
				new Production("SyntacticPrimary", Nonterminal.Of("MetaIdentifier")),
				new Production("SyntacticPrimary", Nonterminal.Of("TerminalString")),
				new Production("SyntacticPrimary", Nonterminal.Of("SpecialSequence")),
				new Production("SyntacticPrimary", Nonterminal.Of("EmptySequence")),

				new Production("OptionalSequence", new Sentence {
					Nonterminal.Of("StartOptionSymbol"),
					Nonterminal.Of("DefinitionsList"),
					Nonterminal.Of("EndOptionSymbol"),
				}),

				new Production("RepeatedSequence", new Sentence {
					Nonterminal.Of("StartRepeatSymbol"),
					Nonterminal.Of("DefinitionsList"),
					Nonterminal.Of("EndRepeatSymbol"),
				}),

				new Production("GroupedSequence", new Sentence {
					Nonterminal.Of("StartGroupSymbol"),
					Nonterminal.Of("DefinitionsList"),
					Nonterminal.Of("EndGroupSymbol"),
				}),

				new Production("EmptySequence", new Sentence()),

			}.Concat(
				MakeList("SyntaxRule", 1)
			).Concat(
				MakeList("SeparatedDefinition", 0)
			).Concat(
				MakeList("ConcatenatedSyntacticTerm", 0)
			);
		}

		private static IEnumerable<Production> MakeList(string baseName, int minimum) {
			if (minimum < 0 || minimum > 1) {
				throw new ArgumentOutOfRangeException();
			}
			var listName = baseName + "List" + minimum;

			IEnumerable<Production> retval = new List<Production>();

			if (minimum == 1) {
				retval = retval.Concat(new List<Production>() {
					new Production(listName, new Sentence {
						Nonterminal.Of(baseName),
						Nonterminal.Of(listName),
					}),
					new Production(listName, new Sentence {
						Nonterminal.Of(baseName),
					}),
				});
			} else {
				retval = retval.Concat(new List<Production>() {
					new Production(listName, new Sentence {
						Nonterminal.Of(baseName),
						Nonterminal.Of(listName),
					}),
					new Production(listName, new Sentence())
				});
			}

			return retval;
		}

		public static IEnumerable<Production> Letter() {
			return LetterUc().Concat(LetterLc());
		}
		public static IEnumerable<Production> DecimalDigit() {
			return Enumerable.Range(0, 10).Select((x) => CFGParser.Production(string.Format("<DecimalDigit> → '{0}'", x)));
		}

		//CFGParser.Production("<S> → '0'"),
		//		CFGParser.Production("<S> → ε"),
		public static IEnumerable<Production> LetterUc() {
			return _lettersUc.Select((x) => CFGParser.Production(string.Format("<Letter> → '{0}'", x)));
		}
		public static IEnumerable<Production> LetterLc() {
			return _lettersUc.Select((x) => CFGParser.Production(string.Format("<Letter> → '{0}'", char.ToLower(x))));
		}
	}
}
