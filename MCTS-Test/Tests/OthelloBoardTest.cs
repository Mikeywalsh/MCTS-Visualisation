using NUnit.Framework;
using MCTS.Core;
using MCTS.Core.Games;

namespace MCTS.Test
{
    class OthelloBoardTest
    {
        [Test]
        public void PossibleMovesTest()
        {
            OthelloBoard board = new OthelloBoard();

            Assert.AreEqual(board.PossibleMoves().Count, 4);
        }

        [Test]
        public void SimulateTest()
        {
            OthelloBoard board = new OthelloBoard();
            board.SimulateUntilEnd();
        }
    }
}
