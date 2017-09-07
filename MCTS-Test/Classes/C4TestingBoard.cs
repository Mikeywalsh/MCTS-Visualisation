using MCTS.Core;
using MCTS.Core.Games;

namespace MCTS.Test
{
    /// <summary>
    /// A special version of a <see cref="C4Board"/>, which allows making of moves in any cell, regardless if any moves have been made beneath it <para/>
    /// Makes unit testing easier, as not every cell below a required cell has to be played in
    /// </summary>
    public class C4TestingBoard : C4Board
    {

        /// <summary>
        /// Makes a move on this Connect 4 board at the specified move position
        /// </summary>
        /// <param name="move">The move to make</param>
        /// <returns>A reference to this testing Connect 4 board</returns>
        public override Board MakeMove(Move move)
        {
            C4Move m = (C4Move)move;

            //Make the move on this board
            boardContents[m.X, m.Y] = 1;

            //Determine if there is a winner
            DetermineWinner(m);

            return this;
        }
    }
}
