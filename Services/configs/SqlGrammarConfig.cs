using System.Linq;
using FerOmega.Entities.InternalSyntax;
using FerOmega.Entities.InternalSyntax.Enums;
using FerOmega.Services.Abstractions;

namespace FerOmega.Services.configs
{
    public class SqlGrammarConfig : IGrammarConfig
    {
        public Operator[] ConfigOperators()
        {
            // there are less priority operators at the bottom and more priority operators at the top
            return ConfigBuilder.Start()
                                .AddOperatorGroup(new Operator(Arity.Unary,
                                                               Associativity.Right,
                                                               OperatorType.Not,
                                                               Fixity.Prefix,
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
                                                               "in"))

                                .AddOperatorGroup(new Operator(Arity.Binary,
                                                               Associativity.Left,
                                                               OperatorType.And,
                                                               Fixity.Infix,
                                                               "&",
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
                                                               "|",
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
        }
    }
}