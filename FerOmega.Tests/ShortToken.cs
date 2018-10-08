﻿using System;
using System.Collections.Generic;
using System.Linq;

using FerOmega.Abstractions;
using FerOmega.Entities;
using FerOmega.Entities.RedBlack;
using FerOmega.Services;

using Newtonsoft.Json;

namespace FerOmega.Tests
{
    internal class ShortToken : IEquatable<ShortToken>
    {
        internal IGrammarService GrammarService;

        public IList<ShortToken> Children { get; set; }

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
            GrammarService = new GrammarService();
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

        public static ShortToken FromTree(Tree<AbstractToken> tree)
        {
            var root = FromNode(tree.Root);

            return root;
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
                token = GrammarService.Get(OperatorType);
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

        public Tree<AbstractToken> ToTree()
        {
            var thisToken = ConvertSelf();
            var tree = new Tree<AbstractToken>(thisToken);

            foreach (var child in Children)
            {
                var childNode = child.ToTree();
                tree.AppendToRoot(childNode);
            }

            return tree;
        }

        public override bool Equals(object obj)
        {
            return obj is ShortToken shortToken && this.Equals(shortToken);
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

        public static bool operator ==(ShortToken left, ShortToken right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ShortToken left, ShortToken right)
        {
            return !Equals(left, right);
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

            if (Children.Any() 
                && Children.Any(child => !other.Children.Any(otherChild => otherChild.Equals(child))))
            {
                return false;
            }

            return this.OperatorType == other.OperatorType
                   && this.Value == other.Value;
        }
    }
}