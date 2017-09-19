using System;
using System.Collections;
using System.Collections.Generic;

namespace MCTS.Core.Games
{
    /// <summary>
    /// A Chess game board, which allows a game of Chess to be played out on it
    /// </summary>
    public class ChessBoard : GridBasedBoard
    {
        /// <summary>
        /// The list of all active white pieces on this board
        /// </summary>
        private List<ChessPiece> whitePieces;

        /// <summary>
        /// The list of all active black pieces on this board
        /// </summary>
        private List<ChessPiece> blackPieces;

        /// <summary>
        /// Creates a new chess board representing an empty game
        /// </summary>
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

        /// <summary>
        /// Makes a move on this board for the current player
        /// </summary>
        /// <param name="move">The move to make</param>
        /// <returns>This board instance</returns>
        public override Board MakeMove(IMove move)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a list of all possible moves for the current player
        /// </summary>
        /// <returns>A list of all possible moves for the current player</returns>
        public override List<IMove> PossibleMoves()
        {
            List<IMove> moves = new List<IMove>();

            return moves;
        }

        /// <summary>
        /// Determines if there is a winner on this chess board <para/>
        /// If there is a winner, the Winner attribute is set to the ID of the winning player <para/>
        /// If there is a draw, the Winner attribute is set to 0 <para/>
        /// If the game is still in progress, the Winner attribute is set to -1
        /// </summary>
        protected override void DetermineWinner()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines if there is a winner on this chess board <para/>
        /// Uses the last played move to narrow down the amount of checks needed <para/>
        /// If there is a winner, the Winner attribute is set to the ID of the winning player <para/>
        /// If there is a draw, the Winner attribute is set to 0 <para/>
        /// If the game is still in progress, the Winner attribute is set to -1
        /// </summary>
        /// <param name="move">The last played move on this board</param>
        protected override void DetermineWinner(IMove move)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the amount of players on this chess board
        /// </summary>
        /// <returns>The amount of players on this chess board</returns>
        protected override int PlayerCount()
        {
            return 2;
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
