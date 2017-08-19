using System;
using System.Collections;
using System.Collections.Generic;

namespace MCTS.Core
{
    public class ChessBoard : GridBasedBoard
    {
        private List<ChessPiece> whitePieces;
        private List<ChessPiece> blackPieces;

        public ChessBoard()
        {
            currentPlayer = 1;
            boardContents = new int[8, 8];

            #region Create starting white pieces
            whitePieces = new List<ChessPiece>();
            whitePieces.Add(new ChessPiece(ChessPieces.WHITE_ROOK, 0, 0));
            whitePieces.Add(new ChessPiece(ChessPieces.WHITE_KNIGHT, 1, 0));
            whitePieces.Add(new ChessPiece(ChessPieces.WHITE_BISHOP, 2, 0));
            whitePieces.Add(new ChessPiece(ChessPieces.WHITE_QUEEN, 3, 0));
            whitePieces.Add(new ChessPiece(ChessPieces.WHITE_KING, 4, 0));
            whitePieces.Add(new ChessPiece(ChessPieces.WHITE_BISHOP, 5, 0));
            whitePieces.Add(new ChessPiece(ChessPieces.WHITE_KNIGHT, 6, 0));
            whitePieces.Add(new ChessPiece(ChessPieces.WHITE_ROOK, 7, 0));
            whitePieces.Add(new ChessPiece(ChessPieces.WHITE_PAWN, 0, 1));
            whitePieces.Add(new ChessPiece(ChessPieces.WHITE_PAWN, 1, 1));
            whitePieces.Add(new ChessPiece(ChessPieces.WHITE_PAWN, 2, 1));
            whitePieces.Add(new ChessPiece(ChessPieces.WHITE_PAWN, 3, 1));
            whitePieces.Add(new ChessPiece(ChessPieces.WHITE_PAWN, 4, 1));
            whitePieces.Add(new ChessPiece(ChessPieces.WHITE_PAWN, 5, 1));
            whitePieces.Add(new ChessPiece(ChessPieces.WHITE_PAWN, 6, 1));
            whitePieces.Add(new ChessPiece(ChessPieces.WHITE_PAWN, 7, 1));
            #endregion

            #region Create starting black pieces
            blackPieces = new List<ChessPiece>();
            blackPieces.Add(new ChessPiece(ChessPieces.BLACK_ROOK, 0, 7));
            blackPieces.Add(new ChessPiece(ChessPieces.BLACK_KNIGHT, 1, 7));
            blackPieces.Add(new ChessPiece(ChessPieces.BLACK_BISHOP, 2, 7));
            blackPieces.Add(new ChessPiece(ChessPieces.BLACK_QUEEN, 3, 7));
            blackPieces.Add(new ChessPiece(ChessPieces.BLACK_KING, 4, 7));
            blackPieces.Add(new ChessPiece(ChessPieces.BLACK_BISHOP, 5, 7));
            blackPieces.Add(new ChessPiece(ChessPieces.BLACK_KNIGHT, 6, 7));
            blackPieces.Add(new ChessPiece(ChessPieces.BLACK_ROOK, 7, 7));
            blackPieces.Add(new ChessPiece(ChessPieces.BLACK_PAWN, 0, 6));
            blackPieces.Add(new ChessPiece(ChessPieces.BLACK_PAWN, 1, 6));
            blackPieces.Add(new ChessPiece(ChessPieces.BLACK_PAWN, 2, 6));
            blackPieces.Add(new ChessPiece(ChessPieces.BLACK_PAWN, 3, 6));
            blackPieces.Add(new ChessPiece(ChessPieces.BLACK_PAWN, 4, 6));
            blackPieces.Add(new ChessPiece(ChessPieces.BLACK_PAWN, 5, 6));
            blackPieces.Add(new ChessPiece(ChessPieces.BLACK_PAWN, 6, 6));
            blackPieces.Add(new ChessPiece(ChessPieces.BLACK_PAWN, 7, 6));
            #endregion

            #region Place pieces on the board
            foreach (ChessPiece piece in whitePieces)
            {
                boardContents[piece.XPos, piece.YPos] = (int)piece.PieceType;
            }

            foreach (ChessPiece piece in blackPieces)
            {
                boardContents[piece.XPos, piece.YPos] = (int)piece.PieceType;
            }
            #endregion
        }

        /// <summary>
        /// Create a new Chess board as a copy from an existing board
        /// </summary>
        /// <param name="board">The board to make a copy of</param>
        private ChessBoard(ChessBoard board)
        {
            currentPlayer = board.CurrentPlayer;
            boardContents = (int[,])board.boardContents.Clone();
        }

        /// <summary>
        /// Duplicates the current Chess board
        /// </summary>
        /// <returns>A clone of the current Chess Board</returns>
        public override Board Duplicate()
        {
            return new ChessBoard(this);
        }

        public override Board MakeMove(Move move)
        {
            throw new NotImplementedException();
        }

        public override List<Move> PossibleMoves()
        {
            List<Move> moves = new List<Move>();

            return moves;
        }

        protected override void DetermineWinner()
        {
            throw new NotImplementedException();
        }

        protected override void DetermineWinner(Move move)
        {
            throw new NotImplementedException();
        }

        protected override int PlayerCount()
        {
            return 2;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
