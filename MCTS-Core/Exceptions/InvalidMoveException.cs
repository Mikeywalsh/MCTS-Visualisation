using System;

namespace MCTS.Core
{
    /// <summary>
    /// An exception which should be thrown when an invalid move is created or used
    /// </summary>
    public class InvalidMoveException : Exception
    {
        /// <summary>
        /// Creates an invalid move exception
        /// </summary>
        public InvalidMoveException() : base() { }

        /// <summary>
        /// Creates an invalid move exception with a message
        /// </summary>
        /// <param name="message">The message to include in the exception</param>
        public InvalidMoveException(string message) : base(message) { }
    }
}
