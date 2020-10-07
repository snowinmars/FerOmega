using System.Linq;
using FerOmega.Entities.InternalSyntax.Enums;
using NUnit.Framework;

namespace FerOmega.Tests.Providers
{
    internal class EscapingTests : BaseTest
    {
        private class Foo
        {
            public Foo(string input, string sql)
            {
                Input = input;
                Sql = sql;
            }

            public string Input { get; }

            public string Sql { get; }
        }

        [Test]
        public void Escape()
        {
            const string allowedProperty = "name";

            var equations = (from internalOperator in InternalGrammarService.Operators
                             from internalOperatorDenotation in internalOperator.Denotations
                             join sqlOperator in SqlGrammarService.Operators
                                 on internalOperator.OperatorType equals sqlOperator.OperatorType
                             where internalOperator.Arity == Arity.Binary && sqlOperator.Arity == Arity.Binary
                             where !new[]
                             {
                                 OperatorType.Contains, OperatorType.StartsWith, OperatorType.EndsWith
                             }.Contains(internalOperator.OperatorType) // different pattern
                             select new
                             {
                                 Input = $"[{allowedProperty}] {internalOperatorDenotation} [{allowedProperty}]",
                                 Sql = $"{allowedProperty} {sqlOperator.MainDenotation} @0",
                             })
                            .Concat(new[]
                            {
                                new
                                {
                                    Input =
                                        $"[{allowedProperty}] {InternalGrammarService.Operators.First(x => x.OperatorType == OperatorType.Contains).MainDenotation} [{allowedProperty}]",
                                    Sql =
                                        $"{allowedProperty} {SqlGrammarService.Operators.First(x => x.OperatorType == OperatorType.Contains).MainDenotation} '%@0%'",
                                },
                                new
                                {
                                    Input =
                                        $"[{allowedProperty}] {InternalGrammarService.Operators.First(x => x.OperatorType == OperatorType.StartsWith).MainDenotation} [{allowedProperty}]",
                                    Sql =
                                        $"{allowedProperty} {SqlGrammarService.Operators.First(x => x.OperatorType == OperatorType.StartsWith).MainDenotation} '@0%'",
                                },
                                new
                                {
                                    Input =
                                        $"[{allowedProperty}] {InternalGrammarService.Operators.First(x => x.OperatorType == OperatorType.EndsWith).MainDenotation} [{allowedProperty}]",
                                    Sql =
                                        $"{allowedProperty} {SqlGrammarService.Operators.First(x => x.OperatorType == OperatorType.EndsWith).MainDenotation} '%@0'",
                                },
                            })
                            .ToArray();

            foreach (var equation in equations)
            {
                var inputEquation = equation.Input;
                var expectedSql = equation.Sql;

                var tokens = TokenizationService.Tokenizate(inputEquation);
                var tree = AstService.Convert(tokens);

                var (actualSql, actualParameters) = SqlProvider.Convert(tree,
                                                                        SqlProvider.DefineProperty()
                                                                            .From(allowedProperty)
                                                                            .ToSql(allowedProperty));

                Assert.AreEqual(expectedSql, actualSql);
                Assert.AreEqual(1, actualParameters.Length);
                Assert.AreEqual(allowedProperty, actualParameters[0]);
            }
        }
    }
}