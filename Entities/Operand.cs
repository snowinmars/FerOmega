using System;

namespace Entities
{
    public class Operand : AbstractToken
    {
        public Operand(int value)
            : this(value.ToString(), false) { }

        // TODO: [DT] 
        public Operand(string value)
            : this(value,
                   value.StartsWith("[", StringComparison.Ordinal) && value.EndsWith("]", StringComparison.Ordinal)) { }

        public Operand(string value, bool isEscaped)
            : base(OperatorType.Literal, 100)
        {
            Value = isEscaped ? value : $"[{value}]";
        }

        public string Value { get; set; }

        public override string ToString()
        {
            return $"{Value}";
        }
    }
}