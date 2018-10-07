using System.Collections.Generic;

namespace FerOmega.Tests
{
    internal interface IEquationGenerator
    {
        EquationGenerator.Equation[] GetEquations();

        EquationGenerator.Equation GetAlgebraEquation();
        IEnumerable<EquationGenerator.Equation> GetAlgebraEquations(int count);
    }
}