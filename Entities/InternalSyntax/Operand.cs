using System;
using Entities.InternalSyntax.Enums;

namespace Entities.InternalSyntax
{
    public class Operand : AbstractToken
    {
        public Operand(string value)
            : this(value,
                   value.StartsWith("[", StringComparison.Ordinal) && value.EndsWith("]", StringComparison.Ordinal)) { }
        
        public Operand(string value, bool isEscaped)
            : base(OperatorType.Literal, int.MaxValue)
        {
            Value = isEscaped ? value : $"[{value}]";
        }
        
        public string Value { get; }
        
        public override string ToString()
        {
            return Value;
        }
    }
}