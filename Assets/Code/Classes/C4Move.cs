using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A move that can be made in Connect 4
/// </summary>
public class C4Move : Move
{
    /// <summary>
    /// X position of this move
    /// </summary>
    public int x;

    /// <summary>
    /// Creates a new Connect 4 move with the given x position
    /// </summary>
    /// <param name="xPos">X position of the move to make</param>
    public C4Move(int xPos)
    {
        if (xPos > 7 || xPos < 0)
        {
            throw new InvalidMoveException("Move: " + "(" + xPos + ")" + " is out of bounds of the 0-7 board space");
        }

        x = xPos;
    }

    /// <summary>
    /// Gives a string representation of this Connect 4 move
    /// </summary>
    /// <returns>A string representation of this connect 4 move</returns>
    public override string ToString()
    {
        return "(" + x + ")";
    }

    /// <summary>
    /// Equality override for a Connect 4 move
    /// Two moves are equal if their x positions are equal
    /// </summary>
    /// <param name="obj">The other C4Move instance to compare this one too</param>
    /// <returns>True if the objects are equal, false otherwise</returns>
    public override bool Equals(object obj)
    {
        if (obj is C4Move)
        {
            C4Move other = (C4Move)obj;
            if (other.x == x)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Returns a unique hash code for this instance
    /// Represented as a 1 digit integer
    /// </summary>
    /// <returns>A unique integer for this instance</returns>
    public override int GetHashCode()
    {
        return x;
    }
}
