using FerOmega.Abstractions;
using FerOmega.Entities;
using FerOmega.Entities.RedBlack;
using FerOmega.Services;

using NUnit.Framework;

namespace FerOmega.Tests
{
    public class RandomizeTests
    {
        private IRandomizeEquationGenerator randomizeEquationGenerator;
        private IShuntingYardService<Tree<AbstractToken>> treeShuntingYardService;
        private ITokenizationService tokenizationService;

        public RandomizeTests()
        {
            randomizeEquationGenerator = new RandomizeEquationGenerator();
            treeShuntingYardService = new TreeShuntingYardService();
            tokenizationService = new TokenizationService();
        }

        [Test]
        public void RandomTest()
        {
            var count = 10;

            var equations = randomizeEquationGenerator.GetAlgebraEquations(count, GrammarSectionType.Algebra);

            foreach (var equation in equations)
            {
                var plainEquation = equation.ToPlainEquation();
                var tokens = tokenizationService.Tokenizate(plainEquation);

                var actualTree = treeShuntingYardService.Parse(tokens);
                var actualToken = ShortToken.FromTree(actualTree);

                Assert.AreEqual(
                    expected: equation,
                    actual: actualToken,
                    message: $"Randomized: {equation}");
            }
        }
    }
}