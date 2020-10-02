using System;
using System.Collections.Generic;
using System.Linq;
using Entities.InternalSyntax;
using Entities.InternalSyntax.Enums;
using Services.Abstractions;

namespace Services
{
    public class GrammarService<T> : IGrammarService<T>
        where T : IGrammarConfig
    {
        public GrammarService(T grammarConfig)
        {
            Operators = CheckOperators(grammarConfig.ConfigOperators());
            OperatorDenotations = Operators.SelectMany(x => x.Denotations).ToArray();
        }

        public Operator[] Operators { get; }
        
        public string[] OperatorDenotations { get; }
        
        public Operator OpenPriorityBracket => Operators.First(x => x.OperatorType == OperatorType.OpenPriorityBracket);
        public Operator ClosePriorityBracket => Operators.First(x => x.OperatorType == OperatorType.ClosePriorityBracket);

        public Operator OpenEscapeOperator => Operators.First(x => x.OperatorType == OperatorType.OpenEscapeOperator);
        
        public Operator CloseEscapeOperator => Operators.First(x => x.OperatorType == OperatorType.CloseEscapeOperator);
        
        public bool IsOperand(string input)
        {
            var isEscaped = input.StartsWith(OpenEscapeOperator.MainDenotation, StringComparison.Ordinal) &&
                            input.EndsWith(CloseEscapeOperator.MainDenotation, StringComparison.Ordinal);

            var isInOperatorsDenotations = OperatorDenotations.Contains(input);

            return isEscaped || !isInOperatorsDenotations;
        }

        public bool IsOperator(string input)
        {
            return !IsOperand(input) && OperatorDenotations.Contains(input);
        }

        public bool IsUniqueByArity(string denotation, Arity arity)
        {
            return Operators.Count(x => x.Denotations.Contains(denotation) && x.Arity == arity) == 1;
        }

        public bool IsUniqueByDenotation(string denotation)
        {
            return GetPossibleOperators(denotation).Length == 1;
        }

        public Operator[] GetPossibleOperators(string denotation)
        {
            return Operators.Where(x => x.Denotations.Contains(denotation)).ToArray();
        }
        
        public bool IsUniqueByFixity(string denotation, Fixity fixity)
        {
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