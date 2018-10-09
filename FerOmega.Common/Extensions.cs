using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
