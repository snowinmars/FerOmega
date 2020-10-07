using System;
using System.Collections.Generic;
using System.Linq;
using FerOmega.Entities.InternalSyntax;
using FerOmega.Entities.InternalSyntax.Enums;
using FerOmega.Services.Abstractions;

namespace FerOmega.Services
{
    internal class GrammarService<T> : IGrammarService<T>
        where T : IGrammarConfig
    {
        public GrammarService(T grammarConfig)
        {
            Operators = CheckOperators(grammarConfig.ConfigOperators());
            OperatorDenotations = Operators.SelectMany(x => x.Denotations).ToArray();
            OperatorsRegex = grammarConfig.GetOperatorsAsRegex(OperatorDenotations);

            // it uses below in getters
            var requiredOperators = new[]
            {
                OperatorType.Separator,
                OperatorType.OpenPriorityBracket,
                OperatorType.ClosePriorityBracket,
                OperatorType.OpenEscapeOperator,
                OperatorType.CloseEscapeOperator,
            };

            var possibleOperators = Operators.Select(x => x.OperatorType);

            if (!new HashSet<OperatorType>(requiredOperators).IsSubsetOf(possibleOperators))
            {
                throw new Exception($"Not enough operators was determined in {typeof(T).Name} grammar config");
            }
        }

        public string OperatorsRegex { get; }

        public Operator[] Operators { get; }

        public string[] OperatorDenotations { get; }

        public Operator Separator => Operators.First(x => x.OperatorType == OperatorType.Separator);

        public Operator OpenPriorityBracket => Operators.First(x => x.OperatorType == OperatorType.OpenPriorityBracket);

        public Operator ClosePriorityBracket =>
            Operators.First(x => x.OperatorType == OperatorType.ClosePriorityBracket);

        public Operator OpenEscapeOperator => Operators.First(x => x.OperatorType == OperatorType.OpenEscapeOperator);

        public Operator CloseEscapeOperator => Operators.First(x => x.OperatorType == OperatorType.CloseEscapeOperator);

        public string EnsureEscaped(string value)
        {
            var unescapedValue = EnsureUnescaped(value);

            return $"{OpenEscapeOperator.MainDenotation}{unescapedValue}{CloseEscapeOperator.MainDenotation}";
        }

        public string EnsureUnescaped(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException();
            }

            if (!value.StartsWith(OpenEscapeOperator.MainDenotation) ||
                !value.EndsWith(CloseEscapeOperator.MainDenotation))
            {
                return value;
            }

            var start = OpenEscapeOperator.MainDenotation.Length;
            var length = value.Length - 1 - CloseEscapeOperator.MainDenotation.Length;

            return $"{value.Substring(start, length)}";
        }

        public Operator[] GetPossibleOperators(string denotation, Arity? arity = null, Fixity? fixity = null)
        {
            if (string.IsNullOrWhiteSpace(denotation))
            {
                throw new ArgumentNullException();
            }

            var query = Operators.AsQueryable().Where(x => x.Denotations.Contains(denotation));

            if (arity != null)
            {
                query = query.Where(x => x.Arity == arity.Value);
            }

            if (fixity != null)
            {
                query = query.Where(x => x.Fixity == fixity.Value);
            }

            return query.ToArray();
        }

        public bool IsOperand(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentNullException();
            }

            var isEscaped = input.StartsWith(OpenEscapeOperator.MainDenotation, StringComparison.Ordinal) &&
                            input.EndsWith(CloseEscapeOperator.MainDenotation, StringComparison.Ordinal);

            var isInOperatorsDenotations = OperatorDenotations.Contains(input);

            return isEscaped || !isInOperatorsDenotations;
        }

        public bool IsOperator(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentNullException();
            }

            return !IsOperand(input) && OperatorDenotations.Contains(input);
        }

        public bool IsUniqueByArity(string denotation, Arity arity)
        {
            if (string.IsNullOrWhiteSpace(denotation))
            {
                throw new ArgumentNullException();
            }

            return Operators.Count(x => x.Denotations.Contains(denotation) && x.Arity == arity) == 1;
        }

        public bool IsUniqueByDenotation(string denotation)
        {
            if (string.IsNullOrWhiteSpace(denotation))
            {
                throw new ArgumentNullException();
            }

            return GetPossibleOperators(denotation).Length == 1;
        }

        public bool IsUniqueByFixity(string denotation, Fixity fixity)
        {
            if (string.IsNullOrWhiteSpace(denotation))
            {
                throw new ArgumentNullException();
            }

            return Operators.Count(x => x.Denotations.Contains(denotation) && x.Fixity == fixity) == 1;
        }

        private Operator[] CheckOperators(Operator[] operators)
        {
            var isOperatorTypePrimaryKey = operators.Select(x => x.OperatorType).Distinct().Count() == operators.Length;

            if (!isOperatorTypePrimaryKey)
            {
                throw new InvalidOperationException();
            }

            var isBrokenDenotations = operators.Any(x => x.Denotations.Length == 0);

            if (isBrokenDenotations)
            {
                throw new InvalidOperationException();
            }

            return operators;
        }
    }
}