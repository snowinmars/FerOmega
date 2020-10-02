using System;
using System.Collections;
using System.Collections.Generic;
using Entities.InternalSyntax.Enums;
using NUnit.Framework;

namespace Tests
{
    internal partial  class TokenizationTests : BaseTest
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
                                              }).SetName(nameof(OperatorType.UnaryMinus) + nameof(OperatorType.Factorial));
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
                                              }).SetName(nameof(OperatorType.InRange) + "length1");
                yield return new TestCaseData("a in (b, c)",
                                              new[]
                                              {
                                                  "a", "in", "(", "b", ",", "c", ")",
                                              }).SetName(nameof(OperatorType.InRange) + "length2");
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
            }
        }

        private static IEnumerable EscapingGrammarCases
        {
            get
            {
                yield return new TestCaseData("[&]&[&]",
                                              new[]
                                              {
                                                  "[&]", "&", "[&]",
                                              }).SetName(nameof(OperatorType.BitwiseAnd));
                yield return new TestCaseData("[&&&]&&[&&]",
                                              new[]
                                              {
                                                  "[&&&]", "&&", "[&&]",
                                              }).SetName(nameof(OperatorType.And));
                yield return new TestCaseData("[andrey]and[sand]",
                                              new[]
                                              {
                                                  "[andrey]", "and", "[sand]",
                                              }).SetName(nameof(OperatorType.And) + "Word");
                yield return new TestCaseData("[|]|[|]",
                                              new[]
                                              {
                                                  "[|]", "|", "[|]",
                                              }).SetName(nameof(OperatorType.BitwiseOr));
                yield return new TestCaseData("[|||]||[||]",
                                              new[]
                                              {
                                                  "[|||]", "||", "[||]",
                                              }).SetName(nameof(OperatorType.Or));
                yield return new TestCaseData("[orrey]or[sor]",
                                              new[]
                                              {
                                                  "[orrey]", "or", "[sor]",
                                              }).SetName(nameof(OperatorType.Or) + "Word");
                yield return new TestCaseData("[+]+[++]",
                                              new[]
                                              {
                                                  "[+]", "+", "[++]",
                                              }).SetName(nameof(OperatorType.Plus));
                yield return new TestCaseData("[-]-[--]",
                                              new[]
                                              {
                                                  "[-]", "-", "[--]",
                                              }).SetName(nameof(OperatorType.Minus));

                yield return new TestCaseData("[*]*[**]",
                                              new[]
                                              {
                                                  "[*]", "*", "[**]",
                                              }).SetName(nameof(OperatorType.Multiple));
                yield return new TestCaseData("[/]/[//]",
                                              new[]
                                              {
                                                  "[/]", "/", "[//]",
                                              }).SetName(nameof(OperatorType.Divide));
                yield return new TestCaseData("[%]%[%%]",
                                              new[]
                                              {
                                                  "[%]", "%", "[%%]",
                                              }).SetName(nameof(OperatorType.Reminder));
                yield return new TestCaseData("[^]^[^^]",
                                              new[]
                                              {
                                                  "[^]", "^", "[^^]",
                                              }).SetName(nameof(OperatorType.Xor));
                yield return new TestCaseData("[=]=[=]",
                                              new[]
                                              {
                                                  "[=]", "=", "[=]",
                                              }).SetName(nameof(OperatorType.Equals) + "length1");
                yield return new TestCaseData("[==]==[=]",
                                              new[]
                                              {
                                                  "[==]", "==", "[=]",
                                              }).SetName(nameof(OperatorType.Equals) + "length2");
                yield return new TestCaseData("[===]===[=]",
                                              new[]
                                              {
                                                  "[===]", "===", "[=]",
                                              }).SetName(nameof(OperatorType.Equals) + "length3");
                yield return new TestCaseData("[!=]!=[!!=]",
                                              new[]
                                              {
                                                  "[!=]", "!=", "[!!=]",
                                              }).SetName(nameof(OperatorType.NotEquals) + "C");
                yield return new TestCaseData("[<>]<>[><><]",
                                              new[]
                                              {
                                                  "[<>]", "<>", "[><><]",
                                              }).SetName(nameof(OperatorType.NotEquals) + "Sql");
                yield return new TestCaseData("[>>]>[>]",
                                              new[]
                                              {
                                                  "[>>]", ">", "[>]",
                                              }).SetName(nameof(OperatorType.GreaterThan));
                yield return new TestCaseData("[>=>=]>=[>==]",
                                              new[]
                                              {
                                                  "[>=>=]", ">=", "[>==]",
                                              }).SetName(nameof(OperatorType.GreaterOrEqualsThan));
                yield return new TestCaseData("[<<]<[<]",
                                              new[]
                                              {
                                                  "[<<]", "<", "[<]",
                                              }).SetName(nameof(OperatorType.LesserThan));
                yield return new TestCaseData("[<=<=]<=[<==]",
                                              new[]
                                              {
                                                  "[<=<=]", "<=", "[<==]",
                                              }).SetName(nameof(OperatorType.LesserOrEqualsThan));
                yield return new TestCaseData("[in]in[in]",
                                              new[]
                                              {
                                                  "[in]", "in", "[in]",
                                              }).SetName(nameof(OperatorType.InRange));
            }
        }

        [Test]
        [TestCaseSource(nameof(PositiveGrammarCases))]
        public void PositiveGrammar(string equation, string[] expectedTokens)
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
        
        [Test]
        [TestCaseSource(nameof(EscapingGrammarCases))]
        public void EscapingGrammar(string equation, string[] expectedTokens)
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
        [TestCaseSource(nameof(PositiveGrammarWithBracketsCases))]
        public void PositiveGrammarWithBrackets(string equation, string[] expectedTokens)
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