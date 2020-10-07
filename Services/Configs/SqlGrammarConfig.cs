using System;
using System.Collections.Generic;
using System.Linq;
using FerOmega.Entities.InternalSyntax;
using FerOmega.Entities.InternalSyntax.Enums;
using FerOmega.Services.Abstractions;

namespace FerOmega.Services.configs
{
    internal class SqlGrammarConfig : AbstractGrammarConfig, IGrammarConfig
    {
        public Operator[] ConfigOperators()
        {
            // there are less priority operators at the bottom and more priority operators at the top
            var operators = ConfigBuilder.Start()
                                .AddOperatorGroup(new Operator(Arity.Unary,
                                                               Associativity.Right,
                                                               OperatorType.Not,
                                                               Fixity.Prefix,
                                                               "!"),
                                                  new Operator(Arity.Unary,
                                                               Associativity.Right,
                                                               OperatorType.UnaryPlus,
                                                               Fixity.Prefix,
                                                               "+"),
                                                  new Operator(Arity.Unary,
                                                               Associativity.Right,
                                                               OperatorType.UnaryMinus,
                                                               Fixity.Prefix,
                                                               "-"))
                                .AddOperatorGroup(new Operator(Arity.Binary,
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
                                                               "%"))
                                .AddOperatorGroup(new Operator(Arity.Binary,
                                                               Associativity.Left,
                                                               OperatorType.Plus,
                                                               Fixity.Infix,
                                                               "+"),
                                                  new Operator(Arity.Binary,
                                                               Associativity.Left,
                                                               OperatorType.Minus,
                                                               Fixity.Infix,
                                                               "-"))
                                .AddOperatorGroup(new Operator(Arity.Binary,
                                                               Associativity.Left,
                                                               OperatorType.GreaterThan,
                                                               Fixity.Infix,
                                                               ">"),
                                                  new Operator(Arity.Binary,
                                                               Associativity.Left,
                                                               OperatorType.LesserThan,
                                                               Fixity.Infix,
                                                               "<"),
                                                  new Operator(Arity.Binary,
                                                               Associativity.Left,
                                                               OperatorType.GreaterOrEqualsThan,
                                                               Fixity.Infix,
                                                               ">="),
                                                  new Operator(Arity.Binary,
                                                               Associativity.Left,
                                                               OperatorType.LesserOrEqualsThan,
                                                               Fixity.Infix,
                                                               "<="))
                                .AddOperatorGroup(new Operator(Arity.Binary,
                                                               Associativity.Left,
                                                               OperatorType.Equals,
                                                               Fixity.Infix,
                                                               "="),
                                                  new Operator(Arity.Binary,
                                                               Associativity.Left,
                                                               OperatorType.NotEquals,
                                                               Fixity.Infix,
                                                               "<>"))
                                .AddOperatorGroup(new Operator(Arity.Multiarity,
                                                               Associativity.Left,
                                                               OperatorType.InRange,
                                                               Fixity.Infix,
                                                               "in"),
                                                  new Operator(Arity.Binary,
                                                               Associativity.Left,
                                                               OperatorType.Contains,
                                                               Fixity.Infix,
                                                               "like"),
                                                  new Operator(Arity.Binary,
                                                               Associativity.Left,
                                                               OperatorType.EndsWith,
                                                               Fixity.Infix,
                                                               "like"),
                                                  new Operator(Arity.Binary,
                                                               Associativity.Left,
                                                               OperatorType.StartsWith,
                                                               Fixity.Infix,
                                                               "like"))
                                .AddOperatorGroup(new Operator(Arity.Binary,
                                                               Associativity.Left,
                                                               OperatorType.Separator,
                                                               Fixity.Infix,
                                                               ","))
                                .AddOperatorGroup(new Operator(Arity.Binary,
                                                               Associativity.Left,
                                                               OperatorType.And,
                                                               Fixity.Infix,
                                                               "and"))
                                .AddOperatorGroup(new Operator(Arity.Binary,
                                                               Associativity.Left,
                                                               OperatorType.Xor,
                                                               Fixity.Infix,
                                                               "^"))
                                .AddOperatorGroup(new Operator(Arity.Binary,
                                                               Associativity.Left,
                                                               OperatorType.Or,
                                                               Fixity.Infix,
                                                               "or"))
                                .AddOperatorGroup(new Operator(Arity.Multiarity,
                                                               Associativity.Left,
                                                               OperatorType.OpenPriorityBracket,
                                                               Fixity.Circumflex,
                                                               "("),
                                                  new Operator(Arity.Multiarity,
                                                               Associativity.Left,
                                                               OperatorType.ClosePriorityBracket,
                                                               Fixity.Circumflex,
                                                               ")"))
                                .AddOperatorGroup(new Operator(Arity.Unary,
                                                               Associativity.Left,
                                                               OperatorType.OpenEscapeOperator,
                                                               Fixity.Circumflex,
                                                               "["),
                                                  new Operator(Arity.Unary,
                                                               Associativity.Left,
                                                               OperatorType.CloseEscapeOperator,
                                                               Fixity.Circumflex,
                                                               "]"))
                                .Build();

            var wrongOperator = operators.FirstOrDefault(x => x.Denotations.Length != 1);

            if (wrongOperator != default)
            {
                throw new InvalidOperationException($"Sql operators can have only one denotation, check out {wrongOperator}");
            }

            return operators;
        }
    }
}
