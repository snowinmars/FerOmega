using System.Collections.Generic;
using FerOmega.Entities;

namespace FerOmega.Tests.Randomize
{
    internal interface IRandomizeEquationGenerator
    {
        IEnumerable<ShortToken> GetArithmeticAlgebraEquations(long count,
            int minOperatorsCount = 5,
            int maxOperatorsCount = 7);

        IEnumerable<ShortToken> GetBooleanAlgebraEquations(long count,
            int minOperatorsCount = 5,
            int maxOperatorsCount = 7);

        IEnumerable<ShortToken> GetEquations(long count, GrammarSectionType grammarSectionType);
    }
}