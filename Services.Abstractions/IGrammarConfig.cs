using System;
using System.Collections.Generic;
using Entities.InternalSyntax;

namespace Services.Abstractions
{
    public interface IGrammarConfig
    {
        Operator[] ConfigOperators();
    }
}