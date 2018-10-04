using System;
using System.Linq;

using FerOmega.Entities;

namespace FerOmega.Services
{
    internal class OperatorResolveService : IOperatorResolveService
    {
        private GrammarService GrammarService { get; }

        public OperatorResolveService()
        {
            GrammarService = new GrammarService();
        }

        public Operator Resolve(OperatorToken token, Operator[] possibleOperators)
        {
            var isUnaryPrefix = IsUnaryPrefix(token, possibleOperators);

            if (isUnaryPrefix)
            {
                var acceptedOperators = possibleOperators.Where(x => x.Fixity == FixityType.Prefix && x.Arity == ArityType.Unary).ToArray();

                if (acceptedOperators.Length != 1)
                {
                    throw new InvalidOperationException("Can't resolve operator overloaded by non-fixity and non-arity");
                }

                return acceptedOperators[0];
            }

            var isUnaryPostfix = IsUnaryPostfix(token, possibleOperators);

            if (isUnaryPostfix)
            {
                var acceptedOperators = possibleOperators.Where(x => x.Fixity == FixityType.Postfix && x.Arity == ArityType.Unary).ToArray();

                if (acceptedOperators.Length != 1)
                {
                    throw new InvalidOperationException("Can't resolve operator overloaded by non-fixity and non-arity");
                }

                return acceptedOperators[0];
            }

            var isInfixBinary = IsInfixBinary(token, possibleOperators);

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

        private bool IsInfixBinary(OperatorToken token, Operator[] possibleOperators)
        {
            if (!possibleOperators.Any(x => x.Arity == ArityType.Binary && x.Fixity == FixityType.Infix))
            {
                return false;
            }

            // like 5 + 2
            //        ^
            var isInfixBinaryCase1 = GrammarService.IsOperand(token.Previous)
                                     && GrammarService.IsOperand(token.Next);

            // like 5 + (2 + 3)
            //        ^
            var isInfixBinaryCase2 = GrammarService.IsOperand(token.Previous)
                                     && GrammarService.BracketsDenotations.Contains(token.Next);

            // like (2 + 3) + 5
            //              ^
            var isInfixBinaryCase3 = GrammarService.BracketsDenotations.Contains(token.Previous)
                                     && GrammarService.IsOperand(token.Next);

            // like 5! + 3
            //         ^
            var isInfixBinaryCase4 = GrammarService.IsOperator(token.Previous)
                                     && GrammarService.IsOperand(token.Next)
                                     && GrammarService.IsUniqueByFixity(token.Previous, FixityType.Postfix);

            return isInfixBinaryCase1 || isInfixBinaryCase2 || isInfixBinaryCase3 || isInfixBinaryCase4;
        }

        private bool IsUnaryPostfix(OperatorToken token, Operator[] possibleOperators)
        {
            if (!possibleOperators.Any(x => x.Arity == ArityType.Unary && x.Fixity == FixityType.Postfix))
            {
                return false;
            }

            // like 1 + 3!
            //           ^
            var isPostfixCase1 = GrammarService.IsOperand(token.Previous)
                                 && token.Next == OperatorToken.NonExistingOperator;

            // like (1 + 3!)
            //            ^
            var isPostfixCase2 = GrammarService.IsOperand(token.Previous)
                                 && GrammarService.IsOperator(token.Next);

            // like 3! + 1
            //       ^
            var isPostfixCase3 = GrammarService.IsOperand(token.Previous)
                                 && GrammarService.IsOperator(token.Next);

            return isPostfixCase1 || isPostfixCase2 || isPostfixCase3;
        }

        private bool IsUnaryPrefix(OperatorToken token, Operator[] possibleOperators)
        {
            if (!possibleOperators.Any(x => x.Arity == ArityType.Unary && x.Fixity == FixityType.Prefix))
            {
                return false;
            }

            // like -1 - 2
            //      ^
            var isPrefixCase1 = GrammarService.IsOperand(token.Next)
                                && token.Previous == OperatorToken.NonExistingOperator;

            // like !a && !b
            //            ^
            var isPrefixCase2 = GrammarService.IsOperand(token.Next)
                                && GrammarService.IsOperator(token.Previous)
                                && GrammarService.IsUniqueByArity(token.Previous, ArityType.Unary);

            // like -(1 - 2)
            //      ^
            var isPrefixCase3 = GrammarService.IsOperator(token.Next)
                                && token.Previous == OperatorToken.NonExistingOperator;

            return isPrefixCase1 || isPrefixCase2 || isPrefixCase3;
        }
    }
}