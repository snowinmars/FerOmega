using System.Linq;
using System.Text.RegularExpressions;

using FerOmega.Abstractions;

namespace FerOmega.Services
{
    public class TokenizationService : ITokenizationService
    {
        private GrammarService GrammarService { get; }

        private string OperatorRegex { get; }

        public TokenizationService()
        {
            GrammarService = new GrammarService();

            OperatorRegex = CreateRegexToSplitByOperators();
        }

        public string[] Tokenizate(string equation)
        {
            const RegexOptions RegexOptions = RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;

            var tokens = Regex.Split(equation, OperatorRegex, RegexOptions)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();

            return tokens;
        }

        private string CreateRegexToSplitByOperators()
        {
            // f.e.,
            //      input: a>5  &&  b+7  ==2
            //      regex: >|   &+| \+|  =+
            var operatorRegex = string.Join("|", GrammarService.OperatorDenotations.OrderByDescending(x => x.Length).Select(EscapeOperatorDenomination).Distinct().Where(x => !string.IsNullOrWhiteSpace(x)));

            // if regex pattern is in the global scope, the delimiters will be included to the Matches collection
            return $"({operatorRegex})";
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
                    return "";

                case "]":
                    return "";

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