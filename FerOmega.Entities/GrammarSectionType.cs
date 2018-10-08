using System;

namespace FerOmega.Entities
{
    [Flags]
    public enum GrammarSectionType
    {
        Unknown = 1,
        Algebra = 2,
        BooleanAlgebra = 3,
        Inequality = 4,
        Equality = 5,
    }
}