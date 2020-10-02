using System.Collections;
using FerOmega.Entities.InternalSyntax.Enums;
using NUnit.Framework;

namespace FerOmega.Tests.Tokenization
{
    internal class GrammarTokenizationTests : BaseTest
    {
        private static IEnumerable PositiveGrammarCases
        {
            get
            {
                yield return new TestCaseData("a + b + c",
                                              new[]
                                              {
                                                  "a", "+", "b", "+", "c",
                                              }).SetName(nameof(OperatorType.Plus));

                yield return new TestCaseData("a - b - c",
                                              new[]
                                              {
                                                  "a", "-", "b", "-", "c",
                                              }).SetName(nameof(OperatorType.Minus));

                yield return new TestCaseData("a * b * c",
                                              new[]
                                              {
                                                  "a", "*", "b", "*", "c",
                                              }).SetName(nameof(OperatorType.Multiple));

                yield return new TestCaseData("a / b / c",
                                              new[]
                                              {
                                                  "a", "/", "b", "/", "c",
                                              }).SetName(nameof(OperatorType.Divide));

                yield return new TestCaseData("a % b % c",
                                              new[]
                                              {
                                                  "a", "%", "b", "%", "c",
                                              }).SetName(nameof(OperatorType.Reminder));

                yield return new TestCaseData("+ a + b + c",
                                              new[]
                                              {
                                                  "+", "a", "+", "b", "+", "c",
                                              }).SetName(nameof(OperatorType.UnaryPlus));

                yield return new TestCaseData("- a - b - c",
                                              new[]
                                              {
                                                  "-", "a", "-", "b", "-", "c",
                                              }).SetName(nameof(OperatorType.UnaryMinus));

                yield return new TestCaseData("a! - b! - c!",
                                              new[]
                                              {
                                                  "a", "!", "-", "b", "!", "-", "c", "!",
                                              }).SetName(nameof(OperatorType.Factorial));

                yield return new TestCaseData("-a! - b! - c!",
                                              new[]
                                              {
                                                  "-", "a", "!", "-", "b", "!", "-", "c", "!",
                                              }).SetName(nameof(OperatorType.UnaryMinus) +
                                                         nameof(OperatorType.Factorial));

                yield return new TestCaseData("!d!-!b!",
                                              new[]
                                              {
                                                  "!", "d", "!", "-", "!", "b", "!",
                                              }).SetName(nameof(OperatorType.Not) + nameof(OperatorType.Factorial));

                yield return new TestCaseData("a + b > c",
                                              new[]
                                              {
                                                  "a", "+", "b", ">", "c",
                                              }).SetName(nameof(OperatorType.GreaterThan));

                yield return new TestCaseData("a + b >= c",
                                              new[]
                                              {
                                                  "a", "+", "b", ">=", "c",
                                              }).SetName(nameof(OperatorType.GreaterOrEqualsThan));

                yield return new TestCaseData("a + b < c",
                                              new[]
                                              {
                                                  "a", "+", "b", "<", "c",
                                              }).SetName(nameof(OperatorType.LesserThan));

                yield return new TestCaseData("a + b <= c",
                                              new[]
                                              {
                                                  "a", "+", "b", "<=", "c",
                                              }).SetName(nameof(OperatorType.LesserOrEqualsThan));

                yield return new TestCaseData("a + b = c",
                                              new[]
                                              {
                                                  "a", "+", "b", "=", "c",
                                              }).SetName("sql" + nameof(OperatorType.Equals));

                yield return new TestCaseData("a + b == c",
                                              new[]
                                              {
                                                  "a", "+", "b", "==", "c",
                                              }).SetName("c" + nameof(OperatorType.Equals));

                yield return new TestCaseData("a + b === c",
                                              new[]
                                              {
                                                  "a", "+", "b", "===", "c",
                                              }).SetName("js" + nameof(OperatorType.Equals));

                yield return new TestCaseData("a + b <> c",
                                              new[]
                                              {
                                                  "a", "+", "b", "<>", "c",
                                              }).SetName("sql" + nameof(OperatorType.NotEquals));

                yield return new TestCaseData("a + b != c",
                                              new[]
                                              {
                                                  "a", "+", "b", "!=", "c",
                                              }).SetName("c" + nameof(OperatorType.NotEquals));

                yield return new TestCaseData("a & b & c",
                                              new[]
                                              {
                                                  "a", "&", "b", "&", "c",
                                              }).SetName(nameof(OperatorType.BitwiseAnd));

                yield return new TestCaseData("a | b | c",
                                              new[]
                                              {
                                                  "a", "|", "b", "|", "c",
                                              }).SetName(nameof(OperatorType.BitwiseOr));

                yield return new TestCaseData("a && b && c",
                                              new[]
                                              {
                                                  "a", "&&", "b", "&&", "c",
                                              }).SetName(nameof(OperatorType.And));

                yield return new TestCaseData("a || b || c",
                                              new[]
                                              {
                                                  "a", "||", "b", "||", "c",
                                              }).SetName(nameof(OperatorType.Or));

                yield return new TestCaseData("a ^ b ^ c",
                                              new[]
                                              {
                                                  "a", "^", "b", "^", "c",
                                              }).SetName(nameof(OperatorType.Xor));

                yield return new TestCaseData("a in (b)",
                                              new[]
                                              {
                                                  "a", "in", "(", "b", ")",
                                              }).SetName(nameof(OperatorType.InRange) + "Length1");

                yield return new TestCaseData("a in (b, c)",
                                              new[]
                                              {
                                                  "a", "in", "(", "b", ",", "c", ")",
                                              }).SetName(nameof(OperatorType.InRange) + "Length2");

                yield return new TestCaseData("a contains b",
                                              new[]
                                              {
                                                  "a", "contains", "b",
                                              }).SetName(nameof(OperatorType.Contains));

                yield return new TestCaseData("a startsWith b",
                                              new[]
                                              {
                                                  "a", "startsWith", "b",
                                              }).SetName(nameof(OperatorType.StartsWith));

                yield return new TestCaseData("a endsWith b",
                                              new[]
                                              {
                                                  "a", "endsWith", "b",
                                              }).SetName(nameof(OperatorType.EndsWith));
                yield return new TestCaseData("a,b",
                                              new []
                                              {
                                                  "a", ",", "b",
                                              }).SetName(nameof(OperatorType.Enumerator));
                yield return new TestCaseData("a;",
                                              new []
                                              {
                                                  "a", ";",
                                              }).SetName(nameof(OperatorType.Terminator));
            }
        }

        private static IEnumerable PositiveGrammarWithBracketsCases
        {
            get
            {
                yield return new TestCaseData("( a + b ) + c",
                                              new[]
                                              {
                                                  "(", "a", "+", "b", ")", "+", "c",
                                              }).SetName("Simple");

                yield return new TestCaseData("(a+b)+c",
                                              new[]
                                              {
                                                  "(", "a", "+", "b", ")", "+", "c",
                                              }).SetName("Compact");

                yield return new TestCaseData("(-a!++b!)*-c!",
                                              new[]
                                              {
                                                  "(", "-", "a", "!", "+", "+", "b", "!", ")", "*", "-", "c", "!",
                                              }).SetName("CompactWithAdditionalOperators");
            }
        }

        [Test]
        [TestCaseSource(nameof(PositiveGrammarCases))]
        public void PositiveGrammar(string equation, string[] expectedTokens)
        {
            var actualTokens = TokenizationService.Tokenizate(equation);
            TokenizationHelper.Test(expectedTokens, actualTokens);
        }

        [Test]
        [TestCaseSource(nameof(PositiveGrammarWithBracketsCases))]
        public void PositiveGrammarWithBrackets(string equation, string[] expectedTokens)
        {
            var actualTokens = TokenizationService.Tokenizate(equation);
            TokenizationHelper.Test(expectedTokens, actualTokens);
        }
    }
}