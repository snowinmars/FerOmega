using System;

namespace FerOmega.Entities
{
    [Flags]
    public enum GrammarSectionType
    {
        Unknown = 1,
        ArithmeticAlgebra = 2,
        BooleanAlgebra = 4,
        Inequality = 8,
        Equality = 16,
    }
}