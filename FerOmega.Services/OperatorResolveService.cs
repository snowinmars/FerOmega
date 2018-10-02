using System;
using System.Linq;

using FerOmega.Entities;

namespace FerOmega.Services
{
    internal class OperatorResolveService
    {
        private GrammarService GrammarService { get; }

        public OperatorResolveService()
        {
            GrammarService = new GrammarService();
        }

        public Operator Resolve(OperatorToken token, Operator[] possibleOperators)
        {
            var isUnaryPrefix = IsUnaryPrefix(token.Previous, token.Next, possibleOperators);

            if (isUnaryPrefix)
            {
                var acceptedOperators = possibleOperators.Where(x => x.Fixity == FixityType.Prefix && x.Arity == ArityType.Unary).ToArray();

                if (acceptedOperators.Length != 1)
                {
                    throw new InvalidOperationException("Can't resolve operator overloaded by non-fixity and non-arity");
                }

                return acceptedOperators[0];
            }

            var isUnaryPostfix = IsUnaryPostfix(token.Previous, token.Next, possibleOperators);

            if (isUnaryPostfix)
            {
                var acceptedOperators = possibleOperators.Where(x => x.Fixity == FixityType.Postfix && x.Arity == ArityType.Unary).ToArray();

                if (acceptedOperators.Length != 1)
                {
                    throw new InvalidOperationException("Can't resolve operator overloaded by non-fixity and non-arity");
                }

                return acceptedOperators[0];
            }

            var isInfixBinary = IsInfixBinary(token.Previous, token.Next, possibleOperators);

            if (isInfixBinary)
            {
                var acceptedOperators = possibleOperators.Where(x => x.Fixity == FixityType.Infix && x.Arity == ArityType.Binary).ToArray();

                if (acceptedOperators.Length != 1)
                {
                    throw new InvalidOperationException("Can't resolve operator overloaded by non-fixity and non-arity");
                }

                return acceptedOperators[0];
            }

            throw new InvalidOperationException("Can't resolve fixity");
        }

        private bool IsInfixBinary(string previousToken, string nextToken, Operator[] possibleOperators)
        {
            if (!possibleOperators.Any(x => x.Arity == ArityType.Binary && x.Fixity == FixityType.Infix))
            {
                return false;
            }

            // like 5 + 2
            //        ^
            var isInfixBinaryCase1 = GrammarService.IsOperand(previousToken)
                                     && GrammarService.IsOperand(nextToken);

            // like 5 + (2 + 3)
            //        ^
            var isInfixBinaryCase2 = GrammarService.IsOperand(previousToken)
                                     && GrammarService.BracketsDenotations.Contains(nextToken);
            ;

            // like (2 + 3) + 5
            //              ^
            var isInfixBinaryCase3 = GrammarService.BracketsDenotations.Contains(previousToken)
                                     && GrammarService.IsOperand(nextToken);
            ;

            return isInfixBinaryCase1 || isInfixBinaryCase2 || isInfixBinaryCase3;
        }

        private bool IsUnaryPostfix(string previousToken, string nextToken, Operator[] possibleOperators)
        {
            if (!possibleOperators.Any(x => x.Arity == ArityType.Unary && x.Fixity == FixityType.Postfix))
            {
                return false;
            }

            // like 1 + 3!
            //           ^
            var isPostfixCase1 = GrammarService.IsOperand(previousToken)
                                 && nextToken == OperatorToken.NonExistingOperator;

            // like (1 + 3!)
            //            ^
            var isPostfixCase2 = GrammarService.IsOperand(previousToken)
                                 && GrammarService.IsOperator(nextToken);

            // like 3! + 1
            //       ^
            var isPostfixCase3 = GrammarService.IsOperand(previousToken)
                                 && GrammarService.IsOperator(nextToken);

            return isPostfixCase1 || isPostfixCase2 || isPostfixCase3;
        }

        private bool IsUnaryPrefix(string previousToken, string nextToken, Operator[] possibleOperators)
        {
            if (!possibleOperators.Any(x => x.Arity == ArityType.Unary && x.Fixity == FixityType.Prefix))
            {
                return false;
            }

            // like -1 - 2
            //      ^
            var isPrefixCase1 = GrammarService.IsOperand(nextToken)
                                && previousToken == OperatorToken.NonExistingOperator;

            // like !a && !b
            //            ^
            var isPrefixCase2 = GrammarService.IsOperand(nextToken)
                                && GrammarService.IsOperator(previousToken);

            // like -(1 - 2)
            //      ^
            var isPrefixCase3 = GrammarService.IsOperator(nextToken)
                                && previousToken == OperatorToken.NonExistingOperator;

            return isPrefixCase1 || isPrefixCase2 || isPrefixCase3;
        }
    }
}