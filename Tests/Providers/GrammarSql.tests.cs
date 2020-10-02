using System;
using System.Collections;
using FerOmega.Entities.AbstractSyntax;
using FerOmega.Entities.InternalSyntax;
using FerOmega.Entities.InternalSyntax.Enums;
using NUnit.Framework;

namespace FerOmega.Tests.Providers
{
    internal class GrammarSqlTests : BaseTest
    {
         private static IEnumerable PositiveGrammarCases
        {
            get
            {
                yield return new TestCaseData("a + b + c",
                                             "[a] + [b] + [c]").SetName(nameof(OperatorType.Plus));

                yield return new TestCaseData("a - b - c",
                                              "[a] - [b] - [c]").SetName(nameof(OperatorType.Minus));

                yield return new TestCaseData("a * b * c",
                                              "[a] * [b] * [c]").SetName(nameof(OperatorType.Multiple));

                yield return new TestCaseData("a / b / c",
                                              "[a] / [b] / [c]").SetName(nameof(OperatorType.Divide));

                yield return new TestCaseData("a % b % c",
                                              "[a] % [b] % [c]").SetName(nameof(OperatorType.Reminder));

                yield return new TestCaseData("+ a + b + c",
                                              "+ [a] + [b] + [c]").SetName(nameof(OperatorType.UnaryPlus));

                yield return new TestCaseData("- a - b - c",
                                              "- [a] - [b] - [c]").SetName(nameof(OperatorType.UnaryMinus));

                yield return new TestCaseData("a! - b! - c!",
                                              "[a]! - [b]! - [c]!").SetName(nameof(OperatorType.Factorial));

                yield return new TestCaseData("-a! - b! - c!",
                                              "-[a]! - [b]! - [c]!").SetName(nameof(OperatorType.UnaryMinus) +
                                                                       nameof(OperatorType.Factorial));

                yield return new TestCaseData("!d!-!b!",
                                              "![d]!-![b]!").SetName(nameof(OperatorType.Not) + nameof(OperatorType.Factorial));

                yield return new TestCaseData("a + b > c",
                                              "[a] + [b] > [c]").SetName(nameof(OperatorType.GreaterThan));

                yield return new TestCaseData("a + b >= c",
                                              "[a] + [b] >= [c]").SetName(nameof(OperatorType.GreaterOrEqualsThan));

                yield return new TestCaseData("a + b < c",
                                              "[a] + [b] < [c]").SetName(nameof(OperatorType.LesserThan));

                yield return new TestCaseData("a + b <= c",
                                              "[a] + [b] <= [c]").SetName(nameof(OperatorType.LesserOrEqualsThan));

                yield return new TestCaseData("a + b = c",
                                              "[a] + [b] = [c]").SetName("sql" + nameof(OperatorType.Equals));

                yield return new TestCaseData("a + b == c",
                                              "[a] + [b] = [c]").SetName("c" + nameof(OperatorType.Equals));

                yield return new TestCaseData("a + b === c",
                                              "[a] + [b] = [c]").SetName("js" + nameof(OperatorType.Equals));

                yield return new TestCaseData("a + b <> c",
                                              "[a] + [b] <> [c]").SetName("sql" + nameof(OperatorType.NotEquals));

                yield return new TestCaseData("a + b != c",
                                              "[a] + [b] <> [c]").SetName("c" + nameof(OperatorType.NotEquals));

                yield return new TestCaseData("a & b & c",
                                              "[a] & [b] & [c]").SetName(nameof(OperatorType.BitwiseAnd));

                yield return new TestCaseData("a | b | c",
                                              "[a] | [b] | [c]").SetName(nameof(OperatorType.BitwiseOr));

                yield return new TestCaseData("a && b && c",
                                              "[a] and [b] and [c]").SetName(nameof(OperatorType.And));

                yield return new TestCaseData("a || b || c",
                                              "[a] or [b] or [c]").SetName(nameof(OperatorType.Or));

                yield return new TestCaseData("a ^ b ^ c",
                                              "[a] ^ [b] ^ [c]").SetName(nameof(OperatorType.Xor));

                yield return new TestCaseData("a in (b)",
                                              "[a] in ([b])").SetName(nameof(OperatorType.InRange) + "Length1");

                yield return new TestCaseData("a in (b, c)",
                                              "[a] in ([b], [c])").SetName(nameof(OperatorType.InRange) + "Length2");

                yield return new TestCaseData("a contains b",
                                              "[a] like \"%b%\"").SetName(nameof(OperatorType.Contains));

                yield return new TestCaseData("a startsWith b",
                                              "[a] like \"b%\"").SetName(nameof(OperatorType.StartsWith));

                yield return new TestCaseData("a endsWith b",
                                              "[a] like \"%b\"").SetName(nameof(OperatorType.EndsWith));
                yield return new TestCaseData("a,b",
                                              "[a], [b]").SetName(nameof(OperatorType.Enumerator));
                yield return new TestCaseData("a;",
                                              "[a];").SetName(nameof(OperatorType.Terminator));
            }
        }
        
        [Test]
        [TestCaseSource(nameof(PositiveGrammarCases))]
        public void Smoke(string equation, string expected)
        {
            var tokens = TokenizationService.Tokenizate(equation);

            Tree<AbstractToken> tree;

            try
            {
                tree = AstService.Convert(tokens);
            }
            catch (Exception e)
            {
                throw new Exception("Ast fails", e);
            }

            string sql;

            try
            {
                sql = SqlProvider.Convert(tree);
            }
            catch (Exception e)
            {
                throw new Exception("Sql fails", e);
            }
            
            Assert.AreEqual(expected, sql);
        }
    }
}