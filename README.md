# CFGLib 
A C# library for (Probabilistic) Context Free Grammars (PCFGs).
Supports conversion to Chomsky Normal Form (CNF) as well as the CYK algorithm for recognition.

## License
CFGLib is licensed using the [MIT license](LICENSE.txt).

## Examples
CFGLib can perform a number of basic operations on context free grammars.
If there are some operations you need, feel free to suggest them on the Issues page!

### Representation and Acceptance
CFGLib can be used to represent grammars and then use those grammars to parse terms:
```cs
// S -> aSa | bSb | ε
var productions = new List<Production> {
	// construct productions by passing arguments...
	Production.New(
		lhs: Nonterminal.Of("S"),
		rhs: new Sentence { Terminal.Of("a"), Nonterminal.Of("S"), Terminal.Of("a") },
		weight: 20
	),
	// or from a string...
	CFGParser.Production(@"<S> -> 'b' <S> 'b' [10]"),
	CFGParser.Production(@"<S> -> ε [5]"),
};
var cfg = new Grammar(productions, Nonterminal.Of("S"));
var cnf = cfg.ToCNF();
// Does this grammar accept the string "aabb"?
Console.WriteLine(cnf.Accepts(Sentence.FromLetters("aabb")));
// How about "abba"?
Console.WriteLine(cnf.Accepts(Sentence.FromLetters("abba")));
```
```
False
True
```

### Explore the Grammar
CFGLib can generate random terms from a given grammar:
```cs
for (int i = 0; i < 5; i++) {
	Console.WriteLine(cfg.ProduceRandom().AsTerminals());
}
```
```
bbbaaaaaabbb
aa
aaabaabaaa
aa
ε
```
It can also explore your grammar, returning all the strings generated up to a particular depth:
```cs
var sentences = cfg.ProduceToDepth(3);
foreach (var sentence in sentences) {
	Console.WriteLine(sentence.Value.AsTerminals());
}
```
```
ε
aa
bb
aaaa
abba
baab
bbbb
```

### Random Grammar Generation
You can also generate random Grammars (either generic, or CNF).
```cs
var gg = new GrammarGenerator(1);
var terminals = new List<Terminal> { Terminal.Of("a"), Terminal.Of("b") };
var randGram = gg.NextCFG(
	numNonterminals: 4,
	numProductions: 10,
	maxProductionLength: 4,
	terminals: terminals
);
Console.WriteLine(randGram);
```
```
Grammar(<X_0>){
  2.75e-001: <X_0> → 'a' <X_3> <X_2> [47.7010679872246]
  1.49e-001: <X_0> → 'b' [25.8029145527645]
  4.19e-001: <X_2> → 'b' 'a' <X_1> [29.2729223967869]
  4.41e-001: <X_3> → <X_3> 'b' 'b' [17.9467153106568]
  5.77e-001: <X_0> → 'a' 'b' <X_3> 'a' [100.187609646091]
  4.05e-001: <X_1> → <X_0> <X_2> <X_1> [68.5405530107862]
  5.81e-001: <X_2> → ε [40.5143099778864]
  2.33e-001: <X_3> → <X_2> <X_0> <X_0> <X_2> [9.50396021665258]
  5.95e-001: <X_1> → 'a' <X_0> <X_3> <X_3> [100.789105122811]
  3.26e-001: <X_3> → <X_0> <X_1> [13.2542167139492]
}
```

## Caveats
CFGLib cannot currently generate parse trees or forests.
This is planned.

Although there is code for determining the probability that a string was generated from a given CNF grammar (e.g., `cnf.CYK(Sentence.FromLetters("abba")`), converting from a generic CFG to CNF currently does not preserve weights properly.
This is still being worked on.


## Other Useful Classes and Methods
* `Sentence`: A list of `Nonterminal`s and `Terminal`s
  * `static Sentence FromWords(string s);` (tokenizes on space)
  * `static Sentence FromTokens(IEnumerable<string> tokens);`
* `BaseGrammar` > {`Grammar`, `CNFGrammar`}: All kinds of grammars inherit from `BaseGrammar`.
  * `Sentence ProduceRandom();`
  * `double EstimateProbabilityNull(Nonterminal nt, long iterations);`
  * `void Simplify();` [^1]

[^1]: Grammars are simplified by default, but this can be suppressed with a constructor argument)
