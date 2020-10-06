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
                                              new[] {"",},
                                              "@2 + @1 + @0",
                                              new[] {"c","b", "a"}).SetName("Simple");

                yield return new TestCaseData(" a    +         b    +  c   ",
                                              new[] {"",},
                                              "@2 + @1 + @0",
                                              new[] {"c","b", "a"}).SetName("Trim");

                yield return new TestCaseData("a + longItem + c",
                                              new[] {"",},
                                              "@2 + @1 + @0",
                                              new[] {"c", "longItem", "a",}).SetName("LongOperand");

                yield return new TestCaseData("a+longItem+c",
                                              new[] {"",},
                                              "@2 + @1 + @0",
                                              new[] {"c", "longItem", "a",}).SetName("Compact");

                yield return new TestCaseData("a",
                                              new[] {"",},
                                              "@0",
                                              new[] {"a",}).SetName("Single");

                yield return new TestCaseData("   ",
                                              new[] {"",},
                                              "",
                                              new string[0]).SetName("Empty");
            }
        }
        
        [Test]
        [TestCaseSource(nameof(PositiveSmokeCases))]
        public void Smoke(string equation,
            string[] allowedProperties,
            string expectedSql,
            object[] expectedParameters)
        {
            var tokens = TokenizationService.Tokenizate(equation);
            var tree = AstService.Convert(tokens);
            var (sql, parameters) = SqlProvider.Convert(tree, allowedProperties);
            
            Assert.AreEqual(expectedSql, sql);
            
            Assert.AreEqual(expectedParameters.Length, parameters.Length);

            for (var i = 0; i < parameters.Length; i++)
            {
                var expectedParameter = expectedParameters[i];
                var parameter = parameters[i];

                Assert.AreEqual(expectedParameter, parameter);
            }
        }
    }
}