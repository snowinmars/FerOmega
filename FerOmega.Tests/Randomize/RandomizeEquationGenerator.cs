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
                return new ShortToken(random.Next(-1024, 1024));
            }

            var result = new ShortToken(@operator.OperatorType);

            switch (@operator.Arity)
            {
                case ArityType.Unary when @operator.Fixity == FixityType.Prefix
                                          || @operator.Fixity == FixityType.Postfix:
                {
                    var operand = new ShortToken(random.Next(-1024, 1024));

                    result.Children.Add(operand);

                    break;
                }

                case ArityType.Binary when @operator.Fixity == FixityType.Infix:
                {
                    var left = new ShortToken(random.Next(-1024, 1024));
                    var right = new ShortToken(random.Next(-1024, 1024));

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

        public ShortToken GetAlgebraEquation(GrammarSectionType grammarSectionType)
        {
            var operators = grammarService.GetOperatorsForSection(grammarSectionType);
            var operatorsCount = random.Next(5, 7);

            ShortToken result = null;

            for (int i = 0; i < operatorsCount; i++)
            {
                var @operator = operators[random.Next(0, operators.Length - 1)];

                var shortToken = ConstructToken(@operator);

                if (result == null)
                {
                    result = shortToken;
                }
                else
                {
                    var anyLocus = GetRandomLocus(result);

                    anyLocus.Children.Add(shortToken);
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
            foreach (var child in token.Children.Where(x => x.OperatorType == OperatorType.Literal))
            {
                yield return child;

                foreach (var shortToken in GetLocuses(child))
                {
                    yield return shortToken;
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