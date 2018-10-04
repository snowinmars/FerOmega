using System.Collections.Generic;

using FerOmega.Abstractions;
using FerOmega.Entities;
using FerOmega.Entities.RedBlack;
using FerOmega.Services;

namespace FerOmega.Tests
{
    internal class ShortToken
    {
        internal IGrammarService grammarService;

        public ShortToken()
        {
            grammarService = new GrammarService();
        }

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
        }

        public OperatorType OperatorType { get; set; }

        public string Value { get; set; }

        public IList<ShortToken> Children { get; set; }

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

        public static ShortToken FromTree(Tree<AbstractToken> tree)
        {
            var root = FromNode(tree.Root);

            return root;
        }

        public Tree<AbstractToken> ToTree()
        {
            var thisToken = ConvertSelf();
            var tree = new Tree<AbstractToken>(thisToken);

            foreach (var child in Children)
            {
                var childNode = child.ConvertSelf();
                tree.AppendToRoot(childNode);
            }

            return tree;
        }

        public AbstractToken ConvertSelf()
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

        public override string ToString()
        {
            if (OperatorType == OperatorType.Literal)
            {
                return $"Val: {Value}";
            }

            return $"{OperatorType} -> ({Children.Count})";
        }
    }
}