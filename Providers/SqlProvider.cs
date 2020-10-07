using System;
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

                                       // support default sql type mapping
                                       // todo [snow]: extend this list
                                       if (int.TryParse(unescapedOperand, out var resInt))
                                       {
                                           parameters.Add(resInt);
                                       }
                                       else if (double.TryParse(unescapedOperand, out var resDouble))
                                       {
                                           parameters.Add(resDouble);
                                       }
                                       else if (Guid.TryParse(unescapedOperand, out var resGuid))
                                       {
                                           parameters.Add(resGuid);
                                       }
                                       else
                                       {
                                           parameters.Add(unescapedOperand);
                                       }
                                   }

                                   break;
                               }

                               case OperatorType.Contains:
                               {
                                   var leftOperand = stack.Pop();
                                   var rightOperand = stack.Pop();

                                   var result = HandleLike(n, leftOperand, rightOperand, "'%{0}%'"); // this is the only difference

                                   stack.Push(result);

                                   break;
                               }

                               case OperatorType.StartsWith:
                               {
                                   var leftOperand = stack.Pop();
                                   var rightOperand = stack.Pop();

                                   var result = HandleLike(n, leftOperand, rightOperand, "'{0}%'"); // this is the only difference

                                   stack.Push(result);

                                   break;
                               }

                               case OperatorType.EndsWith:
                               {
                                   var leftOperand = stack.Pop();
                                   var rightOperand = stack.Pop();

                                   var result = HandleLike(n, leftOperand, rightOperand, "'%{0}'"); // this is the only difference

                                   stack.Push(result);

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
                                       if (internalOperator.Fixity == Fixity.Infix)
                                       {
                                           var leftOperand = stack.Pop();
                                           var rightOperand = stack.Pop();

                                           var sb = new StringBuilder();

                                           // restore brackets from ast
                                           // there are only two children here
                                           // first means left
                                           // last means right
                                           if (n.Children.First().Body.Priority > n.Body.Priority)
                                           {
                                               sb.Append($"{grammarService.OpenPriorityBracket.MainDenotation} {leftOperand} {grammarService.ClosePriorityBracket.MainDenotation}");
                                           }
                                           else
                                           {
                                               sb.Append(leftOperand);
                                           }

                                           sb.Append($" {sqlOperator.MainDenotation} ");

                                           if (n.Children.Last().Body.Priority > n.Body.Priority)
                                           {
                                               sb.Append($"{grammarService.OpenPriorityBracket.MainDenotation} {rightOperand} {grammarService.ClosePriorityBracket.MainDenotation}");
                                           }
                                           else
                                           {
                                               sb.Append(rightOperand);
                                           }

                                           result = sb.ToString();
                                       }
                                       else
                                       {
                                           throw new ArgumentOutOfRangeException();
                                       }

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

        public PropertyDef.IBuilderFrom DefineProperty()
        {
            return new PropertyDef.BuilderFrom();
        }

        private string HandleLike(Node<AbstractToken> n, string leftOperand, string rightOperand, string format)
        {
            var internalOperator = (Operator)n.Body;

            var sqlOperator =
                grammarService.Operators.FirstOrDefault(x => x.OperatorType ==
                                                             internalOperator.OperatorType);

            if (sqlOperator == default)
            {
                throw new ArgumentOutOfRangeException();
            }


            rightOperand = string.Format(format, rightOperand);
            var result = $"{leftOperand} {sqlOperator.MainDenotation} {rightOperand}";

            return result;
        }
    }
}
