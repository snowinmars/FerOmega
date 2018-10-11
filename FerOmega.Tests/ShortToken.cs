using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FerOmega.Abstractions;
using FerOmega.Common;
using FerOmega.Entities;
using FerOmega.Entities.AbstractSyntax;
using FerOmega.Services;

using Newtonsoft.Json;

namespace FerOmega.Tests
{
    internal class ShortToken : IEquatable<ShortToken>
    {
        // TODO: [DT] REMOVE
        private readonly IGrammarService grammarService;

        public IList<ShortToken> Children { get; set; }

        public bool IsEscaped => Value != null && (Value.StartsWith("[", StringComparison.Ordinal) && Value.EndsWith("]", StringComparison.Ordinal));

        public OperatorType OperatorType { get; set; }

        public string Value { get; set; }

        [JsonConstructor]
        public ShortToken(OperatorType operatorType) : this(operatorType, null)
        {
        }

        public ShortToken(int value) : this(OperatorType.Literal, value.ToString())
        {
        }

        public ShortToken(OperatorType operatorType, string value)
        {
            OperatorType = operatorType;
            Value = value;

            Children = new List<ShortToken>(4);
            grammarService = new GrammarService();
        }

        public static ShortToken FromBody(AbstractToken token)
        {
            var shortToken = new ShortToken(token.OperatorType);

            if (token.OperatorType == OperatorType.Literal)
            {
                var operand = (Operand)token;
                shortToken.Value = operand.Value;
            }

            return shortToken;
        }

        public static ShortToken FromNode(Node<AbstractToken> token)
        {
            var body = FromBody(token.Body);

            foreach (var child in token.Children)
            {
                var childToken = FromNode(child);
                body.Children.Add(childToken);
            }

            return body;
        }

        public static ShortToken FromOperand(Operand operand)
        {
            return new ShortToken(OperatorType.Literal, operand.Value);
        }

        public static ShortToken FromTree(Tree<AbstractToken> tree)
        {
            var root = FromNode(tree.Root);

            return root;
        }

        public AbstractToken ToAbstractToken()
        {
            AbstractToken token;

            if (OperatorType == OperatorType.Literal)
            {
                token = new Operand(Value);
            }
            else
            {
                token = grammarService.Get(OperatorType);
            }

            return token;
        }

        public void DeEscape()
        {
            if (IsEscaped)
            {
                Value = Value.Substring(1, Value.Length - 2);
            }
        }

        #region equals
        public static bool operator !=(ShortToken left, ShortToken right)
        {
            return !Equals(left, right);
        }

        public static bool operator ==(ShortToken left, ShortToken right)
        {
            return Equals(left, right);
        }

        public override bool Equals(object obj)
        {
            return obj is ShortToken shortToken && Equals(shortToken);
        }

        public bool Equals(ShortToken other)
        {
            if (this == null && other == null)
            {
                return true;
            }

            if (this == null || other == null)
            {
                return false;
            }

            var wasThisEscaped = IsEscaped;
            var wasOtherEscaped = other.IsEscaped;

            DeEscape();
            other.DeEscape();

            if (!Children.OrderBy(x => x.OperatorType).SequenceEqual(other.Children.OrderBy(x => x.OperatorType)))
            {
                return false;
            }

            bool areEquals = OperatorType == other.OperatorType
                             && Value == other.Value;

            if (wasThisEscaped)
            {
                Escape();
            }

            if (wasOtherEscaped)
            {
                other.Escape();
            }

            return areEquals;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 0;
                hashCode = (hashCode * 397) ^ (int)OperatorType;
                hashCode = (hashCode * 397) ^ (Value != null ? Value.GetHashCode() : 0);
                return hashCode;
            }
        }
        #endregion equals

        public void Escape()
        {
            if (!IsEscaped)
            {
                Value = $"[{Value}]";
            }
        }

        public void ToggleEscape()
        {
            if (IsEscaped)
            {
                DeEscape();
            }
            else
            {
                Escape();
            }
        }

        public string ToPlainEquation()
        {
            var sb = new StringBuilder();

            sb.Append(ToPlainEquation(false));

            return sb.ToString();
        }

        public override string ToString()
        {
            if (OperatorType == OperatorType.Literal)
            {
                return $"Val: {Value}";
            }

            return $"{OperatorType} -> ({Children.Count})";
        }

        public Tree<AbstractToken> ToTree()
        {
            var thisToken = ToAbstractToken();
            var tree = new Tree<AbstractToken>(thisToken);

            foreach (var child in Children)
            {
                var childNode = child.ToTree();
                tree.AppendToRoot(childNode);
            }

            return tree;
        }

        private string ToPlainEquation(bool wrapWithBrackets)
        {
            if (OperatorType == OperatorType.Literal)
            {
                if (Value.StartsWith("-", StringComparison.Ordinal))
                {
                    return $"({Value})";
                }

                return Value;
            }

            var sb = new StringBuilder();
            var shouldEscape = Constants.Random.NextBool();

            if (wrapWithBrackets || shouldEscape)
            {
                sb.Append(" ( ");
            }

            var @operator = (Operator)ToAbstractToken();
            sb.Append(ToPlainEquation(@operator));

            if (wrapWithBrackets || shouldEscape)
            {
                sb.Append(" ) ");
            }

            return sb.ToString();
        }

        private string ToPlainEquation(Operator @operator)
        {
            switch (@operator.Arity)
            {
                case ArityType.Unary when @operator.Fixity == FixityType.Prefix:
                {
                    return $"{@operator.MainDenotation} {Children[0].ToPlainEquation(true)}";
                }

                case ArityType.Unary when @operator.Fixity == FixityType.Postfix:
                {
                    return $"{Children[0].ToPlainEquation(true)} {@operator.MainDenotation}";
                }

                case ArityType.Binary when @operator.Fixity == FixityType.Infix:
                {
                    return $"{Children[0].ToPlainEquation(true)} {@operator.MainDenotation} {Children[1].ToPlainEquation(true)}";
                }

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
}