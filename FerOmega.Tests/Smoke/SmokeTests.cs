using System.Collections.Generic;
using System.Text;

using FerOmega.Abstractions;
using FerOmega.Entities;
using FerOmega.Entities.RedBlack;
using FerOmega.Services;

using NUnit.Framework;

namespace FerOmega.Tests
{
    public class SmokeTests
    {
        private readonly IShuntingYardService<Queue<AbstractToken>> abstractShuntingYardService;
        private readonly ISmokeEquationGenerator smokeEquationGenerator;
        private readonly ITokenizationService tokenizationService;
        private readonly IShuntingYardService<Tree<AbstractToken>> treeShuntingYardService;

        public SmokeTests()
        {
            tokenizationService = new TokenizationService();
            smokeEquationGenerator = new SmokeEquationGenerator();
            treeShuntingYardService = new TreeShuntingYardService();
            abstractShuntingYardService = new AbstractShuntingYardService();
        }

        [Test]
        public void SmokeTest()
        {
            var equations = smokeEquationGenerator.GetEquations();
            foreach (var equation in equations)
            {
                var tokens = tokenizationService.Tokenizate(equation.InfixForm);
                var revertedPolishResult = abstractShuntingYardService.Parse(tokens);
                var treeResult = treeShuntingYardService.Parse(tokens);

                var treeShortTokens = ShortToken.FromTree(treeResult);

                Assert.AreEqual(
                    expected: equation.RevertedPolishForm,
                    actual: RevertedPolishEquationToString(revertedPolishResult),
                    message: $"Reverted polish: {equation.Id}");

                var shortToken = ShortToken.FromTree(equation.ShortTreeFormAsTree);

                Assert.AreEqual(
                    expected: shortToken,
                    actual: treeShortTokens,
                    message: $"Tree: {equation.Id}");
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

            return SmokeEquationGenerator.DeSpacify(sb.ToString());
        }
    }
}