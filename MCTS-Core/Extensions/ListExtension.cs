using System.Collections.Generic;

namespace MCTS.Core
{
    /// <summary>
    /// Adds utility methods to <see cref="List{T}"/>
    /// </summary>
    public static class ListExtension
    {
        /// <summary>
        /// Picks a random element from the given list and returns it <para/>
        /// This method is thread-safe
        /// </summary>
        /// <typeparam name="T"> The type that the source list contains</typeparam>
        /// <param name="source">The source list to choose the element from</param>
        /// <returns>The randomly chosen list element</returns>
        public static T PickRandom<T>(this IList<T> source)
        {
            return source[ThreadSafeRandom.Rand(0, source.Count)];
        }
    }
}
