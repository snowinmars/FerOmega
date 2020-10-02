using System.Linq;
using System.Text.RegularExpressions;
using FerOmega.Services.Abstractions;
using FerOmega.Services.configs;

namespace FerOmega.Services
{
    internal class TokenizationService : ITokenizationService
    {
        public TokenizationService(IGrammarService<InternalGrammarConfig> grammarService)
        {
            this.grammarService = grammarService;
            OperatorRegex = CreateRegexToSplitByOperators();
        }

        private string OperatorRegex { get; }

        private readonly IGrammarService<InternalGrammarConfig> grammarService;

        public string[] Tokenizate(string equation)
        {
            if (string.IsNullOrWhiteSpace(equation))
            {
                return new string[0];
            }

            const RegexOptions regexOptions = RegexOptions.Compiled |
                                              RegexOptions.CultureInvariant |
                                              RegexOptions.IgnoreCase;

            var tokens = Regex.Split(equation, OperatorRegex, regexOptions)
                              .Where(x => !string.IsNullOrWhiteSpace(x))
                              .Select(x => x.Trim())
                              .ToArray();

            return tokens;
        }

        private string CreateRegexToSplitByOperators()
        {
            // f.e.,
            //      input: a>5  &&  b+7  ==2
            //      regex: >|   &+| \+|  =+
            var operators = grammarService
                            .OperatorDenotations
                            .OrderByDescending(x => x.Length) // see the difference between '(!=|!|=)' and '(!|=|!=)' regexes?
                            .Select(EscapeOperatorDenomination) // due to I have to escape some symbols
                            .Distinct() // due to I can have overloaded operators
                            .Where(x => !string.IsNullOrWhiteSpace(x)); // due to I want to ignore some operators

            var operatorRegex = string.Join("|", operators);

            // if regex pattern is in the global scope, the delimiters will be included to the Matches collection
            return $"(\\[.*?\\]|{operatorRegex})";
        }

        private string EscapeOperatorDenomination(string symbol)
        {
            switch (symbol)
            {
            case "+":
                return @"\+";

            case "=":
            case "==":
            case "===":
                return "=+";

            case "&":
            case "&&":
                return "&+";

            case "*":
                return @"\*";

            case "^":
                return @"\^";

            case "/":
                return @"\/";

            case "|":
            case "||":
                return @"\|+";

            case "(":
                return @"\(";

            case ")":
                return @"\)";

            case "[":
                return ""; // ?

            case "]":
                return ""; // ?

            case "{":
                return @"\{";

            case "}":
                return @"\}";

            default:
                return symbol;
            }
        }
    }
}