using NUnit.Framework;
using MCTS.Core;
using MCTS.Core.Games;

namespace MCTS.Test
{
    /// <summary>
    /// This class tests all aspects of the <see cref="ChessBoard"/> class
    /// </summary>
    public class ChessBoardTest
    {

        [Test]
        public void CreateBoardTest()
        {
            ChessBoard board = new ChessBoard();

            //Check that the current player is player 1
            Assert.AreEqual(1, board.CurrentPlayer);

            //Long, but has to be done, check each piece is where it should be
            Assert.AreEqual((int)ChessPieces.WHITE_ROOK, board.GetCell(0, 0));
            Assert.AreEqual((int)ChessPieces.WHITE_KNIGHT, board.GetCell(1, 0));
            Assert.AreEqual((int)ChessPieces.WHITE_BISHOP, board.GetCell(2, 0));
            Assert.AreEqual((int)ChessPieces.WHITE_QUEEN, board.GetCell(3, 0));
            Assert.AreEqual((int)ChessPieces.WHITE_KING, board.GetCell(4, 0));
            Assert.AreEqual((int)ChessPieces.WHITE_BISHOP, board.GetCell(5, 0));
            Assert.AreEqual((int)ChessPieces.WHITE_KNIGHT, board.GetCell(6, 0));
            Assert.AreEqual((int)ChessPieces.WHITE_ROOK, board.GetCell(7, 0));
            Assert.AreEqual((int)ChessPieces.WHITE_PAWN, board.GetCell(0, 1));
            Assert.AreEqual((int)ChessPieces.WHITE_PAWN, board.GetCell(1, 1));
            Assert.AreEqual((int)ChessPieces.WHITE_PAWN, board.GetCell(2, 1));
            Assert.AreEqual((int)ChessPieces.WHITE_PAWN, board.GetCell(3, 1));
            Assert.AreEqual((int)ChessPieces.WHITE_PAWN, board.GetCell(4, 1));
            Assert.AreEqual((int)ChessPieces.WHITE_PAWN, board.GetCell(5, 1));
            Assert.AreEqual((int)ChessPieces.WHITE_PAWN, board.GetCell(6, 1));
            Assert.AreEqual((int)ChessPieces.WHITE_PAWN, board.GetCell(7, 1));
            Assert.AreEqual((int)ChessPieces.BLACK_ROOK, board.GetCell(0, 7));
            Assert.AreEqual((int)ChessPieces.BLACK_KNIGHT, board.GetCell(1, 7));
            Assert.AreEqual((int)ChessPieces.BLACK_BISHOP, board.GetCell(2, 7));
            Assert.AreEqual((int)ChessPieces.BLACK_QUEEN, board.GetCell(3, 7));
            Assert.AreEqual((int)ChessPieces.BLACK_KING, board.GetCell(4, 7));
            Assert.AreEqual((int)ChessPieces.BLACK_BISHOP, board.GetCell(5, 7));
            Assert.AreEqual((int)ChessPieces.BLACK_KNIGHT, board.GetCell(6, 7));
            Assert.AreEqual((int)ChessPieces.BLACK_ROOK, board.GetCell(7, 7));
            Assert.AreEqual((int)ChessPieces.BLACK_PAWN, board.GetCell(0, 6));
            Assert.AreEqual((int)ChessPieces.BLACK_PAWN, board.GetCell(1, 6));
            Assert.AreEqual((int)ChessPieces.BLACK_PAWN, board.GetCell(2, 6));
            Assert.AreEqual((int)ChessPieces.BLACK_PAWN, board.GetCell(3, 6));
            Assert.AreEqual((int)ChessPieces.BLACK_PAWN, board.GetCell(4, 6));
            Assert.AreEqual((int)ChessPieces.BLACK_PAWN, board.GetCell(5, 6));
            Assert.AreEqual((int)ChessPieces.BLACK_PAWN, board.GetCell(6, 6));
            Assert.AreEqual((int)ChessPieces.BLACK_PAWN, board.GetCell(7, 6));

            //Ensure that the winner value is - 1
            Assert.AreEqual(-1, board.Winner);
        }
    }
}
