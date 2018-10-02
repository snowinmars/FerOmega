namespace FerOmega.Entities
{
    public abstract class AbstractToken
    {
        public OperatorType OperatorType { get; set; }

        public int Priority { get; set; }

        protected AbstractToken(OperatorType operatorType, int priority)
        {
            OperatorType = operatorType;
            Priority = priority;
        }

        public override string ToString()
        {
            return $"{OperatorType}";
        }
    }
}