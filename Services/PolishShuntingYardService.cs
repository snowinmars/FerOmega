﻿using System;
using System.Collections.Generic;
using FerOmega.Abstractions;
using FerOmega.Entities;
using FerOmega.Services.InternalEntities;

namespace FerOmega.Services
{
    public class PolishShuntingYardService
    {
        public PolishShuntingYardService()
        {
            GrammarService = new GrammarService();
            OperatorResolveService = new OperatorResolveService();
        }

        private IGrammarService GrammarService { get; }

        private IOperatorResolveService OperatorResolveService { get; }

        /// <summary>
        /// Build reverse Polish queue from tokenized string
        /// </summary>
        public Queue<AbstractToken> Parse(string[] tokens)
        {
            var stack = new Stack<AbstractToken>(tokens.Length);
            var queue = new Queue<AbstractToken>(tokens.Length);

            for (var i = 0; i < tokens.Length; i++)
            {
                var token = new OperatorToken(tokens, i);

                if (GrammarService.IsOperand(token.Current))
                {
                    var operand = new Operand(token.Current);

                    queue.Enqueue(operand);

                    continue;
                }

                if (GrammarService.IsOperator(token.Current))
                {
                    var possibleOperators = GrammarService.GetPossibleOperators(token.Current);

                    var hasOverloads = possibleOperators.Length != 1;

                    var @operator = hasOverloads
                                        ? OperatorResolveService.Resolve(token, possibleOperators)
                                        : possibleOperators[0];

                    if (GrammarService.IsOpenBracket(@operator))
                    {
                        stack.Push(@operator);

                        continue;
                    }

                    if (GrammarService.IsCloseBracket(@operator))
                    {
                        while (stack.Count >= 0)
                        {
                            if (stack.Count == 0)
                            {
                                throw new InvalidOperationException();
                            }

                            var abstractToken = stack.Pop();

                            if (GrammarService.IsOpenBracket(abstractToken))
                            {
                                break;
                            }

                            queue.Enqueue(abstractToken);
                        }

                        continue;
                    }

                    // if we're here, the operator is not any type of bracket, so it's evaluatable operator

                    switch (@operator.Arity)
                    {
                    case ArityType.Unary when @operator.Fixity == FixityType.Postfix:
                        queue.Enqueue(@operator);

                        break;

                    case ArityType.Unary when @operator.Fixity == FixityType.Prefix:
                        stack.Push(@operator);

                        break;

                    case ArityType.Binary:
                        if (stack.Count == 0)
                        {
                            stack.Push(@operator);

                            break;
                        }

                        switch (@operator.Associativity)
                        {
                        case AssociativityType.Left:
                        {
                            var popedOperator = stack.Peek();

                            if (popedOperator.Priority <= @operator.Priority &&
                                stack.Count > 0)
                            {
                                popedOperator = stack.Pop();
                            }

                            while (popedOperator.Priority <= @operator.Priority)
                            {
                                queue.Enqueue(popedOperator);

                                if (stack.Count == 0)
                                {
                                    break;
                                }

                                popedOperator = stack.Peek();

                                if (popedOperator.Priority <= @operator.Priority &&
                                    stack.Count > 0)
                                {
                                    popedOperator = stack.Pop();
                                }
                            }

                            stack.Push(@operator);

                            break;
                        }

                        case AssociativityType.Right:
                        {
                            var popedOperator = stack.Peek();

                            if (popedOperator.Priority <= @operator.Priority &&
                                stack.Count > 0)
                            {
                                popedOperator = stack.Pop();
                                queue.Enqueue(popedOperator);
                            }

                            stack.Push(@operator);

                            break;
                        }
                        
                        default:
                            throw new ArgumentOutOfRangeException();
                        }

                        break;

                    case ArityType.Nulary:
                    case ArityType.Ternary:
                    case ArityType.Kvatery:
                    case ArityType.Multiarity:
                        throw new NotSupportedException();

                    default:
                        throw new ArgumentOutOfRangeException();
                    }
                }
            }

            while (stack.Count > 0)
            {
                queue.Enqueue(stack.Pop());
            }

            return queue;
        }
    }
}