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
                yield return new TestCaseData("[count] + b + 2",
                                              new[]
                                              {
                                                  "count",
                                              },
                                              "[count] + @1 + @0",
                                              new object[]
                                              {
                                                  "2", "b",
                                              }).SetName(nameof(OperatorType.Plus));

                yield return new TestCaseData("[count] - b - 2",
                                              new[]
                                              {
                                                  "count",
                                              },
                                              "[count] - @1 - @0",
                                              new object[]
                                              {
                                                  "2", "b",
                                              }).SetName(nameof(OperatorType.Minus));

                yield return new TestCaseData("[count] * b * 2",
                                              new[]
                                              {
                                                  "count",
                                              },
                                              "[count] * @1 * @0",
                                              new object[]
                                              {
                                                  "2", "b",
                                              }).SetName(nameof(OperatorType.Multiple));

                yield return new TestCaseData("[count] / b / 2",
                                              new[]
                                              {
                                                  "count",
                                              },
                                              "[count] / @1 / @0",
                                              new object[]
                                              {
                                                  "2", "b",
                                              }).SetName(nameof(OperatorType.Divide));

                yield return new TestCaseData("[count] % b % 2",
                                              new[]
                                              {
                                                  "count",
                                              },
                                              "[count] % @1 % @0",
                                              new object[]
                                              {
                                                  "2", "b",
                                              }).SetName(nameof(OperatorType.Reminder));

                yield return new TestCaseData("+count + b + 2",
                                              new[]
                                              {
                                                  "count",
                                              },
                                              "+ [count] + @1 + @0",
                                              new object[]
                                              {
                                                  "2", "b",
                                              }).SetName(nameof(OperatorType.UnaryPlus) + " for property");

                yield return new TestCaseData("+b + count + 2",
                                              new[]
                                              {
                                                  "count",
                                              },
                                              "+ @1 + [count] + @0",
                                              new object[]
                                              {
                                                  "2", "b",
                                              }).SetName(nameof(OperatorType.UnaryPlus) + " for const");

                yield return new TestCaseData("-count - b - 2",
                                              new[]
                                              {
                                                  "count",
                                              },
                                              "- [count] - @1 - @0",
                                              new object[]
                                              {
                                                  "2", "b",
                                              }).SetName(nameof(OperatorType.UnaryMinus) + " for property");

                yield return new TestCaseData("-b - count - 2",
                                              new[]
                                              {
                                                  "count",
                                              },
                                              "- @1 - [count] - @0",
                                              new object[]
                                              {
                                                  "2", "b",
                                              }).SetName(nameof(OperatorType.UnaryMinus) + " for const");

                yield return new TestCaseData("!count + b - 2",
                                              new[]
                                              {
                                                  "count",
                                              },
                                              "! [count] + @1 - @0",
                                              new object[]
                                              {
                                                  "2", "b",
                                              }).SetName(nameof(OperatorType.Not) + " for property");

                yield return new TestCaseData("!b + count - 2",
                                              new[]
                                              {
                                                  "count",
                                              },
                                              "! @1 + [count] - @0",
                                              new object[]
                                              {
                                                  "2", "b",
                                              }).SetName(nameof(OperatorType.Not) + " for const");

                yield return new TestCaseData("[count] + 1 > [length]",
                                              new[]
                                              {
                                                  "count", "length",
                                              },
                                              "[count] + @0 > [length]",
                                              new object[]
                                              {
                                                  "1",
                                              }).SetName(nameof(OperatorType.GreaterThan));

                yield return new TestCaseData("[count] + 1 >= [length]",
                                              new[]
                                              {
                                                  "count", "length",
                                              },
                                              "[count] + @0 >= [length]",
                                              new object[]
                                              {
                                                  "1",
                                              }).SetName(nameof(OperatorType.GreaterOrEqualsThan));

                yield return new TestCaseData("[count] + 1 < [length]",
                                              new[]
                                              {
                                                  "count", "length",
                                              },
                                              "[count] + @0 < [length]",
                                              new object[]
                                              {
                                                  "1",
                                              }).SetName(nameof(OperatorType.LesserThan));

                yield return new TestCaseData("[count] + 1 <= [length]",
                                              new[]
                                              {
                                                  "count", "length",
                                              },
                                              "[count] + @0 <= [length]",
                                              new object[]
                                              {
                                                  "1",
                                              }).SetName(nameof(OperatorType.LesserOrEqualsThan));

                yield return new TestCaseData("[count] + 1 = [length]",
                                              new[]
                                              {
                                                  "count", "length",
                                              },
                                              "[count] + @0 = [length]",
                                              new object[]
                                              {
                                                  "1",
                                              }).SetName(nameof(OperatorType.Equals) + " sql style");

                yield return new TestCaseData("[count] + 1 == [length]",
                                              new[]
                                              {
                                                  "count", "length",
                                              },
                                              "[count] + @0 = [length]",
                                              new object[]
                                              {
                                                  "1",
                                              }).SetName(nameof(OperatorType.Equals) + " C style");

                yield return new TestCaseData("[count] + 1 === [length]",
                                              new[]
                                              {
                                                  "count", "length",
                                              },
                                              "[count] + @0 = [length]",
                                              new object[]
                                              {
                                                  "1",
                                              }).SetName(nameof(OperatorType.Equals) + " js style");

                yield return new TestCaseData("[count] + 1 <> [length]",
                                              new[]
                                              {
                                                  "count", "length",
                                              },
                                              "[count] + @0 <> [length]",
                                              new object[]
                                              {
                                                  "1",
                                              }).SetName(nameof(OperatorType.NotEquals) + " sql style");

                yield return new TestCaseData("[count] + 1 != [length]",
                                              new[]
                                              {
                                                  "count", "length",
                                              },
                                              "[count] + @0 <> [length]",
                                              new object[]
                                              {
                                                  "1",
                                              }).SetName(nameof(OperatorType.NotEquals) + " C style");

                yield return new TestCaseData("[count] + 1 !== [length]",
                                              new[]
                                              {
                                                  "count", "length",
                                              },
                                              "[count] + @0 <> [length]",
                                              new object[]
                                              {
                                                  "1",
                                              }).SetName(nameof(OperatorType.NotEquals) + " js style");

                yield return new TestCaseData("a & b & c",
                                              "[a] & [b] & [c]").SetName(nameof(OperatorType.BitwiseAnd));

                yield return new TestCaseData("a | b | c",
                                              "[a] | [b] | [c]").SetName(nameof(OperatorType.BitwiseOr));

                yield return new TestCaseData("[count] == 1 && [length] < 3",
                                              new[]
                                              {
                                                  "count", "length",
                                              },
                                              "[count] = @1 and [length] < @0",
                                              new object[]
                                              {
                                                  "3", "1",
                                              }).SetName(nameof(OperatorType.And) + " C style");

                yield return new TestCaseData("[count] == 1 and [length] < 3",
                                              new[]
                                              {
                                                  "count", "length",
                                              },
                                              "[count] = @1 and [length] < @0",
                                              new object[]
                                              {
                                                  "3", "1",
                                              }).SetName(nameof(OperatorType.And) + " sql style");

                yield return new TestCaseData("[count] == 1 || [length] < 3",
                                              new[]
                                              {
                                                  "count", "length",
                                              },
                                              "[count] = @1 or [length] < @0",
                                              new object[]
                                              {
                                                  "3", "1",
                                              }).SetName(nameof(OperatorType.Or) + " C style");

                yield return new TestCaseData("[count] == 1 or [length] < 3",
                                              new[]
                                              {
                                                  "count", "length",
                                              },
                                              "[count] = @1 or [length] < @0",
                                              new object[]
                                              {
                                                  "3", "1",
                                              }).SetName(nameof(OperatorType.Or) + " sql style");

                yield return new TestCaseData("[count] == 1 ^ [length] < 3",
                                              new[]
                                              {
                                                  "count", "length",
                                              },
                                              "[count] = @1 ^ [length] < @0",
                                              new object[]
                                              {
                                                  "3", "1",
                                              }).SetName(nameof(OperatorType.Xor));

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
        public void Grammar(string equation,
            string[] allowedProperties,
            string expectedSql,
            object[] expectedParameters)
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
            object[] parameters;

            try
            {
                (sql, parameters) = SqlProvider.Convert(tree, allowedProperties);
            }
            catch (Exception e)
            {
                throw new Exception("Sql fails", e);
            }

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