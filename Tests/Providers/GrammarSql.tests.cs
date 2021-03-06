using System;
using System.Collections;
using System.Linq;
using FerOmega.Entities;
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
                                              "where ( count + @1 ) + @0",
                                              new object[]
                                              {
                                                  2, "b",
                                              }).SetName(nameof(OperatorType.Plus));

                yield return new TestCaseData("[count] - b - 2",
                                              new[]
                                              {
                                                  "count",
                                              },
                                              "where ( count - @1 ) - @0",
                                              new object[]
                                              {
                                                  2, "b",
                                              }).SetName(nameof(OperatorType.Minus));

                yield return new TestCaseData("[count] * b * 2",
                                              new[]
                                              {
                                                  "count",
                                              },
                                              "where ( count * @1 ) * @0",
                                              new object[]
                                              {
                                                  2, "b",
                                              }).SetName(nameof(OperatorType.Multiple));

                yield return new TestCaseData("[count] / b / 2",
                                              new[]
                                              {
                                                  "count",
                                              },
                                              "where ( count / @1 ) / @0",
                                              new object[]
                                              {
                                                  2, "b",
                                              }).SetName(nameof(OperatorType.Divide));

                yield return new TestCaseData("[count] % b % 2",
                                              new[]
                                              {
                                                  "count",
                                              },
                                              "where ( count % @1 ) % @0",
                                              new object[]
                                              {
                                                  2, "b",
                                              }).SetName(nameof(OperatorType.Reminder));

                yield return new TestCaseData("+count + b + 2",
                                              new[]
                                              {
                                                  "count",
                                              },
                                              "where ( + count + @1 ) + @0",
                                              new object[]
                                              {
                                                  2, "b",
                                              }).SetName(nameof(OperatorType.UnaryPlus) + " for property");

                yield return new TestCaseData("+b + count + 2",
                                              new[]
                                              {
                                                  "count",
                                              },
                                              "where ( + @1 + count ) + @0",
                                              new object[]
                                              {
                                                  2, "b",
                                              }).SetName(nameof(OperatorType.UnaryPlus) + " for const");

                yield return new TestCaseData("-count - b - 2",
                                              new[]
                                              {
                                                  "count",
                                              },
                                              "where ( - count - @1 ) - @0",
                                              new object[]
                                              {
                                                  2, "b",
                                              }).SetName(nameof(OperatorType.UnaryMinus) + " for property");

                yield return new TestCaseData("-b - count - 2",
                                              new[]
                                              {
                                                  "count",
                                              },
                                              "where ( - @1 - count ) - @0",
                                              new object[]
                                              {
                                                  2, "b",
                                              }).SetName(nameof(OperatorType.UnaryMinus) + " for const");

                yield return new TestCaseData("!count + b - 2",
                                              new[]
                                              {
                                                  "count",
                                              },
                                              "where ( ! count + @1 ) - @0",
                                              new object[]
                                              {
                                                  2, "b",
                                              }).SetName(nameof(OperatorType.Not) + " for property");

                yield return new TestCaseData("!b + count - 2",
                                              new[]
                                              {
                                                  "count",
                                              },
                                              "where ( ! @1 + count ) - @0",
                                              new object[]
                                              {
                                                  2, "b",
                                              }).SetName(nameof(OperatorType.Not) + " for const");

                yield return new TestCaseData("[count] + 1 > [length]",
                                              new[]
                                              {
                                                  "count", "length",
                                              },
                                              "where count + @0 > length",
                                              new object[]
                                              {
                                                  1,
                                              }).SetName(nameof(OperatorType.GreaterThan));

                yield return new TestCaseData("[count] + 1 >= [length]",
                                              new[]
                                              {
                                                  "count", "length",
                                              },
                                              "where count + @0 >= length",
                                              new object[]
                                              {
                                                  1,
                                              }).SetName(nameof(OperatorType.GreaterOrEqualsThan));

                yield return new TestCaseData("[count] + 1 < [length]",
                                              new[]
                                              {
                                                  "count", "length",
                                              },
                                              "where count + @0 < length",
                                              new object[]
                                              {
                                                  1,
                                              }).SetName(nameof(OperatorType.LesserThan));

                yield return new TestCaseData("[count] + 1 <= [length]",
                                              new[]
                                              {
                                                  "count", "length",
                                              },
                                              "where count + @0 <= length",
                                              new object[]
                                              {
                                                  1,
                                              }).SetName(nameof(OperatorType.LesserOrEqualsThan));

                yield return new TestCaseData("[count] + 1 = [length]",
                                              new[]
                                              {
                                                  "count", "length",
                                              },
                                              "where count + @0 = length",
                                              new object[]
                                              {
                                                  1,
                                              }).SetName(nameof(OperatorType.Equals) + " sql style");

                yield return new TestCaseData("[count] + 1 == [length]",
                                              new[]
                                              {
                                                  "count", "length",
                                              },
                                              "where count + @0 = length",
                                              new object[]
                                              {
                                                  1,
                                              }).SetName(nameof(OperatorType.Equals) + " C style");

                yield return new TestCaseData("[count] + 1 === [length]",
                                              new[]
                                              {
                                                  "count", "length",
                                              },
                                              "where count + @0 = length",
                                              new object[]
                                              {
                                                  1,
                                              }).SetName(nameof(OperatorType.Equals) + " js style");

                yield return new TestCaseData("[count] + 1 <> [length]",
                                              new[]
                                              {
                                                  "count", "length",
                                              },
                                              "where count + @0 <> length",
                                              new object[]
                                              {
                                                  1,
                                              }).SetName(nameof(OperatorType.NotEquals) + " sql style");

                yield return new TestCaseData("[count] + 1 != [length]",
                                              new[]
                                              {
                                                  "count", "length",
                                              },
                                              "where count + @0 <> length",
                                              new object[]
                                              {
                                                  1,
                                              }).SetName(nameof(OperatorType.NotEquals) + " C style");

                yield return new TestCaseData("[count] + 1 !== [length]",
                                              new[]
                                              {
                                                  "count", "length",
                                              },
                                              "where count + @0 <> length",
                                              new object[]
                                              {
                                                  1,
                                              }).SetName(nameof(OperatorType.NotEquals) + " js style");

                yield return new TestCaseData("[count] == 1 && [length] < 3",
                                              new[]
                                              {
                                                  "count", "length",
                                              },
                                              "where count = @1 and length < @0",
                                              new object[]
                                              {
                                                  3, 1,
                                              }).SetName(nameof(OperatorType.And) + " C style");

                yield return new TestCaseData("[count] == 1 and [length] < 3",
                                              new[]
                                              {
                                                  "count", "length",
                                              },
                                              "where count = @1 and length < @0",
                                              new object[]
                                              {
                                                  3, 1,
                                              }).SetName(nameof(OperatorType.And) + " sql style");

                yield return new TestCaseData("[count] == 1 || [length] < 3",
                                              new[]
                                              {
                                                  "count", "length",
                                              },
                                              "where count = @1 or length < @0",
                                              new object[]
                                              {
                                                  3, 1,
                                              }).SetName(nameof(OperatorType.Or) + " C style");

                yield return new TestCaseData("[count] == 1 or [length] < 3",
                                              new[]
                                              {
                                                  "count", "length",
                                              },
                                              "where count = @1 or length < @0",
                                              new object[]
                                              {
                                                  3, 1,
                                              }).SetName(nameof(OperatorType.Or) + " sql style");

                yield return new TestCaseData("[count] == 1 ^ [length] < 3",
                                              new[]
                                              {
                                                  "count", "length",
                                              },
                                              "where count = @1 ^ length < @0",
                                              new object[]
                                              {
                                                  3, 1,
                                              }).SetName(nameof(OperatorType.Xor));

                yield return new TestCaseData("([count] + 1) * 2 < [length]",
                                              new[]
                                              {
                                                  "count", "length",
                                              },
                                              "where ( count + @1 ) * @0 < length",
                                              new object[]
                                              {
                                                  2, 1,
                                              }).SetName("Simple brackets");

                yield return new TestCaseData("(([count] + 1) / ([length] - 1)) * 2 > 0",
                                              new[]
                                              {
                                                  "count", "length",
                                              },
                                              "where ( ( count + @3 ) / ( length - @2 ) ) * @1 > @0",
                                              new object[]
                                              {
                                                  0, 2, 1, 1,
                                              }).SetName("Nesting brackets");

                yield return new TestCaseData("1, 2, 3",
                                              new string[0],
                                              "where @2 , @1 , @0",
                                              new object[]
                                              {
                                                  3, 2, 1,
                                              }).SetName(nameof(OperatorType.Separator));

                yield return new TestCaseData("[count] in (1)",
                                              new[]
                                              {
                                                  "count",
                                              },
                                              "where count in ( @0 )",
                                              new object[]
                                              {
                                                  3, 2, 1,
                                              }).SetName(nameof(OperatorType.InRange) + " with range length of one");

                yield return new TestCaseData("[count] in (1, 2, 3)",
                                              new[]
                                              {
                                                  "count",
                                              },
                                              "where count in ( @2 , @1 , @0 )",
                                              new object[]
                                              {
                                                  3, 2, 1,
                                              }).SetName(nameof(OperatorType.InRange) + " with range length of three");

                yield return new TestCaseData("[count] contains [value]",
                                              new[]
                                              {
                                                  "count",
                                              },
                                              "where count like @0",
                                              new object[]
                                              {
                                                  "%value%",
                                              }).SetName(nameof(OperatorType.Contains));

                yield return new TestCaseData("[count] startsWith [value]",
                                              new[]
                                              {
                                                  "count",
                                              },
                                              "where count like @0",
                                              new object[]
                                              {
                                                  "value%",
                                              }).SetName(nameof(OperatorType.StartsWith));

                yield return new TestCaseData("[count] endsWith [value]",
                                              new[]
                                              {
                                                  "count",
                                              },
                                              "where count like @0",
                                              new object[]
                                              {
                                                  "%value",
                                              }).SetName(nameof(OperatorType.EndsWith));

                yield return new TestCaseData("[count] = [count]",
                                              new[]
                                              {
                                                  "count",
                                              },
                                              "where count = @0",
                                              new object[]
                                              {
                                                  "count",
                                              }).SetName("Escaping");

                yield return new TestCaseData("a & b & c",
                                              "[a] & [b] & [c]").SetName(nameof(OperatorType.BitwiseAnd))
                                                                .Ignore("Implement later");

                yield return new TestCaseData("a | b | c",
                                              "[a] | [b] | [c]").SetName(nameof(OperatorType.BitwiseOr))
                                                                .Ignore("Implement later");

                yield return new TestCaseData("a;",
                                              "[a];").SetName(nameof(OperatorType.Terminator))
                                                     .Ignore("Implement later");
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

            SqlFilter sql;
            object[] parameters;

            try
            {
                var propertyDefinitions = allowedProperties.Select(x => new PropertyDef(x, x)).ToArray();
                (sql, parameters) = SqlProvider.Convert(tree, propertyDefinitions);
            }
            catch (Exception e)
            {
                throw new Exception("Sql fails", e);
            }

            Assert.AreEqual(expectedSql, sql.ToString());

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
