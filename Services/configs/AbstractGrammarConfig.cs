using System.Collections.Generic;
using FerOmega.Entities.InternalSyntax;

namespace FerOmega.Services.configs
{
    public abstract class AbstractGrammarConfig
    {
        protected AbstractGrammarConfig()
        {
            Operators = new List<Operator>();
            RegexEscapedDenotations = new Dictionary<Operator, string>();
        }

        protected readonly IList<Operator> Operators;

        protected readonly IDictionary<Operator, string> RegexEscapedDenotations;

        protected int AddOperatorGroup(int priority, params Operator[] operators)
        {
            foreach (var @operator in operators)
            {
                @operator.Priority = priority;
                Operators.Add(@operator);
            }

            return ++priority;
        }
    }
}