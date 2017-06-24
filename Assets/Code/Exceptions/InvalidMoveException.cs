using System;

/// <summary>
/// An exception which should be thrown when an invalid move is created or used
/// </summary>
public class InvalidMoveException : Exception {

    public InvalidMoveException() : base() { }

    public InvalidMoveException(string message) : base(message) { }
}
