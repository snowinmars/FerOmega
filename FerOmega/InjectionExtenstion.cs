using FerOmega.Providers;
using FerOmega.Providers.Abstractions;
using FerOmega.Services;
using FerOmega.Services.Abstractions;
using FerOmega.Services.configs;
using Microsoft.Extensions.DependencyInjection;

namespace FerOmega.FerOmega
{
    public static class InjectionExtenstion
    {
        public static IServiceCollection AddFerOmega(this IServiceCollection services)
        {
            var internalGrammarConfig = new InternalGrammarConfig();
            var internalGrammarService = new GrammarService<InternalGrammarConfig>(internalGrammarConfig);

            services.AddSingleton<ITokenizationService, TokenizationService<InternalGrammarConfig>>((_) =>
                new TokenizationService<InternalGrammarConfig>(internalGrammarService));

            var operatorService = new OperatorService(internalGrammarService);

            services.AddSingleton<IAstService, AstService>((_) =>
                new AstService(internalGrammarService, operatorService));
            
            var sqlGrammarConfig = new SqlGrammarConfig();
            var sqlGrammarService = new GrammarService<SqlGrammarConfig>(sqlGrammarConfig);

            services.AddSingleton<ISqlProvider, SqlProvider<SqlGrammarConfig>>((_) =>
                new SqlProvider<SqlGrammarConfig>(sqlGrammarService));
            
            return services;
        }
    }
}