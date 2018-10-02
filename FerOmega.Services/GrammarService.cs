using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using FerOmega.Entities;

namespace FerOmega.Services
{
    public class GrammarService
    {
        private class Token
        {
            public string Current { get; set; }

            public string Next { get; set; }

            public string Previous { get; set; }

            public Token(string[] tokens, int i)
            {
                Previous = i == 0 ? NonExistingOperator : tokens[i - 1].Trim();
                Current = tokens[i].Trim();
                Next = i == tokens.Length - 1 ? NonExistingOperator : tokens[i + 1].Trim();
            }
        }

        private const string NonExistingOperator = "";

        private string[] BracketsDenotations { get; }

        private Operator CloseEscapeOperator { get; }

        private Operator OpenEscapeOperator { get; }

        private string[] OperatorDenotations { get; }

        private string OperatorRegex { get; }

        private IList<Operator> Operators { get; }

        public GrammarService()
        {
            Operators = new List<Operator>();
            SetOperators();

            OpenEscapeOperator = GetOperator(OperatorType.OpenSquareBracket);
            CloseEscapeOperator = GetOperator(OperatorType.CloseSquareBracket);

            OperatorDenotations = Operators.SelectMany(x => x.Denotations).ToArray();
            BracketsDenotations = Operators.Where(x => x.IsBracket()).SelectMany(x => x.Denotations).ToArray();

            OperatorRegex = CreateRegexToSplitByOperators();

            CheckOperators();
        }

        public Queue<AbstractToken> Parse(string equation)
        {
            var tokens = Regex.Split(equation, OperatorRegex, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

            var stack = new Stack<AbstractToken>(tokens.Length);
            var queue = new Queue<AbstractToken>(tokens.Length);

            for (int i = 0; i < tokens.Length; i++)
            {
                var token = new Token(tokens, i);

                if (IsOperand(token.Current))
                {
                    var operand = new Operand(token.Current);

                    queue.Enqueue(operand);
                    continue;
                }

                if (IsOperator(token.Current))
                {
                    var possibleOperators = GetPossibleOperators(token.Current);

                    // length != 1 means we have overloads
                    var @operator = possibleOperators.Length == 1 ? possibleOperators[0] : Resolve(token, possibleOperators);

                    if (@operator.IsOpenBracket())
                    {
                        stack.Push(@operator);
                        continue;
                    }

                    if (@operator.IsCloseBracket())
                    {
                        while (stack.Count >= 0)
                        {
                            if (stack.Count == 0)
                            {
                                throw new InvalidOperationException();
                            }

                            var abstractToken = stack.Pop();

                            if (abstractToken.IsOpenBracket())
                            {
                                break;
                            }

                            queue.Enqueue(abstractToken);
                        }

                        continue;
                    }

                    // if we're here, the operator is not any type of bracket, so it's evaluatable operator

                    if (@operator.Arity == ArityType.Unary && @operator.Fixity == FixityType.Postfix)
                    {
                        queue.Enqueue(@operator);
                        continue;
                    }

                    if (@operator.Arity == ArityType.Unary && @operator.Fixity == FixityType.Prefix)
                    {
                        stack.Push(@operator);
                        continue;
                    }

                    if (@operator.Arity == ArityType.Binary)
                    {
                        if (stack.Count > 0)
                        {
                            if (@operator.Associativity == AssociativityType.Left)
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

                                continue;
                            }

                            if (@operator.Associativity == AssociativityType.Right)
                            {
                                var popedOperator = stack.Peek();

                                if (popedOperator.Priority <= @operator.Priority && stack.Count > 0)
                                {
                                    popedOperator = stack.Pop();
                                    queue.Enqueue(popedOperator);
                                }

                                stack.Push(@operator);

                                continue;
                            }
                        }

                        stack.Push(@operator);
                    }
                }
            }

            while (stack.Count > 0)
            {
                queue.Enqueue(stack.Pop());
            }

            return queue;
        }

        private int AddOperators(int priority, params Operator[] operators)
        {
            foreach (var @operator in operators)
            {
                @operator.Priority = priority;
                Operators.Add(@operator);
            }

            return ++priority;
        }

        private void CheckOperators()
        {
            var isOperatorTypePrimaryKey = Operators.Select(x => x.OperatorType).Distinct().Count() == Operators.Count;

            if (!isOperatorTypePrimaryKey)
            {
                throw new InvalidOperationException();
            }

            var isNonExistingOperatorExists = OperatorDenotations.Any(x => x == NonExistingOperator);

            if (isNonExistingOperatorExists)
            {
                throw new InvalidOperationException();
            }
        }

        private string CreateRegexToSplitByOperators()
        {
            // f.e.,
            //      input: a>5  &&  b+7  ==2
            //      regex: >|   &+| \+|  =+
            var operatorRegex = string.Join("|", OperatorDenotations.Select(EscapeOperatorDenomination).Distinct().Where(x => !string.IsNullOrWhiteSpace(x)));

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

        private Operator GetOperator(OperatorType type)
        {
            if (!Enum.IsDefined(typeof(OperatorType), type))
            {
                throw new ArgumentOutOfRangeException(nameof(type), type, $"Enum {nameof(OperatorType)} is out of range");
            }

            return Operators.First(x => x.OperatorType == type);
        }

        private Operator[] GetPossibleOperators(string denotation)
        {
            return Operators.Where(x => x.Denotations.Contains(denotation)).ToArray();
        }

        private bool IsInfixBinary(string previousToken, string nextToken, Operator[] possibleOperators)
        {
            // like 5 + 2
            //        ^
            var isInfixBinaryCase1 = IsOperand(previousToken) && IsOperand(nextToken) && possibleOperators.Any(x => x.Arity == ArityType.Binary && x.Fixity == FixityType.Infix);

            // like 5 + (2 + 3)
            //        ^
            var isInfixBinaryCase2 = IsOperand(previousToken) && BracketsDenotations.Contains(nextToken) && possibleOperators.Any(x => x.Arity == ArityType.Binary && x.Fixity == FixityType.Infix);

            // like (2 + 3) + 5
            //              ^
            var isInfixBinaryCase3 = BracketsDenotations.Contains(previousToken) && IsOperand(nextToken) && possibleOperators.Any(x => x.Arity == ArityType.Binary && x.Fixity == FixityType.Infix);

            return isInfixBinaryCase1 || isInfixBinaryCase2 || isInfixBinaryCase3;
        }

        private bool IsOperand(string input)
        {
            return input.StartsWith(OpenEscapeOperator.MainDenotation, StringComparison.Ordinal)
                   && input.EndsWith(CloseEscapeOperator.MainDenotation, StringComparison.Ordinal);
        }

        private bool IsOperator(string input)
        {
            return !IsOperand(input) && OperatorDenotations.Contains(input);
        }

        private bool IsUnaryPostfix(string previousToken, string nextToken, Operator[] possibleOperators)
        {
            // like 1 + 3!
            //           ^
            var isPostfixCase1 = IsOperand(previousToken)
                                 && nextToken == NonExistingOperator
                                 && possibleOperators.Any(x => x.Arity == ArityType.Unary && x.Fixity == FixityType.Postfix);

            // like (1 + 3!)
            //            ^
            var isPostfixCase2 = IsOperand(previousToken)
                                 && IsOperator(nextToken)
                                 && possibleOperators.Any(x => x.Arity == ArityType.Unary && x.Fixity == FixityType.Postfix);

            // like 3! + 1
            //       ^
            var isPostfixCase3 = IsOperand(previousToken)
                                 && IsOperator(nextToken)
                                 && possibleOperators.Any(x => x.Arity == ArityType.Unary && x.Fixity == FixityType.Postfix);

            return isPostfixCase1 || isPostfixCase2 || isPostfixCase3;
        }

        private bool IsUnaryPrefix(string previousToken, string nextToken, Operator[] possibleOperators)
        {
            // like -1 - 2
            //      ^
            var isPrefixCase1 = IsOperand(nextToken)
                                && previousToken == NonExistingOperator
                                && possibleOperators.Any(x => x.Arity == ArityType.Unary && x.Fixity == FixityType.Prefix);

            // like !a && !b
            //            ^
            var isPrefixCase2 = IsOperand(nextToken)
                                && IsOperator(previousToken)
                                && possibleOperators.Any(x => x.Arity == ArityType.Unary && x.Fixity == FixityType.Prefix);

            // like -(1 - 2)
            //      ^
            var isPrefixCase3 = IsOperator(nextToken)
                                && previousToken == NonExistingOperator
                                && possibleOperators.Any(x => x.Arity == ArityType.Unary && x.Fixity == FixityType.Prefix);

            return isPrefixCase1 || isPrefixCase2 || isPrefixCase3;
        }

        private Operator Resolve(Token token, Operator[] possibleOperators)
        {
            var isUnaryPrefix = IsUnaryPrefix(token.Previous, token.Next, possibleOperators);

            if (isUnaryPrefix)
            {
                var acceptedOperators = possibleOperators.Where(x => x.Fixity == FixityType.Prefix && x.Arity == ArityType.Unary).ToArray();

                if (acceptedOperators.Length != 1)
                {
                    throw new InvalidOperationException("Can't resolve operator overloaded by non-fixity and non-arity");
                }

                return acceptedOperators[0];
            }

            var isUnaryPostfix = IsUnaryPostfix(token.Previous, token.Next, possibleOperators);

            if (isUnaryPostfix)
            {
                var acceptedOperators = possibleOperators.Where(x => x.Fixity == FixityType.Postfix && x.Arity == ArityType.Unary).ToArray();

                if (acceptedOperators.Length != 1)
                {
                    throw new InvalidOperationException("Can't resolve operator overloaded by non-fixity and non-arity");
                }

                return acceptedOperators[0];
            }

            var isInfixBinary = IsInfixBinary(token.Previous, token.Next, possibleOperators);

            if (isInfixBinary)
            {
                var acceptedOperators = possibleOperators.Where(x => x.Fixity == FixityType.Infix && x.Arity == ArityType.Binary).ToArray();

                if (acceptedOperators.Length != 1)
                {
                    throw new InvalidOperationException("Can't resolve operator overloaded by non-fixity and non-arity");
                }

                return acceptedOperators[0];
            }

            throw new InvalidOperationException("Can't resolve fixity");
        }

        private void SetOperators()
        {
            // there are less priority operators at the bottom and more priority operators at the top
            var priority = 1;

            priority = AddOperators(priority,
                new Operator(ArityType.Unary, AssociativityType.Right, OperatorType.Not, FixityType.Prefix, "!", "not"),
                new Operator(ArityType.Unary, AssociativityType.Right, OperatorType.UnaryPlus, FixityType.Prefix, "+"),
                new Operator(ArityType.Unary, AssociativityType.Right, OperatorType.UnaryMinus, FixityType.Prefix, "-"),
                new Operator(ArityType.Unary, AssociativityType.Right, OperatorType.Invert, FixityType.Prefix, "~"));

            priority = AddOperators(priority,
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.Multiple, FixityType.Infix, "*"),
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.Divide, FixityType.Infix, "/"),
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.Reminder, FixityType.Infix, "%"));

            priority = AddOperators(priority,
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.Plus, FixityType.Infix, "+"),
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.Minus, FixityType.Infix, "-"));

            priority = AddOperators(priority,
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.GreaterThan, FixityType.Infix, ">", "gt"),
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.LesserThan, FixityType.Infix, "<", "lt"),
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.GreaterOrEqualsThan, FixityType.Infix, ">=", "gte"),
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.LesserOrEqualsThan, FixityType.Infix, "<=", "lte"));

            priority = AddOperators(priority,
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.Equals, FixityType.Infix, "==", "eq"),
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.NotEquals, FixityType.Infix, "!=", "<>", "neq"));

            priority = AddOperators(priority,
                new Operator(ArityType.Multiarity, AssociativityType.Right, OperatorType.InRange, FixityType.Infix, "in"),
                new Operator(ArityType.Binary, AssociativityType.Right, OperatorType.Contains, FixityType.Infix, "con"),
                new Operator(ArityType.Binary, AssociativityType.Right, OperatorType.StartsWith, FixityType.Infix, "stw"),
                new Operator(ArityType.Binary, AssociativityType.Right, OperatorType.EndsWith, FixityType.Infix, "edw"),
                new Operator(ArityType.Unary, AssociativityType.Right, OperatorType.Empty, FixityType.Prefix, "emp"),
                new Operator(ArityType.Unary, AssociativityType.Right, OperatorType.NotEmpty, FixityType.Prefix, "nep"));

            priority = AddOperators(priority,
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.And, FixityType.Infix, "&", "&&", "and"));

            priority = AddOperators(priority,
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.Xor, FixityType.Infix, "^"));

            priority = AddOperators(priority,
                new Operator(ArityType.Binary, AssociativityType.Left, OperatorType.Or, FixityType.Infix, "|", "||", "or"));

            priority = AddOperators(priority,
                new Operator(ArityType.Multiarity, AssociativityType.Left, OperatorType.OpenRoundBracket, FixityType.Circumflex, "("),
                new Operator(ArityType.Multiarity, AssociativityType.Left, OperatorType.CloseRoundBracket, FixityType.Circumflex, ")"));


            priority = AddOperators(priority,
                new Operator(ArityType.Multiarity, AssociativityType.Left, OperatorType.OpenCurlyBracket, FixityType.Circumflex, "{"),
                new Operator(ArityType.Multiarity, AssociativityType.Left, OperatorType.CloseCurlyBracket, FixityType.Circumflex, "}"));

            priority = AddOperators(priority,
                new Operator(ArityType.Unary, AssociativityType.Left, OperatorType.OpenSquareBracket, FixityType.Circumflex, "["),
                new Operator(ArityType.Unary, AssociativityType.Left, OperatorType.CloseSquareBracket, FixityType.Circumflex, "]"));
        }
    }
}