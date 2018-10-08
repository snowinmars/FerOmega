﻿using System;
using System.Collections.Generic;
using System.Linq;

using FerOmega.Abstractions;
using FerOmega.Entities;
using FerOmega.Entities.RedBlack;

namespace FerOmega.Services
{
    public class TreeShuntingYardService : IShuntingYardService<Tree<AbstractToken>>
    {
        private GrammarService GrammarService { get; }

        private OperatorResolveService OperatorResolveService { get; }

        public TreeShuntingYardService()
        {
            GrammarService = new GrammarService();
            OperatorResolveService = new OperatorResolveService();
        }

        public Tree<AbstractToken> Parse(string[] tokens)
        {
            var stack = new Stack<Operator>(tokens.Length);
            var trees = new List<Tree<AbstractToken>>();

            for (int i = 0; i < tokens.Length; i++)
            {
                var token = new OperatorToken(tokens, i);

                if (GrammarService.IsOperand(token.Current))
                {
                    var operand = new Operand(token.Current);

                    var tree = new Tree<AbstractToken>(operand);

                    trees.Add(tree);
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

                            NewMethod(trees, abstractToken);
                        }

                        continue;
                    }

                    // if we're here, the operator is not any type of bracket, so it's evaluatable operator

                    switch (@operator.Arity)
                    {
                        case ArityType.Unary when @operator.Fixity == FixityType.Postfix:
                        {
                            NewMethod(trees, @operator);
                            break;
                        }

                        case ArityType.Unary when @operator.Fixity == FixityType.Prefix:
                        {
                            // TODO: [DT] 
                            stack.Push(@operator);
                            break;
                        }

                        case ArityType.Binary:
                        {
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
                                        NewMethod(trees, popedOperator);

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
                                        NewMethod(trees, stack.Pop());
                                    }

                                    stack.Push(@operator);

                                    break;
                                }
                            }

                            break;
                        }

                        case ArityType.Nulary:
                        case ArityType.Ternary:
                        case ArityType.Kvatery:
                        case ArityType.Multiarity:
                        {
                            throw new NotSupportedException();
                        }

                        default:
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }

            while (stack.Count > 0)
            {
                NewMethod(trees, stack.Pop());
            }

            if (trees.Count != 1)
            {
                throw new InvalidOperationException();
            }

            return trees[0];
        }

        private void NewMethod(List<Tree<AbstractToken>> trees, Operator @operator)
        {
            var operatorTree = new Tree<AbstractToken>(@operator);

            switch (@operator.Arity)
            {
                case ArityType.Unary:
                {
                    var operandTree = trees[trees.Count - 1];
                    trees.RemoveAt(trees.Count - 1);
                    operatorTree.AppendToRoot(operandTree);
                    trees.Add(operatorTree);
                    break;
                }

                case ArityType.Binary:
                {
                    var leftOperandTree = trees[trees.Count - 1];
                    trees.RemoveAt(trees.Count - 1);

                    var rightOperandTree = trees[trees.Count - 1];
                    trees.RemoveAt(trees.Count - 1);

                    operatorTree.AppendToRoot(leftOperandTree);
                    operatorTree.AppendToRoot(rightOperandTree);

                    trees.Add(operatorTree);
                    break;
                }

                case ArityType.Nulary:
                case ArityType.Ternary:
                case ArityType.Kvatery:
                case ArityType.Multiarity:
                {
                    throw new NotSupportedException();
                }

                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}