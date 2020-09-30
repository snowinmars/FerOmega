using FerOmega.Abstractions;
using FerOmega.Services;
using NUnit.Framework;

namespace FerOmega.Tests.SqlProvider
{
    internal class ConvertTests
    {
        private AstShuntingYardService astService;

        private Providers.SqlProvider sqlProvider;

        private ITokenizationService tokenizeService;

        public ConvertTests()
        {
            tokenizeService = new TokenizationService();
            astService = new AstShuntingYardService();
            sqlProvider = new Providers.SqlProvider();
        }
        
        [Test]
        public void Foo()
        {
            const string equation = "familyId = 1415 or count % 20 = 3";

            var tokens = tokenizeService.Tokenizate(equation);
            var tree = astService.Parse(tokens);
            var sql = sqlProvider.Convert(tree);
        }
    }
}