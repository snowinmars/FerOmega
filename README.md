# FerOmega
Data query language provider

The basic idea is to allow a developer to write some equation in frontend, send it to server and get it as an abstraction syntax tree/sql/C# expression/etc in backend.

## Basic usage
```
// di
ITokenizationService tokenizationService;
IAstService astService;
ISqlProvider sqlProvider;

const string equation = "count > 3 and (length + 1) * 2 === 14"

string[] tokens = tokenizationService.Tokenizate(equation);
Tree<AbstractToken> tree = astService.Convert(tokens);

string[] allowedProperties = new[] 
{
  "count",
  "length",
}

(string sql, object[] parameters) = sqlProvider.Convert(tree, allowedProperties);
// "[conut] > @1 and ([length] + @2) * @3 === @4", [3, 1, 2, 14]
```

Override any part of the flow if you have to:
- `TokenizationService` should parse string into a list of tokens. This is how you say that in `"-1--1=0` is `"-" "1" "-" "-" "1" "=" "0"`
- `AstService` convert tokens into abstract syntax tree. It could wrap any literal with escape symbols.
- `SqlProvider` converts tree into sql string and parameters. It consumes allowed properies to understand what literal should be extracted into sql parameters.

## Syntax
The basic syntax describes in `/Services/configs/InternalGrammarConfig.cs`. You can override it with your own grammar config.

If you do it, please, run all tests on your configuration.

```
IGrammarConfig customGrammarConfig = new CustomGrammarConfig();
IGrammarService<CustomGrammarConfig> customGrammarService = new GrammarService<CustomGrammarConfig>();
ITokenizationService tokenizationService  = new TokenizationService(customGrammarService);
```

## Operators
Every operator has arity, fixity, associativity, priority and denominations.

**Arity** - how many operands the operator consumes.
Could be 
  * *Nulary*: operator doesn't consumes operands (like `,` in `[1, 2]`)
  * *Unary*: operator consumes one operand (like factorial)
  * *Binary*: operator consumes two operands (like plus)
  * *Ternary*: operator consumes three operands (like ?:)
  * *Kvatery*: operator consumes four operands
  * *Multiarity*: operator consumes several operands (like round brackets: its' operands are other operators and it consumes several operands)

**Fixity** - operator positioning
Could be
  * *Prefix*: operator before operand (like unary minus)
  * *Postfix*: operator after operand (like factorial)
  * *Infix*: operator between operands (like multiply)
  * *Circumflex*: operator around operands (like brackets)
  * *Postcircumflex*: postfix operator to first operand and circumflex operator to second operand (like array itterator: arr[i]: 'arr' and 'i' are operands)

**Assotiativity** - Should the operator be calculated from left to right or from right to left

**Priority** - operator with priority 3 will be calculated before operator with priority 5

**Denominations** - symbols that presents the operator. One operator can have several denominations. One denomination can present several operators (like '+' is unary and binary plus). Each operator has a main denomination.
