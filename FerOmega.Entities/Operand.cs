namespace FerOmega.Entities
{
    public class Operand : AbstractToken
    {
        public Operand(string value) : base(OperatorType.Literal, 100)
        {
            Value = value;
        }

        public string Value { get; set; }

        public override string ToString()
        {
            return $"Var: {Value}";
        }
    }
}