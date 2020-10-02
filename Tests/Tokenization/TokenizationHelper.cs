using NUnit.Framework;

namespace FerOmega.Tests.Tokenization
{
    internal static class TokenizationHelper
    {
        public static void Test(string[] expectedTokens, string[] actualTokens)
        {
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