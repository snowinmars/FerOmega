using System.Collections.Generic;
using FerOmega.Entities.InternalSyntax;

namespace FerOmega.Services.Abstractions
{
    public interface IGrammarConfig
    {
        Operator[] ConfigOperators();

        string GetOperatorsAsRegex(IEnumerable<string> denotations);
    }
}