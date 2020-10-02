using FerOmega.Providers;
using FerOmega.Providers.Abstractions;
using FerOmega.Services;
using FerOmega.Services.Abstractions;
using FerOmega.Services.configs;
using NUnit.Framework;

namespace FerOmega.Tests
{
    internal abstract class BaseTest
    {
        protected IAstService AstService;

        protected ISqlProvider SqlProvider;

        protected ITokenizationService TokenizationService;

        [OneTimeSetUp]
        public void Init()
        {
            var internalGrammarConfig = new InternalGrammarConfig();
            var internalGrammarService = new GrammarService<InternalGrammarConfig>(internalGrammarConfig);
            TokenizationService = new TokenizationService(internalGrammarService);
            var operatorService = new OperatorService(internalGrammarService);
            AstService = new AstService(internalGrammarService, operatorService);
            var sqlGrammarConfig = new SqlGrammarConfig();
            var sqlGrammarService = new GrammarService<SqlGrammarConfig>(sqlGrammarConfig);
            SqlProvider = new SqlProvider(sqlGrammarService);
        }
    }
}