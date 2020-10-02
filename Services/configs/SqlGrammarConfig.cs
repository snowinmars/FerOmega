using System.Linq;
using FerOmega.Entities.InternalSyntax;
using FerOmega.Entities.InternalSyntax.Enums;
using FerOmega.Services.Abstractions;

namespace FerOmega.Services.configs
{
    public class SqlGrammarConfig : AbstractGrammarConfig, IGrammarConfig
    {
        public Operator[] ConfigOperators()
        {
            // there are less priority operators at the bottom and more priority operators at the top
            var priority = 1;

            priority = AddOperatorGroup(priority,
                                        new Operator(Arity.Unary,
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
                                                     "-"));

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
                                                     "<="));

            priority = AddOperatorGroup(priority,
                                        new Operator(Arity.Binary,
                                                     Associativity.Left,
                                                     OperatorType.Equals,
                                                     Fixity.Infix,
                                                     "="),
                                        new Operator(Arity.Binary,
                                                     Associativity.Left,
                                                     OperatorType.NotEquals,
                                                     Fixity.Infix,
                                                     "<>"));

            priority = AddOperatorGroup(priority,
                                        new Operator(Arity.Multiarity,
                                                     Associativity.Left,
                                                     OperatorType.InRange,
                                                     Fixity.Infix,
                                                     "in"));

            priority = AddOperatorGroup(priority,
                                        new Operator(Arity.Binary,
                                                     Associativity.Left,
                                                     OperatorType.And,
                                                     Fixity.Infix,
                                                     "&",
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

            priority = AddOperatorGroup(priority,
                                        new Operator(Arity.Unary,
                                                     Associativity.Left,
                                                     OperatorType.OpenEscapeOperator,
                                                     Fixity.Circumflex,
                                                     "["),
                                        new Operator(Arity.Unary,
                                                     Associativity.Left,
                                                     OperatorType.CloseEscapeOperator,
                                                     Fixity.Circumflex,
                                                     "]"));

            return Operators.ToArray();
        }
    }
}