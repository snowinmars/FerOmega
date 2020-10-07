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
        public AstService(IGrammarService<IGrammarConfig> grammarService, IOperatorService operatorService)
        {
            this.grammarService = grammarService;
            this.operatorService = operatorService;
        }

        private readonly IGrammarService<IGrammarConfig> grammarService;

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
            var forest = new List<Tree<AbstractToken>>();

            for (var i = 0; i < tokens.Length; i++)
            {
                var token = new StringToken(tokens, i);

                if (grammarService.IsOperand(token.Current))
                {
                    var operand = new Operand(token.Current);

                    var tree = new Tree<AbstractToken>(operand);

                    forest.Add(tree);

                    continue;
                }

                if (grammarService.IsOperator(token.Current))
                {
                    var possibleOperators = grammarService.GetPossibleOperators(token.Current);

                    var @operator = operatorService.Resolve(token, possibleOperators);

                    // check brackets
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

                            MergeTreesByOperator(forest, abstractToken);
                        }

                        continue;
                    }

                    // if we're here, the operator is not any type of bracket, so it's an evaluatable operator
                    switch (@operator.Arity)
                    {
                    case Arity.Unary when @operator.Fixity == Fixity.Postfix:
                    {
                        MergeTreesByOperator(forest, @operator);

                        break;
                    }

                    case Arity.Unary when @operator.Fixity == Fixity.Prefix:
                    {
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
                            var topOperator = stack.Peek();

                            if (topOperator.Priority <= @operator.Priority &&
                                stack.Count > 0)
                            {
                                topOperator = stack.Pop();
                            }

                            while (topOperator.Priority <= @operator.Priority)
                            {
                                MergeTreesByOperator(forest, topOperator);

                                if (stack.Count == 0)
                                {
                                    break;
                                }

                                topOperator = stack.Peek();

                                if (topOperator.Priority <= @operator.Priority &&
                                    stack.Count > 0)
                                {
                                    topOperator = stack.Pop();
                                }
                            }

                            stack.Push(@operator);

                            break;
                        }

                        case Associativity.Right:
                        {
                            var topOperator = stack.Peek();

                            if (topOperator.Priority <= @operator.Priority &&
                                stack.Count > 0)
                            {
                                MergeTreesByOperator(forest, stack.Pop());
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
                        // snowinmars@yandex.ru has a copy of the article

                        var nextOperators = grammarService.GetPossibleOperators(token.Next);

                        var isOpenPriorityBracket = nextOperators.Length == 1 &&
                                                    nextOperators.First().OperatorType ==
                                                    OperatorType.OpenPriorityBracket;

                        if (!isOpenPriorityBracket)
                        {
                            throw new
                                InvalidOperationException($"The {nameof(OperatorType.InRange)} operator should be followed by {nameof(OperatorType.OpenPriorityBracket)}");
                        }

                        stack.Push(@operator);

                        break;
                    }

                    case Arity.Nulary:
                        throw new
                            NotSupportedException($"Ast service doesn't support {nameof(Arity.Nulary)} operator {@operator} implicitly");
                    case Arity.Ternary:
                        throw new
                            NotSupportedException($"Ast service doesn't support {nameof(Arity.Ternary)} operator {@operator} implicitly");
                    case Arity.Kvatery:
                        throw new
                            NotSupportedException($"Ast service doesn't support {nameof(Arity.Kvatery)} operator {@operator} implicitly");

                    default:
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                    }
                }
            }

            while (stack.Count > 0)
            {
                MergeTreesByOperator(forest, stack.Pop());
            }

            if (forest.Count != 1)
            {
                throw new
                    InvalidOperationException("The forest is broken. That means that some operators is unable to find required operands");
            }

            return forest[0];
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

            case Arity.Multiarity:
                throw new
                    NotSupportedException($"Ast service doesn't support tree merging with {nameof(Arity.Nulary)} operator {@operator}");
            case Arity.Nulary:
                throw new
                    NotSupportedException($"Ast service doesn't support tree merging with {nameof(Arity.Nulary)} operator {@operator}");
            case Arity.Ternary:
                throw new
                    NotSupportedException($"Ast service doesn't support tree merging with {nameof(Arity.Ternary)} operator {@operator}");
            case Arity.Kvatery:
                throw new
                    NotSupportedException($"Ast service doesn't support tree merging with {nameof(Arity.Kvatery)} operator {@operator}");

            default:
            {
                throw new ArgumentOutOfRangeException();
            }
            }
        }
    }
}
