using System;

using FerOmega.Abstractions;
using FerOmega.Entities;
using FerOmega.Entities.AbstractSyntax;
using FerOmega.Services;

using NUnit.Framework;

namespace FerOmega.Tests
{
    public class RandomizeTests
    {
        private readonly IRandomizeEquationGenerator randomizeEquationGenerator;
        private readonly ITokenizationService tokenizationService;
        private readonly IShuntingYardService<Tree<AbstractToken>> treeShuntingYardService;

        public RandomizeTests()
        {
            randomizeEquationGenerator = new RandomizeEquationGenerator();
            treeShuntingYardService = new TreeShuntingYardService();
            tokenizationService = new TokenizationService();
        }

        [Test]
        public void RandomTest()
        {
            var count = (long)Math.Pow(2, 10);

            const GrammarSectionType GrammarSectionType = GrammarSectionType.ArithmeticAlgebra
                                                          | GrammarSectionType.BooleanAlgebra
                                                          | GrammarSectionType.Equality
                                                          | GrammarSectionType.Inequality;

            var equations = randomizeEquationGenerator.GetEquations(count, GrammarSectionType);

            foreach (var equation in equations)
            {
                var plainEquation = equation.ToPlainEquation();
                var tokens = tokenizationService.Tokenizate(plainEquation);

                var actualTree = treeShuntingYardService.Parse(tokens);
                var actualToken = ShortToken.FromTree(actualTree);

                Assert.AreEqual(
                    expected: equation,
                    actual: actualToken,
                    message: $"Randomized: {plainEquation}");
            }
        }
    }
}