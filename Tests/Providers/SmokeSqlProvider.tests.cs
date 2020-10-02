using System.Collections;
using NUnit.Framework;

namespace FerOmega.Tests.Providers
{
    internal class SmokeSqlProviderTests : BaseTest
    {
        private static IEnumerable PositiveSmokeCases
        {
            get
            {
                yield return new TestCaseData("a + b + c",
                                              "[a] + [b] + [c]").SetName("Simple");

                yield return new TestCaseData("      a        +   b   +    c      ",
                                              "[a] + [b] + [c]").SetName("Trim");

                yield return new TestCaseData("a + betaTest + c",
                                              "[a] + [betaTest] + [c]").SetName("LongOperand");

                yield return new TestCaseData("a+b+c",
                                              "[a] + [b] + [c]").SetName("Compact");

                yield return new TestCaseData("a",
                                              "[a]").SetName("Single");

                yield return new TestCaseData("    ",
                                              "").SetName("Empty");
            }
        }
        
        [Test]
        [TestCaseSource(nameof(PositiveSmokeCases))]
        public void Smoke(string equation, string expected)
        {
            var tokens = TokenizationService.Tokenizate(equation);
            var tree = AstService.Convert(tokens);
            var sql = SqlProvider.Convert(tree);
            
            Assert.AreEqual(expected, sql);
        }
    }
}