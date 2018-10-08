using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerOmega.Entities
{
    [Flags]
    public enum GrammarSectionType
    {
        Unknown = 1,
        Algebra = 1,
        BooleanAlgebra = 2,
        Inequality = 3,
        Equality = 4,
    }
}
