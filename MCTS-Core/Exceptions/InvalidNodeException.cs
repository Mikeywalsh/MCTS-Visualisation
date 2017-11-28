using System;

namespace MCTS.Core
{
    /// <summary>
    /// An exception which should be thrown when an invalid node is being used <para/>
    /// For example, trying to add children to a leaf node
    /// </summary>
    public class InvalidNodeException : Exception
    {
        /// <summary>
        /// Creates an invalid node exception
        /// </summary>
        public InvalidNodeException() : base() { }

        /// <summary>
        /// Creates an invalid node exception with a message
        /// </summary>
        /// <param name="message">The message to include in the exception</param>
        public InvalidNodeException(string message) : base(message) { }
    }
}
