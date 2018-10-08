using System.Collections.Generic;

using FerOmega.Entities;

namespace FerOmega.Tests
{
    internal interface IRandomizeEquationGenerator
    {

        ShortToken GetAlgebraEquation(GrammarSectionType grammarSectionType);

        IEnumerable<ShortToken> GetAlgebraEquations(int count, GrammarSectionType grammarSectionType);
    }
}