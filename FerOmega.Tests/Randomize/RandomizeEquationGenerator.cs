using System;
using System.Collections.Generic;
using System.Linq;

using FerOmega.Abstractions;
using FerOmega.Common;
using FerOmega.Entities;
using FerOmega.Services;

namespace FerOmega.Tests
{
    internal class RandomizeEquationGenerator : IRandomizeEquationGenerator
    {
        private readonly IGrammarService grammarService;

        public RandomizeEquationGenerator()
        {
            grammarService = new GrammarService();
        }

        public IEnumerable<ShortToken> GetArithmeticAlgebraEquations(long count, int minOperatorsCount = 5, int maxOperatorsCount = 7)
        {
            var operators = grammarService.GetOperatorsForSection(GrammarSectionType.ArithmeticAlgebra);

            for (long i = 0; i < count; i++)
            {
                yield return GetEquation(minOperatorsCount, maxOperatorsCount, GrammarSectionType.ArithmeticAlgebra, operators);
            }
        }

        public IEnumerable<ShortToken> GetBooleanAlgebraEquations(long count, int minOperatorsCount = 5, int maxOperatorsCount = 7)
        {
            var operators = grammarService.GetOperatorsForSection(GrammarSectionType.BooleanAlgebra);

            for (long i = 0; i < count; i++)
            {
                yield return GetEquation(minOperatorsCount, maxOperatorsCount, GrammarSectionType.BooleanAlgebra, operators);
            }
        }

        public IEnumerable<ShortToken> GetEquations(long count, GrammarSectionType grammarSectionType)
        {
            if (grammarSectionType.HasFlag(GrammarSectionType.ArithmeticAlgebra))
            {
                foreach (var equation in GetArithmeticAlgebraEquations(count))
                {
                    yield return equation;
                }

                var equalityOperators = grammarService
                    .GetOperatorsForSection(GrammarSectionType.Equality | GrammarSectionType.Inequality)
                    .Where(x => x.Arity == ArityType.Binary)
                    .ToArray();

                if (grammarSectionType.HasFlag(GrammarSectionType.Equality)
                    || grammarSectionType.HasFlag(GrammarSectionType.Inequality))
                {
                    foreach (var token in NewMethod(count, equalityOperators))
                    {
                        yield return token;
                    }
                }
            }

            if (grammarSectionType.HasFlag(GrammarSectionType.BooleanAlgebra))
            {
                foreach (var equation in GetBooleanAlgebraEquations(count))
                {
                    yield return equation;
                }
            }
        }

        private static ShortToken ConstructLiteral(GrammarSectionType algebra)
        {
            switch (algebra)
            {
                case GrammarSectionType.Unknown:
                    throw new NotSupportedException();

                case GrammarSectionType.ArithmeticAlgebra:
                {
                    var value = Constants.Random.Next(0, 1024);

                    string literalValue = FormatLiteralValue(value);

                    return new ShortToken(OperatorType.Literal, literalValue);
                }

                case GrammarSectionType.BooleanAlgebra:
                {
                    var value = Constants.Random.NextAlphabetSymbol();

                    string literalValue = FormatLiteralValue(value);

                    return new ShortToken(OperatorType.Literal, literalValue);
                }

                case GrammarSectionType.Inequality:
                    throw new NotSupportedException();

                case GrammarSectionType.Equality:
                    throw new NotSupportedException();

                default:
                    throw new ArgumentOutOfRangeException(nameof(algebra), algebra, null);
            }
        }

        private static string FormatLiteralValue<T>(T value)
        {
            var shouldEscape = Constants.Random.NextBool();

            return shouldEscape ? $"[{value}]" : $"{value}";
        }

        private ShortToken ConstructToken(Operator @operator, GrammarSectionType algebra)
        {
            if (@operator.OperatorType == OperatorType.Literal)
            {
                return ConstructLiteral(algebra);
            }

            var result = new ShortToken(@operator.OperatorType);

            switch (@operator.Arity)
            {
                case ArityType.Unary when @operator.Fixity == FixityType.Prefix
                                          || @operator.Fixity == FixityType.Postfix:
                {
                    var operand = ConstructLiteral(algebra);

                    result.Children.Add(operand);

                    break;
                }

                case ArityType.Binary when @operator.Fixity == FixityType.Infix:
                {
                    var left = ConstructLiteral(algebra);
                    var right = ConstructLiteral(algebra);

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

        private ShortToken GetEquation(int minOperatorsCount, int maxOperatorsCount, GrammarSectionType grammarSectionType, Operator[] allowedOperators)
        {
            var operatorsCount = Constants.Random.Next(minOperatorsCount, maxOperatorsCount);

            ShortToken result = null;

            for (int i = 0; i < operatorsCount; i++)
            {
                var @operator = allowedOperators[Constants.Random.Next(0, allowedOperators.Length - 1)];

                var newToken = ConstructToken(@operator, grammarSectionType);

                if (result == null)
                {
                    result = newToken;
                }
                else
                {
                    // replace any leaf with new token
                    var anyLeaf = GetRandomLeaf(result);
                    if (anyLeaf.Children.Count == 0)
                    {
                        anyLeaf.Children = newToken.Children;
                        anyLeaf.OperatorType = newToken.OperatorType;
                        anyLeaf.Value = newToken.Value;
                    }
                    else
                    {
                        var position = Constants.Random.Next(0, anyLeaf.Children.Count);
                        anyLeaf.Children[position] = newToken;
                    }
                }
            }

            return result;
        }

        private IEnumerable<ShortToken> GetLeaves(ShortToken token)
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
                    foreach (var shortToken in GetLeaves(child))
                    {
                        yield return shortToken;
                    }
                }
            }
        }

        private ShortToken GetRandomLeaf(ShortToken token)
        {
            var leaves = GetLeaves(token).ToArray();

            return leaves[Constants.Random.Next(0, leaves.Length - 1)];
        }

        ShortToken MergeEquation(IEnumerable<ShortToken> inputTokens, Operator[] allowedOperators)
        {
            var tokens = new Queue<ShortToken>(inputTokens);

            while (tokens.Count > 1)
            {
                Operator @operator;
                if (tokens.Count == 1)
                {
                    @operator = allowedOperators
                        .Where(x => x.Arity == ArityType.Unary)
                        .ElementAt(Constants.Random.Next(0, allowedOperators.Length - 1));
                }
                else
                {
                    @operator = allowedOperators[Constants.Random.Next(0, allowedOperators.Length - 1)];
                }

                switch (@operator.Arity)
                {
                    case ArityType.Unary:
                    {
                        var token = new ShortToken(@operator.OperatorType);

                        var operand = tokens.Dequeue();
                        token.Children.Add(operand);

                        tokens.Enqueue(token);
                        break;
                    }

                    case ArityType.Binary:
                    {
                        var token = new ShortToken(@operator.OperatorType);

                        var operand = tokens.Dequeue();
                        token.Children.Add(operand);

                        operand = tokens.Dequeue();
                        token.Children.Add(operand);

                        tokens.Enqueue(token);
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
            }

            return tokens.Dequeue();
        }

        private IEnumerable<ShortToken> NewMethod(long count, Operator[] equalityOperators)
        {
            using (var leftAlgebraEquations = GetArithmeticAlgebraEquations(count).GetEnumerator())
            {
                using (var rightAlgebraEquations = GetArithmeticAlgebraEquations(count).GetEnumerator())
                {
                    for (int j = 0; j < count; j++)
                    {
                        if (!leftAlgebraEquations.MoveNext() || !rightAlgebraEquations.MoveNext())
                        {
                            break;
                        }

                        var leftAlgebraEquation = leftAlgebraEquations.Current;
                        var rightAlgebraEquation = rightAlgebraEquations.Current;

                        var @operator = equalityOperators[Constants.Random.Next(0, equalityOperators.Length - 1)];

                        switch (@operator.Arity)
                        {
                            case ArityType.Nulary:
                            case ArityType.Unary:
                                throw new NotSupportedException();

                            case ArityType.Binary:
                            {
                                var token = new ShortToken(@operator.OperatorType);

                                token.Children.Add(leftAlgebraEquation);
                                token.Children.Add(rightAlgebraEquation);

                                yield return token;

                                break;
                            }

                            case ArityType.Ternary:
                            case ArityType.Kvatery:
                            case ArityType.Multiarity:
                                throw new NotSupportedException();

                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }
        }
    }
}