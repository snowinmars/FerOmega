using System;
using FerOmega.Entities.InternalSyntax.Enums;

namespace FerOmega.Entities.InternalSyntax
{
    public class Operand : AbstractToken
    {
        public Operand(string value)
            : this(value,
                   !value.StartsWith("[", StringComparison.Ordinal) &&
                   !value.EndsWith("]", StringComparison.Ordinal)) { } // do I need escaping here?

        public Operand(string value, bool shouldEscape)
            : base(OperatorType.Literal, 0)
        {
            Value = shouldEscape ? $"[{value}]" : value;
        }

        public string Value { get; }

        public override string ToString()
        {
            return Value;
        }
    }
}
