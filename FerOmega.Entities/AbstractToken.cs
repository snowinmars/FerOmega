using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerOmega.Entities
{
    public abstract class AbstractToken
    {
        public OperatorType OperatorType { get; set; }

        public int Priority { get; set; }

        protected AbstractToken(OperatorType operatorType, int priority)
        {
            this.OperatorType = operatorType;
            this.Priority = priority;
        }

        public override string ToString()
        {
            return $"{OperatorType}";
        }
    }
}
