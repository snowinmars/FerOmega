using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using FerOmega.Entities;

namespace FerOmega.Services
{
    public class GrammarService
    {
        public string[] BracketsDenotations { get; }

        public Operator CloseEscapeOperator { get; }

        public Operator OpenEscapeOperator { get; }

        public string[] OperatorDenotations { get; }

        public IList<Operator> Operators { get; }

        public GrammarService()
        {
            Operators = new List<Operator>();
            SetOperators();

            OpenEscapeOperator = GetOperator(OperatorType.OpenSquareBracket);
            CloseEscapeOperator = GetOperator(OperatorType.CloseSquareBracket);

            OperatorDenotations = Operators.SelectMany(x => x.Denotations).ToArray();
            BracketsDenotations = Operators.Where(IsBracket).SelectMany(x => x.Denotations).ToArray();

            CheckOperators();
        }


        private readonly OperatorType[] openBrackets =
        {
            OperatorType.OpenCurlyBracket,
            OperatorType.OpenRoundBracket,
            OperatorType.OpenSquareBracket,
        };

        private readonly OperatorType[] closeBrackets =
        {
            OperatorType.CloseCurlyBracket,
            OperatorType.CloseRoundBracket,
            OperatorType.CloseSquareBracket,
        };

        public bool IsOpenBracket(OperatorType operatorType)
        {
            return openBrackets.Contains(operatorType);
        }

        public bool IsCloseBracket(OperatorType operatorType)
        {
            return closeBrackets.Contains(operatorType);
        }

        public bool IsBracket(OperatorType operatorType)
        {
            return IsOpenBracket(operatorType) || IsCloseBracket(operatorType);
        }

        public bool IsOpenBracket(AbstractToken @operator)
        {
            return IsOpenBracket(@operator.OperatorType);
        }

        public bool IsCloseBracket(AbstractToken @operator)
        {
            return IsCloseBracket(@operator.OperatorType);
        }

        public bool IsBracket(AbstractToken @operator)
        {
            return IsOpenBracket(@operator) || IsCloseBracket(@operator);
        }

        public bool IsOperand(string input)
        {
            return input.StartsWith(OpenEscapeOperator.MainDenotation, StringComparison.Ordinal)
                   && input.EndsWith(CloseEscapeOperator.MainDenotation, StringComparison.Ordinal);
        }

        public bool IsOperator(string input)
        {
            return !IsOperand(input) && OperatorDenotations.Contains(input);
        }

        private int AddOperators(int priority, params Operator[] operators)
        {
            foreach (var @operator in operators)
            {
                @operator.Priority = priority;
                Operators.Add(@operator);
            }

            return ++priority;
        }

        private void CheckOperators()
        {
            var isOperatorTypePrimaryKey = Operators.Select(x => x.OperatorType).Distinct().Count() == Operators.Count;

            if (!isOperatorTypePrimaryKey)
            {
                throw new InvalidOperationException();
            }

            var isNonExistingOperatorExists = OperatorDenotations.Any(x => x == OperatorToken.NonExistingOperator);

            if (isNonExistingOperatorExists)
            {
                throw new InvalidOperationException();
            }
        }

       

       

        private Operator GetOperator(OperatorType type)
        {
            if (!Enum.IsDefined(typeof(OperatorType), type))
            {
                throw new ArgumentOutOfRangeException(nameof(type), type, $"Enum {nameof(OperatorType)} is out of range");
            }

            return Operators.First(x => x.OperatorType == type);
        }

        



      

        private void SetOperators()
        {
            // there are less priority operators at the bottom and more priority operators at the top
            var priority = 1;

            priority = AddOperators(priority,
                new Operator(ArityType.Unary, AssociativityType.Right, OperatorType.Not, FixityType.Prefix, "!", "not"),
                new Operator(ArityType.Unary, AssociativityType.Right, OperatorType.UnaryPlus, FixityType.Prefix, "+"),
                new Operator(ArityType.Unary, AssociativityType.Right, OperatorType.UnaryMinus, FixityType.Prefix, "-"),
                new Operator(ArityType.Unary, AssociativityType.Right, OperatorType.Invert, FixityType.Prefix, "~"));

            priority = AddOperators(priority,
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.Multiple, FixityType.Infix, "*"),
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.Divide, FixityType.Infix, "/"),
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.Reminder, FixityType.Infix, "%"));

            priority = AddOperators(priority,
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.Plus, FixityType.Infix, "+"),
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.Minus, FixityType.Infix, "-"));

            priority = AddOperators(priority,
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.GreaterThan, FixityType.Infix, ">", "gt"),
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.LesserThan, FixityType.Infix, "<", "lt"),
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.GreaterOrEqualsThan, FixityType.Infix, ">=", "gte"),
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.LesserOrEqualsThan, FixityType.Infix, "<=", "lte"));

            priority = AddOperators(priority,
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.Equals, FixityType.Infix, "==", "eq"),
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.NotEquals, FixityType.Infix, "!=", "<>", "neq"));

            priority = AddOperators(priority,
                new Operator(ArityType.Multiarity, AssociativityType.Right, OperatorType.InRange, FixityType.Infix, "in"),
                new Operator(ArityType.Binary, AssociativityType.Right, OperatorType.Contains, FixityType.Infix, "con"),
                new Operator(ArityType.Binary, AssociativityType.Right, OperatorType.StartsWith, FixityType.Infix, "stw"),
                new Operator(ArityType.Binary, AssociativityType.Right, OperatorType.EndsWith, FixityType.Infix, "edw"),
                new Operator(ArityType.Unary, AssociativityType.Right, OperatorType.Empty, FixityType.Prefix, "emp"),
                new Operator(ArityType.Unary, AssociativityType.Right, OperatorType.NotEmpty, FixityType.Prefix, "nep"));

            priority = AddOperators(priority,
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.And, FixityType.Infix, "&", "&&", "and"));

            priority = AddOperators(priority,
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.Xor, FixityType.Infix, "^"));

            priority = AddOperators(priority,
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.Or, FixityType.Infix, "|", "||", "or"));

            priority = AddOperators(priority,
                new Operator(ArityType.Multiarity, AssociativityType.Left, OperatorType.OpenRoundBracket, FixityType.Circumflex, "("),
                new Operator(ArityType.Multiarity, AssociativityType.Left, OperatorType.CloseRoundBracket, FixityType.Circumflex, ")"));

            priority = AddOperators(priority,
                new Operator(ArityType.Multiarity, AssociativityType.Left, OperatorType.OpenCurlyBracket, FixityType.Circumflex, "{"),
                new Operator(ArityType.Multiarity, AssociativityType.Left, OperatorType.CloseCurlyBracket, FixityType.Circumflex, "}"));

            priority = AddOperators(priority,
                new Operator(ArityType.Unary, AssociativityType.Left, OperatorType.OpenSquareBracket, FixityType.Circumflex, "["),
                new Operator(ArityType.Unary, AssociativityType.Left, OperatorType.CloseSquareBracket, FixityType.Circumflex, "]"));
        }
    }
}