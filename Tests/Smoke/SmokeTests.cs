using System.Collections.Generic;
using System.Text;
using FerOmega.Abstractions;
using FerOmega.Entities;
using FerOmega.Entities.AbstractSyntax;
using FerOmega.Services;
using NUnit.Framework;

namespace FerOmega.Tests.Smoke
{
    public class SmokeTests
    {
        public SmokeTests()
        {
            tokenizationService = new TokenizationService();
            smokeEquationGenerator = new SmokeEquationGenerator();
            astShuntingYardService = new AstShuntingYardService();
            polishShuntingYardService = new PolishShuntingYardService();
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

        private readonly PolishShuntingYardService polishShuntingYardService;

        private readonly ISmokeEquationGenerator smokeEquationGenerator;

        private readonly ITokenizationService tokenizationService;

        private readonly AstShuntingYardService astShuntingYardService;

        [Test]
        public void SmokeTest()
        {
            var equations = smokeEquationGenerator.GetEquations();

            foreach (var equation in equations)
            {
                var tokens = tokenizationService.Tokenizate(equation.InfixForm);
                var revertedPolishResult = polishShuntingYardService.Parse(tokens);
                var treeResult = astShuntingYardService.Parse(tokens);

                var treeShortTokens = ShortToken.FromTree(treeResult);

                Assert.AreEqual(equation.RevertedPolishForm,
                                RevertedPolishEquationToString(revertedPolishResult),
                                $"Reverted polish: {equation.Id}");

                var shortToken = ShortToken.FromTree(equation.ShortTreeFormAsTree);

                Assert.AreEqual(shortToken,
                                treeShortTokens,
                                $"Tree: {equation.Id}");
            }
        }
    }
}