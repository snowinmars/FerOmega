using System;
using System.Linq;
using FerOmega.Entities.InternalSyntax;
using FerOmega.Entities.InternalSyntax.Enums;
using FerOmega.Services.Abstractions;
using FerOmega.Services.configs;

namespace FerOmega.Services
{
    internal class OperatorService : IOperatorService
    {
        public OperatorService(IGrammarService<IGrammarConfig> grammarService)
        {
            this.grammarService = grammarService;
        }

        private readonly IGrammarService<IGrammarConfig> grammarService;

        public Operator Resolve(StringToken token, Operator[] possibleOperators)
        {
            if (token == default ||
                possibleOperators == default)
            {
                throw new ArgumentNullException();
            }

            if (!possibleOperators.Any())
            {
                throw new InvalidOperationException("Operators collection is empty");
            }

            if (possibleOperators.Length == 1)
            {
                return possibleOperators[0];
            }

            var isUnaryPrefix = IsUnaryPrefix(token, possibleOperators);

            if (isUnaryPrefix)
            {
                var acceptedOperators = possibleOperators
                                        .Where(x => x.Fixity == Fixity.Prefix && x.Arity == Arity.Unary)
                                        .ToArray();

                if (acceptedOperators.Length != 1)
                {
                    throw
                        new InvalidOperationException($"Can't resolve operator: {token}");
                }

                return acceptedOperators[0];
            }

            var isUnaryPostfix = IsUnaryPostfix(token, possibleOperators);

            if (isUnaryPostfix)
            {
                var acceptedOperators = possibleOperators
                                        .Where(x => x.Fixity == Fixity.Postfix && x.Arity == Arity.Unary)
                                        .ToArray();

                if (acceptedOperators.Length != 1)
                {
                    throw
                        new InvalidOperationException($"Can't resolve operator: {token}");
                }

                return acceptedOperators[0];
            }

            var isInfixBinary = IsInfixBinary(token, possibleOperators);

            if (isInfixBinary)
            {
                var acceptedOperators = possibleOperators
                                        .Where(x => x.Fixity == Fixity.Infix && x.Arity == Arity.Binary)
                                        .ToArray();

                if (acceptedOperators.Length != 1)
                {
                    throw
                        new InvalidOperationException($"Can't resolve operator: {token}");
                }

                return acceptedOperators[0];
            }

            throw new InvalidOperationException($"Can't resolve fixity for {token}");
        }

        private bool IsInfixBinary(StringToken token, Operator[] possibleOperators)
        {
            if (!possibleOperators.Any(x => x.Fixity == Fixity.Infix && x.Arity == Arity.Binary))
            {
                return false;
            }

            var isPrecededByCloseBracket = grammarService.ClosePriorityBracket.Denotations.Contains(token.Previous);
            var isFollowedByOpenBracket = grammarService.OpenPriorityBracket.Denotations.Contains(token.Next);

            // like 5 + 2
            //        ^
            var isInfixBinaryCase1 = grammarService.IsOperand(token.Previous) && grammarService.IsOperand(token.Next);

            // like 5 + (2 + 3)
            //        ^
            var isInfixBinaryCase2 = grammarService.IsOperand(token.Previous) &&
                                     isFollowedByOpenBracket;

            // like (2 + 3) + 5
            //              ^
            var isInfixBinaryCase3 = isPrecededByCloseBracket &&
                                     grammarService.IsOperand(token.Next);

            // like 5! + 3
            //         ^
            var isInfixBinaryCase4 = grammarService.IsOperator(token.Previous) &&
                                     grammarService.IsOperand(token.Next) &&
                                     grammarService.IsUniqueByFixity(token.Previous, Fixity.Postfix);

            // like (1 + 2) + (3 + 4)
            //              ^
            var isInfixBinaryCase5 = isPrecededByCloseBracket &&
                                     grammarService.IsOperator(token.Current) &&
                                     isFollowedByOpenBracket;

            return isInfixBinaryCase1 ||
                   isInfixBinaryCase2 ||
                   isInfixBinaryCase3 ||
                   isInfixBinaryCase4 ||
                   isInfixBinaryCase5;
        }

        private bool IsUnaryPostfix(StringToken token, Operator[] possibleOperators)
        {
            if (!possibleOperators.Any(x => x.Arity == Arity.Unary && x.Fixity == Fixity.Postfix))
            {
                return false;
            }

            var isPrecededByCloseBracket = grammarService.ClosePriorityBracket.Denotations.Contains(token.Previous);
            var isFollowedByCloseBracket = grammarService.ClosePriorityBracket.Denotations.Contains(token.Next);

            // like 2 + 3!
            //           ^
            var isPostfixCase1 = grammarService.IsOperand(token.Previous) &&
                                 token.Next == StringToken.NonExistingOperator;

            // like (2 + 3!)
            //            ^
            var isPostfixCase2 = grammarService.IsOperand(token.Previous) && isFollowedByCloseBracket;

            // like 2! + 3
            //       ^
            var isPostfixCase3 = grammarService.IsOperand(token.Previous) && grammarService.IsOperator(token.Next);

            // like ((2 + 3)! )
            //              ^
            var isPostfixCase4 = isPrecededByCloseBracket &&
                                 grammarService.IsOperator(token.Current) &&
                                 isFollowedByCloseBracket;

            // like (2 + 3)! + 2
            //             ^
            var isPostfixCase5 = isPrecededByCloseBracket &&
                                 grammarService.IsOperator(token.Current) &&
                                 grammarService.IsOperator(token.Next);

            // like (2 + 3)!
            //             ^
            var isPostfixCase6 = isPrecededByCloseBracket &&
                                 token.Next == StringToken.NonExistingOperator;

            return isPostfixCase1 ||
                   isPostfixCase2 ||
                   isPostfixCase3 ||
                   isPostfixCase4 ||
                   isPostfixCase5 ||
                   isPostfixCase6;
        }

        private bool IsUnaryPrefix(StringToken token, Operator[] possibleOperators)
        {
            if (!possibleOperators.Any(x => x.Arity == Arity.Unary && x.Fixity == Fixity.Prefix))
            {
                return false;
            }

            var isPrecededByOpenBracket = token.Previous != default &&
                                          grammarService.OpenPriorityBracket.Denotations.Contains(token.Previous);

            var isFollowedByOpenBracket = token.Next != default &&
                                          grammarService.OpenPriorityBracket.Denotations.Contains(token.Next);

            // like -1 - 2
            //      ^
            var isPrefixCase1 = grammarService.IsOperand(token.Next) &&
                                token.Previous == StringToken.NonExistingOperator;

            // like !a && !b
            //            ^
            var isPrefixCase2 = grammarService.IsOperand(token.Next) &&
                                grammarService.IsOperator(token.Previous) &&
                                grammarService.IsUniqueByArity(token.Previous, Arity.Unary);

            // like -(1 - 2)
            //      ^
            var isPrefixCase3 = isFollowedByOpenBracket &&
                                token.Previous == StringToken.NonExistingOperator;

            // like ( -(1 - 2))
            //        ^
            var isPrefixCase4 = isPrecededByOpenBracket &&
                                grammarService.IsOperator(token.Current) &&
                                isFollowedByOpenBracket;

            // like (-5)
            //       ^
            var isPrefixCase5 = isPrecededByOpenBracket &&
                                grammarService.IsOperator(token.Current) &&
                                grammarService.IsOperand(token.Next);

            return isPrefixCase1 ||
                   isPrefixCase2 ||
                   isPrefixCase3 ||
                   isPrefixCase4 ||
                   isPrefixCase5;
        }
    }
}
