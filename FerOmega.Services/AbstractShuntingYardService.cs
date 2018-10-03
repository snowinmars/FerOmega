using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using FerOmega.Entities;

namespace FerOmega.Services
{
    public class AbstractShuntingYardService : IShuntingYardService<Queue<AbstractToken>>
    {
        private GrammarService GrammarService { get; }

        private OperatorResolveService OperatorResolveService { get; }

        private string OperatorRegex { get; }

        public AbstractShuntingYardService()
        {
            GrammarService = new GrammarService();
            OperatorResolveService = new OperatorResolveService();

            OperatorRegex = CreateRegexToSplitByOperators();
        }

        public Queue<AbstractToken> Parse(string equation)
        {
            const RegexOptions RegexOptions = RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;
            var tokens = Regex.Split(equation, OperatorRegex, RegexOptions).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

            var stack = new Stack<AbstractToken>(tokens.Length);
            var queue = new Queue<AbstractToken>(tokens.Length);

            for (int i = 0; i < tokens.Length; i++)
            {
                var token = new OperatorToken(tokens, i);

                if (GrammarService.IsOperand(token.Current))
                {
                    var operand = new Operand(token.Current);

                    queue.Enqueue(operand);
                    continue;
                }

                if (GrammarService.IsOperator(token.Current))
                {
                    var possibleOperators = GetPossibleOperators(token.Current);

                    // length != 1 means we have overloads
                    var @operator = possibleOperators.Length == 1 ? possibleOperators[0] : OperatorResolveService.Resolve(token, possibleOperators);

                    if (GrammarService.IsOpenBracket(@operator))
                    {
                        stack.Push(@operator);
                        continue;
                    }

                    if (GrammarService.IsCloseBracket(@operator))
                    {
                        while (stack.Count >= 0)
                        {
                            if (stack.Count == 0)
                            {
                                throw new InvalidOperationException();
                            }

                            var abstractToken = stack.Pop();

                            if (GrammarService.IsOpenBracket(abstractToken))
                            {
                                break;
                            }

                            queue.Enqueue(abstractToken);
                        }

                        continue;
                    }

                    // if we're here, the operator is not any type of bracket, so it's evaluatable operator

                    switch (@operator.Arity)
                    {
                        case ArityType.Unary when @operator.Fixity == FixityType.Postfix:
                            queue.Enqueue(@operator);
                            break;

                        case ArityType.Unary when @operator.Fixity == FixityType.Prefix:
                            stack.Push(@operator);
                            break;

                        case ArityType.Binary:
                            if (stack.Count == 0)
                            {
                                stack.Push(@operator);
                                break;
                            }

                            switch (@operator.Associativity)
                            {
                                case AssociativityType.Left:
                                {
                                    var popedOperator = stack.Peek();

                                    if (popedOperator.Priority <= @operator.Priority && stack.Count > 0)
                                    {
                                        popedOperator = stack.Pop();
                                    }

                                    while (popedOperator.Priority <= @operator.Priority)
                                    {
                                        queue.Enqueue(popedOperator);

                                        if (stack.Count == 0)
                                        {
                                            break;
                                        }

                                        popedOperator = stack.Peek();

                                        if (popedOperator.Priority <= @operator.Priority && stack.Count > 0)
                                        {
                                            popedOperator = stack.Pop();
                                        }
                                    }

                                    stack.Push(@operator);

                                    break;
                                }

                                case AssociativityType.Right:
                                {
                                    var popedOperator = stack.Peek();

                                    if (popedOperator.Priority <= @operator.Priority && stack.Count > 0)
                                    {
                                        popedOperator = stack.Pop();
                                        queue.Enqueue(popedOperator);
                                    }

                                    stack.Push(@operator);

                                    break;
                                }
                            }

                            break;

                        case ArityType.Nulary:
                        case ArityType.Ternary:
                        case ArityType.Kvatery:
                        case ArityType.Multiarity:
                            throw new NotSupportedException();

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            while (stack.Count > 0)
            {
                queue.Enqueue(stack.Pop());
            }

            return queue;
        }

        private string CreateRegexToSplitByOperators()
        {
            // f.e.,
            //      input: a>5  &&  b+7  ==2
            //      regex: >|   &+| \+|  =+
            var operatorRegex = string.Join("|", GrammarService.OperatorDenotations.Select(EscapeOperatorDenomination).Distinct().Where(x => !string.IsNullOrWhiteSpace(x)));

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

        private Operator[] GetPossibleOperators(string denotation)
        {
            return GrammarService.Operators.Where(x => x.Denotations.Contains(denotation)).ToArray();
        }
    }
}