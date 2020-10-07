using System;
using System.Collections.Generic;
using System.Linq;
using FerOmega.Entities.AbstractSyntax;
using FerOmega.Entities.InternalSyntax;
using FerOmega.Entities.InternalSyntax.Enums;
using FerOmega.Services.Abstractions;
using FerOmega.Services.configs;

namespace FerOmega.Services
{
    internal class AstService : IAstService
    {
        public AstService(IGrammarService<InternalGrammarConfig> grammarService, IOperatorService operatorService)
        {
            this.grammarService = grammarService;
            this.operatorService = operatorService;
        }

        private readonly IGrammarService<InternalGrammarConfig> grammarService;

        private readonly IOperatorService operatorService;

        /// <summary>
        /// Build abstract syntax tree from tokenized string
        /// using extended shunting yard algorithm
        /// </summary>
        public Tree<AbstractToken> Convert(string[] tokens)
        {
            if (tokens == default ||
                !tokens.Any())
            {
                return new Tree<AbstractToken>();
            }

            var stack = new Stack<Operator>(tokens.Length);
            var trees = new List<Tree<AbstractToken>>();

            for (var i = 0; i < tokens.Length; i++)
            {
                var token = new StringToken(tokens, i);

                if (grammarService.IsOperand(token.Current))
                {
                    var operand = new Operand(token.Current);

                    var tree = new Tree<AbstractToken>(operand);

                    trees.Add(tree);

                    continue;
                }

                if (grammarService.IsOperator(token.Current))
                {
                    var possibleOperators = grammarService.GetPossibleOperators(token.Current);

                    var @operator = operatorService.Resolve(token, possibleOperators);

                    // two ifs check brackets

                    if (@operator == grammarService.OpenPriorityBracket)
                    {
                        stack.Push(@operator);

                        continue;
                    }

                    if (@operator == grammarService.ClosePriorityBracket)
                    {
                        while (stack.Count >= 0)
                        {
                            if (stack.Count == 0)
                            {
                                throw new InvalidOperationException();
                            }

                            var abstractToken = stack.Pop();

                            if (grammarService.OpenPriorityBracket == abstractToken)
                            {
                                break;
                            }

                            MergeTreesByOperator(trees, abstractToken);
                        }

                        continue;
                    }

                    // if we're here, the operator is not any type of bracket, so it's an evaluatable operator
                    switch (@operator.Arity)
                    {
                    case Arity.Unary when @operator.Fixity == Fixity.Postfix:
                    {
                        MergeTreesByOperator(trees, @operator);

                        break;
                    }

                    case Arity.Unary when @operator.Fixity == Fixity.Prefix:
                    {
                        // TODO: [snow]
                        stack.Push(@operator);

                        break;
                    }

                    case Arity.Binary:
                    {
                        if (stack.Count == 0)
                        {
                            stack.Push(@operator);

                            break;
                        }

                        switch (@operator.Associativity)
                        {
                        case Associativity.Left:
                        {
                            var popedOperator = stack.Peek();

                            if (popedOperator.Priority <= @operator.Priority &&
                                stack.Count > 0)
                            {
                                popedOperator = stack.Pop();
                            }

                            while (popedOperator.Priority <= @operator.Priority)
                            {
                                MergeTreesByOperator(trees, popedOperator);

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

                        case Associativity.Right:
                        {
                            var popedOperator = stack.Peek();

                            if (popedOperator.Priority <= @operator.Priority &&
                                stack.Count > 0)
                            {
                                MergeTreesByOperator(trees, stack.Pop());
                            }

                            stack.Push(@operator);

                            break;
                        }

                        default:
                            throw new ArgumentOutOfRangeException();
                        }

                        break;
                    }

                    case Arity.Multiarity:
                    {
                        // right operand of multiarity operator should be wrapped in priority brackets
                        // like a in (1, 2)
                        //           ^----^
                        // there's a way to avoid it: https://blog.kallisti.net.nz/2008/02/extension-to-the-shunting-yard-algorithm-to-allow-variable-numbers-of-arguments-to-functions/

                        var nextOperators = grammarService.GetPossibleOperators(token.Next);

                        var isOpenPriorityBracket = nextOperators.Length == 1 &&
                                                    nextOperators.First().OperatorType ==
                                                    OperatorType.OpenPriorityBracket;

                        if (!isOpenPriorityBracket)
                        {
                            throw new InvalidOperationException();
                        }

                        stack.Push(@operator);

                        break;
                    }

                    case Arity.Nulary:
                    case Arity.Ternary:
                    case Arity.Kvatery:
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
                MergeTreesByOperator(trees, stack.Pop());
            }

            if (trees.Count != 1)
            {
                throw new InvalidOperationException();
            }

            return trees[0];
        }

        private void MergeTreesByOperator(List<Tree<AbstractToken>> trees, Operator @operator)
        {
            var operatorTree = new Tree<AbstractToken>(@operator);

            switch (@operator.Arity)
            {
            case Arity.Unary:
            {
                var operandTree = trees[^1];

                trees.RemoveAt(trees.Count - 1);
                operatorTree.AppendToRoot(operandTree);

                trees.Add(operatorTree);

                break;
            }

            case Arity.Binary:
            {
                var leftOperandTree = trees[^1];
                trees.RemoveAt(trees.Count - 1);

                var rightOperandTree = trees[^1];
                trees.RemoveAt(trees.Count - 1);

                // normally I append left operand; then - right operand
                // this is the only case by now where I have to append right operand before left due to they exists in stack in reversed order
                operatorTree.AppendToRoot(rightOperandTree);
                operatorTree.AppendToRoot(leftOperandTree);

                trees.Add(operatorTree);

                break;
            }

            case Arity.Nulary:
            case Arity.Ternary:
            case Arity.Kvatery:
            case Arity.Multiarity:
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
