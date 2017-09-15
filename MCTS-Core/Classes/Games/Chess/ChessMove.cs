using System.Collections;
using System.Collections.Generic;

namespace MCTS.Core.Games
{
    /// <summary>
    /// A move that can be made in Chess
    /// </summary>
    public class ChessMove : IMove
    {
        /// <summary>
        /// The chess piece being moves
        /// </summary>
        public ChessPiece PieceToMove { get; private set; }

        /// <summary>
        /// The destination X position for this move
        /// </summary>
        public int ToX { get; private set; }

        /// <summary>
        /// The destination Y position for this move
        /// </summary>
        public int ToY { get; private set; }

        /// <summary>
        /// Creates a new chess move, given a piece and destination X and Y coordinates
        /// </summary>
        /// <param name="toMove">The chess piece to move</param>
        /// <param name="toX">The X position to move to</param>
        /// <param name="toY">The Y position to move to</param>
        public ChessMove(ChessPiece toMove, int toX, int toY)
        {
            if (toX < 0 || toY < 0 || toX > 7 || toY > 7)
            {
                throw new InvalidMoveException("Move: " + "(" + toX + "," + toY + ") is out of bounds of the 8x8 game board area");
            }
            PieceToMove = toMove;
            ToX = toX;
            ToY = toY;
        }

        /// <summary>
        /// Equality checker against another object
        /// </summary>
        /// <param name="obj">The object to check against, should be a <see cref="ChessPiece"/></param>
        /// <returns>True if this instance is equal to the passed in object, false otherwise</returns>
        public override bool Equals(object obj)
        {
            if (obj is ChessMove)
            {
                ChessMove other = (ChessMove)obj;
                return other.PieceToMove == PieceToMove &&
                        other.ToX == ToX &&
                        other.ToY == ToY;
            }

            return false;
        }

        /// <summary>
        /// Returns a unique hashcode representing this chess move
        /// </summary>
        /// <returns>A unique hashcode representing this chess move</returns>
        public override int GetHashCode()
        {
            return int.Parse(((int)PieceToMove.PieceType).ToString() + PieceToMove.XPos.ToString() + PieceToMove.YPos.ToString() + ToX.ToString() + ToY.ToString());
        }

        /// <summary>
        /// Returns a string representation of this chess move
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "(" + ToX + "," + ToY + ")";
        }
    }
}
