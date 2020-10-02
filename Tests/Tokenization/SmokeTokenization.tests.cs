using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace Tests
{
    internal partial class TokenizationTests : BaseTest
    {
        private static IEnumerable PositiveSmokeCases
        {
            get
            {
                yield return new TestCaseData("a + b + c",
                                              new[]
                                              {
                                                  "a", "+", "b", "+", "c",
                                              }).SetName("Simple");
                yield return new TestCaseData("      a        +   b   +    c      ",
                                              new[]
                                              {
                                                  "a", "+", "b", "+", "c",
                                              }).SetName("Trim");
                yield return new TestCaseData("a + betaTest + c",
                                              new[]
                                              {
                                                  "a", "+", "betaTest", "+", "c",
                                              }).SetName("LongOperand");
                yield return new TestCaseData("a+b+c",
                                              new[]
                                              {
                                                  "a", "+", "b", "+", "c",
                                              }).SetName("Compact");
                yield return new TestCaseData("a",
                                              new[]
                                              {
                                                  "a",
                                              }).SetName("Single");
                yield return new TestCaseData("    ",
                                              new string[0]).SetName("Empty");
            }
        }

        [Test]
        [TestCaseSource(nameof(PositiveSmokeCases))]
        public void PositiveSmoke(string equation, string[] expectedTokens)
        {
            var actualTokens = TokenizationService.Tokenizate(equation);

            Assert.AreEqual(expectedTokens.Length, actualTokens.Length);

            for (var i = 0; i < expectedTokens.Length; i++)
            {
                var actualToken = actualTokens[i];
                var expectedToken = expectedTokens[i];
                
                Assert.AreEqual(actualToken, expectedToken);
            }
        }
    }
}