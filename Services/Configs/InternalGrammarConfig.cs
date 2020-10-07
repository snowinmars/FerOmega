using System.Collections.Generic;
using System.Linq;
using FerOmega.Entities.InternalSyntax;
using FerOmega.Entities.InternalSyntax.Enums;
using FerOmega.Services.Abstractions;

namespace FerOmega.Services.configs
{
    public class InternalGrammarConfig : IGrammarConfig
    {
        private static IEnumerable<string> Escape(IEnumerable<string> denotations)
        {
            foreach (var denotation in denotations)
            {
                switch (denotation)
                {
                case "+":
                    yield return @"\+";

                    break;

                case "=":
                case "==":
                case "===":
                    yield return "=+";

                    break;

                case "&":
                case "&&":
                    yield return "&+";

                    break;

                case "*":
                    yield return @"\*";

                    break;

                case "^":
                    yield return @"\^";

                    break;

                case "/":
                    yield return @"\/";

                    break;

                case "|":
                case "||":
                    yield return @"\|+";

                    break;

                case "(":
                    yield return @"\(";

                    break;

                case ")":
                    yield return @"\)";

                    break;

                case "[":
                    yield return ""; // ?

                    break;

                case "]":
                    yield return ""; // ?

                    break;

                case "{":
                    yield return @"\{";

                    break;

                case "}":
                    yield return @"\}";

                    break;

                default:
                    yield return denotation;

                    break;
                }
            }
        }

        public Operator[] ConfigOperators()
        {
            // there are less priority operators at the bottom and more priority operators at the top
            return ConfigBuilder.Start()
                                .AddOperatorGroup(new Operator(Arity.Unary,
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
                                                               "~"))
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
                                                               "lte"))
                                .AddOperatorGroup(new Operator(Arity.Binary,
                                                               Associativity.Left,
                                                               OperatorType.Equals,
                                                               Fixity.Infix,
                                                               "=",
                                                               "==",
                                                               "===",
                                                               "eq"),
                                                  new Operator(Arity.Binary,
                                                               Associativity.Left,
                                                               OperatorType.NotEquals,
                                                               Fixity.Infix,
                                                               "!=",
                                                               "!==",
                                                               "<>",
                                                               "neq"))
                                .AddOperatorGroup(new Operator(Arity.Binary,
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
                                                               "endsWith"))
                                .AddOperatorGroup(new Operator(Arity.Binary,
                                                               Associativity.Left,
                                                               OperatorType.Separator,
                                                               Fixity.Infix,
                                                               ","))
                                .AddOperatorGroup(new Operator(Arity.Binary,
                                                               Associativity.Left,
                                                               OperatorType.And,
                                                               Fixity.Infix,
                                                               "&",
                                                               "&&",
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
                                                               "||",
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
                                .AddOperatorGroup(new Operator(Arity.Nulary,
                                                               Associativity.Left,
                                                               OperatorType.Terminator,
                                                               Fixity.Postfix,
                                                               ";"))
                                .Build();
        }

        public string GetOperatorsAsRegex(IEnumerable<string> denotations)
        {
            // OrderByDescending - see the difference between '(!=|!|=)' and '(!|=|!=)' regexes?
            var escapes = Escape(denotations.OrderByDescending(x => x.Length))
                          .Distinct()                                 // I can have overloaded operators
                          .Where(x => !string.IsNullOrWhiteSpace(x)); // I want to ignore some operators

            // f.e.,
            //      input: a>5  &&  b+7  ==2
            //      regex: >|   &+| \+|  =+
            var operatorRegex = string.Join("|", escapes);

            const string valueRegex = "\\[.*?\\]";

            // if regex pattern is in the global scope, the delimiters will be included to the Matches collection
            return $"({valueRegex}|{operatorRegex})";
        }
    }
}
