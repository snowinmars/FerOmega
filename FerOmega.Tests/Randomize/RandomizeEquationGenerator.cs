using System;
using System.Collections.Generic;
using System.Linq;

using FerOmega.Abstractions;
using FerOmega.Entities;
using FerOmega.Services;

namespace FerOmega.Tests
{
    internal class RandomizeEquationGenerator : IRandomizeEquationGenerator
    {
        private IGrammarService grammarService;

        public RandomizeEquationGenerator()
        {
            grammarService = new GrammarService();
        }

        // TODO: [DT] 
        private static Random random = new Random();

        private ShortToken ConstructToken(Operator @operator)
        {
            if (@operator.OperatorType == OperatorType.Literal)
            {
                return ConstruclLiteral();
            }

            var result = new ShortToken(@operator.OperatorType);

            switch (@operator.Arity)
            {
                case ArityType.Unary when @operator.Fixity == FixityType.Prefix
                                          || @operator.Fixity == FixityType.Postfix:
                {
                    var operand = ConstruclLiteral();

                    result.Children.Add(operand);

                    break;
                }

                case ArityType.Binary when @operator.Fixity == FixityType.Infix:
                {
                    var left = ConstruclLiteral();
                    var right = ConstruclLiteral();

                    result.Children.Add(left);
                    result.Children.Add(right);

                    break;
                }

                case ArityType.Nulary:
                case ArityType.Ternary:
                case ArityType.Kvatery:
                case ArityType.Multiarity:
                    throw new NotSupportedException();

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return result;
        }

        private static ShortToken ConstruclLiteral()
        {
            return new ShortToken(random.Next(0, 1024));
        }

        public ShortToken GetAlgebraEquation(GrammarSectionType grammarSectionType)
        {
            var operators = grammarService.GetOperatorsForSection(grammarSectionType);
            var operatorsCount = random.Next(5, 7);

            ShortToken result = null;

            for (int i = 0; i < operatorsCount; i++)
            {
                var @operator = operators[random.Next(0, operators.Length - 1)];

                var newToken = ConstructToken(@operator);

                if (result == null)
                {
                    result = newToken;
                }
                else
                {
                    var anyLocus = GetRandomLocus(result);
                    if (anyLocus.Children.Count == 0)
                    {
                        anyLocus.Children = newToken.Children;
                        anyLocus.OperatorType = newToken.OperatorType;
                        anyLocus.Value = newToken.Value;
                    }
                    else
                    {
                        var position = random.Next(0, anyLocus.Children.Count);
                        anyLocus.Children[position] = newToken;
                    }
                }
            }

            return result;
        }

        private ShortToken GetRandomLocus(ShortToken token)
        {
            var locuses = GetLocuses(token).ToArray();

            return locuses[random.Next(0, locuses.Length - 1)];
        }

        private IEnumerable<ShortToken> GetLocuses(ShortToken token)
        {
            if (!token.Children.Any())
            {
                yield return token;
            }

            foreach (var child in token.Children)
            {
                if (child.OperatorType == OperatorType.Literal)
                {
                    yield return child;
                }
                else
                {
                    foreach (var shortToken in GetLocuses(child))
                    {
                        yield return shortToken;
                    }
                }
            }
        }

        public IEnumerable<ShortToken> GetAlgebraEquations(int count, GrammarSectionType grammarSectionType)
        {
            for (int i = 0; i < count; i++)
            {
                yield return GetAlgebraEquation(grammarSectionType);
            }
        }
    }
}