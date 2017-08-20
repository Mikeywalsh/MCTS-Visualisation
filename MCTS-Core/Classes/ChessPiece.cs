using System.Collections;
using System.Collections.Generic;

namespace MCTS.Core
{
    /// <summary>
    /// Represents a chess piece, which can be played on a <see cref="ChessBoard"/>
    /// </summary>
    public class ChessPiece
    {
        /// <summary>
        /// The type of piece that this chess piece is
        /// </summary>
        public ChessPieces PieceType { get; private set; }

        /// <summary>
        /// The X position of this chess piece
        /// </summary>
        public int XPos { get; private set; }

        /// <summary>
        /// The Y position of this chess piece
        /// </summary>
        public int YPos { get; private set; }

        /// <summary>
        /// Constructs a new <see cref="ChessPiece"/> instance
        /// </summary>
        /// <param name="pieceType">The type of piece to assign this chess piece to</param>
        /// <param name="xPos">The X position of this chess piece</param>
        /// <param name="yPos">The Y position of this chess piece</param>
        public ChessPiece(ChessPieces pieceType, int xPos, int yPos)
        {
            PieceType = pieceType;
            XPos = xPos;
            YPos = yPos;
        }

        /// <summary>
        /// Checks equality for two given chess pieces
        /// </summary>
        /// <param name="p1">The first chess piece to check</param>
        /// <param name="p2">The second chess piece to check</param>
        /// <returns>True if both passed in pieces are equal, false otherwise</returns>
        public static bool operator ==(ChessPiece p1, ChessPiece p2)
        {
            //If one piece is null and the other isn't, return false, if they are both null, return true
            if (object.ReferenceEquals(p1, null) || object.ReferenceEquals(p2, null))
            {
                return object.ReferenceEquals(p1, null) && object.ReferenceEquals(p2, null);
            }

            return p1.PieceType == p2.PieceType &&
                    p1.XPos == p2.XPos &&
                    p1.YPos == p2.YPos;
        }

        /// <summary>
        /// Checks inequality for two given chess pieces
        /// </summary>
        /// <param name="p1">The first chess piece to check</param>
        /// <param name="p2">The second chess piece to check</param>
        /// <returns>False if both passed in pieces are equal, true otherwise</returns>
        public static bool operator !=(ChessPiece p1, ChessPiece p2)
        {
            return !(p1 == p2);
        }

        /// <summary>
        /// Checks equality against another object
        /// </summary>
        /// <param name="obj">An object to check equality against. Should be a <see cref="ChessPiece"/></param>
        /// <returns>True if this object is equal to the passed in piece, false otherwise</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is ChessPiece))
                return false;

            return this == (ChessPiece)obj;
        }

        /// <summary>
        /// Returns a 4 digit hash code which uniquely describes a ChessPiece
        /// For example, a White Queen at position 3,4 would have a hashcode of - 1534
        /// </summary>
        /// <returns>A hashcode uniquely describing this chess piece</returns>
        public override int GetHashCode()
        {
            return int.Parse(((int)PieceType).ToString() + XPos.ToString() + YPos.ToString());
        }

        /// <summary>
        /// Returns a string representation of this chess board
        /// </summary>
        /// <returns>A string representation of this chess board</returns>
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
