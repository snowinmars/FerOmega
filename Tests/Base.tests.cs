using NUnit.Framework;
using Providers;
using Providers.Abstractions;
using Services;
using Services.Abstractions;

namespace Tests
{
    internal abstract class BaseTest
    {
        protected ITokenizationService TokenizationService;

        protected IAstService AstService;

        protected ISqlProvider SqlProvider;

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