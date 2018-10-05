using System;
using System.Collections.Generic;
using System.Linq;

using FerOmega.Abstractions;
using FerOmega.Entities;

namespace FerOmega.Services
{
    public class AbstractShuntingYardService : IShuntingYardService<Queue<AbstractToken>>
    {
        private GrammarService GrammarService { get; }

        private OperatorResolveService OperatorResolveService { get; }

        public AbstractShuntingYardService()
        {
            GrammarService = new GrammarService();
            OperatorResolveService = new OperatorResolveService();
        }

        public Queue<AbstractToken> Parse(string[] tokens)
        {
            var stack = new Stack<AbstractToken>(tokens.Length);
            var queue = new Queue<AbstractToken>(tokens.Length);

            for (int i = 0; i < tokens.Length; i++)
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

                    // length != 1 means we have overloads
                    var @operator = possibleOperators.Length == 1 ? possibleOperators[0] : OperatorResolveService.Resolve(token, possibleOperators);

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

                                    if (popedOperator.Priority <= @operator.Priority && stack.Count > 0)
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

                                        if (popedOperator.Priority <= @operator.Priority && stack.Count > 0)
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

                                    if (popedOperator.Priority <= @operator.Priority && stack.Count > 0)
                                    {
                                        popedOperator = stack.Pop();
                                        queue.Enqueue(popedOperator);
                                    }

                                    stack.Push(@operator);

                                    break;
                                }
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