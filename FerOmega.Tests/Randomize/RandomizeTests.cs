using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public async Task RandomTest()
        {
            var count = (int)Math.Pow(2, 10);

            var grammarSectionType = GrammarSectionType.ArithmeticAlgebra | GrammarSectionType.BooleanAlgebra | GrammarSectionType.Equality | GrammarSectionType.Inequality;
            var equations = randomizeEquationGenerator.GetEquations(count, grammarSectionType);

            long avg = 0;
            var avgs = new List<long>();

            foreach (var equation in equations)
            {
                var stopwatch = Stopwatch.StartNew();

                var plainEquation = equation.ToPlainEquation();
                var tokens = tokenizationService.Tokenizate(plainEquation);

                var actualTree = treeShuntingYardService.Parse(tokens);
                var actualToken = ShortToken.FromTree(actualTree);

                Assert.AreEqual(
                    expected: equation,
                    actual: actualToken,
                    message: $"Randomized: {plainEquation}");

                stopwatch.Stop();

                avg += stopwatch.ElapsedMilliseconds;
                avgs.Add(stopwatch.ElapsedMilliseconds);
            }

            double d = avg / (double)count;
            await WriteTextAsync(@"D:\tmp.txt", $"\navg: {d}\np{avgs.Select(x => Math.Abs(x - d)).Sum() / avgs.Count}").ConfigureAwait(false);
        }

        static async Task WriteTextAsync(string filePath, string text)
        {
            var encodedText = Encoding.Unicode.GetBytes(text);

            using (var sourceStream = new FileStream(filePath,
                FileMode.Append, FileAccess.Write, FileShare.None,
                bufferSize: 4096, useAsync: true))
            {
                await sourceStream.WriteAsync(encodedText, 0, encodedText.Length).ConfigureAwait(false);
            };
        }
    }
}