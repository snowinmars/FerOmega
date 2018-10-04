using System.Collections.Generic;
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
        private IShuntingYardService<Tree<AbstractToken>> treeShuntingYardService;
        private IEquationGenerator equationGenerator;

        public SmokeTests()
        {
            abstractShuntingYardService = new AbstractShuntingYardService();
            tokenizationService = new TokenizationService();
            treeShuntingYardService = new TreeShuntingYardService();
            equationGenerator = new EquationGenerator();

            equations = equationGenerator.GetEquations();
        }

        private EquationGenerator.Equation[] equations;

        [Test]
        public void Foo()
        {
            foreach (var equation in equations)
            {
                var tokens = tokenizationService.Tokenizate(equation.InfixForm);
                var revertedPolishResult = abstractShuntingYardService.Parse(tokens);
                var treeResult = treeShuntingYardService.Parse(tokens);

                var revertedPolishResultString = RevertedPolishEquationToString(revertedPolishResult);
                var treeResultString = TreeToString(treeResult);

                Assert.AreEqual(expected: equation.RevertedPolishForm, actual: revertedPolishResultString, message: $"Reverted polish: {equation.Id}");
                Assert.AreEqual(expected: equation.ShortTreeForm, actual: treeResultString, message: $"Tree: {equation.Id}");
            }
        }

        private string TreeToString(Tree<AbstractToken> treeResult)
        {
            return EquationGenerator.DeSpacify(JsonConvert.SerializeObject(ShortToken.FromTree(treeResult), Formatting.None));
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
    }
}