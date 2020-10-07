using FerOmega.FerOmega;
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

        protected IGrammarService<IGrammarConfig> InternalGrammarService;

        protected IGrammarService<IGrammarConfig> SqlGrammarService;

        protected ISqlProvider SqlProvider;

        protected ITokenizationService TokenizationService;

        [OneTimeSetUp]
        public void Init()
        {
            InternalGrammarService = FerOmegaInjections.InternalGrammarService;
            TokenizationService = FerOmegaInjections.TokenizationService;
            AstService = FerOmegaInjections.AstService;
            SqlGrammarService = FerOmegaInjections.SqlGrammarService;
            SqlProvider = FerOmegaInjections.SqlProvider;
        }
    }
}
