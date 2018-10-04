# FerOmega
Data query language provider

The basic idea is to allow a developer to write some equation in frontend, send it to server and get it as an abstraction syntax tree in backend.

## Developer map
  * [DONE] Define main operators with arity and  fixity overloads
  * [DONE] Implement reverse polish notation algorithm to transform infix equation to abstract syntax tree
  * [EXEC] Test it as deep as possible
  * [TODO] Implement Tree providers to convert abstract syntax tree to
    * c# expression
    * sql code

## Usage
Every operator has arity, fixity, associativity, priority and denominations.

You could combine operators as you used to bases on their returned types like if you want to check, could a child go to a carousel in a park, you can write

`[age] in [(12,13,14,15,16,17)] and [height] > [150] or [hasMother]`

**Arity** - how many operands the operator consumes.
Could be 
  * *Nulary*: operator doesn't consumes operands
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

**Denominations** - symbols that presents the operator.

## Current priority list
TODO check FerOmega.Services.GrammarService::SetOperators() for now

## Operators
  
  * OperatorType.Literal = 1
    * Variable, field or value. Should be explicitly escaped with '[' and ']'. This operator type doesn't includes to the grammar services Operators list
    * Usage: 
      * [name]
      * [54]
  
  * OperatorType.Equals = 2
    * Binary infix operator. 
      * Possible forms: '==', 'eq'
      * Consumes (left/right): T/T
      * Returns bool
    * Usage:
      * [fee] == [5]
      * [name] eq [Boris]
  
  * OperatorType.NotEquals = 3
    * Binary infix operator.
      * Possible forms: '!=', '<>', 'neq'
      * Consumes (left/right): T/T
      * Returns bool
    * Usage:
      * [fee] == [6]
      * [name] eq [Michael]
  
  * OperatorType.Not = 4
    * Unary prefix operator.
      * Possible forms: '!', 'not'
      * Consumes (right): bool
      * Returns bool
    * Usage:
      * ![isDone]
      * not [isBlack]
  
  * OperatorType.GreaterThan = 5
    * Binary infix operator.
      * Possible forms: '>', 'gt'
      * Consumes (left/right): number/number
      * Returns bool
    * Usage:
      * [seconds] > [40]
      * [cSharp] gt [java]
  
  * OperatorType.LesserThan = 6
    * Binary infix operator.
      * Possible forms: '<', 'lt'
      * Consumes (left/right): number/number
      * Returns bool
    * Usage:
      * [count] < 2
      * [cookies] lt [25]
  
  * OperatorType.GreaterOrEqualsThan = 7
    * Binary infix operator.
      * Possible forms: '>=', 'geq'
      * Consumes (left/right): number/number
      * Returns bool
    * Usage:
      * [seconds] >= [50]
      * [money] geq [57]
  
  * OperatorType.LesserOrEqualsThan = 8
    * Binary infix operator.
      * Possible forms: '<=', 'leq'
      * Consumes (left/right): number/number
      * Returns bool
    * Usage:
      * [count] <= 1
      * [cookies] leq [20]
  
  * OperatorType.InRange = 9
    * Binary infix operator.
      * Possible forms: 'in'
      * Consumes (left/right): number/number[]
      * Returns bool
    * Usage:
      * [age] in [(2,3,4,5)]
  
  * OperatorType.And = 10
    * Binary infix operator.
      * Possible forms: '&', '&&', 'and'
      * Consumes (left/right): bool/bool
      * Returns bool
    * Usage:
      * [isBlack] & [isWhite]
      * [isHarry] && [isPotter]
      * [cats] and [cats] and [cats]
  
  * OperatorType.Or = 11
    * Binary infix operator.
      * Possible forms: '|', '||', 'or'
      * Consumes (left/right): bool/bool
      * Returns bool
    * Usage:
      * [be] | ![be]
      * [isDead] || [isAlive]
      * [usePrefixIncrement] or [usePostfixIncrement]
  
  * OperatorType.Xor = 12
    * Binary infix operator.
      * Possible forms: '^' 'xor'
      * Consumes (left/right): bool/bool
      * Returns bool
    * Usage: 
  
  * OperatorType.Contains = 13
    * Binary infix operator.
      * Possible forms: 'con'
      * Consumes (left/right): string/string
      * Returns bool
    * Usage:
      * [myself] con [self]
  
  * OperatorType.StartsWith = 14
    * Binary infix operator.
      * Possible forms: 'stw'
      * Consumes (left/right): string/string
      * Returns bool
    * Usage:
      * [motherland] stw [mo]
  
  * OperatorType.EndsWith = 15
    * Binary infix operator.
      * Possible forms: 'edw'
      * Consumes (left/right): string/string
      * Returns bool
    * Usage:
      * [life] edw [e]
  
  * OperatorType.Empty = 16
    * Unary prefix operator.
      * Possible forms: 'emp'
      * Consumes (right): T
      * Returns bool
    * Usage:
      * emp [soul]
  
  * OperatorType.NotEmpty = 17
    * Unary prefix operator.
      * Possible forms: 'nep'
      * Consumes (right): T
      * Returns bool
    * Usage:
      * nep [mind]
  
  * OperatorType.UnaryPlus = 18
    * Unary prefix operator.
      * Possible forms: '+'
      * Consumes (right): number
      * Returns number
    * Usage:
      * +[5]
  
  * OperatorType.Plus = 19
    * Binary infix operator.
      * Possible forms: '+'
      * Consumes (left/right): number/number
      * Returns number
    * Usage:
      * [2] + [4]
  
  * OperatorType.UnaryMinus = 20
    * Unary prefix operator.
      * Possible forms: '-'
      * Consumes (right): number
      * Returns number
    * Usage:
      * -[5]
  
  * OperatorType.Minus = 21
    * Binary infix operator.
      * Possible forms: '-'
      * Consumes (left/right): number/number
      * Returns number
    * Usage:
      * [6] - [8]
  
  * OperatorType.Multiple = 22
    * Binary infix operator.
      * Possible forms: '\*'
      * Consumes (left/right): number/number
      * Returns number
    * Usage:
      * [4] * [4]
  
  * OperatorType.Reminder = 23
    * Binary infix operator.
      * Possible forms: '%'
      * Consumes (left/right): number/number
      * Returns number
    * Usage:
      * [4] % [3]
  
  * OperatorType.Divide = 24
    * Binary infix operator.
      * Possible forms: '/'
      * Consumes (left/right): number/number
      * Returns number
    * Usage:
      * [4] / [1]
  
  * OperatorType.Invert = 25
    * Binary infix operator.
      * Possible forms: '~'
      * Consumes (right): bool
      * Returns bool
    * Usage:
      * ~[isOn]
  
  * OperatorType.OpenRoundBracket = 26
    * Multiarity circumflex operator.
      * Possible forms: '('
      * Consumes (some): operators
      * Returns T
  
  * OperatorType.CloseRoundBracket = 27
    * Multiarity circumflex operator.
      * Possible forms: ')'
      * Consumes (some): operators
      * Returns T
  
  * OperatorType.OpenCurlyBracket = 28
    * Multiarity circumflex operator.
      * Possible forms: '{'
      * To be developed
  
  * OperatorType.CloseCurlyBracket = 29
    * Multiarity circumflex operator.
      * Possible forms: '}'
      * To be developed
  
  * OperatorType.OpenSquareBracket = 30
    * Multiarity circumflex operator.
      * Possible forms: '['
      * To be developed
  
  * OperatorType.CloseSquareBracket = 31
    * Multiarity circumflex operator.
      * Possible forms: ']'
      * To be developed
  
  * OperatorType.Factorial = 32   
    * Unary postfix operator.
      * Possible forms: '!'
      * Consumes (left): number
      * Returns number
    * Usage:
      * ![0]