using System.Collections.Generic;
using Entities.InternalSyntax;

namespace Services
{
    public abstract class AbstractGrammarConfig
    {
        protected int AddOperatorGroup(int priority, params Operator[] operators)
        {
            foreach (var @operator in operators)
            {
                @operator.Priority = priority;
                this.Operators.Add(@operator);
            }

            return ++priority;
        }

        protected readonly IList<Operator> Operators;
        
        protected readonly IDictionary<Operator, string> RegexEscapedDenotations;

        protected AbstractGrammarConfig()
        {
            Operators = new List<Operator>();
            RegexEscapedDenotations = new Dictionary<Operator, string>();
        }
    }
}