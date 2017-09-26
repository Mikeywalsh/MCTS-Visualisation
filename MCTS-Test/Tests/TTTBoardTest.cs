using NUnit.Framework;
using MCTS.Core;
using MCTS.Core.Games;

namespace MCTS.Test
{
    /// <summary>
    /// This class tests all aspects of the <see cref="TTTBoard"/> class
    /// </summary>
    public class TTTBoardTest
    {
        [Test]
        public void CreateBoardTest()
        {
            TTTBoard board = new TTTBoard();

            //Check that the current player is player 1
            Assert.AreEqual(1, board.CurrentPlayer);

            //Ensure that the created board is empty
            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    Assert.AreEqual(0, board.GetCell(x, y));
                }
            }

            //Ensure that the winner value is - 1
            Assert.AreEqual(-1, board.Winner);
        }

        [Test]
        public void MakeMoveTest()
        {
            //Create a new board and make a move in it
            TTTBoard board = new TTTBoard();
            board.MakeMove(new TTTMove(1, 0));

            //Check that the move was made correctly
            Assert.AreEqual(1, board.GetCell(1, 0));

            //Make a move on the board for the second player
            board.MakeMove(new TTTMove(2, 1));

            //Check that the move was made correctly
            Assert.AreEqual(2, board.GetCell(2, 1));
        }

        [Test]
        public void MakeMoveInNonEmptyCell()
        {
            TTTBoard board = new TTTBoard();

            //Make a move on this board
            board.MakeMove(new TTTMove(2, 2));

            //Attempt to make another move in the same place as the last move, which should throw an InvalidMoveException
            Assert.Throws<InvalidMoveException>(() => board.MakeMove(new TTTMove(2, 2)));
        }

        [Test]
        public void GetPossibleMovesEmptyBoard()
        {
            TTTBoard board = new TTTBoard();
            Assert.AreEqual(9, board.PossibleMoves().Count);
        }

        [Test]
        public void GetPossibleMovesOneMoveMade()
        {
            TTTBoard board = new TTTBoard();
            board.MakeMove(new TTTMove(1, 2));
            Assert.AreEqual(8, board.PossibleMoves().Count);
        }

        [Test]
        public void GetPossibleMovesFullBoard()
        {
            TTTBoard board = new TTTBoard();
            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    board.MakeMove(new TTTMove(x, y));
                }
            }
            Assert.AreEqual(0, board.PossibleMoves().Count);
        }

        [Test]
        public void DuplicateTest()
        {
            //Create a new board and make a move in it
            TTTBoard boardA = new TTTBoard();
            boardA.MakeMove(new TTTMove(1, 1));

            //Duplicate the board and store it in a new board instance
            TTTBoard boardB = (TTTBoard)boardA.Duplicate();

            //Ensure the move made before duplication is present in both boards
            Assert.AreEqual(1, boardA.GetCell(1, 1));
            Assert.AreEqual(1, boardB.GetCell(1, 1));

            //These two board instances should share no memory, lets prove it by making moves in each of them and checking the other
            boardA.MakeMove(new TTTMove(2, 0));
            Assert.AreEqual(2, boardA.GetCell(2, 0));
            Assert.AreEqual(0, boardB.GetCell(2, 0));

            boardB.MakeMove(new TTTMove(1, 2));
            Assert.AreEqual(0, boardA.GetCell(1, 2));
            Assert.AreEqual(2, boardB.GetCell(1, 2));
        }

        [Test]
        public void NoWinnerTest()
        {
            TTTBoard board = new TTTBoard();

            //Make a move, if there is a winner, the winner flag will be set, but there is no winner, so it shouldn't
            board.MakeMove(new TTTMove(0, 0));

            //Check that the winner flag has not been set, as there is no winner
            Assert.AreEqual(-1, board.Winner);
        }

        [Test]
        public void DrawTest()
        {
            TTTBoard board = new TTTBoard();

            //Make moves on the board until it is full and there is no winner
            board.MakeMove(new TTTMove(0, 0));
            board.MakeMove(new TTTMove(1, 1));
            board.MakeMove(new TTTMove(2, 2));
            board.MakeMove(new TTTMove(0, 1));
            board.MakeMove(new TTTMove(0, 2));
            board.MakeMove(new TTTMove(2, 0));
            board.MakeMove(new TTTMove(2, 1));
            board.MakeMove(new TTTMove(1, 2));
            board.MakeMove(new TTTMove(1, 0));

            //Check that the game has ended in a draw
            Assert.AreEqual(0, board.Winner);
        }

        [Test]
        public void WinTestHorizontal()
        {
            TTTBoard board = new TTTBoard();

            //Make moves such that player 1 should win with a horizontal victory
            board.MakeMove(new TTTMove(0, 2));
            board.MakeMove(new TTTMove(0, 0));
            board.MakeMove(new TTTMove(1, 2));
            board.MakeMove(new TTTMove(2, 0));
            board.MakeMove(new TTTMove(2, 2));

            //Player 1 should have won the game
            Assert.AreEqual(1, board.Winner);
        }

        [Test]
        public void WinTestVertical()
        {
            TTTBoard board = new TTTBoard();

            //Make moves such that player 2 should win with a vertical victory
            board.MakeMove(new TTTMove(0, 0));
            board.MakeMove(new TTTMove(2, 2));
            board.MakeMove(new TTTMove(1, 1));
            board.MakeMove(new TTTMove(2, 0));
            board.MakeMove(new TTTMove(0, 2));
            board.MakeMove(new TTTMove(2, 1));

            //Player 2 should have won the game
            Assert.AreEqual(2, board.Winner);
        }

        [Test]
        public void WinTestUpwardsDiagonal()
        {
            TTTBoard board = new TTTBoard();

            //Make moves such that player 2 should win with an upwards diagonal victory
            board.MakeMove(new TTTMove(0, 0));
            board.MakeMove(new TTTMove(2, 0));
            board.MakeMove(new TTTMove(1, 2));
            board.MakeMove(new TTTMove(0, 2));
            board.MakeMove(new TTTMove(2, 2));
            board.MakeMove(new TTTMove(1, 1));

            //Player 2 should have won the game
            Assert.AreEqual(2, board.Winner);
        }

        [Test]
        public void WinTestDownwardsDiagonal()
        {
            TTTBoard board = new TTTBoard();

            //Make moves such that player 1 should win with a downwards diagonal victory
            board.MakeMove(new TTTMove(0, 0));
            board.MakeMove(new TTTMove(0, 1));
            board.MakeMove(new TTTMove(1, 1));
            board.MakeMove(new TTTMove(2, 0));
            board.MakeMove(new TTTMove(2, 2));

            //Player 1 should have won the game
            Assert.AreEqual(1, board.Winner);
        }

        [Test]
        public void SimulateTest()
        {
            TTTBoard board = new TTTBoard();
            board.SimulateUntilEnd();
        }
    }
}
