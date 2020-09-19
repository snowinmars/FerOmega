using System.Linq;
using System.Text.RegularExpressions;
using FerOmega.Abstractions;

namespace FerOmega.Services
{
    public class TokenizationService : ITokenizationService
    {
        public TokenizationService()
        {
            GrammarService = new GrammarService();

            OperatorRegex = CreateRegexToSplitByOperators();
        }

        private GrammarService GrammarService { get; }

        private string OperatorRegex { get; }

        /// <summary>
        /// Split string into array of tokens
        /// Each token could be an operator or an operand
        /// </summary>
        public string[] Tokenizate(string equation)
        {
            const RegexOptions RegexOptions =
                RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;

            var tokens = Regex.Split(equation, OperatorRegex, RegexOptions)
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
            var operators = GrammarService
                            .OperatorDenotations
                            .OrderByDescending(x => x.Length) // see the difference between '(!=|!|=)' and '(!|=|!=)' regexes?
                            .Select(EscapeOperatorDenomination) // due to I have to escape some symbols
                            .Distinct() // due to I can have overloaded operators
                            .Where(x => !string.IsNullOrWhiteSpace(x)); // due to I want to ignore some operators

            var operatorRegex = string.Join("|", operators);

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