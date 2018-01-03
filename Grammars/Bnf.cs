using CFGLib;
using CFGLib.Parsers.Sppf;
using CFGLib.ProductionAnnotations;
using CFGLib.ProductionAnnotations.Actioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grammars
{
    public static class Bnf {

		/*
		Based on https://en.wikipedia.org/wiki/Backus%E2%80%93Naur_form#Further_examples
		This work was released under CC-BY-SA (https://creativecommons.org/licenses/by-sa/3.0/)

<syntax>         ::= <rule> | <rule> <syntax>
<rule>           ::= <opt-whitespace> "<" <rule-name> ">" <opt-whitespace> "::=" <opt-whitespace> <expression> <line-end>
<opt-whitespace> ::= " " <opt-whitespace> | ""
<expression>     ::= <list> | <list> <opt-whitespace> "|" <opt-whitespace> <expression>
<line-end>       ::= <opt-whitespace> <EOL> | <line-end> <line-end>
<list>           ::= <term> | <term> <opt-whitespace> <list>
<term>           ::= <literal> | "<" <rule-name> ">"
<literal>        ::= '"' <text1> '"' | "'" <text2> "'"
<text1>          ::= "" | <character1> <text1>
<text2>          ::= "" | <character2> <text2>
<character>      ::= <letter> | <digit> | <symbol>
<letter>         ::= "A" | "B" | "C" | "D" | "E" | "F" | "G" | "H" | "I" | "J" | "K" | "L" | "M" | "N" | "O" | "P" | "Q" | "R" | "S" | "T" | "U" | "V" | "W" | "X" | "Y" | "Z" | "a" | "b" | "c" | "d" | "e" | "f" | "g" | "h" | "i" | "j" | "k" | "l" | "m" | "n" | "o" | "p" | "q" | "r" | "s" | "t" | "u" | "v" | "w" | "x" | "y" | "z"
<digit>          ::= "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9"
<symbol>         ::=  "|" | " " | "-" | "!" | "#" | "$" | "%" | "&" | "(" | ")" | "*" | "+" | "," | "-" | "." | "/" | ":" | ";" | ">" | "=" | "<" | "?" | "@" | "[" | "\" | "]" | "^" | "_" | "`" | "{" | "}" | "~"
<character1>     ::= <character> | "'"
<character2>     ::= <character> | '"'
<rule-name>      ::= <letter> | <rule-name> <rule-char>
<rule-char>      ::= <letter> | <digit> | "-"
		*/
		public static Grammar Grammar() {
			var productions = new List<Production> {
				new Production(
					Nonterminal.Of("syntax"),
					new Sentence {
						Nonterminal.Of("rule"),
					},
					1,
					new Annotations(new List<IAnnotation>{
						new ActionAnnotation(args => ((IEnumerable<Production>)args[0].Payload).ToList())
					})
				),
				new Production(
					Nonterminal.Of("syntax"), 
					new Sentence {
						Nonterminal.Of("rule"),
						Nonterminal.Of("syntax"),
					},
					1,
					new Annotations(new List<IAnnotation>{
						new ActionAnnotation(args => {
							var list = (List<Production>)args[1].Payload;
							list.AddRange((IEnumerable<Production>)args[0].Payload);
							return list;
						})
					})
				),

				new Production(Nonterminal.Of("opt_comments"), new Sentence {
				}),
				new Production(Nonterminal.Of("opt_comments"), new Sentence {
					Nonterminal.Of("comment"),
					Nonterminal.Of("opt_comments"),
				}),

				new Production(
					Nonterminal.Of("rule"),
					new Sentence {
						Nonterminal.Of("opt_comments"),
						Nonterminal.Of("opt_whitespace"),
						Terminal.Of("<"),
						Nonterminal.Of("rule_name"),
						Terminal.Of(">"),
						Nonterminal.Of("opt_whitespace"),
						Terminal.Of(":"),
						Terminal.Of(":"),
						Terminal.Of("="),
						Nonterminal.Of("opt_whitespace"),
						Nonterminal.Of("expression"),
						Nonterminal.Of("line_end"),
					},
					1,
					new Annotations(new List<IAnnotation> {
						new ActionAnnotation((args) => {
							var lhs = Nonterminal.Of((string)args[3].Payload);
							var rhss = (List<Sentence>)args[10].Payload;

							return rhss.Select(rhs => new Production(lhs, rhs));
						})
					})
				),

				new Production(Nonterminal.Of("comment"), new Sentence {
					Nonterminal.Of("opt_whitespace"),
					Terminal.Of("/"),
					Terminal.Of("/"),
					Nonterminal.Of("comment_text"),
					Nonterminal.Of("line_end"),
				}), // ambiguous with space at end

				new Production(Nonterminal.Of("opt_whitespace"), new Sentence {
					Terminal.Of(" "),
					Nonterminal.Of("opt_whitespace"),
				}),
				new Production(Nonterminal.Of("opt_whitespace"), new Sentence {
				}),

				new Production(
					Nonterminal.Of("expression"), 
					new Sentence {
						Nonterminal.Of("list"),
					},
					1,
					new Annotations(new List<IAnnotation>{
						new ActionAnnotation(args => new List<Sentence> { (Sentence)args[0].Payload })
					})
				),
				new Production(
					Nonterminal.Of("expression"), 
					new Sentence {
						Nonterminal.Of("list"),
						Nonterminal.Of("opt_whitespace"),
						Terminal.Of("|"),
						Nonterminal.Of("opt_whitespace"),
						Nonterminal.Of("expression"),
					},
					1,
					new Annotations(new List<IAnnotation>{
						new ActionAnnotation(args => {
							var list = (List<Sentence>)args[4].Payload;
							list.Add((Sentence)args[0].Payload);
							return list;
						})
					})
				),

				new Production(Nonterminal.Of("line_end"), new Sentence {
					Nonterminal.Of("opt_whitespace"),
					Nonterminal.Of("EOL"),
				}),
				new Production(Nonterminal.Of("line_end"), new Sentence {
					Nonterminal.Of("line_end"),
					Nonterminal.Of("line_end"),
				}),

				new Production(
					Nonterminal.Of("list"), 
					new Sentence {
						Nonterminal.Of("term"),
					},
					1,
					new Annotations(new List<IAnnotation>{
						new ActionAnnotation(args => new Sentence((Word)args[0].Payload))
					})
				),
				new Production(
					Nonterminal.Of("list"), 
					new Sentence {
						Nonterminal.Of("term"),
						Nonterminal.Of("opt_whitespace"),
						Nonterminal.Of("list"),
					},
					1,
					new Annotations(new List<IAnnotation>{
						new ActionAnnotation(args => {
							var sentence = (Sentence)args[2].Payload;
							sentence.Add((Word)args[0].Payload);
							return sentence;
						})
					})
				),

				new Production(Nonterminal.Of("term"), new Sentence {
					Nonterminal.Of("literal"),
				}),
				new Production(
					Nonterminal.Of("term"), 
					new Sentence {
						Terminal.Of("<"),
						Nonterminal.Of("rule_name"),
						Terminal.Of(">"),
					},
					1,
					new Annotations(new List<IAnnotation>{
						new ActionAnnotation(args => Nonterminal.Of((string)args[1].Payload))
					})
				),

				new Production(
					Nonterminal.Of("literal"), 
					new Sentence {
						Terminal.Of("\""),
						Nonterminal.Of("text1"),
						Terminal.Of("\""),
					},
					1,
					new Annotations(new List<IAnnotation>{
						new ActionAnnotation(args => Terminal.Of((string)args[1].Payload))
					})
				),
				new Production(
					Nonterminal.Of("literal"), 
					new Sentence {
						Terminal.Of("'"),
						Nonterminal.Of("text2"),
						Terminal.Of("'"),
					},
					1,
					new Annotations(new List<IAnnotation>{
						new ActionAnnotation(args => Terminal.Of((string)args[1].Payload))
					})
				),

				new Production(Nonterminal.Of("comment_text"), new Sentence {
				}),
				new Production(Nonterminal.Of("comment_text"), new Sentence {
					Nonterminal.Of("comment_char"),
					Nonterminal.Of("comment_text"),
				}),

				new Production(Nonterminal.Of("text1"), new Sentence {
				}),
				new Production(Nonterminal.Of("text1"), new Sentence {
					Nonterminal.Of("character1"),
					Nonterminal.Of("text1"),
				}),

				new Production(Nonterminal.Of("text2"), new Sentence {
				}),
				new Production(Nonterminal.Of("text2"), new Sentence {
					Nonterminal.Of("character2"),
					Nonterminal.Of("text2"),
				}),

				new Production(Nonterminal.Of("character"), new Sentence {
					Nonterminal.Of("letter"),
				}),
				new Production(Nonterminal.Of("character"), new Sentence {
					Nonterminal.Of("digit"),
				}),
				new Production(Nonterminal.Of("character"), new Sentence {
					Nonterminal.Of("symbol"),
				}),

				new Production(Nonterminal.Of("comment_char"), new Sentence {
					Nonterminal.Of("character"),
				}),
				new Production(Nonterminal.Of("comment_char"), new Sentence {
					Terminal.Of("\""),
				}),
				new Production(Nonterminal.Of("comment_char"), new Sentence {
					Terminal.Of("'"),
				}),

				// letters
				// digits
				// symbols
				
				new Production(Nonterminal.Of("character1"), new Sentence {
					Nonterminal.Of("character"),
				}),
				new Production(Nonterminal.Of("character1"), new Sentence {
					Terminal.Of("'"),
				}),

				new Production(Nonterminal.Of("character2"), new Sentence {
					Nonterminal.Of("character"),
				}),
				new Production(Nonterminal.Of("character2"), new Sentence {
					Terminal.Of("\""),
				}),

				new Production(Nonterminal.Of("rule_name"), new Sentence {
					Nonterminal.Of("letter"),
				}),
				new Production(Nonterminal.Of("rule_name"), new Sentence {
					Nonterminal.Of("rule_name"),
					Nonterminal.Of("rule_char"),
				}),

				new Production(Nonterminal.Of("rule_char"), new Sentence {
					Nonterminal.Of("letter"),
				}),
				new Production(Nonterminal.Of("rule_char"), new Sentence {
					Nonterminal.Of("digit"),
				}),
				new Production(Nonterminal.Of("rule_char"), new Sentence {
					Terminal.Of("_"),
				}),
				new Production(Nonterminal.Of("rule_char"), new Sentence {
					Terminal.Of("-"),
				}),

				new Production(Nonterminal.Of("EOL"), new Sentence {
					Terminal.Of("\n"),
				}),
				new Production(Nonterminal.Of("EOL"), new Sentence {
					Terminal.Of("\r"),
				}),
			};
			foreach (var letter in new string[] { "A" , "B" , "C" , "D" , "E" , "F" , "G" , "H" , "I" , "J" , "K" , "L" , "M" , "N" , "O" , "P" , "Q" , "R" , "S" , "T" , "U" , "V" , "W" , "X" , "Y" , "Z" , "a" , "b" , "c" , "d" , "e" , "f" , "g" , "h" , "i" , "j" , "k" , "l" , "m" , "n" , "o" , "p" , "q" , "r" , "s" , "t" , "u" , "v" , "w" , "x" , "y" , "z" }) {
				productions.Add(
					new Production(Nonterminal.Of("letter"), new Sentence {
						Terminal.Of(letter),
					})
				);
			}
			foreach (var digit in new string[] { "0" , "1" , "2" , "3" , "4" , "5" , "6" , "7" , "8" , "9" }) {
				productions.Add(
					new Production(Nonterminal.Of("digit"), new Sentence {
						Terminal.Of(digit),
					})
				);
			}
			foreach (var symbol in new string[] { "|" , " " , "-" , "!" , "#" , "$" , "%" , "&" , "(" , ")" , "*" , "+" , "," , "-" , "." , "/" , ":" , ";" , ">" , "=" , "<" , "?" , "@" , "[" , "\\" , "]" , "^" , "_" , "`" , "{" , "}" , "~" }) {
				productions.Add(
					new Production(Nonterminal.Of("symbol"), new Sentence {
						Terminal.Of(symbol),
					})
				);
			}

			var start = Nonterminal.Of("syntax");
			var g = new Grammar(productions, start);
			return g;
		}
    }
}
