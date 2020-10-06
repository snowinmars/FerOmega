using System;
using System.Collections.Generic;
using System.Linq;
using FerOmega.Entities.AbstractSyntax;
using FerOmega.Entities.InternalSyntax;
using FerOmega.Entities.InternalSyntax.Enums;
using FerOmega.Providers.Abstractions;
using FerOmega.Services.Abstractions;
using FerOmega.Services.configs;

namespace FerOmega.Providers
{
    internal class SqlProvider<T> : ISqlProvider
        where T : IGrammarConfig
    {
        public SqlProvider(IGrammarService<T> grammarService)
        {
            this.grammarService = grammarService;
        }

        private readonly IGrammarService<T> grammarService;

        public (string sql, object[] parameters) Convert(Tree<AbstractToken> tree, params string[] allowedProperties)
        {
            if (tree.IsEmpty)
            {
                return ("", new object[0]);
            }
            
            var stack = new Stack<string>();

            var parameters = new List<object>();
            
            tree.DeepFirst(default,
                           (n) =>
                           {
                               switch (n.Body.OperatorType)
                               {
                               case OperatorType.Literal:
                               {
                                   var operand = (Operand)n.Body;

                                   var unescapedOperand = grammarService.EnsureUnescaped(operand);
                                   
                                   var shouldBeEscaped = !allowedProperties.Contains(unescapedOperand.Value);

                                   if (shouldBeEscaped)
                                   {
                                       stack.Push($"@{parameters.Count}");
                                       parameters.Add(unescapedOperand.Value);
                                   }
                                   else
                                   {
                                       var escapedOperand = grammarService.EnsureEscaped(operand);
                                       stack.Push(escapedOperand.Value);
                                   }

                                   break;
                               }

                               default:
                               {
                                   var internalOperator = (Operator)n.Body;

                                   var sqlOperator =
                                       grammarService.Operators.FirstOrDefault(x => x.OperatorType ==
                                                                                   internalOperator.OperatorType);

                                   if (sqlOperator == default)
                                   {
                                       throw new ArgumentOutOfRangeException();
                                   }

                                   string result;

                                   switch (internalOperator.Arity)
                                   {
                                   case Arity.Unary:
                                   {
                                       var operand = stack.Pop();

                                       result = internalOperator.Fixity switch
                                       {
                                           Fixity.Prefix => $"{sqlOperator.MainDenotation} {operand}",
                                           Fixity.Postfix => $"{operand} {sqlOperator.MainDenotation}",
                                           _ => throw new ArgumentOutOfRangeException(),
                                       };

                                       break;
                                   }
                                   case Arity.Binary:
                                   {
                                       var leftOperand = stack.Pop();
                                       var rightOperand = stack.Pop();

                                       result = internalOperator.Fixity switch
                                       {
                                           Fixity.Infix => $"{leftOperand} {sqlOperator.MainDenotation} {rightOperand}",
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

            return (stack.Pop(), parameters.ToArray());
        }
    }
}