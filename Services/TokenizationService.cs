using System.Linq;
using System.Text.RegularExpressions;
using FerOmega.Services.Abstractions;
using FerOmega.Services.configs;

namespace FerOmega.Services
{
    internal class TokenizationService<T> : ITokenizationService
        where T : IGrammarConfig
    {
        public TokenizationService(IGrammarService<T> grammarService)
        {
            this.grammarService = grammarService;
        }

        private readonly IGrammarService<T> grammarService;

        public string[] Tokenizate(string equation)
        {
            if (string.IsNullOrWhiteSpace(equation))
            {
                return new string[0];
            }

            const RegexOptions regexOptions = RegexOptions.Compiled |
                                              RegexOptions.CultureInvariant |
                                              RegexOptions.IgnoreCase;

            var tokens = Regex.Split(equation, grammarService.OperatorsRegex, regexOptions)
                              .Where(x => !string.IsNullOrWhiteSpace(x))
                              .Select(x => x.Trim())
                              .ToArray();

            return tokens;
        }
    }
}