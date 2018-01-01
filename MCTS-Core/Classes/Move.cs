using System;

namespace MCTS.Core
{
    /// <summary>
    /// An abstract class that any moves of <see cref="Board"/> types must implement
    /// </summary>
    [Serializable]
    public abstract class Move
    {
        /// <summary>
        /// Equality override which returns true if this instance is equal to a passed in object
        /// </summary>
        /// <param name="obj">The other object to compare with</param>
        /// <returns>True if this instance is equal to the passed in object</returns>
        public override abstract bool Equals(object obj);

        /// <summary>
        /// Returns a unique hash code for this move instance
        /// </summary>
        /// <returns>A unique integer for this instance</returns>
        public override abstract int GetHashCode();

        /// <summary>
        /// Returns a string representation of this move instance
        /// </summary>
        /// <returns>A string representation of this move instance</returns>
        public override abstract string ToString();
    }
}
