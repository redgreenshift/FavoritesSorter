using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FavoritesSorter
{
    /// <summary>
    /// Provides a string comparison strategy that delegates ordering decisions to an external prompt mechanism with
    /// memoization for repeat comparisons.
    /// </summary>
    /// <remarks>Handles ReferenceEquals, null references, and value equality. Stores comparison results in
    /// the decisions cache using ordered pairs as keys, and maintains symmetric entries for reverse comparisons. Throws
    /// ArgumentOutOfRangeException when the delegate returns values outside the range of -1 through 1.</remarks>
    internal class InteractiveStringComparer : IComparer<string>
    {
        private readonly Func<string, string, int> prompt;
        private readonly Dictionary<(string, string), int> decisions = new Dictionary<(string, string), int>();

        /// <summary>
        /// Initializes a new instance of the InteractiveStringComparer class with the specified prompt delegate.
        /// </summary>
        /// <param name="prompt">The comparison function that handles interactive string comparisons and returns a sort code.</param>
        public InteractiveStringComparer(Func<string, string, int> prompt)
        {
            this.prompt = prompt;
        }

        /// <summary>
        /// Compares two string objects.
        /// </summary>
        /// <remarks>The method uses a <see cref="Dictionary{TKey,TValue}">Decisions</see> dictionary to
        /// memoize comparison results for transitive optimization. Results for reverse pairs (y, x) are stored as
        /// negated values. Initial comparison is delegated to a <c>prompt</c> callback.</remarks>
        /// <param name="x"><c>x</c></param>
        /// <param name="y"><c>y</c></param>
        /// <returns>-1: <c>x</c> is less than <c>y</c>, 0: they are equal, 1: <c>x</c> is greater than <c>y</c>. Results are
        /// memoized in the <see cref="Decisions"/> dictionary.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the initial comparison result from <paramref name="prompt"/> is outside the range of -1 to 1,
        /// indicating an invalid comparator result. The actual value being compared is unknown at this point.</exception>
        public int Compare(string x, string y)
        {
            if (ReferenceEquals(x, y))
                return 0;

            if (x is null)
                return -1;

            if (y is null)
                return 1;

            if (x == y)
                return 0;

            var key = (x, y);

            if (decisions.TryGetValue(key, out int result))
                return result;

            result = prompt(x, y);

            //if (result is < -1 or > 1)
            if (result < -1 || result > 1)
                throw new ArgumentOutOfRangeException(nameof(result));

            decisions[(x, y)] = result;
            decisions[(y, x)] = -result;

            return result;
        }
    }
}
