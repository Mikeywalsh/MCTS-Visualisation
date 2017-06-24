using System.Collections.Generic;
using System;

public static class EnumerableExtension {

    /// <summary>
    /// Picks a random element from the given list and returns it
    /// </summary>
    /// <typeparam name="T"> The type that the source list contains</typeparam>
    /// <param name="source">The source list to choose the element from</param>
    /// <param name="r">The random instance used to choose a proper random element</param>
    /// <returns>The randomly chosen list element</returns>
    public static T PickRandom<T>(this IList<T> source, Random r)
    {
        return source[r.Next(0, source.Count)];
    }
}
