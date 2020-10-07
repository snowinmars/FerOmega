using System.Collections.Generic;
using System.Linq;

namespace FerOmega.Services.configs
{
    internal class AbstractGrammarConfig
    {
        // some operators can not be inserted in regex 'as is'
        private static IEnumerable<string> Escape(IEnumerable<string> denotations)
        {
            foreach (var denotation in denotations)
            {
                switch (denotation)
                {
                case "+":
                    yield return @"\+";

                    break;

                case "=":
                case "==":
                case "===":
                    yield return "=+";

                    break;

                case "&":
                case "&&":
                    yield return "&+";

                    break;

                case "*":
                    yield return @"\*";

                    break;

                case "^":
                    yield return @"\^";

                    break;

                case "/":
                    yield return @"\/";

                    break;

                case "|":
                case "||":
                    yield return @"\|+";

                    break;

                case "(":
                    yield return @"\(";

                    break;

                case ")":
                    yield return @"\)";

                    break;

                case "[":
                    yield return ""; // ?

                    break;

                case "]":
                    yield return ""; // ?

                    break;

                case "{":
                    yield return @"\{";

                    break;

                case "}":
                    yield return @"\}";

                    break;

                default:
                    yield return denotation;

                    break;
                }
            }
        }

        public string GetOperatorsAsRegex(IEnumerable<string> denotations)
        {
            // OrderByDescending - see the difference between '(!=|!|=)' and '(!|=|!=)' regexes?
            var escapes = Escape(denotations.OrderByDescending(x => x.Length))
                          .Distinct()                                 // I can have overloaded operators
                          .Where(x => !string.IsNullOrWhiteSpace(x)); // I want to ignore some operators

            // f.e.,
            //      input: a>5  &&  b+7  ==2
            //      regex: >|   &+| \+|  =+
            var operatorRegex = string.Join("|", escapes);

            const string valueRegex = "\\[.*?\\]";

            // if regex pattern is in the global scope, the delimiters will be included to the Matches collection
            return $"({valueRegex}|{operatorRegex})";
        }
    }
}