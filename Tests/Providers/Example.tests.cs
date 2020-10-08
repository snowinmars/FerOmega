using System;
using NUnit.Framework;

namespace FerOmega.Tests.Providers
{
    internal class ExampleTests : BaseTest
    {
        [Test]
        public void Mess()
        {
            const string input =
                "[id] === [1690ffef-7249-4384-8cba-58842e8d48df] and (([length] + 1) * 2 <= 14 or [email] = [email])";

            const string expectedSql = "id = @4 and ( ( table.length + @3 ) * @2 <= @1 or table2.email = @0 )";

            var expectedParameters = new object[]
            {
                "email", 14, 2, 1, Guid.Parse("1690ffef-7249-4384-8cba-58842e8d48df"),
            };

            var tokens = TokenizationService.Tokenizate(input);
            var tree = AstService.Convert(tokens);

            var (actualSql, actualParameters) = SqlProvider.Convert(tree,
                                                                    SqlProvider.DefineProperty().From("id").ToSql("id"),
                                                                    SqlProvider.DefineProperty()
                                                                               .From("length")
                                                                               .ToSql("table.length"),
                                                                    SqlProvider.DefineProperty()
                                                                               .From("email")
                                                                               .ToSql("table2.email"));

            Assert.AreEqual(expectedSql, actualSql);

            Assert.AreEqual(expectedParameters.Length, actualParameters.Length);

            for (var i = 0; i < actualParameters.Length; i++)
            {
                var expectedParameter = expectedParameters[i];
                var actualParameter = actualParameters[i];

                Assert.AreEqual(expectedParameter, actualParameter);
            }
        }

        [Test]
        public void PropertyRemapping()
        {
            const string input = "[storesCount] > 3 or [salaryCostsPerMonth] > 10000 * [dollarCourse]";
            const string expectedSql = "storesCount > @1 or salarySlice.perMonth > @0 * bank.currentDollarCourse";

            var expectedParameters = new object[]
            {
                10000, 3,
            };

            var tokens = TokenizationService.Tokenizate(input);
            var tree = AstService.Convert(tokens);

            var (actualSql, actualParameters) = SqlProvider.Convert(tree,
                                                                    SqlProvider.DefineProperty()
                                                                               .From("storesCount")
                                                                               .ToSql("storesCount"),
                                                                    SqlProvider.DefineProperty()
                                                                               .From("salaryCostsPerMonth")
                                                                               .ToSql("salarySlice.perMonth"),
                                                                    SqlProvider.DefineProperty()
                                                                               .From("dollarCourse")
                                                                               .ToSql("bank.currentDollarCourse"));

            Assert.AreEqual(expectedSql, actualSql);

            Assert.AreEqual(expectedParameters.Length, actualParameters.Length);

            for (var i = 0; i < actualParameters.Length; i++)
            {
                var expectedParameter = expectedParameters[i];
                var actualParameter = actualParameters[i];

                Assert.AreEqual(expectedParameter, actualParameter);
            }
        }

        [Test]
        public void RangeMath()
        {
            const string input = "[location] in ([Moscow], [St. Petersburg]) and [country] in ([ru], [us])";
            const string expectedSql = "location in ( @3 , @2 ) and country in ( @1 , @0 )";

            var expectedParameters = new object[]
            {
                "us", "ru", "St. Petersburg", "Moscow",
            };

            var tokens = TokenizationService.Tokenizate(input);
            var tree = AstService.Convert(tokens);

            var (actualSql, actualParameters) = SqlProvider.Convert(tree,
                                                                    SqlProvider.DefineProperty()
                                                                               .From("location")
                                                                               .ToSql("location"),
                                                                    SqlProvider.DefineProperty()
                                                                               .From("country")
                                                                               .ToSql("country"));

            Assert.AreEqual(expectedSql, actualSql);

            Assert.AreEqual(expectedParameters.Length, actualParameters.Length);

            for (var i = 0; i < actualParameters.Length; i++)
            {
                var expectedParameter = expectedParameters[i];
                var actualParameter = actualParameters[i];

                Assert.AreEqual(expectedParameter, actualParameter);
            }
        }

        [Test]
        public void SimpleMath()
        {
            const string input = "([age] >= 16 and [country] == [ru]) or ([age] >= 13 and [country] === [ja])";
            const string expectedSql = "age >= @3 and country = @2 or age >= @1 and country = @0";

            var expectedParameters = new object[]
            {
                "ja", 13, "ru", 16,
            };

            var tokens = TokenizationService.Tokenizate(input);
            var tree = AstService.Convert(tokens);

            var (actualSql, actualParameters) = SqlProvider.Convert(tree,
                                                                    SqlProvider.DefineProperty()
                                                                               .From("age")
                                                                               .ToSql("age"),
                                                                    SqlProvider.DefineProperty()
                                                                               .From("country")
                                                                               .ToSql("country"));

            Assert.AreEqual(expectedSql, actualSql);

            Assert.AreEqual(expectedParameters.Length, actualParameters.Length);

            for (var i = 0; i < actualParameters.Length; i++)
            {
                var expectedParameter = expectedParameters[i];
                var actualParameter = actualParameters[i];

                Assert.AreEqual(expectedParameter, actualParameter);
            }
        }

        [Test]
        public void StringLike()
        {
            const string input = "[name] contains [and] or ([name]   startsWith   [Alex] and [name]endsWith[ndr])";
            const string expectedSql = "name like @2 or name like @1 and name like @0";

            var expectedParameters = new object[]
            {
                "%ndr", "Alex%", "%and%",
            };

            var tokens = TokenizationService.Tokenizate(input);
            var tree = AstService.Convert(tokens);

            var (actualSql, actualParameters) = SqlProvider.Convert(tree,
                                                                    SqlProvider.DefineProperty()
                                                                               .From("name")
                                                                               .ToSql("name"));

            Assert.AreEqual(expectedSql, actualSql);

            Assert.AreEqual(expectedParameters.Length, actualParameters.Length);

            for (var i = 0; i < actualParameters.Length; i++)
            {
                var expectedParameter = expectedParameters[i];
                var actualParameter = actualParameters[i];

                Assert.AreEqual(expectedParameter, actualParameter);
            }
        }
    }
}
