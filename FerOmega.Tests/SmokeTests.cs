using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FerOmega.Abstractions;
using FerOmega.Entities;
using FerOmega.Entities.RedBlack;
using FerOmega.Services;

using Newtonsoft.Json;

using NUnit.Framework;

namespace FerOmega.Tests
{
    public class SmokeTests
    {
        private readonly IShuntingYardService<Queue<AbstractToken>> abstractShuntingYardService;
        private readonly ITokenizationService tokenizationService;
        private IEquationGenerator equationGenerator;
        private EquationGenerator.Equation[] equations;
        private IShuntingYardService<Tree<AbstractToken>> treeShuntingYardService;

        public SmokeTests()
        {
            abstractShuntingYardService = new AbstractShuntingYardService();
            tokenizationService = new TokenizationService();
            treeShuntingYardService = new TreeShuntingYardService();
            equationGenerator = new EquationGenerator();

            equations = equationGenerator.GetEquations();
        }

        [Test]
        public void Foo()
        {
            foreach (var equation in equations)
            {
                var tokens = tokenizationService.Tokenizate(equation.InfixForm);
                var revertedPolishResult = abstractShuntingYardService.Parse(tokens);
                var treeResult = treeShuntingYardService.Parse(tokens);

                var treeShortTokens = TreeToShortTokenTree(treeResult);

                Assert.AreEqual(expected: equation.RevertedPolishForm, actual: RevertedPolishEquationToString(revertedPolishResult), message: $"Reverted polish: {equation.Id}");
                var shortToken = ShortToken.FromTree(equation.ShortTreeFormAsTree);
                Assert.AreEqual(expected: shortToken, actual: treeShortTokens, message: $"Tree: {equation.Id}");
            }
        }

        private static string RevertedPolishEquationToString(Queue<AbstractToken> abstractResult)
        {
            var sb = new StringBuilder();

            foreach (var token in abstractResult)
            {
                if (token.OperatorType == OperatorType.Literal)
                {
                    var op = (Operand)token;
                    sb.Append($" {op.Value} ");
                }
                else
                {
                    var op = (Operator)token;
                    sb.Append($" {op.MainDenotation} ");
                }
            }

            return EquationGenerator.DeSpacify(sb.ToString());
        }

        private static ShortToken RevertedPolishEquationToShortTokenTree(Queue<AbstractToken> abstractResult)
        {
            var trees = new List<Tree<AbstractToken>>();

            foreach (var token in abstractResult)
            {
                if (token.OperatorType == OperatorType.Literal)
                {
                    trees.Add(new Tree<AbstractToken>(token));
                }
                else
                {
                    NewMethod(trees, (Operator)token);
                }
            }

            if (trees.Count != 1)
            {
                throw new InvalidOperationException();
            }

            return ShortToken.FromTree(trees[0]);
        }

        private ShortToken TreeToShortTokenTree(Tree<AbstractToken> treeResult)
        {
            return ShortToken.FromTree(treeResult);
        }

        private static void NewMethod(List<Tree<AbstractToken>> trees, Operator @operator)
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