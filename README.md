# FerOmega
Data query language provider, inspired by oData.

The basic idea is to allow a developer to write some equation in frontend, send it to backend and consume it as an abstraction syntax tree, or sql, or C# expression tree or whatever.

## Status

Unstable beta.

The core operators works: `+`, `-`, `*`, `/`, `%`, `>`, `>=`, `<`, `<=`, `=`, `!=`, `()`, `[]`, `contains`, `startsWith`, `endsWith`, `in`.

See [examples](TranslationExamples.md).

## Installation

Currently I don't recommend you to install it via nuget or whatever. It's simply unstable.

I'll fix some issues, test it on my prod and then I'll provide an instructions.

Feel free to copypaste it, if you'd like too. Check out the nuget `FerOmega` package, but on your own risk.

## Basic usage
```csharp
// get these items from anywhere (see 'Dependency injections' section below)
ITokenizationService tokenizationService;
IAstService astService;
ISqlProvider sqlProvider;

const string equation = "[id] === [1690ffef-7249-4384-8cba-58842e8d48df] and (([length] + 1) * 2 <= 14 or [email] = [email])"; // [] means escaping

string[] tokens = tokenizationService.Tokenizate(equation);
Tree<AbstractToken> tree = astService.Convert(tokens);

PropertyDef[] allowedProperties = new[] 
{
  sqlProvider.DefineProperty().From("id").ToSql("id"),
  sqlProvider.DefineProperty().From("length").ToSql("table.length"),
  sqlProvider.DefineProperty().From("email").ToSql("table2.email"),
};

(SqlFilter where, object[] parameters) = sqlProvider.Convert(tree, allowedProperties);
// "where id = @4 and ( ( table.length + @3 ) * @2 <= @1 or table2.email = @0 )"
// ["email", 14, 2, 1, Guid.Parse("1690ffef-7249-4384-8cba-58842e8d48df")]

where.AppPage(0).AddCount(10); // adds 'limit 10 offset 0'

// use it in your query it any way kinda like
const string sql = @"select * from table";
db.Execute($"{sql} {where}", parameters);
```

Override any part of the flow if you have to.

- `TokenizationService` should parse string into a list of tokens. This is how you say that in `"-1--1=0` is `"-" "1" "-" "-" "1" "=" "0"`
- `AstService` convert tokens into abstract syntax tree using extended shunting yarn algorithm. It could wrap any literal with escape symbols.
- `SqlProvider` converts tree into sql string and parameters. It consumes allowed properies to understand what literal should be extracted into sql parameters.

## What I don't like

- `db.Execute($"{sql} where {where}");` seems like a bad solution. I mean, not in security, but it's easy to misplace something here. Should I provide a builder?
- How to call sql functions?

## Dependency injections

It should cover 90% of cases:

#### As instances

It will provide default implementations as singletons.

```csharp
// main
var tokenizationService = FerOmegaInjections.TokenizationService;
var astService = FerOmegaInjections.AstService;
var sqlProvider = FerOmegaInjections.SqlProvider;

// extra
var internalGrammarConfig = FerOmegaInjections.InternalGrammarConfig;
var internalGrammarService = FerOmegaInjections.InternalGrammarService;
var operatorService = FerOmegaInjections.OperatorService;
var sqlGrammarConfig = FerOmegaInjections.SqlGrammarConfig;
var sqlGrammarService = FerOmegaInjections.SqlGrammarService;
```

#### As services

It will provide default implementations as singletons.

```csharp
public void ConfigureServices(IServiceCollection services)
{
  services.AddFerOmega();
}
```

## Sql injections

It looks like it's possible to make it fully secured.

## Syntax
The basic syntax (operator, etc.) describes in `/Services/configs/InternalGrammarConfig.cs`. You can override it with your own grammar config.

If you do it, please, run all tests on your configuration.

```csharp
IGrammarConfig customGrammarConfig = new CustomGrammarConfig();
IGrammarService<CustomGrammarConfig> customGrammarService = new GrammarService<CustomGrammarConfig>();
ITokenizationService tokenizationService  = new TokenizationService(customGrammarService);
```

## Providers

For now, the only one provider exists: sql provider. But it's possible to implement any ast>custom syntax provider.

## Operators
Every operator has arity, fixity, associativity, priority and denominations.

**Arity** - how many operands the operator consumes.
Could be 
  * *Nulary*: operator doesn't consumes operands
  * *Unary*: operator consumes one operand (like ! (factorial))
  * *Binary*: operator consumes two operands (like + (binary plus))
  * *Ternary*: operator consumes three operands (like ?: (ternary))
  * *Kvatery*: operator consumes four operands
  * *Multiarity*: operator consumes several operands (like round brackets: its' operands are other operators and it consumes several operands)

**Fixity** - operator positioning
Could be
  * *Prefix*: operator before operand (like - (unary minus))
  * *Postfix*: operator after operand (like ! (factorial))
  * *Infix*: operator between operands (like * (multiply))
  * *Circumflex*: operator around operands (like brackets)
  * *Postcircumflex*: postfix operator to first operand and circumflex operator to second operand (like array iterator: arr[i]: 'arr' and 'i' are operands)

**Associativity** - Should the operator be calculated from left to right or from right to left

**Priority** - operator with priority 3 will be calculated before operator with priority 5

**Denominations** - symbols that presents the operator. One operator can have several denominations. One denomination can present several operators (like '+' is unary and binary plus). Each operator has a main denomination.

#### List
- `+2` = 2
- `-2` = -2

- `2 + 3` = 5
- `2 - 3` = -1
- `2 * 3` = 6
- `2 / 3` = 0
- `2 % 3` = 2
- `2 + 2 * 2` = 6
- `(2 + 2) * 2` = 8

- `2 > 2` = false
- `2 >= 2` = true
- `2 < 2` = false
- `2 <= 2` = true
- `2 == 2` = true
- `2 != 2` = false

- `true and false` = false
- `true or false` = true
- `true xor false` = true
- `not true` = false

- `[name] contains [am]` = true
- `[name] startsWith [am]` = false
- `[name] endsWith [am]` = false
- `2 in (1, 2, 3)` = true
