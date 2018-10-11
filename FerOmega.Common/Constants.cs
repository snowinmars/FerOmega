using System;

using SandS.Algorithm.Library.GeneratorNamespace;

namespace FerOmega.Common
{
    public static class Constants
    {
        public static Random Random { get; } = new Random();

        public static TextGenerator TextGenerator { get; } = new TextGenerator();
    }
}