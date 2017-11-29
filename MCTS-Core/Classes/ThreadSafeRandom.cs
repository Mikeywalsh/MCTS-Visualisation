using System;
using System.Threading;

namespace MCTS.Core
{
    /// <summary>
    /// A thread-safe random class, that can be used to generate random numbers concurrently across multiple threads
    /// </summary>
    public static class ThreadSafeRandom
    {
        /// <summary>
        /// The random seed, determined from the environment tickcount
        /// </summary>
        static int seed = Environment.TickCount;

        /// <summary>
        /// A ThreadLocal Random instance which can be used to generate random numbers in a thread-safe way
        /// </summary>
        static readonly ThreadLocal<Random> random = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));

        /// <summary>
        /// Gets a new random integer value
        /// </summary>
        /// <returns>A random integer value</returns>
        public static int Rand()
        {
            return random.Value.Next();
        }

        /// <summary>
        /// Gets a new random integer value that is at least as small as the passed in minimum value
        /// </summary>
        /// <param name="minValue">The minimum value that the random integer can be</param>
        /// <returns>A random integer value that is at least as big as the minimum value</returns>
        public static int Rand(int minValue)
        {
            return random.Value.Next(minValue);
        }

        /// <summary>
        /// Gets a new random integer value that is between the inclusive minimum value and exclusing maximum value
        /// </summary>
        /// <param name="minValue">The inclusive minimum value that the random integer can be</param>
        /// <param name="maxValue">The exclusive maximum value that the random integer can be</param>
        /// <returns>A new random integer value that is between the inclusive minimum value and exclusing maximum value</returns>
        public static int Rand(int minValue, int maxValue)
        {
            return random.Value.Next(minValue, maxValue);
        }
    }
}
