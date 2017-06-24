/// <summary>
/// A move that can be made in Tic-Tac-Toe
/// </summary>
public class TTTMove : Move {

    /// <summary>
    /// X position of this move
    /// </summary>
    public int x;

    /// <summary>
    /// Y position of this move
    /// </summary>
    public int y;

    /// <summary>
    /// Creates a new Tic-Tac-Toe move with the given x and y positions
    /// </summary>
    /// <param name="xPos">X position of the move to make</param>
    /// <param name="yPos">Y position of the move to make</param>
    public TTTMove(int xPos, int yPos)
    {
        if(xPos > 2 || yPos > 2)
        {
            throw new InvalidMoveException("Move: " + "(" + xPos + "," + yPos + ")" + " is out of bounds of the 3x3 game area");
        }

        x = xPos;
        y = yPos;
    }

    /// <summary>
    /// Gives a string representation of this Tic-Tac-Toe move
    /// </summary>
    /// <returns>A string representation of this Tic-Tac-Toe move</returns>
    public override string ToString()
    {
        return "(" + x + "," + y + ")";
    }

    public override bool Equals(object obj)
    {
        if(obj is TTTMove)
        {
            TTTMove other = (TTTMove)obj;
            if (other.x == x && other.y == y)
                return true;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return int.Parse(x.ToString() + y.ToString());
    }
}
