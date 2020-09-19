using System;
using System.Collections.Generic;
using System.Linq;

namespace FerOmega.Common
{
    public static class Extensions
    {
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

        private static readonly string[] Letters;

        private static int lastAlphabetLetter;

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
            if (lastAlphabetLetter >= Letters.Length ||
                lastAlphabetLetter < 0)
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

            for (var i = elements.Length - 1; i > 0; i--)
            {
                var swapIndex = random.Next(i + 1);

                yield return elements[swapIndex];

                elements[swapIndex] = elements[i];
            }

            yield return elements[0];
        }

        public static int ToInt<T>(this T value)
        {
            return Convert.ToInt32(value);
        }

        public static T ToEnum<T>(this int value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }

        public static bool IsDefined<T>(this int enumValue)
        {
            var isDefined = Enum.IsDefined(typeof(T), enumValue);

            if (isDefined)
            {
                return true;
            }

            for (var i = 0; i < 32; i++)
            {
                var bit = enumValue << i;

                if ((enumValue & bit) == 0)
                {
                    continue;
                }

                isDefined = Enum.IsDefined(typeof(T), enumValue & bit);

                if (!isDefined)
                {
                    return false;
                }
            }

            return true;
        }
    }
}