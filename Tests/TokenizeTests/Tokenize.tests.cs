using System;
using System.Linq;
using FerOmega.Entities;
using FerOmega.Services;
using NUnit.Framework;

namespace FerOmega.Tests.TokenizeTests
{
    internal class Tokenizetests
    {
        private TokenizationService tokenizationService;

        public Tokenizetests()
        {
            tokenizationService = new TokenizationService();
        }

        private static string[] GetUnaryPrefixOperators() => GetOperators(ArityType.Unary, FixityType.Prefix);
        private static string[] GetUnaryPostfixOperators() => GetOperators(ArityType.Unary, FixityType.Prefix);
        private static string[] GetBinaryOperators() => GetOperators(ArityType.Binary);
        
        private static string[] GetOperators(ArityType arityType)
        {
            var grammarService = new GrammarService();

            return grammarService.Operators
                                 .Where(x => x.Arity == arityType)
                                 .Select(x => x.MainDenotation)
                                 .ToArray();
        }
        
        private static string[] GetOperators(ArityType arityType, FixityType fixityType)
        {
            var grammarService = new GrammarService();

            return grammarService.Operators
                                 .Where(x => x.Arity == arityType && x.Fixity == fixityType)
                                 .Select(x => x.MainDenotation)
                                 .ToArray();
        }
        
        [Test, Combinatorial]
        public void UnaryPrefix(
            [Values("1", "g", "  g  ", "many", "  many     ")]string lhs, 
            [ValueSource(nameof(GetUnaryPrefixOperators))]string op)
        {
            var eq = $"{op}{lhs}";

            var exp = new[]
            {
                op.Trim(), lhs.Trim(),
            };

            Tokenize(eq, exp);
        }
        
        [Test, Combinatorial]
        public void UnaryPostfix(
            [Values("1", "g", "  g  ", "many", "  many     ")]string lhs, 
            [ValueSource(nameof(GetUnaryPostfixOperators))]string op)
        {
            var eq = $"{lhs}{op}";

            var exp = new[]
            {
                lhs.Trim(), op.Trim(),
            };

            Tokenize(eq, exp);
        }
        
        [Test, Combinatorial]
        public void Binary(
            [Values("1", "g", "  g  ", "many", "  many     ")]string lhs, 
            [ValueSource(nameof(GetBinaryOperators))]string op,
            [Values("1", "g", "  g  ", "many", "  many     ")]string rhs)
        {
            var eq = $"{lhs}{op}{rhs}";

            var exp = new[]
            {
                lhs.Trim(), op.Trim(), rhs.Trim(),
            };

            Tokenize(eq, exp);
        }
        
        public void Tokenize(string equation, string[] expectedTokens)
        {
            var tokens = tokenizationService.Tokenizate(equation);

            Assert.AreEqual(expectedTokens.Length, tokens.Length);

            for (var i = 0; i < tokens.Length; i++)
            {
                var token = tokens[i];
                var expectedToken = expectedTokens[i];

                Assert.AreEqual(expectedToken, token);
            }
        }
    }
}