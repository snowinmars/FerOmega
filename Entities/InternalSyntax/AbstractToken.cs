using FerOmega.Entities.InternalSyntax.Enums;

namespace FerOmega.Entities.InternalSyntax
{
    public abstract class AbstractToken
    {
        protected AbstractToken(OperatorType operatorType, int priority)
        {
            OperatorType = operatorType;
            Priority = priority;
        }

        public OperatorType OperatorType { get; }

        public int Priority { get; set; }

        public override string ToString()
        {
            return OperatorType.ToString();
        }
    }
}
