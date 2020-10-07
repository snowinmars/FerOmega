using FerOmega.Providers;
using FerOmega.Providers.Abstractions;
using FerOmega.Services;
using FerOmega.Services.Abstractions;
using FerOmega.Services.configs;
using Microsoft.Extensions.DependencyInjection;

namespace FerOmega.FerOmega
{
    public static class FerOmegaInjections
    {
        static FerOmegaInjections()
        {
            InternalGrammarConfig = new InternalGrammarConfig();
            InternalGrammarService = new GrammarService<IGrammarConfig>(InternalGrammarConfig);

            TokenizationService = new TokenizationService<IGrammarConfig>(InternalGrammarService);

            OperatorService = new OperatorService(InternalGrammarService);

            AstService = new AstService(InternalGrammarService, OperatorService);

            SqlGrammarConfig = new SqlGrammarConfig();
            SqlGrammarService = new GrammarService<IGrammarConfig>(SqlGrammarConfig);

            SqlProvider = new SqlProvider<IGrammarConfig>(SqlGrammarService);
        }

        public static IGrammarConfig InternalGrammarConfig { get; }

        public static IGrammarService<IGrammarConfig> InternalGrammarService { get; }

        public static ITokenizationService TokenizationService { get; }

        public static IOperatorService OperatorService { get; }

        public static IAstService AstService { get; }

        public static IGrammarConfig SqlGrammarConfig { get; }

        public static IGrammarService<IGrammarConfig> SqlGrammarService { get; }

        public static ISqlProvider SqlProvider { get; }

        // ReSharper disable once UnusedMember.Global
        public static IServiceCollection AddFerOmega(this IServiceCollection services)
        {
            services.AddSingleton<ITokenizationService, TokenizationService<IGrammarConfig>>(_ =>
                new TokenizationService<IGrammarConfig>(InternalGrammarService));

            services.AddSingleton<IAstService, AstService>(_ =>
                new AstService(InternalGrammarService, OperatorService));

            services.AddSingleton<ISqlProvider, SqlProvider<IGrammarConfig>>(_ =>
                new SqlProvider<IGrammarConfig>(SqlGrammarService));

            return services;
        }
    }
}
