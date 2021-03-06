﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FerOmega.Entities;
using FerOmega.Entities.AbstractSyntax;
using FerOmega.Entities.InternalSyntax;
using FerOmega.Entities.InternalSyntax.Enums;
using FerOmega.Providers.Abstractions;
using FerOmega.Services.Abstractions;

namespace FerOmega.Providers
{
    internal class SqlProvider<T> : ISqlProvider
        where T : IGrammarConfig
    {
        public SqlProvider(IGrammarService<T> sqlGrammarService)
        {
            this.sqlGrammarService = sqlGrammarService;
        }

        private readonly IGrammarService<T> sqlGrammarService;

        public (SqlFilter sql, object[] parameters) Convert(Tree<AbstractToken> tree,
            params PropertyDef[] properties)
        {
            if (tree.IsEmpty)
            {
                return (new SqlFilter(""), new object[0]);
            }

            var stack = new Stack<string>();

            var parameters = new List<object>();

            tree.DeepFirst(default,
                           node =>
                           {
                               switch (node.Body.OperatorType)
                               {
                               case OperatorType.Literal:
                               {
                                   var internalOperand = (Operand)node.Body;

                                   var unescapedOperand = sqlGrammarService.EnsureUnescaped(internalOperand.Value);

                                   var isSqlColumn = properties.Any(x => x.From == unescapedOperand);

                                   if (isSqlColumn)
                                   {
                                       var sqlProperty = properties.First(x => x.From == unescapedOperand).To;
                                       var columnName = sqlGrammarService.EnsureUnescaped(sqlProperty);

                                       // case: where table.name = 'name'
                                       // resolve: replace any 'name' with parameter
                                       if (stack.Count != 0)
                                       {
                                           var previousValue = stack.Peek();

                                           if (previousValue == columnName)
                                           {
                                               stack.Pop(); // replace in correct (beeline) order
                                               stack.Push($"@{parameters.Count}");
                                               stack.Push(columnName);
                                               parameters.Add(Parse(unescapedOperand));

                                               break;
                                           }
                                       }

                                       stack.Push(columnName);
                                   }
                                   else
                                   {
                                       stack.Push($"@{parameters.Count}");

                                       // support default sql type mapping
                                       parameters.Add(Parse(unescapedOperand));
                                   }

                                   break;
                               }

                               case OperatorType.Contains:
                               {
                                   parameters[^1] = $"%{parameters[^1]}%"; // % should be inside parameter, not in sql query

                                   goto default;
                               }
                                   
                               case OperatorType.StartsWith:
                               {
                                   parameters[^1] = $"{parameters[^1]}%"; // % should be inside parameter, not in sql query

                                   goto default;
                               }
                                   
                               case OperatorType.EndsWith:
                               {
                                   parameters[^1] = $"%{parameters[^1]}"; // % should be inside parameter, not in sql query

                                   goto default;
                               }

                               case OperatorType.Separator:
                               {
                                   var leftOperand = stack.Pop();
                                   var rightOperand = stack.Pop();

                                   var internalOperator = (Operator)node.Body;

                                   var sqlOperator =
                                       sqlGrammarService.Operators.FirstOrDefault(x => x.OperatorType ==
                                           internalOperator.OperatorType);

                                   if (sqlOperator == default)
                                   {
                                       throw new ArgumentOutOfRangeException();
                                   }

                                   var result = $"{leftOperand} {sqlOperator.MainDenotation} {rightOperand}";

                                   stack.Push(result);

                                   break;
                               }

                               case OperatorType.InRange:
                               {
                                   // InRange doesn't allow the right operand to be unwrapped with brackets
                                   // and InRange doesn't allow the 'a in (1)' syntax
                                   // Literal - because if there is any operator here, there are implicit brackets
                                   if (node.Children.Last().Body.OperatorType == OperatorType.Literal)
                                   {
                                       throw new InvalidOperationException($"{nameof(OperatorType.InRange)} requires right operand to be wrapped with {nameof(OperatorType.OpenPriorityBracket)} {nameof(OperatorType.ClosePriorityBracket)} and InRange operator required right operand list have length greater then one");
                                   }

                                   goto default;
                               }

                               // most operators go here
                               default:
                               {
                                   var internalOperator = (Operator)node.Body;

                                   var sqlOperator =
                                       sqlGrammarService.Operators.FirstOrDefault(x => x.OperatorType ==
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
                                       if (internalOperator.Fixity != Fixity.Infix)
                                       {
                                           throw new ArgumentOutOfRangeException();
                                       }

                                       var leftOperand = stack.Pop();
                                       var rightOperand = stack.Pop();

                                       var sb = new StringBuilder();

                                       // restore brackets from ast
                                       // there are only two children here
                                       // first means left
                                       // last means right
                                       if (node.Children.First().Body.Priority >= node.Body.Priority)
                                       {
                                           sb.Append($"{sqlGrammarService.OpenPriorityBracket.MainDenotation} {leftOperand} {sqlGrammarService.ClosePriorityBracket.MainDenotation}");
                                       }
                                       else
                                       {
                                           sb.Append(leftOperand);
                                       }

                                       sb.Append($" {sqlOperator.MainDenotation} ");

                                       if (node.Children.Last().Body.Priority >= node.Body.Priority)
                                       {
                                           sb.Append($"{sqlGrammarService.OpenPriorityBracket.MainDenotation} {rightOperand} {sqlGrammarService.ClosePriorityBracket.MainDenotation}");
                                       }
                                       else
                                       {
                                           sb.Append(rightOperand);
                                       }

                                       result = sb.ToString();

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

            return (new SqlFilter(stack.Pop()), parameters.ToArray());
        }

        public IPropertyBuilderFrom DefineProperty()
        {
            return new PropertyPropertyPropertyBuilder();
        }
        
        private string HandleLike(Operator internalOperator, string leftOperand, string rightOperand, string format)
        {
            var sqlOperator =
                sqlGrammarService.Operators.FirstOrDefault(x => x.OperatorType ==
                                                                internalOperator.OperatorType);

            if (sqlOperator == default)
            {
                throw new ArgumentOutOfRangeException();
            }

            rightOperand = string.Format(format, rightOperand);
            var result = $"{leftOperand} {sqlOperator.MainDenotation} {rightOperand}";

            return result;
        }

        
        private object Parse(string value)
        {
            // todo [snow]: extend this list

            if (int.TryParse(value, out var resInt))
            {
                return resInt;
            }

            if (double.TryParse(value, out var resDouble))
            {
                return resDouble;
            }

            if (Guid.TryParse(value, out var resGuid))
            {
                return resGuid;
            }

            return value;
        }
    }
}
