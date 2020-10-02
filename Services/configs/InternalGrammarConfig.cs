using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Entities.InternalSyntax;
using Entities.InternalSyntax.Enums;
using Services.Abstractions;

namespace Services
{
    public class InternalGrammarConfig : AbstractGrammarConfig, IGrammarConfig
    {
        public Operator[] ConfigOperators()
        {
            // there are less priority operators at the bottom and more priority operators at the top
            var priority = 1;

            priority = AddOperatorGroup(priority,
                                        new Operator(Arity.Unary,
                                                     Associativity.Left,
                                                     OperatorType.Factorial,
                                                     Fixity.Postfix,

                                                     "!"),
                                        new Operator(Arity.Unary,
                                                     Associativity.Right,
                                                     OperatorType.Not,
                                                     Fixity.Prefix,

                                                     "!",
                                                     "not"),
                                        new Operator(Arity.Unary,
                                                     Associativity.Right,
                                                     OperatorType.UnaryPlus,
                                                     Fixity.Prefix,

                                                     "+"),
                                        new Operator(Arity.Unary,
                                                     Associativity.Right,
                                                     OperatorType.UnaryMinus,
                                                     Fixity.Prefix,

                                                     "-"),
                                        new Operator(Arity.Unary,
                                                     Associativity.Right,
                                                     OperatorType.Invert,
                                                     Fixity.Prefix,

                                                     "~"));

            priority = AddOperatorGroup(priority,
                                        new Operator(Arity.Binary,
                                                     Associativity.Left,
                                                     OperatorType.Multiple,
                                                     Fixity.Infix,

                                                     "*"),
                                        new Operator(Arity.Binary,
                                                     Associativity.Left,
                                                     OperatorType.Divide,
                                                     Fixity.Infix,

                                                     "/"),
                                        new Operator(Arity.Binary,
                                                     Associativity.Left,
                                                     OperatorType.Reminder,
                                                     Fixity.Infix,

                                                     "%"));

            priority = AddOperatorGroup(priority,
                                        new Operator(Arity.Binary,
                                                     Associativity.Left,
                                                     OperatorType.Plus,
                                                     Fixity.Infix,

                                                     "+"),
                                        new Operator(Arity.Binary,
                                                     Associativity.Left,
                                                     OperatorType.Minus,
                                                     Fixity.Infix,

                                                     "-"));

            priority = AddOperatorGroup(priority,
                                        new Operator(Arity.Binary,
                                                     Associativity.Left,
                                                     OperatorType.GreaterThan,
                                                     Fixity.Infix,

                                                     ">",
                                                     "gt"),
                                        new Operator(Arity.Binary,
                                                     Associativity.Left,
                                                     OperatorType.LesserThan,
                                                     Fixity.Infix,

                                                     "<",
                                                     "lt"),
                                        new Operator(Arity.Binary,
                                                     Associativity.Left,
                                                     OperatorType.GreaterOrEqualsThan,
                                                     Fixity.Infix,

                                                     ">=",
                                                     "gte"),
                                        new Operator(Arity.Binary,
                                                     Associativity.Left,
                                                     OperatorType.LesserOrEqualsThan,
                                                     Fixity.Infix,

                                                     "<=",
                                                     "lte"));

            priority = AddOperatorGroup(priority,
                                        new Operator(Arity.Binary,
                                                     Associativity.Left,
                                                     OperatorType.Equals,
                                                     Fixity.Infix,

                                                     "=",
                                                     "==",
                                                     "eq"),
                                        new Operator(Arity.Binary,
                                                     Associativity.Left,
                                                     OperatorType.NotEquals,
                                                     Fixity.Infix,

                                                     "!=",
                                                     "<>",
                                                     "neq"));

            priority = AddOperatorGroup(priority,
                                        new Operator(Arity.Multiarity,
                                                     Associativity.Left,
                                                     OperatorType.InRange,
                                                     Fixity.Infix,

                                                     "in"),
                                        new Operator(Arity.Binary,
                                                     Associativity.Left,
                                                     OperatorType.Contains,
                                                     Fixity.Infix,

                                                     "contains"),
                                        new Operator(Arity.Binary,
                                                     Associativity.Left,
                                                     OperatorType.StartsWith,
                                                     Fixity.Infix,

                                                     "startsWith"),
                                        new Operator(Arity.Binary,
                                                     Associativity.Left,
                                                     OperatorType.EndsWith,
                                                     Fixity.Infix,

                                                     "endsWith"));
                                       
            priority = AddOperatorGroup(priority,
                                        new Operator(Arity.Binary,
                                                     Associativity.Left,
                                                     OperatorType.And,
                                                     Fixity.Infix,

                                                     "&",
                                                     "&&",
                                                     "and"));

            priority = AddOperatorGroup(priority,
                                        new Operator(Arity.Binary,
                                                     Associativity.Left,
                                                     OperatorType.Xor,
                                                     Fixity.Infix,

                                                     "^"));

            priority = AddOperatorGroup(priority,
                                        new Operator(Arity.Binary,
                                                     Associativity.Left,
                                                     OperatorType.Or,
                                                     Fixity.Infix,

                                                     "|",
                                                     "||",
                                                     "or"));

            priority = AddOperatorGroup(priority,
                                        new Operator(Arity.Multiarity,
                                                     Associativity.Left,
                                                     OperatorType.OpenPriorityBracket,
                                                     Fixity.Circumflex,

                                                     "("),
                                        new Operator(Arity.Multiarity,
                                                     Associativity.Left,
                                                     OperatorType.ClosePriorityBracket,
                                                     Fixity.Circumflex,

                                                     ")"));

            return Operators.ToArray();
        }
    }
}