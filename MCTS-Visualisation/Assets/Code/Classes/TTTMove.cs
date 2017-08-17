/// <summary>
/// A move that can be made in Tic-Tac-Toe
/// </summary>
public class TTTMove : Move {

    /// <summary>
    /// X position of this move
    /// </summary>
    public int X { get; private set; }

    /// <summary>
    /// Y position of this move
    /// </summary>
    public int Y { get; private set; }

    /// <summary>
    /// Creates a new Tic-Tac-Toe move with the given x and y positions
    /// </summary>
    /// <param name="xPos">X position of the move to make</param>
    /// <param name="yPos">Y position of the move to make</param>
    public TTTMove(int xPos, int yPos)
    {
        if(xPos > 2 || yPos > 2 || xPos < 0 || yPos < 0)
        {
            throw new InvalidMoveException("Move: " + "(" + xPos + "," + yPos + ")" + " is out of bounds of the 3x3 game area");
        }

        X = xPos;
        Y = yPos;
    }

    /// <summary>
    /// Gives a string representation of this Tic-Tac-Toe move
    /// </summary>
    /// <returns>A string representation of this Tic-Tac-Toe move</returns>
    public override string ToString()
    {
        return "(" + X + "," + Y + ")";
    }

    /// <summary>
    /// Equality override for a Tic-Tac-Toe move
    /// Two moves are equal if their x and y positions are equal
    /// </summary>
    /// <param name="obj">The other TTTMove instance to compare this one too</param>
    /// <returns>True if the objects are equal, false otherwise</returns>
    public override bool Equals(object obj)
    {
        if(obj is TTTMove)
        {
            TTTMove other = (TTTMove)obj;
            if (other.X == X && other.Y == Y)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Returns a unique hash code for this instance
    /// Represented as a 4 digit integer
    /// </summary>
    /// <returns>A unique integer for this instance</returns>
    public override int GetHashCode()
    {
        return int.Parse(X.ToString() + Y.ToString());
    }
}
