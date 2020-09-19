using System;
using Abstractions;
using Entities;
using Entities.AbstractSyntax;
using NUnit.Framework;
using Services;

namespace Tests.Randomize
{
    public class RandomizeTests
    {
        public RandomizeTests()
        {
            randomizeEquationGenerator = new RandomizeEquationGenerator();
            treeShuntingYardService = new TreeShuntingYardService();
            tokenizationService = new TokenizationService();
        }

        private readonly IRandomizeEquationGenerator randomizeEquationGenerator;

        private readonly ITokenizationService tokenizationService;

        private readonly IShuntingYardService<Tree<AbstractToken>> treeShuntingYardService;

        [Test]
        public void RandomTest()
        {
            var count = (long)Math.Pow(2, 10);

            const GrammarSectionType grammarSectionType =
                GrammarSectionType.ArithmeticAlgebra |
                GrammarSectionType.BooleanAlgebra |
                GrammarSectionType.Equality |
                GrammarSectionType.Inequality;

            var equations = randomizeEquationGenerator.GetEquations(count, grammarSectionType);

            foreach (var equation in equations)
            {
                var plainEquation = equation.ToPlainEquation();
                var tokens = tokenizationService.Tokenizate(plainEquation);

                var actualTree = treeShuntingYardService.Parse(tokens);
                var actualToken = ShortToken.FromTree(actualTree);

                Assert.AreEqual(equation,
                                actualToken,
                                $"Randomized: {plainEquation}");
            }
        }
    }
}