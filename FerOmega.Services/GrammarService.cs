﻿using System;
using System.Collections.Generic;
using System.Linq;

using FerOmega.Abstractions;
using FerOmega.Entities;

namespace FerOmega.Services
{
    public class GrammarService : IGrammarService
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

            OpenEscapeOperator = Get(OperatorType.OpenSquareBracket);
            CloseEscapeOperator = Get(OperatorType.CloseSquareBracket);

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

        public Operator Get(OperatorType operatorType)
        {
            if (!Enum.IsDefined(typeof(OperatorType), operatorType))
            {
                throw new ArgumentOutOfRangeException(nameof(operatorType), operatorType, $"Enum {nameof(OperatorType)} is out of range");
            }

            return Operators.First(x => x.OperatorType == operatorType).DeepClone();
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

        public bool IsUniqueByArity(string denotation, ArityType arity)
        {
            return Operators.Count(x => x.Denotations.Contains(denotation) && x.Arity == arity) == 1;
        }

        public bool IsUniqueByFixity(string denotation, FixityType fixity)
        {
            return Operators.Count(x => x.Denotations.Contains(denotation) && x.Fixity == fixity) == 1;
        }

        public bool IsUniqueByDenotation(string denotation)
        {
            return GetPossibleOperators(denotation).Length == 1;
        }

        public Operator[] GetPossibleOperators(string denotation)
        {
            return Operators.Where(x => x.Denotations.Contains(denotation)).ToArray();
        }

        private void SetOperators()
        {
            // there are less priority operators at the bottom and more priority operators at the top
            var priority = 1;

            priority = AddOperators(priority,
                new Operator(ArityType.Unary, AssociativityType.Left, OperatorType.Factorial, FixityType.Postfix, "!"),
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
                new Operator(ArityType.Multiarity, AssociativityType.Left, OperatorType.InRange, FixityType.Infix, "in"),
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.Contains, FixityType.Infix, "con"),
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.StartsWith, FixityType.Infix, "stw"),
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.EndsWith, FixityType.Infix, "edw"),
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