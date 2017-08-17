using System.Collections;
using System.Collections.Generic;

namespace MCTS_Core
{
    public class ChessMove : Move
    {

        public ChessPiece PieceToMove { get; private set; }

        public int ToX { get; private set; }

        public int ToY { get; private set; }

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

        public override int GetHashCode()
        {
            return int.Parse(((int)PieceToMove.PieceType).ToString() + PieceToMove.XPos.ToString() + PieceToMove.YPos.ToString() + ToX.ToString() + ToY.ToString());
        }

        public override string ToString()
        {
            return "(" + ToX + "," + ToY + ")";
        }
    }
}
