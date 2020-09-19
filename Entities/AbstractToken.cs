namespace Entities
{
    public abstract class AbstractToken
    {
        protected AbstractToken(OperatorType operatorType, int priority)
        {
            OperatorType = operatorType;
            Priority = priority;
        }

        public OperatorType OperatorType { get; set; }

        public int Priority { get; set; }

        public override string ToString()
        {
            return $"{OperatorType}";
        }
    }
}