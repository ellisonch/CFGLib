# CFGLib 
A C# library for (Probabilistic) Context Free Grammars (PCFGs).
Supports probability-preserving conversion to Chomsky Normal Form (CNF), as well as the CYK and Earley parsing algorithms.
These algorithms allow you to tell whether or not a grammar could have generated a given string, and if so, at what probability they are generated.
In addition, the Earley algorithm supports returning a parse forest (containing all possible parse trees) in Shared Packed Parse Forest (SPPF) format.

Earley is significantly faster, while CYK is simpler and the code is easier to understand.
Having both allows me to test them against one another.

## License
CFGLib is licensed using the [MIT license](LICENSE.txt).

## Examples
CFGLib can perform a number of basic operations on context free grammars.
If there are some operations you need, feel free to suggest them on the [Issues](https://github.com/ellisonch/CFGLib/issues) page!

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
var ep = new EarleyParser(cfg);

// How frequently does the grammar generate the string "aabb"?
Console.WriteLine("aabb: {0}", ep.ParseGetProbability(Sentence.FromLetters("aabb")));
// How about "abba"?
Console.WriteLine("abba: {0}", ep.ParseGetProbability(Sentence.FromLetters("abba")));
```
```
aabb: 0
abba: 0.0233236151603499
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

### Parse Forests
Consider the following simple, but ambiguous grammar:
```
<S> → <S> '+' <S>
<S> → '0'
````
Using this grammar to parse the string `0 + 0 + 0` yields a parse forest, not just a parse tree, because the parsing is ambiguous.
That is, it could either be grouped like `(0 + 0) + 0` or `0 + (0 + 0)`.
The tool returns a shared packed parse forest (SPPF):
<a href="https://github.com/ellisonch/CFGLib/blob/master/wiki/additionRaw.png"><img alt="Addition SPPF" src="https://github.com/ellisonch/CFGLib/blob/master/wiki/additionRaw.png" width="600"/></a>

or for convenience (but not efficiency), a flattened version:
<a href="https://github.com/ellisonch/CFGLib/blob/master/wiki/additionFlat.png"><img alt="Addition SPPF Flattened" src="https://github.com/ellisonch/CFGLib/blob/master/wiki/additionFlat.png" width="600"/></a>

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
I used randomly generated grammars and both parsing algorithms to help test and make sure everything is correct.

## Caveats
There are still a few weaknesses:
* No support for EBNF
* No easy to use forest visitor


## Other Useful Classes and Methods
Here are some other useful classes and methods, in addition to the ones exhibited above.
* `Sentence`: A list of `Nonterminal`s and `Terminal`s
  * `static Sentence FromWords(string s);` (tokenizes on space)
  * `static Sentence FromTokens(IEnumerable<string> tokens);`
* `BaseGrammar` > {`Grammar`, `CNFGrammar`}: All kinds of grammars inherit from `BaseGrammar`.
  * `Sentence ProduceRandom();`
  * `CNFGrammar ToCNF();`: Returns an equivalent grammar in CNF form
  * `double EstimateProbabilityNull(Nonterminal nt, long iterations);`
  * `void Simplify();` [Note: Grammars are simplified by default, but if you edit a production afterwards (making it unproducing), you might need to force a simplification]
* `Parser` > {`EarleyParser`, `CykParser`}: The currently implemented parsers
  * `Sppf ParseGetForest();`
