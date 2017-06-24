using System;

/// <summary>
/// An exception which should be thrown when an invalid node is being used
/// For example, trying to add children to a leaf node
/// </summary>
public class InvalidNodeException : Exception
{
    public InvalidNodeException() : base() { }

    public InvalidNodeException(string message) : base(message) { }
}
