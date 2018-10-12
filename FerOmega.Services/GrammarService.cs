using System;
using System.Collections.Generic;
using System.Linq;

using FerOmega.Abstractions;
using FerOmega.Entities;
using FerOmega.Common;

namespace FerOmega.Services
{
    public class GrammarService : IGrammarService
    {
        private readonly OperatorType[] closeBrackets =
        {
            OperatorType.CloseCurlyBracket,
            OperatorType.CloseRoundBracket,
            OperatorType.CloseSquareBracket,
        };

        private readonly OperatorType[] openBrackets =
        {
            OperatorType.OpenCurlyBracket,
            OperatorType.OpenRoundBracket,
            OperatorType.OpenSquareBracket,
        };

        public string[] BracketsDenotations { get; }

        public Operator CloseEscapeOperator { get; }

        public Operator OpenEscapeOperator { get; }

        public string[] OperatorDenotations { get; }

        public IList<Operator> Operators { get; }

        public GrammarService()
        {
            Operators = new List<Operator>();
            SetOperators();

            OpenEscapeOperator = Get(OperatorType.OpenSquareBracket);
            CloseEscapeOperator = Get(OperatorType.CloseSquareBracket);

            OperatorDenotations = Operators.SelectMany(x => x.Denotations).ToArray();
            BracketsDenotations = Operators.Where(IsBracket).SelectMany(x => x.Denotations).ToArray();

            CheckOperators();
        }

        public Operator Get(OperatorType operatorType)
        {
            if (!Enum.IsDefined(typeof(OperatorType), operatorType))
            {
                throw new ArgumentOutOfRangeException(nameof(operatorType), operatorType, $"Enum {nameof(OperatorType)} is out of range");
            }

            return Operators.First(x => x.OperatorType == operatorType).DeepClone();
        }

        public Operator[] GetOperatorsForSection(GrammarSectionType grammarSectionType)
        {
            if (!grammarSectionType.ToInt().IsDefined<GrammarSectionType>())
            {
                throw new ArgumentOutOfRangeException(nameof(grammarSectionType), grammarSectionType, $"Enum {nameof(GrammarSectionType)} is out of range");
            }

            var result = new List<Operator>(16);

            foreach (var enumValue in Enum.GetValues(typeof(GrammarSectionType)).Cast<GrammarSectionType>())
            {
                if (grammarSectionType.HasFlag(enumValue))
                {
                    result.AddRange(Operators.Where(x => x.GrammarSectionType.HasFlag(enumValue)));
                }
            }

            return result.ToArray();
        }

        public Operator[] GetPossibleOperators(string denotation)
        {
            return Operators.Where(x => x.Denotations.Contains(denotation)).ToArray();
        }

        public bool IsBracket(OperatorType operatorType)
        {
            return IsOpenBracket(operatorType) || IsCloseBracket(operatorType);
        }

        public bool IsBracket(string denotation)
        {
            return IsOpenBracket(denotation) || IsCloseBracket(denotation);
        }

        public bool IsBracket(AbstractToken @operator)
        {
            return IsOpenBracket(@operator) || IsCloseBracket(@operator);
        }

        public bool IsCloseBracket(OperatorType operatorType)
        {
            return closeBrackets.Contains(operatorType);
        }

        public bool IsCloseBracket(string denotation)
        {
            return Operators.Where(x => closeBrackets.Contains(x.OperatorType)).SelectMany(x => x.Denotations).Contains(denotation);
        }

        public bool IsCloseBracket(AbstractToken @operator)
        {
            return IsCloseBracket(@operator.OperatorType);
        }

        public bool IsOpenBracket(OperatorType operatorType)
        {
            return openBrackets.Contains(operatorType);
        }

        public bool IsOpenBracket(string denotation)
        {
            return Operators.Where(x => openBrackets.Contains(x.OperatorType)).SelectMany(x => x.Denotations).Contains(denotation);
        }

        public bool IsOpenBracket(AbstractToken @operator)
        {
            return IsOpenBracket(@operator.OperatorType);
        }

        public bool IsOperand(string input)
        {
            var isEscaped = input.StartsWith(OpenEscapeOperator.MainDenotation, StringComparison.Ordinal)
                            && input.EndsWith(CloseEscapeOperator.MainDenotation, StringComparison.Ordinal);

            var isInOperatorsDenotations = OperatorDenotations.Contains(input);

            return isEscaped || !isInOperatorsDenotations;
        }

        public bool IsOperator(string input)
        {
            return !IsOperand(input) && OperatorDenotations.Contains(input);
        }

        public bool IsUniqueByArity(string denotation, ArityType arity)
        {
            return Operators.Count(x => x.Denotations.Contains(denotation) && x.Arity == arity) == 1;
        }

        public bool IsUniqueByDenotation(string denotation)
        {
            return GetPossibleOperators(denotation).Length == 1;
        }

        public bool IsUniqueByFixity(string denotation, FixityType fixity)
        {
            return Operators.Count(x => x.Denotations.Contains(denotation) && x.Fixity == fixity) == 1;
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

        private void SetOperators()
        {
            // there are less priority operators at the bottom and more priority operators at the top
            var priority = 1;

            priority = AddOperators(priority,
                new Operator(ArityType.Unary, AssociativityType.Left, OperatorType.Factorial, FixityType.Postfix, GrammarSectionType.ArithmeticAlgebra, "!"),
                new Operator(ArityType.Unary, AssociativityType.Right, OperatorType.Not, FixityType.Prefix, GrammarSectionType.BooleanAlgebra, "!", "not"),
                new Operator(ArityType.Unary, AssociativityType.Right, OperatorType.UnaryPlus, FixityType.Prefix, GrammarSectionType.ArithmeticAlgebra, "+"),
                new Operator(ArityType.Unary, AssociativityType.Right, OperatorType.UnaryMinus, FixityType.Prefix, GrammarSectionType.ArithmeticAlgebra, "-"),
                new Operator(ArityType.Unary, AssociativityType.Right, OperatorType.Invert, FixityType.Prefix, GrammarSectionType.BooleanAlgebra, "~"));

            priority = AddOperators(priority,
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.Multiple, FixityType.Infix, GrammarSectionType.ArithmeticAlgebra, "*"),
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.Divide, FixityType.Infix, GrammarSectionType.ArithmeticAlgebra, "/"),
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.Reminder, FixityType.Infix, GrammarSectionType.ArithmeticAlgebra, "%"));

            priority = AddOperators(priority,
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.Plus, FixityType.Infix, GrammarSectionType.ArithmeticAlgebra, "+"),
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.Minus, FixityType.Infix, GrammarSectionType.ArithmeticAlgebra, "-"));

            priority = AddOperators(priority,
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.GreaterThan, FixityType.Infix, GrammarSectionType.Inequality, ">", "gt"),
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.LesserThan, FixityType.Infix, GrammarSectionType.Inequality, "<", "lt"),
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.GreaterOrEqualsThan, FixityType.Infix, GrammarSectionType.Inequality, ">=", "gte"),
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.LesserOrEqualsThan, FixityType.Infix, GrammarSectionType.Inequality, "<=", "lte"));

            priority = AddOperators(priority,
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.Equals, FixityType.Infix, GrammarSectionType.Equality, "==", "eq"),
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.NotEquals, FixityType.Infix, GrammarSectionType.Inequality, "!=", "<>", "neq"));

            priority = AddOperators(priority,
                new Operator(ArityType.Multiarity, AssociativityType.Left, OperatorType.InRange, FixityType.Infix, GrammarSectionType.Unknown, "in"),
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.Contains, FixityType.Infix, GrammarSectionType.Unknown, "con"),
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.StartsWith, FixityType.Infix, GrammarSectionType.Unknown, "stw"),
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.EndsWith, FixityType.Infix, GrammarSectionType.Unknown, "edw"),
                new Operator(ArityType.Unary, AssociativityType.Right, OperatorType.Empty, FixityType.Prefix, GrammarSectionType.Unknown, "emp"),
                new Operator(ArityType.Unary, AssociativityType.Right, OperatorType.NotEmpty, FixityType.Prefix, GrammarSectionType.Unknown, "nep"));

            priority = AddOperators(priority,
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.And, FixityType.Infix, GrammarSectionType.BooleanAlgebra, "&", "&&", "and"));

            priority = AddOperators(priority,
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.Xor, FixityType.Infix, GrammarSectionType.BooleanAlgebra, "^"));

            priority = AddOperators(priority,
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.Or, FixityType.Infix, GrammarSectionType.BooleanAlgebra, "|", "||", "or"));

            priority = AddOperators(priority,
                new Operator(ArityType.Multiarity, AssociativityType.Left, OperatorType.OpenRoundBracket, FixityType.Circumflex, GrammarSectionType.Unknown, "("),
                new Operator(ArityType.Multiarity, AssociativityType.Left, OperatorType.CloseRoundBracket, FixityType.Circumflex, GrammarSectionType.Unknown, ")"));

            priority = AddOperators(priority,
                new Operator(ArityType.Multiarity, AssociativityType.Left, OperatorType.OpenCurlyBracket, FixityType.Circumflex, GrammarSectionType.Unknown, "{"),
                new Operator(ArityType.Multiarity, AssociativityType.Left, OperatorType.CloseCurlyBracket, FixityType.Circumflex, GrammarSectionType.Unknown, "}"));

            priority = AddOperators(priority,
                new Operator(ArityType.Unary, AssociativityType.Left, OperatorType.OpenSquareBracket, FixityType.Circumflex, GrammarSectionType.Unknown, "["),
                new Operator(ArityType.Unary, AssociativityType.Left, OperatorType.CloseSquareBracket, FixityType.Circumflex, GrammarSectionType.Unknown, "]"));
        }
    }
}