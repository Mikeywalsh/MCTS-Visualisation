using NUnit.Framework;
using MCTS_Core;

namespace MCTS_Test
{
    public class TTTMoveTest
    {
        [Test]
        public void CreateValidMoveTest()
        {
            TTTMove move = new TTTMove(1, 2);
            Assert.AreEqual(1, move.X);
            Assert.AreEqual(2, move.Y);
        }

        [Test]
        public void CreateInvalidMoveTestLowX()
        {
            //Attempt to create a move with an x position of -1, which is not a valid positon
            //This should throw an InvalidMoveException
            Assert.Throws<InvalidMoveException>(() => CreateMove(-1, 0));
        }

        [Test]
        public void CreateInvalidMoveTestHighX()
        {
            //Attempt to create a move with an x position of 4, which is not a valid positon
            //This should throw an InvalidMoveException
            Assert.Throws<InvalidMoveException>(() => CreateMove(4, 0));
        }

        [Test]
        public void CreateInvalidMoveTestLowY()
        {
            //Attempt to create a move with a y position of -2, which is not a valid positon
            //This should throw an InvalidMoveException
            Assert.Throws<InvalidMoveException>(() => CreateMove(1, -2));
        }

        [Test]
        public void CreateInvalidMoveTestHighY()
        {
            //Attempt to create a move with a y position of 5, which is not a valid positon
            //This should throw an InvalidMoveException
            Assert.Throws<InvalidMoveException>(() => CreateMove(2, 5));
        }

        [Test]
        public void EqualityTest()
        {
            TTTMove moveA = CreateMove(1, 2);
            TTTMove moveB = CreateMove(1, 2);

            Assert.IsTrue(moveA.Equals(moveB));
        }

        [Test]
        public void InequalityTest()
        {
            TTTMove moveA = CreateMove(2, 1);
            TTTMove moveB = CreateMove(1, 2);

            Assert.IsFalse(moveA.Equals(moveB));
        }

        [Test]
        public void ValidHashCodeTest()
        {
            TTTMove move = CreateMove(1, 1);

            Assert.AreEqual(11, move.GetHashCode());
        }

        [Test]
        public void ToStringTest()
        {
            TTTMove move = CreateMove(2, 0);
            Assert.AreEqual("(2,0)", move.ToString());
        }

        /// <summary>
        /// Conveinience method used for creating moves
        /// Allows the creation of moves to be passed as a delegate to <see cref="Assert.Throws(System.Type, TestDelegate)"/>
        /// </summary>
        /// <param name="x">The x position of the move to create</param>
        /// <param name="x">The y position of the move to create</param>
        private TTTMove CreateMove(int x, int y)
        {
            return new TTTMove(x, y);
        }
    }
}