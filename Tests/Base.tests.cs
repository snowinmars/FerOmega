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

        protected IGrammarService<InternalGrammarConfig> InternalGrammarService;

        protected IGrammarService<SqlGrammarConfig> SqlGrammarService;

        protected ISqlProvider SqlProvider;

        protected ITokenizationService TokenizationService;

        [OneTimeSetUp]
        public void Init()
        {
            var internalGrammarConfig = new InternalGrammarConfig();
            InternalGrammarService = new GrammarService<InternalGrammarConfig>(internalGrammarConfig);
            TokenizationService = new TokenizationService<InternalGrammarConfig>(InternalGrammarService);
            var operatorService = new OperatorService(InternalGrammarService);
            AstService = new AstService(InternalGrammarService, operatorService);
            var sqlGrammarConfig = new SqlGrammarConfig();
            SqlGrammarService = new GrammarService<SqlGrammarConfig>(sqlGrammarConfig);
            SqlProvider = new SqlProvider<SqlGrammarConfig>(SqlGrammarService);
        }
    }
}