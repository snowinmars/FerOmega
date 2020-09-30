using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using FerOmega.Entities;
using FerOmega.Entities.AbstractSyntax;

namespace Providers
{
    public class SqlProvider
    {
        private IDictionary<OperatorType, string> map = new Dictionary<OperatorType, string>
        {
            { OperatorType.Equals, "=" },
            { OperatorType.NotEquals, "!=" },
            { OperatorType.Not, "!" },
            { OperatorType.GreaterThan, ">" },
            { OperatorType.LesserThan, "<" },
            { OperatorType.GreaterOrEqualsThan, ">=" },
            { OperatorType.LesserOrEqualsThan, "<=" },
            // { OperatorType.InRange, "" },
            { OperatorType.And, "and" },
            { OperatorType.Or, "or" },
            // { OperatorType.Xor, "" },
            // { OperatorType.Contains, "" },
            // { OperatorType.StartsWith, "" },
            // { OperatorType.EndsWith, "" },
            // { OperatorType.Empty, "" },
            // { OperatorType.NotEmpty, "" },
            { OperatorType.UnaryPlus, "+" },
            { OperatorType.Plus, "+" },
            { OperatorType.UnaryMinus, "-" },
            { OperatorType.Minus, "-" },
            { OperatorType.Multiple, "*" },
            { OperatorType.Reminder, "%" },
            { OperatorType.Divide, "/" },
            // { OperatorType.Invert, "" },
            // { OperatorType.OpenRoundBracket, "" },
            // { OperatorType.CloseRoundBracket, "" },
            // { OperatorType.OpenCurlyBracket, "" },
            // { OperatorType.CloseCurlyBracket, "" },
            // { OperatorType.OpenSquareBracket, "" },
            // { OperatorType.CloseSquareBracket, "" },
            // { OperatorType.Factorial, "" },
        };
        
        public string Convert(Tree<AbstractToken> tree)
        {
            var stack = new Stack<string>();
            
            tree.DeepFirst(default, (n) =>
            {
                switch (n.Body.OperatorType)
                {
                case OperatorType.Literal:
                {
                    var operand = (Operand)n.Body;
                    stack.Push(operand.Value);

                    break;
                }

                default:
                {
                    var @operator = (Operator)n.Body;

                    if (!map.TryGetValue(@operator.OperatorType, out var value))
                    {
                        throw new ArgumentOutOfRangeException();
                    }


                    string result;

                    switch (@operator.Arity)
                    {
                    case ArityType.Unary:
                    {
                        var operand = stack.Pop();

                        result = @operator.Fixity switch
                        {
                            FixityType.Prefix => $"{value}{operand}",
                            FixityType.Postfix => $"{operand}{value}",
                            _ => throw new ArgumentOutOfRangeException(),
                        };

                        break;
                    }
                    case ArityType.Binary:
                    {
                        var leftOperand = stack.Pop();
                        var rightOperand = stack.Pop();

                        result = @operator.Fixity switch
                        {
                            FixityType.Infix => $"{leftOperand}{value}{rightOperand}",
                            _ => throw new ArgumentOutOfRangeException(),
                        };

                        break;
                    }
                    default: throw new ArgumentOutOfRangeException();
                    }

                    stack.Push(result);

                    break;
                }
                }
            });

            if (stack.Count != 1)
            {
                throw new InvalidOperationException();
            }

            return stack.Pop();
        }
    }

    internal static class Ext
    {
        public static StringBuilder AppendSql(this StringBuilder sb, string sql)
        {
            return sb.Append($" {sql} ");
        }
    }
}