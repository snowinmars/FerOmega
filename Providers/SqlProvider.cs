using System;
using System.Collections.Generic;
using System.Linq;
using FerOmega.Entities;
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

        public (string sql, object[] parameters) Convert(Tree<AbstractToken> tree, 
            params PropertyDef[] properties)
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

                                   var unescapedOperand = grammarService.EnsureUnescaped(operand.Value);

                                   var isSqlColumn = properties.Any(x => x.From == unescapedOperand);

                                   if (isSqlColumn)
                                   {
                                       var sqlProperty = properties.First(x => x.From == unescapedOperand).To;
                                       var columnName = grammarService.EnsureUnescaped(sqlProperty);
                                       stack.Push(columnName);
                                   }
                                   else
                                   {
                                       stack.Push($"@{parameters.Count}");
                                       parameters.Add(unescapedOperand);
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