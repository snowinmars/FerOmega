using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SandS.Algorithm.Library.GeneratorNamespace;

namespace FerOmega.Common
{
    public static class Constants
    {
        public static TextGenerator TextGenerator { get; } = new TextGenerator();

        public static Random Random { get; } = new Random();
    }
}
