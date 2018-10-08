using System.Collections.Generic;

using FerOmega.Entities;

namespace FerOmega.Tests
{
    internal interface ISmokeEquationGenerator
    {
        SmokeEquationGenerator.Equation[] GetEquations();

    }
}