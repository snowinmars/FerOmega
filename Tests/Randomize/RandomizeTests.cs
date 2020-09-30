using System;
using FerOmega.Abstractions;
using FerOmega.Entities;
using FerOmega.Entities.AbstractSyntax;
using FerOmega.Services;
using NUnit.Framework;

namespace FerOmega.Tests.Randomize
{
    public class RandomizeTests
    {
        public RandomizeTests()
        {
            randomizeEquationGenerator = new RandomizeEquationGenerator();
            astShuntingYardService = new AstShuntingYardService();
            tokenizationService = new TokenizationService();
        }

        private readonly IRandomizeEquationGenerator randomizeEquationGenerator;

        private readonly ITokenizationService tokenizationService;

        private readonly AstShuntingYardService astShuntingYardService;

        [Test]

        // it doesn't test anything but...
        // build equation with random blocks
        // convert it into plain equation
        // convert plain equation into tree
        // rebuild equation back from tree
        // assert that the result is the same
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

                var actualTree = astShuntingYardService.Parse(tokens);
                var actualToken = ShortToken.FromTree(actualTree);

                Assert.AreEqual(equation,
                                actualToken,
                                $"Randomized: {plainEquation}");
            }
        }
    }
}