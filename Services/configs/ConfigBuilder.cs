using System.Collections.Generic;
using System.Linq;
using FerOmega.Entities.InternalSyntax;

namespace FerOmega.Services.configs
{
    internal class ConfigBuilder
    {
        private int priority;

        private readonly IList<Operator> operators;
        
        private ConfigBuilder()
        {
            priority = 1;
            operators = new List<Operator>();
        }

        public static ConfigBuilder Start()
        {
            return new ConfigBuilder();
        }

        public ConfigBuilder AddOperatorGroup(params Operator[] operators)
        {
            foreach (var @operator in operators)
            {
                @operator.Priority = priority;
                this.operators.Add(@operator);
            }

            priority++;

            return this;
        }

        public Operator[] Build()
        {
            return operators.ToArray();
        }
    }
}