using System.Collections.Generic;

namespace MCTS.Core.Games
{
    /// <summary>
    /// A move that can be made in Othello
    /// </summary>
    public class OthelloMove : Move
    {
        /// <summary>
        /// Position of this move
        /// </summary>
        public Point Position { get; private set; }

        /// <summary>
        /// A list of cells that will be captured as a result of this move
        /// </summary>
        public List<Point> CapturableCells { get; private set; }

        /// <summary>
        /// Creates a new Othello move with the given x and y positions
        /// </summary>
        /// <param name="pos">Position of the move to make</param>
        /// <param name="cellsToCapture">A list of cells that will be captured as a result of this move</param>
        public OthelloMove(Point pos, List<Point> cellsToCapture)
        {
            if (pos.X > 7 || pos.Y > 7 || pos.X < 0 || pos.Y < 0)
            {
                throw new InvalidMoveException("Move: " + "(" + pos.X + "," + pos.Y + ")" + " is out of bounds of the 8x8 game area");
            }

            if(cellsToCapture.Count == 0)
            {
                throw new InvalidMoveException("No cells will be captured as a result of this move, at least one cell must be captured");
            }

            CapturableCells = cellsToCapture;
            Position = pos;
        }

        /// <summary>
        /// Gives a string representation of this Othello move
        /// </summary>
        /// <returns>A string representation of this Othello move</returns>
        public override string ToString()
        {
            return "(" + X + "," + Y + ")";
        }

        /// <summary>
        /// Equality override for a Othello move <para/>
        /// Two moves are equal if their x and y positions are equal
        /// </summary>
        /// <param name="obj">The other OthelloMove instance to compare this one too</param>
        /// <returns>True if the objects are equal, false otherwise</returns>
        public override bool Equals(object obj)
        {
            if (obj is OthelloMove)
            {
                OthelloMove other = (OthelloMove)obj;
                if (other.X == X && other.Y == Y)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns a unique hash code for this instance <para/>
        /// Represented as a 4 digit integer
        /// </summary>
        /// <returns>A unique integer for this instance</returns>
        public override int GetHashCode()
        {
            return int.Parse(X.ToString() + Y.ToString());
        }

        /// <summary>
        /// X position of this move
        /// </summary>
        public int X
        {
            get { return Position.X; }
        }

        /// <summary>
        /// Y position of this move
        /// </summary>
        public int Y
        {
            get { return Position.Y; }
        }
    }
}