# CFGLib 
A C# library for (Probabilistic) Context Free Grammars (PCFGs).
Supports conversion to Chomsky Normal Form (CNF) as well as the CYK algorithm for recognition.

## License
Sobriquet is licensed using the [MIT license](LICENSE.txt).

## Examples
CFGLib can be used to represent grammars and then use them to parse terms:
```cs
// S -> aSa | bSb | ε
var productions = new List<BaseProduction> {
	new Production(
		lhs: Nonterminal.Of("S"),
		rhs: new Sentence { Terminal.Of("a"), Nonterminal.Of("S"), Terminal.Of("a") },
		weight: 20
	),
	new Production(
		Nonterminal.Of("S"),
		new Sentence { Terminal.Of("b"), Nonterminal.Of("S"), Terminal.Of("b") },
		10
	),
	new Production(
		Nonterminal.Of("S"),
		new Sentence { },
		5
	)
};
var cfg = new Grammar(productions, Nonterminal.Of("S"));
var cnf = cfg.ToCNF();
Console.WriteLine(cnf.Cyk(Sentence.FromLetters("aabb")));
Console.WriteLine(cnf.Cyk(Sentence.FromLetters("abba")));
```
```
0
0.0277777777777778
```
It can also generate random terms from a given grammar:
```
for (int i = 0; i < 5; i++) {
	Console.WriteLine(cnf.ProduceRandom().AsTerminals());
}
```
```
ε
ε
ε
baab
abbbbbbbba
```