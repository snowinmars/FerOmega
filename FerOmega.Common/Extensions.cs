using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SandS.Algorithm.Library.GeneratorNamespace;

namespace FerOmega.Common
{
    public static class Extensions
    {
        /// <summary>
        /// Returns sequence of random elements from source.
        /// </summary>
        /// <param name="source">Source collection</param>
        /// <param name="random">Random generator</param>
        /// <exception cref="ArgumentNullException">If any parameter is null</exception>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random random)
        {
            if (source == null || random == null)
            {
                throw new ArgumentNullException();
            }

            var elements = source.ToArray();

            for (int i = elements.Length - 1; i > 0; i--)
            {
                int swapIndex = random.Next(i + 1);
                yield return elements[swapIndex];
                elements[swapIndex] = elements[i];
            }

            yield return elements[0];
        }

        private static string[] letters;

        static Extensions()
        {
            letters = new[] {
                "a",
                "b",
                "c",
                "d",
                "e",
                "f",
                "g",
                "h",
                "i",
                "j",
                "k",
                "l",
                "m",
                "n",
                "o",
                "p",
                "q",
                "r",
                "s",
                "t",
                "u",
                "v",
                "w",
                "x",
                "y",
                "z",
            };
        }

        private static int lastAlphabetLetter;

        public static string NextAlphabetSymbol(this Random random)
        {
            if (lastAlphabetLetter >= letters.Length || lastAlphabetLetter < 0)
            {
                lastAlphabetLetter = 0;
            }

            var symbol = letters[lastAlphabetLetter];

            lastAlphabetLetter++;

            return symbol;
        }

        public static bool NextBool(this Random random)
        {
            return random.Next() % 2 == 0;
        }
    }
}
