using System;
using System.Collections.Generic;
using System.Linq;

namespace FerOmega.Common
{
    public static class Extensions
    {
        private static readonly string[] Letters;

        private static int lastAlphabetLetter;

        static Extensions()
        {
            Letters = new[]
            {
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

        public static T GetRandomElement<T>(this IList<T> collection, Random random = null)
        {
            if (random == null)
            {
                random = Constants.Random;
            }

            return collection[random.Next(0, collection.Count - 1)];
        }

        public static string NextAlphabetSymbol(this Random random)
        {
            if (lastAlphabetLetter >= Letters.Length || lastAlphabetLetter < 0)
            {
                lastAlphabetLetter = 0;
            }

            var symbol = Letters[lastAlphabetLetter];

            lastAlphabetLetter++;

            return symbol;
        }

        public static bool NextBool(this Random random)
        {
            return random.Next() % 2 == 0;
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random random = null)
        {
            if (random == null)
            {
                random = Constants.Random;
            }

            if (source == null)
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
    }
}