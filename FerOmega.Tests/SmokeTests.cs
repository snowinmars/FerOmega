using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FerOmega.Entities;
using FerOmega.Services;

using NUnit.Framework;

namespace FerOmega.Tests
{
    public class SmokeTests
    {
        private readonly GrammarService grammarService;

        public SmokeTests()
        {
            this.grammarService = new GrammarService();
        }

        [Test]
        public void Foo()
        {
            var infixes = new[]
            {
                "[7] - [3] + [6]",
                "[6] / [2] * [8] / [3]",
                "[2] + [2] * [2]",
                "[17] - [5] * [6] / [3] - [2] + [4] / [2]",
                "[5] + ( [7] - [2] * [3] ) * ( [6] - [4] ) / [2]",
                "[4] + ( [3] + [1] + [4] * ( [2] + [3] ) )",
            };

            foreach (var infix in infixes)
            {
                var result = this.grammarService.Parse(infix);

                var sb = new StringBuilder();

                foreach (var token in result)
                {
                    if (token.OperatorType == Entities.OperatorType.Variable)
                    {
                        var op = (Operand)token;
                        sb.Append($" {op.Value} ");
                    }
                    else
                    {
                        var op = (Operator)token;
                        sb.Append($" {op.MainDenotation} ");
                    }
                }
            }
        }
    }
}
